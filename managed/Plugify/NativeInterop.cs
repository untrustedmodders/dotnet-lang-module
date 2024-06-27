using System;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;

namespace Plugify;

public delegate void InvokeMethodDelegate(Guid managedMethodGuid, Guid thisObjectGuid, nint paramsPtr, nint outPtr);

public static class NativeInterop
{
	public static readonly int ApiVersion = 1;
    public static InvokeMethodDelegate InvokeMethodDelegate = InvokeMethod;

    [UnmanagedCallersOnly]
    public static void InitializeAssembly(nint outErrorStringPtr, nint outAssemblyGuid, nint classHolderPtr, nint assemblyPathStringPtr)
    {
        // Create a managed string from the pointer
        string? assemblyPath = Marshal.PtrToStringAnsi(assemblyPathStringPtr);
        if (assemblyPath == null)
        {
            OutError(outErrorStringPtr, "Assembly path is invalid");
            return;
        }
        
        AssemblyInstance? assemblyInstance = AssemblyCache.Instance.Get(assemblyPath);
        if (assemblyInstance != null)
        {
            OutError(outErrorStringPtr, "Assembly already loaded: " + assemblyPath + " (" + assemblyInstance.Guid + ")");
            return;
        }

        Guid assemblyGuid = Guid.NewGuid();
        Marshal.StructureToPtr(assemblyGuid, outAssemblyGuid, false);

        Logger.Log(Severity.Info, "Loading assembly: {0}...", assemblyPath);

        assemblyInstance = AssemblyCache.Instance.Add(assemblyGuid, assemblyPath);
        Assembly? assembly = assemblyInstance.Assembly;

        if (assembly == null)
        {
            OutError(outErrorStringPtr, "Failed to load assembly: " + assemblyPath);
            return;
        }

        foreach (Type type in assembly.GetExportedTypes())
        {
            if (type is { IsClass: true, IsAbstract: false })
            {
                InitManagedClass(assemblyGuid, classHolderPtr, type);
            }
        }

        NativeInterop_SetInvokeMethodFunction(ref assemblyGuid, classHolderPtr, Marshal.GetFunctionPointerForDelegate<InvokeMethodDelegate>(InvokeMethod));
    }
    
    [UnmanagedCallersOnly]
    public static void UnloadAssembly(nint assemblyGuidPtr, nint outResult)
    {
        bool result = true;

        try
        {
            Guid assemblyGuid = Marshal.PtrToStructure<Guid>(assemblyGuidPtr);

            if (AssemblyCache.Instance.Get(assemblyGuid) == null)
            {
                Logger.Log(Severity.Warning, "Failed to unload assembly: {0} not found", assemblyGuid);

                foreach (var kv in AssemblyCache.Instance.Assemblies)
                {
                    Logger.Log(Severity.Info, "Assembly: {0}", kv.Key);
                }

                result = false;
            }
            else
            {
                Logger.Log(Severity.Info, "Unloading assembly: {0}...", assemblyGuid);

                AssemblyCache.Instance.Remove(assemblyGuid);
            }
        }
        catch (Exception err)
        {
            Logger.Log(Severity.Error, "Error unloading assembly: {0}", err);

            result = false;
        }

        Marshal.WriteInt32(outResult, result ? 1 : 0);
    }

    [UnmanagedCallersOnly]
    public static unsafe void AddMethodToCache(nint assemblyGuidPtr, nint methodGuidPtr, nint methodInfoPtr)
    {
        Guid assemblyGuid = Marshal.PtrToStructure<Guid>(assemblyGuidPtr);
        Guid methodGuid = Marshal.PtrToStructure<Guid>(methodGuidPtr);

        ref MethodInfo methodInfo = ref Unsafe.AsRef<MethodInfo>(methodInfoPtr.ToPointer());

        ManagedMethodCache.Instance.AddMethod(assemblyGuid, methodGuid, methodInfo);
    }

    [UnmanagedCallersOnly]
    public static unsafe void AddObjectToCache(nint assemblyGuidPtr, nint objectGuidPtr, nint objectPtr, nint outManagedObjectPtr)
    {
        Guid assemblyGuid = Marshal.PtrToStructure<Guid>(assemblyGuidPtr);
        Guid objectGuid = Marshal.PtrToStructure<Guid>(objectGuidPtr);

        // read object as reference
        ref object obj = ref Unsafe.AsRef<object>(objectPtr.ToPointer());

        ManagedObject managedObject = ManagedObjectCache.Instance.AddObject(assemblyGuid, objectGuid, obj);

        // write managedObject to outManagedObjectPtr
        Marshal.StructureToPtr(managedObject, outManagedObjectPtr, false);
    }

    private static void CollectMethods(Type type, Dictionary<string, MethodInfo> methods)
    {
        MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        foreach (MethodInfo methodInfo in methodInfos)
        {
            // Skip duplicates in hierarchy
            if (methods.ContainsKey(methodInfo.Name))
            {
                continue;
            }

            // Skip constructors
            if (methodInfo.IsConstructor)
            {
                continue;
            }

            methods.Add(methodInfo.Name, methodInfo);
        }

        if (type.BaseType != null)
        {
            CollectMethods(type.BaseType, methods);
        }
    }

    private static ManagedClass InitManagedClass(Guid assemblyGuid, nint classHolderPtr, Type type)
    {
        string typeName = type.Name;
        nint typeNamePtr = Marshal.StringToHGlobalAnsi(typeName);
        bool isPlugin = type.IsAssignableFrom(typeof(Plugin));

        ManagedClass_Create(ref assemblyGuid, classHolderPtr, type.GetHashCode(), typeNamePtr, isPlugin, out ManagedClass managedClass);

        Marshal.FreeHGlobal(typeNamePtr);

        Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        CollectMethods(type, methods);

        foreach (KeyValuePair<string, MethodInfo> item in methods)
        {
            MethodInfo methodInfo = item.Value;

            // Add the objects being pointed to to the delegate cache so they don't get GC'd
            Guid methodGuid = Guid.NewGuid();
            managedClass.AddMethod(item.Key, methodGuid, methodInfo);

            ManagedMethodCache.Instance.AddMethod(assemblyGuid, methodGuid, methodInfo);
        }

        // Add new object, free object delegates
        managedClass.NewObjectFunction = () =>
        {
            // Allocate the object
            object obj = RuntimeHelpers.GetUninitializedObject(type);
            // GCHandle objHandle = GCHandle.Alloc(obj);

            // Call the constructor
            ConstructorInfo? constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

            if (constructorInfo == null)
            {
                throw new Exception("Failed to find empty constructor for type: " + type.Name);
            }

            constructorInfo.Invoke(obj, null);

            Guid objectGuid = Guid.NewGuid();

            // @TODO: Reduce complexity - this is a bit of a mess between C# adn C++ interop
            // ManagedObject managedObject = new ManagedObject();

            // NativeInterop_AddObjectToCache(ref assemblyGuid, ref objectGuid, GCHandle.Tonint(objHandle), out managedObject);

            // objHandle.Free();

            // return managedObject;
            
            return ManagedObjectCache.Instance.AddObject(assemblyGuid, objectGuid, obj);
        };

        managedClass.FreeObjectFunction = FreeObject;

        return managedClass;
    }

    private static unsafe void HandleParameters(nint paramsPtr, MethodInfo methodInfo, out object?[]? parameters)
    {
        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
        int numParams = parameterInfos.Length;

        if (numParams == 0 || paramsPtr == nint.Zero)
        {
            parameters = null;
            return;
        }

        parameters = new object?[numParams];

        int paramsOffset = 0;

        for (int i = 0; i < numParams; i++)
        {
            // params is stored as void**
            nint paramAddress = *(nint*)((byte*)paramsPtr + paramsOffset);
            paramsOffset += nint.Size;

            ParameterInfo parameterInfo = parameterInfos[i];
            Type paramType = parameterInfo.ParameterType;
            object[] paramAttributes = parameterInfo.GetCustomAttributes(typeof(MarshalAsAttribute), false);
            
            if (paramType.IsEnum)
            {
                Type underlyingType = Enum.GetUnderlyingType(paramType);
                parameters[i] = Enum.ToObject(paramType, ExtractValue(underlyingType, paramAttributes, paramAddress));
            } 
            else if (paramType.IsArray || paramType.IsArrayRef())
            {
                parameters[i] = ExtractArray(paramType, paramAttributes, paramAddress);
            }
            else
            {
                parameters[i] = ExtractValue(paramType, paramAttributes, paramAddress);
            }
        }
    }

    public static unsafe void PullReferences(nint paramsPtr, MethodInfo methodInfo, object?[]? parameters)
    {
        if (parameters == null)
        {
            return;
        }

        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
        int numParams = parameterInfos.Length;

        int paramsOffset = 0;

        for (int i = 0; i < numParams; i++)
        {
            // params is stored as void**
            nint paramAddress = *(nint*)((byte*)paramsPtr + paramsOffset);
            paramsOffset += nint.Size;

            ParameterInfo parameterInfo = parameterInfos[i];
            Type paramType = parameterInfo.ParameterType;
            
            if (!paramType.IsByRef)
            {
                continue;
            }

            paramType = TypeUtils.ConvertToUnrefType(paramType);
            
            object? paramValue = parameters[i];
            nint paramPtr = *(nint*)paramAddress;
            object[] paramAttributes = parameterInfo.GetCustomAttributes(typeof(MarshalAsAttribute), false);

            if (paramType.IsEnum)
            {
                // If paramType is an enum we need to get the underlying type
                paramType = Enum.GetUnderlyingType(paramType);
                paramValue = Convert.ChangeType(paramValue, paramType);
            }

            if (paramType.IsArray)
            {
                AssignArray(paramType, paramValue, paramAttributes, paramPtr);
            }
            else
            {
                AssignValue(paramType, paramValue, paramAttributes, paramPtr);
            }
        }
    }

    public static void InvokeMethod(Guid managedMethodGuid, Guid thisObjectGuid, nint paramsPtr, nint outPtr)
    {
        MethodInfo? methodInfo = ManagedMethodCache.Instance.GetMethod(managedMethodGuid);
        if (methodInfo == null)
        {
            throw new Exception("Failed to get method from GUID: " + managedMethodGuid);
        }

        HandleParameters(paramsPtr, methodInfo, out var parameters);

        object? thisObject = null;

        if (!methodInfo.IsStatic)
        {
            StoredManagedObject? storedObject = ManagedObjectCache.Instance.GetObject(thisObjectGuid);

            if (storedObject == null)
            {
                throw new Exception("Failed to get target from GUID: " + thisObjectGuid);
            }

            thisObject = storedObject.obj;
        }

        object? returnValue = methodInfo.Invoke(thisObject, parameters);

        PullReferences(paramsPtr, methodInfo, parameters);

        Type returnType = methodInfo.ReturnType;

        if (returnType.IsEnum)
        {
            // If returnType is an enum we need to get the underlying type and cast the value to it
            returnType = Enum.GetUnderlyingType(returnType);
            returnValue = Convert.ChangeType(returnValue, returnType);
        }

        if (returnType == typeof(void))
        {
            // No need to fill out the outPtr
            return;
        }

        object[] returnAttributes = methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(MarshalAsAttribute), false);
        
        if (returnType.IsArray)
        {
            AssignArray(returnType, returnValue, returnAttributes, outPtr);
        }
        else
        {
            AssignValue(returnType, returnValue, returnAttributes, outPtr);
        }
    }

    private static void AssignArray(Type paramType, object? paramValue, object[] paramAttributes, nint paramPtr)
    {
        Type? elementType = paramType.GetElementType();
        switch (Type.GetTypeCode(elementType))
        {
            case TypeCode.Char:
                if (paramValue is char[] arrChar)
                {
                    if (TypeUtils.IsUseAnsi(paramAttributes))
                    {
                        NativeMethods.AssignVectorChar8(paramPtr, arrChar, arrChar.Length);
                    }
                    else
                    {
                        NativeMethods.AssignVectorChar16(paramPtr, arrChar, arrChar.Length);
                    }
                }
                break;
            case TypeCode.Boolean:
                if (paramValue is bool[] arrBool) NativeMethods.AssignVectorBool(paramPtr, arrBool, arrBool.Length);
                break;
            case TypeCode.SByte:
                if (paramValue is sbyte[] arrInt8) NativeMethods.AssignVectorInt8(paramPtr, arrInt8, arrInt8.Length);
                break;
            case TypeCode.Int16:
                if (paramValue is short[] arrInt16) NativeMethods.AssignVectorInt16(paramPtr, arrInt16, arrInt16.Length);
                break;
            case TypeCode.Int32:
                if (paramValue is int[] arrInt32) NativeMethods.AssignVectorInt32(paramPtr, arrInt32, arrInt32.Length);
                break;
            case TypeCode.Int64:
                if (paramValue is long[] arrInt64) NativeMethods.AssignVectorInt64(paramPtr, arrInt64, arrInt64.Length);
                break;
            case TypeCode.Byte:
                if (paramValue is byte[] arrUInt8) NativeMethods.AssignVectorUInt8(paramPtr, arrUInt8, arrUInt8.Length);
                break;
            case TypeCode.UInt16:
                if (paramValue is ushort[] arrUInt16) NativeMethods.AssignVectorUInt16(paramPtr, arrUInt16, arrUInt16.Length);
                break;
            case TypeCode.UInt32:
                if (paramValue is uint[] arrUInt32) NativeMethods.AssignVectorUInt32(paramPtr, arrUInt32, arrUInt32.Length);
                break;
            case TypeCode.UInt64:
                if (paramValue is ulong[] arrUInt64) NativeMethods.AssignVectorUInt64(paramPtr, arrUInt64, arrUInt64.Length);
                break;
            case TypeCode.Single:
                if (paramValue is float[] arrFloat) NativeMethods.AssignVectorFloat(paramPtr, arrFloat, arrFloat.Length);
                break;
            case TypeCode.Double:
                if (paramValue is double[] arrDouble) NativeMethods.AssignVectorDouble(paramPtr, arrDouble, arrDouble.Length);
                break;
            case TypeCode.String:
                if (paramValue is string[] arrString) NativeMethods.AssignVectorString(paramPtr, arrString, arrString.Length);
                break;
            default:
                if (elementType == typeof(nint))
                {
                    if (paramValue is nint[] arrIntPtr) NativeMethods.AssignVectorIntPtr(paramPtr, arrIntPtr, arrIntPtr.Length);
                }
                else
                    throw new NotImplementedException($"Unknown array type {elementType?.Name ?? ""}");
                break;
        }
    }

    private static unsafe void AssignValue(Type paramType, object? paramValue, object[] paramAttributes, nint paramPtr)
    {
        switch (Type.GetTypeCode(paramType))
        {
            case TypeCode.Char:
                if (TypeUtils.IsUseAnsi(paramAttributes))
                {
                    *((byte*)paramPtr) = (byte)((char)paramValue!);
                }
                else
                {
                    *((short*)paramPtr) = (short)((char)paramValue!);
                }
                return;
            case TypeCode.Boolean:
                *((byte*)paramPtr) = (byte)((bool)paramValue! ? 1 : 0);
                return;
            case TypeCode.SByte:
                *((sbyte*)paramPtr) = (sbyte)paramValue!;
                return;
            case TypeCode.Int16:
                *((short*)paramPtr) = (short)paramValue!;
                return;
            case TypeCode.Int32:
                *((int*)paramPtr) = (int)paramValue!;
                return;
            case TypeCode.Int64:
                *((long*)paramPtr) = (long)paramValue!;
                return;
            case TypeCode.Byte:
                *((byte*)paramPtr) = (byte)paramValue!;
                return;
            case TypeCode.UInt16:
                *((ushort*)paramPtr) = (ushort)paramValue!;
                return;
            case TypeCode.UInt32:
                *((uint*)paramPtr) = (uint)paramValue!;
                return;
            case TypeCode.UInt64:
                *((ulong*)paramPtr) = (ulong)paramValue!;
                return;
            case TypeCode.Single:
                *((float*)paramPtr) = (float)paramValue!;
                return;
            case TypeCode.Double:
                *((double*)paramPtr) = (double)paramValue!;
                return;
            case TypeCode.String:
                if (paramValue is string str) NativeMethods.AssignString(paramPtr, str);
                return;
        }

        if (paramType == typeof(nint))
        {
            *((nint*)paramPtr) = (nint)paramValue!;
            return;
        }

        if (paramType.IsValueType)
        {
            Marshal.StructureToPtr(paramValue!, paramPtr, false);
            return;
        }

        throw new NotImplementedException($"Parameter type {paramType.Name} not implemented");
    }

    private static unsafe object ExtractArray(Type paramType, object[] paramAttributes, nint paramAddress)
    {
        if (paramType.IsByRef)
        {
            paramType = TypeUtils.ConvertToUnrefType(paramType);
        }

        nint paramPtr = *(nint*)paramAddress;

        Type? elementType = paramType.GetElementType();
        switch (Type.GetTypeCode(elementType))
        {
            case TypeCode.Char:
                if (TypeUtils.IsUseAnsi(paramAttributes))
                {
                    var arrChar = new char[NativeMethods.GetVectorSizeChar8(paramPtr)];
                    NativeMethods.GetVectorDataChar8(paramPtr, arrChar);
                    return arrChar;
                }
                else
                {
                    var arrChar = new char[NativeMethods.GetVectorSizeChar16(paramPtr)];
                    NativeMethods.GetVectorDataChar16(paramPtr, arrChar);
                    return arrChar;
                }
            case TypeCode.Boolean:
                var arrBool = new bool[NativeMethods.GetVectorSizeBool(paramPtr)];
                NativeMethods.GetVectorDataBool(paramPtr, arrBool);
                return arrBool;
            case TypeCode.SByte:
                var arrInt8 = new sbyte[NativeMethods.GetVectorSizeInt8(paramPtr)];
                NativeMethods.GetVectorDataInt8(paramPtr, arrInt8);
                return arrInt8;
            case TypeCode.Int16:
                var arrInt16 = new short[NativeMethods.GetVectorSizeInt16(paramPtr)];
                NativeMethods.GetVectorDataInt16(paramPtr, arrInt16);
                return arrInt16;
            case TypeCode.Int32:
                var arrInt32 = new int[NativeMethods.GetVectorSizeInt32(paramPtr)];
                NativeMethods.GetVectorDataInt32(paramPtr, arrInt32);
                return arrInt32;
            case TypeCode.Int64:
                var arrInt64 = new long[NativeMethods.GetVectorSizeInt64(paramPtr)];
                NativeMethods.GetVectorDataInt64(paramPtr, arrInt64);
                return arrInt64;
            case TypeCode.Byte:
                var arrUInt8 = new byte[NativeMethods.GetVectorSizeUInt8(paramPtr)];
                NativeMethods.GetVectorDataUInt8(paramPtr, arrUInt8);
                return arrUInt8;
            case TypeCode.UInt16:
                var arrUInt16 = new ushort[NativeMethods.GetVectorSizeUInt16(paramPtr)];
                NativeMethods.GetVectorDataUInt16(paramPtr, arrUInt16);
                return arrUInt16;
            case TypeCode.UInt32:
                var arrUInt32 = new uint[NativeMethods.GetVectorSizeUInt32(paramPtr)];
                NativeMethods.GetVectorDataUInt32(paramPtr, arrUInt32);
                return arrUInt32;
            case TypeCode.UInt64:
                var arrUInt64 = new ulong[NativeMethods.GetVectorSizeUInt64(paramPtr)];
                NativeMethods.GetVectorDataUInt64(paramPtr, arrUInt64);
                return arrUInt64;
            case TypeCode.Single:
                var arrFloat = new float[NativeMethods.GetVectorSizeFloat(paramPtr)];
                NativeMethods.GetVectorDataFloat(paramPtr, arrFloat);
                return arrFloat;
            case TypeCode.Double:
                var arrDouble = new double[NativeMethods.GetVectorSizeDouble(paramPtr)];
                NativeMethods.GetVectorDataDouble(paramPtr, arrDouble);
                return arrDouble;
            case TypeCode.String:
                var arrString = new string[NativeMethods.GetVectorSizeString(paramPtr)];
                NativeMethods.GetVectorDataString(paramPtr, arrString);
                return arrString;
            default:
                if (elementType == typeof(nint))
                {
                    var arrIntPtr = new nint[NativeMethods.GetVectorSizeIntPtr(paramPtr)];
                    NativeMethods.GetVectorDataIntPtr(paramPtr, arrIntPtr);
                    return arrIntPtr;
                }
                
                throw new NotImplementedException($"Array type {elementType?.Name ?? ""} not implemented");
        }
    }

    private static unsafe object ExtractValue(Type paramType, object[] paramAttributes, nint paramAddress)
    {
        if (paramType.IsByRef)
        {
            paramType = TypeUtils.ConvertToUnrefType(paramType);

            // Should be pass by pointers in unmanaged
            nint paramPtr = *(nint*)paramAddress;

            switch (Type.GetTypeCode(paramType))
            {
                case TypeCode.Char:
                    if (TypeUtils.IsUseAnsi(paramAttributes))
                    {
                        return (char)(*(byte*)paramPtr);
                    }
                    else
                    {
                        return (char)(*(short*)paramPtr);
                    }
                case TypeCode.Boolean:
                    return *(byte*)paramPtr == 1;
                case TypeCode.SByte:
                    return *(sbyte*)paramPtr;
                case TypeCode.Int16:
                    return *(short*)paramPtr;
                case TypeCode.Int32:
                    return *(int*)paramPtr;
                case TypeCode.Int64:
                    return *(long*)paramPtr;
                case TypeCode.Byte:
                    return *(byte*)paramPtr;
                case TypeCode.UInt16:
                    return *(ushort*)paramPtr;
                case TypeCode.UInt32:
                    return *(uint*)paramPtr;
                case TypeCode.UInt64:
                    return *(ulong*)paramPtr;
                case TypeCode.Single:
                    return *(float*)paramPtr;
                case TypeCode.Double:
                    return *(double*)paramPtr;
                case TypeCode.String:
                    return NativeMethods.GetStringData(paramPtr);
            }

            if (paramType == typeof(nint))
            {
                return *(nint*)paramPtr;
            }
            
            if (paramType.IsValueType)
            {
                return Marshal.PtrToStructure(paramPtr, paramType) ?? throw new InvalidOperationException("Invalid structure!");
            }
        }
        else
        {
            switch (Type.GetTypeCode(paramType))
            {
                case TypeCode.Char:
                    if (TypeUtils.IsUseAnsi(paramAttributes))
                    {
                        return (char)(*(byte*)paramAddress);
                    }
                    else
                    {
                        return (char)(*(short*)paramAddress);
                    }
                case TypeCode.Boolean:
                    return *(byte*)paramAddress == 1;
                case TypeCode.SByte:
                    return *(sbyte*)paramAddress;
                case TypeCode.Int16:
                    return *(short*)paramAddress;
                case TypeCode.Int32:
                    return *(int*)paramAddress;
                case TypeCode.Int64:
                    return *(long*)paramAddress;
                case TypeCode.Byte:
                    return *(byte*)paramAddress;
                case TypeCode.UInt16:
                    return *(ushort*)paramAddress;
                case TypeCode.UInt32:
                    return *(uint*)paramAddress;
                case TypeCode.UInt64:
                    return *(ulong*)paramAddress;
                case TypeCode.Single:
                    return *(float*)paramAddress;
                case TypeCode.Double:
                    return *(double*)paramAddress;
                case TypeCode.String:
                    nint paramPtr = *(nint*)paramAddress;
                    return NativeMethods.GetStringData(paramPtr);
            }
            
            if (paramType.IsDelegate())
            {
                nint funcAddress = *(nint*)paramAddress;
                // TODO: If no strings or arrays use Marshal.CreateDelegate
                //return MethodUtils.CreateObjectArrayDelegate(paramType, ExternalInvoke(funcAddress, paramType));
            }
            
            if (paramType == typeof(nint))
            {
                return *(nint*)paramAddress;
            }
            
            if (paramType.IsValueType)
            {
                nint paramPtr = *(nint*)paramAddress;
                return Marshal.PtrToStructure(paramPtr, paramType) ?? throw new InvalidOperationException("Invalid structure!");
            }
        }

        throw new NotImplementedException($"Parameter type {paramType.Name} not implemented");
    }

    /*private static Func<object[], object> ExternalInvoke(nint funcAddress, Type paramType)
    {
        MethodInfo methodInfo = paramType.GetMethod("Invoke") ?? throw new InvalidOperationException("Invalid delegate!");
        Type returnType = methodInfo.ReturnType;
        bool hasRet = returnType.IsHiddenObjectParam();
         
        var types = new List<Type>();
        
        if (hasRet)
        {
            types.Add(typeof(nint));
        }
        
        foreach (var param in methodInfo.GetParameters())
        {
            Type type = param.ParameterType;
            if (type.IsArray || type.IsArrayRef() || type == typeof(string))
            {
                types.Add(typeof(nint));
            }
            else
            {
                types.Add(type);
            }
        }
        
        if (returnType.IsArray || returnType.IsArrayRef() || returnType == typeof(string))
        {
            types.Add(typeof(nint));
        }
        else
        {
            types.Add(returnType);
        }
        
        Type delegateType = Expression.GetDelegateType(types.ToArray());
        
        return parameters =>
        {
            object[] args = new object[hasRet ? parameters.Length + 1 : parameters.Length];

            if (hasRet)
            {
                if (returnType.IsArray)
                {
                    Type? elementType = returnType.GetElementType();
                    switch (Type.GetTypeCode(elementType))
                    {
                        case TypeCode.Boolean:
                            args[0] = NativeMethods.AllocateVectorBool();
                            break;
                        case TypeCode.Char:
                            if (TypeUtils.IsUseAnsi(methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(MarshalAsAttribute), false)))
                            {
                                args[0] = NativeMethods.AllocateVectorChar8();
                            }
                            else
                            {
                                args[0] = NativeMethods.AllocateVectorChar16();
                            }
                            break;
                        case TypeCode.SByte:
                            args[0] = NativeMethods.AllocateVectorInt8();
                            break;
                        case TypeCode.Int16:
                            args[0] = NativeMethods.AllocateVectorInt16();
                            break;
                        case TypeCode.Int32:
                            args[0] = NativeMethods.AllocateVectorInt32();
                            break;
                        case TypeCode.Int64:
                            args[0] = NativeMethods.AllocateVectorInt64();
                            break;
                        case TypeCode.Byte:
                            args[0] = NativeMethods.AllocateVectorUInt8();
                            break;
                        case TypeCode.UInt16:
                            args[0] = NativeMethods.AllocateVectorUInt16();
                            break;
                        case TypeCode.UInt32:
                            args[0] = NativeMethods.AllocateVectorUInt32();
                            break;
                        case TypeCode.UInt64:
                            args[0] = NativeMethods.AllocateVectorUInt64();
                            break;
                        case TypeCode.Single:
                            args[0] = NativeMethods.AllocateVectorFloat();
                            break;
                        case TypeCode.Double:
                            args[0] = NativeMethods.AllocateVectorDouble();
                            break;
                        case TypeCode.String:
                            args[0] = NativeMethods.AllocateVectorString();
                            break;
                        default:
                            if (elementType == typeof(nint))
                            {
                                args[0] = NativeMethods.AllocateVectorIntPtr();
                                break;
                            }
                    
                            throw new NotImplementedException($"Array type {elementType?.Name ?? ""} not implemented");
                    }
                }
                else
                {
                    if (returnType == typeof(string))
                    {
                        args[0] = NativeMethods.AllocateString();
                    }
                }
            }
            
            for (int i = 0; i < parameters.Length; i++)
            {
                object paramValue = parameters[i];
                Type paramType = paramValue.GetType();
            
                int j = hasRet ? i + 1 : i;

                if (paramType.IsEnum)
                {
                    // If paramType is an enum we need to get the underlying type
                    paramType = Enum.GetUnderlyingType(paramType);
                    args[j] = Convert.ChangeType(paramValue, paramType);
                }
                else if (paramType.IsArray || paramType.IsArrayRef())
                {
                    if (paramType.IsByRef)
                    {
                        paramType = TypeUtils.ConvertToUnrefType(paramType);
                    }
                    
                    Type? elementType = paramType.GetElementType();
                    switch (Type.GetTypeCode(elementType))
                    {
                        case TypeCode.Char:
                            if (paramValue is char[] arrChar)
                            {
                                if (TypeUtils.IsUseAnsi(methodInfo.GetParameters()[i].GetCustomAttributes(typeof(MarshalAsAttribute), false)))
                                {
                                    args[j] = NativeMethods.CreateVectorChar8(arrChar, arrChar.Length);
                                }
                                else
                                {
                                    args[j] = NativeMethods.CreateVectorChar16(arrChar, arrChar.Length);
                                }
                            }
                            else
                            {
                                if (TypeUtils.IsUseAnsi(methodInfo.GetParameters()[i].GetCustomAttributes(typeof(MarshalAsAttribute), false)))
                                {
                                    args[j] = NativeMethods.CreateVectorChar8([], 0);
                                }
                                else
                                {
                                    args[j] = NativeMethods.CreateVectorChar16([], 0);
                                }
                            }
                            break;
                        case TypeCode.Boolean:
                            if (paramValue is bool[] arrBool)
                            {
                                args[j] = NativeMethods.CreateVectorBool(arrBool, arrBool.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorBool([], 0);
                            }
                            break;
                        case TypeCode.SByte:
                            if (paramValue is sbyte[] arrInt8)
                            {
                                args[j] = NativeMethods.CreateVectorInt8(arrInt8, arrInt8.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorInt8([], 0);
                            }
                            break;
                        case TypeCode.Int16:
                            if (paramValue is short[] arrInt16)
                            {
                                args[j] = NativeMethods.CreateVectorInt16(arrInt16, arrInt16.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorInt16([], 0);
                            }
                            break;
                        case TypeCode.Int32:
                            if (paramValue is int[] arrInt32)
                            {
                                args[j] = NativeMethods.CreateVectorInt32(arrInt32, arrInt32.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorInt32([], 0);
                            }
                            break;
                        case TypeCode.Int64:
                            if (paramValue is long[] arrInt64)
                            {
                                args[j] = NativeMethods.CreateVectorInt64(arrInt64, arrInt64.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorInt64([], 0);
                            }
                            break;
                        case TypeCode.Byte:
                            if (paramValue is byte[] arrUInt8)
                            {
                                args[j] = NativeMethods.CreateVectorUInt8(arrUInt8, arrUInt8.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorUInt8([], 0);
                            }
                            break;
                        case TypeCode.UInt16:
                            if (paramValue is ushort[] arrUInt16)
                            {
                                args[j] = NativeMethods.CreateVectorUInt16(arrUInt16, arrUInt16.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorUInt16([], 0);
                            }
                            break;
                        case TypeCode.UInt32:
                            if (paramValue is uint[] arrUInt32)
                            {
                                args[j] = NativeMethods.CreateVectorUInt32(arrUInt32, arrUInt32.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorUInt32([], 0);
                            }
                            break;
                        case TypeCode.UInt64:
                            if (paramValue is ulong[] arrUInt64)
                            {
                                args[j] = NativeMethods.CreateVectorUInt64(arrUInt64, arrUInt64.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorUInt64([], 0);
                            }
                            break;
                        case TypeCode.Single:
                            if (paramValue is float[] arrFloat)
                            {
                                args[j] = NativeMethods.CreateVectorFloat(arrFloat, arrFloat.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorFloat([], 0);
                            }
                            break;
                        case TypeCode.Double:
                            if (paramValue is double[] arrDouble)
                            {
                                args[j] = NativeMethods.CreateVectorDouble(arrDouble, arrDouble.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorDouble([], 0);
                            }
                            break;
                        case TypeCode.String:
                            if (paramValue is string[] arrString)
                            {
                                args[j] = NativeMethods.CreateVectorString(arrString, arrString.Length);
                            }
                            else
                            {
                                args[j] = NativeMethods.CreateVectorString([], 0);
                            }
                            break;
                        default:
                            if (elementType == typeof(nint))
                            {
                                if (paramValue is nint[] arrIntPtr)
                                {
                                    args[j] = NativeMethods.CreateVectorIntPtr(arrIntPtr, arrIntPtr.Length);
                                }
                                else
                                {
                                    args[j] = NativeMethods.CreateVectorIntPtr([], 0);
                                }
                            }
                            else
                                throw new NotImplementedException($"Unknown array type {elementType?.Name ?? ""}");
                            break;
                    }
                }
                else
                {
                    if (paramType == typeof(string))
                    {
                        if (paramValue is string str)
                        {
                            args[j] = NativeMethods.CreateString(str);
                        }
                        else
                        {
                            args[j] = NativeMethods.CreateString("");
                        }
                    }
                    else if (paramType.IsDelegate())
                    {
                        args[j] = Marshal.GetFunctionPointerForDelegate(paramValue);
                    }
                    else
                    {
                        args[j] = paramValue;
                    }
                }
            }
            
            Delegate func = Marshal.GetDelegateForFunctionPointer(funcAddress, delegateType);
            object? ret = func.DynamicInvoke(args);
            
            for (int i = 0; i < parameters.Length; i++)
            {
                object paramValue = parameters[i];
                Type paramType = paramValue.GetType();
            
                int j = hasRet ? i + 1 : i;

                if (paramType.IsEnum)
                {
                    // If paramType is an enum we need to get the underlying type
                    paramType = Enum.GetUnderlyingType(paramType);
                    parameters[i] = Enum.ToObject(paramType, args[j]);
                }
                else if (paramType.IsArray || paramType.IsArrayRef())
                {
                    if (paramType.IsByRef)
                    {
                        paramType = TypeUtils.ConvertToUnrefType(paramType);
                    }
                    
                    Type? elementType = paramType.GetElementType();
                    switch (Type.GetTypeCode(elementType))
                    {
                        case TypeCode.Char:
                            nint ptrChar = (nint)args[j];
                            if (TypeUtils.IsUseAnsi(methodInfo.GetParameters()[i].GetCustomAttributes(typeof(MarshalAsAttribute), false)))
                            {
                                if (paramType.IsByRef) NativeMethods.GetVectorDataChar8(ptrChar, (char[])parameters[i]); 
                                NativeMethods.DeleteVectorChar8(ptrChar);
                            }
                            else
                            {
                                if (paramType.IsByRef) NativeMethods.GetVectorDataChar16(ptrChar, (char[])parameters[i]); 
                                NativeMethods.DeleteVectorChar16(ptrChar);
                            }
                            break;
                        case TypeCode.Boolean:
                            nint ptrBoolean = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataBool(ptrBoolean, (bool[])parameters[i]); 
                            NativeMethods.DeleteVectorBool(ptrBoolean);
                            break;
                        case TypeCode.SByte:
                            nint ptrInt8 = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataInt8(ptrInt8, (sbyte[])parameters[i]); 
                            NativeMethods.DeleteVectorInt8(ptrInt8);
                            break;
                        case TypeCode.Int16:
                            nint ptrInt16 = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataInt16(ptrInt16, (short[])parameters[i]); 
                            NativeMethods.DeleteVectorInt16(ptrInt16);
                            break;
                        case TypeCode.Int32:
                            nint ptrInt32 = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataInt32(ptrInt32, (int[])parameters[i]); 
                            NativeMethods.DeleteVectorInt32(ptrInt32);
                            break;
                        case TypeCode.Int64:
                            nint ptrInt64 = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataInt64(ptrInt64, (long[])parameters[i]); 
                            NativeMethods.DeleteVectorInt64(ptrInt64);
                            break;
                        case TypeCode.Byte:
                            nint ptrUInt8 = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataUInt8(ptrUInt8, (byte[])parameters[i]); 
                            NativeMethods.DeleteVectorUInt8(ptrUInt8);
                            break;
                        case TypeCode.UInt16:
                            nint ptrUInt16 = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataUInt16(ptrUInt16, (ushort[])parameters[i]); 
                            NativeMethods.DeleteVectorUInt16(ptrUInt16);
                            break;
                        case TypeCode.UInt32:
                            nint ptrUInt32 = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataUInt32(ptrUInt32, (uint[])parameters[i]); 
                            NativeMethods.DeleteVectorUInt32(ptrUInt32);
                            break;
                        case TypeCode.UInt64:
                            nint ptrUInt64 = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataUInt64(ptrUInt64, (ulong[])parameters[i]); 
                            NativeMethods.DeleteVectorUInt64(ptrUInt64);
                            break;
                        case TypeCode.Single:
                            nint ptrFloat = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataFloat(ptrFloat, (float[])parameters[i]); 
                            NativeMethods.DeleteVectorFloat(ptrFloat);
                            break;
                        case TypeCode.Double:
                            nint ptrDouble = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataDouble(ptrDouble, (double[])parameters[i]); 
                            NativeMethods.DeleteVectorDouble(ptrDouble);
                            break;
                        case TypeCode.String:
                            nint ptrString = (nint)args[j];
                            if (paramType.IsByRef) NativeMethods.GetVectorDataString(ptrString, (string[])parameters[i]); 
                            NativeMethods.DeleteVectorString(ptrString);
                            break;
                        default:
                            if (elementType == typeof(nint))
                            {
                                nint ptrIntPtr = (nint)args[j];
                                if (paramType.IsByRef) NativeMethods.GetVectorDataIntPtr(ptrIntPtr, (nint[])parameters[i]); 
                                NativeMethods.DeleteVectorIntPtr(ptrIntPtr);
                            }
                            else
                                throw new NotImplementedException($"Unknown array type {elementType?.Name ?? ""}");
                            break;
                    }
                }
                else
                {
                    if (paramType == typeof(string))
                    {
                        nint ptr = (nint)args[j];
                        if (paramType.IsByRef) parameters[i] = NativeMethods.GetStringData(ptr);
                        NativeMethods.DeleteString(ptr);
                    }
                }
            }
            
            if (hasRet)
            {
                if (returnType.IsArray)
                {
                    Type? elementType = returnType.GetElementType();
                    switch (Type.GetTypeCode(elementType))
                    {
                        case TypeCode.Boolean:
                            nint ptrBool = (nint)args[0];
                            bool[] arrBool = new bool[NativeMethods.GetVectorSizeBool(ptrBool)];
                            NativeMethods.GetVectorDataBool(ptrBool, arrBool);
                            NativeMethods.FreeVectorBool(ptrBool);
                            ret = arrBool;
                            break;
                        case TypeCode.Char:
                            nint ptrChar = (nint)args[0];
                            if (TypeUtils.IsUseAnsi(methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(MarshalAsAttribute), false)))
                            {
                                char[] arrChar = new char[NativeMethods.GetVectorSizeChar8(ptrChar)];
                                NativeMethods.GetVectorDataChar8(ptrChar, arrChar);
                                NativeMethods.FreeVectorChar8(ptrChar);
                                ret = arrChar;
                            }
                            else
                            {
                                char[] arrChar = new char[NativeMethods.GetVectorSizeChar16(ptrChar)];
                                NativeMethods.GetVectorDataChar16(ptrChar, arrChar);
                                NativeMethods.FreeVectorChar16(ptrChar);
                                ret = arrChar;
                            }
                            break;
                        case TypeCode.SByte:
                            nint ptrInt8 = (nint)args[0];
                            sbyte[] arrInt8 = new sbyte[NativeMethods.GetVectorSizeInt8(ptrInt8)];
                            NativeMethods.GetVectorDataInt8(ptrInt8, arrInt8);
                            NativeMethods.FreeVectorInt8(ptrInt8);
                            ret = arrInt8;
                            break;
                        case TypeCode.Int16:
                            nint ptrInt16 = (nint)args[0];
                            short[] arrInt16 = new short[NativeMethods.GetVectorSizeInt16(ptrInt16)];
                            NativeMethods.GetVectorDataInt16(ptrInt16, arrInt16);
                            NativeMethods.FreeVectorInt16(ptrInt16);
                            ret = arrInt16;
                            break;
                        case TypeCode.Int32:
                            nint ptrInt32 = (nint)args[0];
                            int[] arrInt32 = new int[NativeMethods.GetVectorSizeInt32(ptrInt32)];
                            NativeMethods.GetVectorDataInt32(ptrInt32, arrInt32);
                            NativeMethods.FreeVectorInt32(ptrInt32);
                            ret = arrInt32;
                            break;
                        case TypeCode.Int64:
                            nint ptrInt64 = (nint)args[0];
                            long[] arrInt64 = new long[NativeMethods.GetVectorSizeInt64(ptrInt64)];
                            NativeMethods.GetVectorDataInt64(ptrInt64, arrInt64);
                            NativeMethods.FreeVectorInt64(ptrInt64);
                            ret = arrInt64;
                            break;
                        case TypeCode.Byte:
                            nint ptrUInt8 = (nint)args[0];
                            byte[] arrUInt8 = new byte[NativeMethods.GetVectorSizeUInt8(ptrUInt8)];
                            NativeMethods.GetVectorDataUInt8(ptrUInt8, arrUInt8);
                            NativeMethods.FreeVectorUInt8(ptrUInt8);
                            ret = arrUInt8;
                            break;
                        case TypeCode.UInt16:
                            nint ptrUInt16 = (nint)args[0];
                            ushort[] arrUInt16 = new ushort[NativeMethods.GetVectorSizeUInt16(ptrUInt16)];
                            NativeMethods.GetVectorDataUInt16(ptrUInt16, arrUInt16);
                            NativeMethods.FreeVectorUInt16(ptrUInt16);
                            ret = arrUInt16;
                            break;
                        case TypeCode.UInt32:
                            nint ptrUInt32 = (nint)args[0];
                            uint[] arrUInt32 = new uint[NativeMethods.GetVectorSizeUInt32(ptrUInt32)];
                            NativeMethods.GetVectorDataUInt32(ptrUInt32, arrUInt32);
                            NativeMethods.FreeVectorUInt32(ptrUInt32);
                            ret = arrUInt32;
                            break;
                        case TypeCode.UInt64:
                            nint ptrUInt64 = (nint)args[0];
                            ulong[] arrUInt64 = new ulong[NativeMethods.GetVectorSizeUInt64(ptrUInt64)];
                            NativeMethods.GetVectorDataUInt64(ptrUInt64, arrUInt64);
                            NativeMethods.FreeVectorUInt64(ptrUInt64);
                            ret = arrUInt64;
                            break;
                        case TypeCode.Single:
                            nint ptrFloat = (nint)args[0];
                            float[] arrFloat = new float[NativeMethods.GetVectorSizeFloat(ptrFloat)];
                            NativeMethods.GetVectorDataFloat(ptrFloat, arrFloat);
                            NativeMethods.FreeVectorFloat(ptrFloat);
                            ret = arrFloat;
                            break;
                        case TypeCode.Double:
                            nint ptrDouble = (nint)args[0];
                            double[] arrDouble = new double[NativeMethods.GetVectorSizeDouble(ptrDouble)];
                            NativeMethods.GetVectorDataDouble(ptrDouble, arrDouble);
                            NativeMethods.FreeVectorDouble(ptrDouble);
                            ret = arrDouble;
                            break;
                        case TypeCode.String:
                            nint ptrString = (nint)args[0];
                            string[] arrString = new string[NativeMethods.GetVectorSizeString(ptrString)];
                            NativeMethods.GetVectorDataString(ptrString, arrString);
                            NativeMethods.FreeVectorString(ptrString);
                            ret = arrString;
                            break;
                        default:
                            if (elementType == typeof(nint))
                            {
                                nint ptrIntPtr = (nint)args[0];
                                nint[] arrIntPtr = new nint[NativeMethods.GetVectorSizeIntPtr(ptrIntPtr)];
                                NativeMethods.GetVectorDataIntPtr(ptrIntPtr, arrIntPtr);
                                NativeMethods.FreeVectorIntPtr(ptrIntPtr);
                                break;
                            }
                    
                            throw new NotImplementedException($"Array type {elementType?.Name ?? ""} not implemented");
                    }
                }
                else
                {
                    if (returnType == typeof(string))
                    {
                        nint ptr = (nint)args[0];
                        ret = NativeMethods.GetStringData(ptr);
                        NativeMethods.FreeString(ptr);
                    }
                }
            }
            
            return ret ?? throw new InvalidOperationException();
        };
    }*/

    public static void FreeObject(ManagedObject obj)
    {
        if (!ManagedObjectCache.Instance.RemoveObject(obj.guid))
        {
            throw new Exception("Failed to remove object from cache: " + obj.guid);
        }
    }

    private static void OutError(nint outErrorStringPtr, string message)
    {
        if (outErrorStringPtr == nint.Zero)
            return;
        
        byte[] bytes = Encoding.ASCII.GetBytes(message);
        Marshal.Copy(bytes, 0, outErrorStringPtr, bytes.Length);
    }

    [DllImport(NativeMethods.DllName)]
    private static extern void ManagedClass_Create(ref Guid assemblyGuid, nint classHolderPtr, int typeHash, nint typeNamePtr, bool isPlugin, [Out] out ManagedClass result);

    [DllImport(NativeMethods.DllName)]
    private static extern void NativeInterop_SetInvokeMethodFunction([In] ref Guid assemblyGuid, nint classHolderPtr, nint invokeMethodPtr);
}
