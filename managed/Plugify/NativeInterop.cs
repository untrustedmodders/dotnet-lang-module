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
    
    private static readonly DCCallVM vm = new DCCallVM(4096);

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
                parameters[i] = Enum.ToObject(paramType, Extract(underlyingType, paramAttributes, paramAddress));
            } 
            else
            {
                parameters[i] = Extract(paramType, paramAttributes, paramAddress);
            }
        }
    }

    private static unsafe void HandleReferences(nint paramsPtr, MethodInfo methodInfo, object?[]? parameters)
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

            Assign(paramType, paramValue, paramAttributes, paramPtr);
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

        HandleReferences(paramsPtr, methodInfo, parameters);

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

        Assign(returnType, returnValue, returnAttributes, outPtr);
    }
    
    private static unsafe void Assign(Type paramType, object? paramValue, object[] paramAttributes, nint paramPtr)
    {
        switch (TypeUtils.ConvertToValueType(paramType))
        {
	        case ValueType.Bool:
		        *((byte*)paramPtr) = (byte)((bool)paramValue! ? 1 : 0);
		        return;
	        case ValueType.Char8:
	        case ValueType.Char16:
		        if (TypeUtils.IsUseAnsi(paramAttributes))
		        {
			        *((byte*)paramPtr) = (byte)((char)paramValue!);
		        }
		        else
		        {
			        *((short*)paramPtr) = (short)((char)paramValue!);
		        }
		        return;
	        case ValueType.Int8:
		        *((sbyte*)paramPtr) = (sbyte)paramValue!;
		        return;
	        case ValueType.Int16:
		        *((short*)paramPtr) = (short)paramValue!;
		        return;
	        case ValueType.Int32:
		        *((int*)paramPtr) = (int)paramValue!;
		        return;
	        case ValueType.Int64:
		        *((long*)paramPtr) = (long)paramValue!;
		        return;
	        case ValueType.UInt8:
		        *((byte*)paramPtr) = (byte)paramValue!;
		        return;
	        case ValueType.UInt16:
		        *((ushort*)paramPtr) = (ushort)paramValue!;
		        return;
	        case ValueType.UInt32:
		        *((uint*)paramPtr) = (uint)paramValue!;
		        return;
	        case ValueType.UInt64:
		        *((ulong*)paramPtr) = (ulong)paramValue!;
		        return;
	        case ValueType.Pointer:
		        *((nint*)paramPtr) = (nint)paramValue!;
		        return;
	        case ValueType.Float:
		        *((float*)paramPtr) = (float)paramValue!;
		        return;
	        case ValueType.Double:
		        *((double*)paramPtr) = (double)paramValue!;
		        return;
	        case ValueType.String:
		        if (paramValue is string str) NativeMethods.AssignString(paramPtr, str);
		        return;
	        case ValueType.ArrayBool:
		        if (paramValue is bool[] arrBool) NativeMethods.AssignVectorBool(paramPtr, arrBool, arrBool.Length);
		        return;
	        case ValueType.ArrayChar8:
	        case ValueType.ArrayChar16:
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
		        return;
	        case ValueType.ArrayInt8:
		        if (paramValue is sbyte[] arrInt8) NativeMethods.AssignVectorInt8(paramPtr, arrInt8, arrInt8.Length);
		        return;
	        case ValueType.ArrayInt16:
		        if (paramValue is short[] arrInt16) NativeMethods.AssignVectorInt16(paramPtr, arrInt16, arrInt16.Length);
		        return;
	        case ValueType.ArrayInt32:
		        if (paramValue is int[] arrInt32) NativeMethods.AssignVectorInt32(paramPtr, arrInt32, arrInt32.Length);
		        return;
	        case ValueType.ArrayInt64:
		        if (paramValue is long[] arrInt64) NativeMethods.AssignVectorInt64(paramPtr, arrInt64, arrInt64.Length);
		        return;
	        case ValueType.ArrayUInt8:
		        if (paramValue is byte[] arrUInt8) NativeMethods.AssignVectorUInt8(paramPtr, arrUInt8, arrUInt8.Length);
		        return;
	        case ValueType.ArrayUInt16:
		        if (paramValue is ushort[] arrUInt16) NativeMethods.AssignVectorUInt16(paramPtr, arrUInt16, arrUInt16.Length);
		        return;
	        case ValueType.ArrayUInt32:
		        if (paramValue is uint[] arrUInt32) NativeMethods.AssignVectorUInt32(paramPtr, arrUInt32, arrUInt32.Length);
		        return;
	        case ValueType.ArrayUInt64:
		        if (paramValue is ulong[] arrUInt64) NativeMethods.AssignVectorUInt64(paramPtr, arrUInt64, arrUInt64.Length);
		        return;
	        case ValueType.ArrayPointer:
		        if (paramValue is nint[] arrIntPtr) NativeMethods.AssignVectorIntPtr(paramPtr, arrIntPtr, arrIntPtr.Length);
		        return;
	        case ValueType.ArrayFloat:
		        if (paramValue is float[] arrFloat) NativeMethods.AssignVectorFloat(paramPtr, arrFloat, arrFloat.Length);
		        return;
	        case ValueType.ArrayDouble:
		        if (paramValue is double[] arrDouble) NativeMethods.AssignVectorDouble(paramPtr, arrDouble, arrDouble.Length);
		        return;
	        case ValueType.ArrayString:
		        if (paramValue is string[] arrString) NativeMethods.AssignVectorString(paramPtr, arrString, arrString.Length);
		        return;
	        case ValueType.Vector2:
	        case ValueType.Vector3:
	        case ValueType.Vector4:
	        case ValueType.Matrix4x4:
		        Marshal.StructureToPtr(paramValue!, paramPtr, false);
		        return;
        }

        if (paramType.IsValueType)
        {
            Marshal.StructureToPtr(paramValue!, paramPtr, false);
            return;
        }

        throw new NotImplementedException($"Parameter type {paramType.Name} not implemented");
    }

    private static unsafe object Extract(Type paramType, object[] paramAttributes, nint paramAddress)
    {
	    object? LoadFromAddress(nint paramPtr, ValueType valueType)
	    {
		    switch (valueType)
		    {
			    case ValueType.Function:
				    return GetDelegateForFunctionPointer(paramPtr, paramType);
			    case ValueType.String:
				    return NativeMethods.GetStringData(paramPtr);
			    case ValueType.ArrayBool:
				    var arrBool = new bool[NativeMethods.GetVectorSizeBool(paramPtr)];
				    NativeMethods.GetVectorDataBool(paramPtr, arrBool);
				    return arrBool;
			    case ValueType.ArrayChar8:
			    case ValueType.ArrayChar16:
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
			    case ValueType.ArrayInt8:
				    var arrInt8 = new sbyte[NativeMethods.GetVectorSizeInt8(paramPtr)];
				    NativeMethods.GetVectorDataInt8(paramPtr, arrInt8);
				    return arrInt8;
			    case ValueType.ArrayInt16:
				    var arrInt16 = new short[NativeMethods.GetVectorSizeInt16(paramPtr)];
				    NativeMethods.GetVectorDataInt16(paramPtr, arrInt16);
				    return arrInt16;
			    case ValueType.ArrayInt32:
				    var arrInt32 = new int[NativeMethods.GetVectorSizeInt32(paramPtr)];
				    NativeMethods.GetVectorDataInt32(paramPtr, arrInt32);
				    return arrInt32;
			    case ValueType.ArrayInt64:
				    var arrInt64 = new long[NativeMethods.GetVectorSizeInt64(paramPtr)];
				    NativeMethods.GetVectorDataInt64(paramPtr, arrInt64);
				    return arrInt64;
			    case ValueType.ArrayUInt8:
				    var arrUInt8 = new byte[NativeMethods.GetVectorSizeUInt8(paramPtr)];
				    NativeMethods.GetVectorDataUInt8(paramPtr, arrUInt8);
				    return arrUInt8;
			    case ValueType.ArrayUInt16:
				    var arrUInt16 = new ushort[NativeMethods.GetVectorSizeUInt16(paramPtr)];
				    NativeMethods.GetVectorDataUInt16(paramPtr, arrUInt16);
				    return arrUInt16;
			    case ValueType.ArrayUInt32:
				    var arrUInt32 = new uint[NativeMethods.GetVectorSizeUInt32(paramPtr)];
				    NativeMethods.GetVectorDataUInt32(paramPtr, arrUInt32);
				    return arrUInt32;
			    case ValueType.ArrayUInt64:
				    var arrUInt64 = new ulong[NativeMethods.GetVectorSizeUInt64(paramPtr)];
				    NativeMethods.GetVectorDataUInt64(paramPtr, arrUInt64);
				    return arrUInt64;
			    case ValueType.ArrayPointer:
				    var arrIntPtr = new nint[NativeMethods.GetVectorSizeIntPtr(paramPtr)];
				    NativeMethods.GetVectorDataIntPtr(paramPtr, arrIntPtr);
				    return arrIntPtr;
			    case ValueType.ArrayFloat:
				    var arrFloat = new float[NativeMethods.GetVectorSizeFloat(paramPtr)];
				    NativeMethods.GetVectorDataFloat(paramPtr, arrFloat);
				    return arrFloat;
			    case ValueType.ArrayDouble:
				    var arrDouble = new double[NativeMethods.GetVectorSizeDouble(paramPtr)];
				    NativeMethods.GetVectorDataDouble(paramPtr, arrDouble);
				    return arrDouble;
			    case ValueType.ArrayString:
				    var arrString = new string[NativeMethods.GetVectorSizeString(paramPtr)];
				    NativeMethods.GetVectorDataString(paramPtr, arrString);
				    return arrString;
			    case ValueType.Vector2:
			    case ValueType.Vector3:
			    case ValueType.Vector4:
			    case ValueType.Matrix4x4:
				    return Marshal.PtrToStructure(paramPtr, paramType) ?? throw new InvalidOperationException("Invalid structure!");
			    default:
				    return null;
		    }
	    }

	    if (paramType.IsByRef)
	    {
		    paramType = TypeUtils.ConvertToUnrefType(paramType);
		    ValueType valueType = TypeUtils.ConvertToValueType(paramType);
		    
		    nint paramPtr = *(nint*)paramAddress;
		    
		    switch (valueType)
		    {
			    case ValueType.Bool:
				    return *(byte*)paramPtr == 1;
			    case ValueType.Char8:
			    case ValueType.Char16:
				    if (TypeUtils.IsUseAnsi(paramAttributes))
				    {
					    return (char)(*(byte*)paramPtr);
				    }
				    else
				    {
					    return (char)(*(short*)paramPtr);
				    }
			    case ValueType.Int8:
				    return *(sbyte*)paramPtr;
			    case ValueType.Int16:
				    return *(short*)paramPtr;
			    case ValueType.Int32:
				    return *(int*)paramPtr;
			    case ValueType.Int64:
				    return *(long*)paramPtr;
			    case ValueType.UInt8:
				    return *(byte*)paramPtr;
			    case ValueType.UInt16:
				    return *(ushort*)paramPtr;
			    case ValueType.UInt32:
				    return *(uint*)paramPtr;
			    case ValueType.UInt64:
				    return *(ulong*)paramPtr;
			    case ValueType.Pointer:
				    return *(nint*)paramPtr;
			    case ValueType.Float:
				    return *(float*)paramPtr;
			    case ValueType.Double:
				    return *(double*)paramPtr;
			    default:
				    var result = LoadFromAddress(paramPtr, valueType);
				    if (result != null)
					    return result;
				    break;
		    }
		    
		    if (paramType.IsValueType)
		    {
			    return Marshal.PtrToStructure(paramPtr, paramType) ?? throw new InvalidOperationException("Invalid structure!");
		    }
	    }
	    else
	    {
		    ValueType valueType = TypeUtils.ConvertToValueType(paramType);
		    switch (valueType)
		    {
			    case ValueType.Bool:
				    return *(byte*)paramAddress == 1;
			    case ValueType.Char8:
			    case ValueType.Char16:
				    if (TypeUtils.IsUseAnsi(paramAttributes))
				    {
					    return (char)(*(byte*)paramAddress);
				    }
				    else
				    {
					    return (char)(*(short*)paramAddress);
				    }
			    case ValueType.Int8:
				    return *(sbyte*)paramAddress;
			    case ValueType.Int16:
				    return *(short*)paramAddress;
			    case ValueType.Int32:
				    return *(int*)paramAddress;
			    case ValueType.Int64:
				    return *(long*)paramAddress;
			    case ValueType.UInt8:
				    return *(byte*)paramAddress;
			    case ValueType.UInt16:
				    return *(ushort*)paramAddress;
			    case ValueType.UInt32:
				    return *(uint*)paramAddress;
			    case ValueType.UInt64:
				    return *(ulong*)paramAddress;
			    case ValueType.Pointer:
				    return *(nint*)paramAddress;
			    case ValueType.Float:
				    return *(float*)paramAddress;
			    case ValueType.Double:
				    return *(double*)paramAddress;
			    default:
				    nint paramPtr = *(nint*)paramAddress;
				    var result = LoadFromAddress(paramPtr, valueType);
				    if (result != null)
					    return result;
				    break;
		    }
		    
		    if (paramType.IsValueType)
		    {
			    nint paramPtr = *(nint*)paramAddress;
			    return Marshal.PtrToStructure(paramPtr, paramType) ?? throw new InvalidOperationException("Invalid structure!");
		    }
	    }
	    
	    throw new NotImplementedException($"Parameter type {paramType.Name} not implemented");
    }
    
    public static Delegate GetDelegateForFunctionPointer(nint funcAddress, Type delegateType)
    {
	    MethodInfo methodInfo = delegateType.GetMethod("Invoke") ?? throw new InvalidOperationException("Invalid delegate!");
	    bool NeedMarshal(Type paramType)
	    {
		    if (paramType.IsByRef)
		    {
			    paramType = TypeUtils.ConvertToUnrefType(paramType);
		    }
		    ValueType valueType = TypeUtils.ConvertToValueType(paramType);
		    return valueType is >= ValueType.String and <= ValueType.ArrayString;
	    }

	    if (NeedMarshal(methodInfo.ReturnType) || methodInfo.GetParameters().Any(p => NeedMarshal(p.ParameterType)))
	    {
		    return MethodUtils.CreateObjectArrayDelegate(delegateType, DynamicInvoke(funcAddress, methodInfo));
	    }
	    else
	    {
		    return Marshal.GetDelegateForFunctionPointer(funcAddress, delegateType);
	    }
    }
    
    private static Func<object[], object> DynamicInvoke(nint funcAddress, MethodInfo methodInfo)
    {
	    ManagedType returnType =  new ManagedType(methodInfo.ReturnType, methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(MarshalAsAttribute), false));
	    ManagedType[] parameterTypes = methodInfo.GetParameters().Select(p => new ManagedType(p.ParameterType, p.GetCustomAttributes(typeof(MarshalAsAttribute), false))).ToArray();
	    
        return parameters =>
        {
	        lock (vm)
	        {
		        vm.Reset();

		        bool hasRet = true;
		        bool hasRefs = false;

		        // Store parameters

		        List<(nint, ValueType)> handlers = [];

		        ValueType type = returnType.ValueType;
		        switch (type)
		        {
			        case ValueType.String:
			        {
				        nint ptr = NativeMethods.AllocateString();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayBool:
			        {
				        nint ptr = NativeMethods.AllocateVectorBool();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayChar8:
			        {
				        nint ptr = NativeMethods.AllocateVectorChar8();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayChar16:
			        {
				        nint ptr = NativeMethods.AllocateVectorChar16();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayInt8:
			        {
				        nint ptr = NativeMethods.AllocateVectorInt8();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayInt16:
			        {
				        nint ptr = NativeMethods.AllocateVectorInt16();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayInt32:
			        {
				        nint ptr = NativeMethods.AllocateVectorInt32();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayInt64:
			        {
				        nint ptr = NativeMethods.AllocateVectorInt64();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayUInt8:
			        {
				        nint ptr = NativeMethods.AllocateVectorUInt8();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayUInt16:
			        {
				        nint ptr = NativeMethods.AllocateVectorUInt16();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayUInt32:
			        {
				        nint ptr = NativeMethods.AllocateVectorUInt32();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayUInt64:
			        {
				        nint ptr = NativeMethods.AllocateVectorUInt64();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayPointer:
			        {
				        nint ptr = NativeMethods.AllocateVectorIntPtr();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayFloat:
			        {
				        nint ptr = NativeMethods.AllocateVectorFloat();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayDouble:
			        {
				        nint ptr = NativeMethods.AllocateVectorDouble();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        case ValueType.ArrayString:
			        {
				        nint ptr = NativeMethods.AllocateVectorString();
				        handlers.Add((ptr, type));
				        vm.ArgPointer(ptr);
				        break;
			        }
			        default:
				        // Should not require storage
				        hasRet = false;
				        break;
		        }

		        List<GCHandle> pins = [];

		        for (int i = 0; i < parameters.Length; i++)
		        {
			        object paramValue = parameters[i];
			        ManagedType paramType = parameterTypes[i];
			        ValueType valueType = paramType.ValueType;

			        if (paramType.IsByRef)
			        {
				        switch (valueType)
				        {
					        case ValueType.Invalid:
					        case ValueType.Void:
						        break;
					        case ValueType.Bool:
						        vm.ArgPointer(Pin<bool>(paramValue, pins));
						        break;
					        case ValueType.Char8:
						        vm.ArgPointer(Pin<char>(paramValue, pins));
						        break;
					        case ValueType.Char16:
						        vm.ArgPointer(Pin<char>(paramValue, pins));
						        break;
					        case ValueType.Int8:
						        vm.ArgPointer(Pin<sbyte>(paramValue, pins));
						        break;
					        case ValueType.Int16:
						        vm.ArgPointer(Pin<short>(paramValue, pins));
						        break;
					        case ValueType.Int32:
						        vm.ArgPointer(Pin<int>(paramValue, pins));
						        break;
					        case ValueType.Int64:
						        vm.ArgPointer(Pin<long>(paramValue, pins));
						        break;
					        case ValueType.UInt8:
						        vm.ArgPointer(Pin<byte>(paramValue, pins));
						        break;
					        case ValueType.UInt16:
						        vm.ArgPointer(Pin<ushort>(paramValue, pins));
						        break;
					        case ValueType.UInt32:
						        vm.ArgPointer(Pin<ulong>(paramValue, pins));
						        break;
					        case ValueType.UInt64:
						        vm.ArgPointer(Pin<ulong>(paramValue, pins));
						        break;
					        case ValueType.Pointer:
						        vm.ArgPointer(Pin<nint>(paramValue, pins));
						        break;
					        case ValueType.Float:
						        vm.ArgPointer(Pin<float>(paramValue, pins));
						        break;
					        case ValueType.Double:
						        vm.ArgPointer(Pin<double>(paramValue, pins));
						        break;
					        case ValueType.Vector2:
						        vm.ArgPointer(Pin<Vector2>(paramValue, pins));
						        break;
					        case ValueType.Vector3:
						        vm.ArgPointer(Pin<Vector3>(paramValue, pins));
						        break;
					        case ValueType.Vector4:
						        vm.ArgPointer(Pin<Vector4>(paramValue, pins));
						        break;
					        case ValueType.Matrix4x4:
						        vm.ArgPointer(Pin<Matrix4x4>(paramValue, pins));
						        break;
					        /*case ValueType.Function:
						        vm.ArgPointer(g_monolm.MonoDelegateToArg(p->GetArgument<MonoDelegate*>(i), *param.prototype));
						         break;*/
					        case ValueType.String:
					        {
						        nint ptr = NativeMethods.CreateString((string)paramValue);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayBool:
					        {
						        var arr = (bool[])paramValue;
						        nint ptr = NativeMethods.CreateVectorBool(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayChar8:
					        {
						        var arr = (char[])paramValue;
						        nint ptr = NativeMethods.CreateVectorChar8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayChar16:
					        {
						        var arr = (char[])paramValue;
						        nint ptr = NativeMethods.CreateVectorChar16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt8:
					        {
						        var arr = (sbyte[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt16:
					        {
						        var arr = (short[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt32:
					        {
						        var arr = (int[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt32(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt64:
					        {
						        var arr = (long[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt64(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt8:
					        {
						        var arr = (byte[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt16:
					        {
						        var arr = (ushort[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt32:
					        {
						        var arr = (uint[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt32(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt64:
					        {
						        var arr = (ulong[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt64(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayPointer:
					        {
						        var arr = (nint[])paramValue;
						        nint ptr = NativeMethods.CreateVectorIntPtr(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayFloat:
					        {
						        var arr = (float[])paramValue;
						        nint ptr = NativeMethods.CreateVectorFloat(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayDouble:
					        {
						        var arr = (double[])paramValue;
						        nint ptr = NativeMethods.CreateVectorDouble(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayString:
					        {
						        var arr = (string[])paramValue;
						        nint ptr = NativeMethods.CreateVectorString(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        default:
						        throw new ArgumentOutOfRangeException();
				        }

				        hasRefs = true;
			        }
			        else
			        {
				        switch (valueType)
				        {
					        case ValueType.Invalid:
					        case ValueType.Void:
						        break;
					        case ValueType.Bool:
						        vm.ArgBool((bool)paramValue);
						        break;
					        case ValueType.Char8:
						        vm.ArgChar8((char)paramValue);
						        break;
					        case ValueType.Char16:
						        vm.ArgChar16((char)paramValue);
						        break;
					        case ValueType.Int8:
						        vm.ArgInt8((sbyte)paramValue);
						        break;
					        case ValueType.Int16:
						        vm.ArgInt16((short)paramValue);
						        break;
					        case ValueType.Int32:
						        vm.ArgInt32((int)paramValue);
						        break;
					        case ValueType.Int64:
						        vm.ArgInt64((long)paramValue);
						        break;
					        case ValueType.UInt8:
						        vm.ArgUInt8((byte)paramValue);
						        break;
					        case ValueType.UInt16:
						        vm.ArgUInt16((ushort)paramValue);
						        break;
					        case ValueType.UInt32:
						        vm.ArgUInt32((uint)paramValue);
						        break;
					        case ValueType.UInt64:
						        vm.ArgUInt64((ulong)paramValue);
						        break;
					        case ValueType.Pointer:
						        vm.ArgPointer((nint)paramValue);
						        break;
					        case ValueType.Float:
						        vm.ArgFloat((float)paramValue);
						        break;
					        case ValueType.Double:
						        vm.ArgDouble((double)paramValue);
						        break;
					        case ValueType.Vector2:
						        vm.ArgPointer(Pin<Vector2>(paramValue, pins));
						        break;
					        case ValueType.Vector3:
						        vm.ArgPointer(Pin<Vector3>(paramValue, pins));
						        break;
					        case ValueType.Vector4:
						        vm.ArgPointer(Pin<Vector4>(paramValue, pins));
						        break;
					        case ValueType.Matrix4x4:
						        vm.ArgPointer(Pin<Matrix4x4>(paramValue, pins));
						        break;
					        /*case ValueType.Function:
						         vm.ArgPointer(g_monolm.MonoDelegateToArg(p->GetArgument<MonoDelegate*>(i), *param.prototype));
						         break;*/
					        case ValueType.String:
					        {
						        nint ptr = NativeMethods.CreateString((string)paramValue);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayBool:
					        {
						        var arr = (bool[])paramValue;
						        nint ptr = NativeMethods.CreateVectorBool(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayChar8:
					        {
						        var arr = (char[])paramValue;
						        nint ptr = NativeMethods.CreateVectorChar8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayChar16:
					        {
						        var arr = (char[])paramValue;
						        nint ptr = NativeMethods.CreateVectorChar16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt8:
					        {
						        var arr = (sbyte[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt16:
					        {
						        var arr = (short[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt32:
					        {
						        var arr = (int[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt32(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayInt64:
					        {
						        var arr = (long[])paramValue;
						        nint ptr = NativeMethods.CreateVectorInt64(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt8:
					        {
						        var arr = (byte[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt8(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt16:
					        {
						        var arr = (ushort[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt16(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt32:
					        {
						        var arr = (uint[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt32(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayUInt64:
					        {
						        var arr = (ulong[])paramValue;
						        nint ptr = NativeMethods.CreateVectorUInt64(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayPointer:
					        {
						        var arr = (nint[])paramValue;
						        nint ptr = NativeMethods.CreateVectorIntPtr(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayFloat:
					        {
						        var arr = (float[])paramValue;
						        nint ptr = NativeMethods.CreateVectorFloat(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayDouble:
					        {
						        var arr = (double[])paramValue;
						        nint ptr = NativeMethods.CreateVectorDouble(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        case ValueType.ArrayString:
					        {
						        var arr = (string[])paramValue;
						        nint ptr = NativeMethods.CreateVectorString(arr, arr.Length);
						        handlers.Add((ptr, valueType));
						        vm.ArgPointer(ptr);
						        break;
					        }
					        default:
						        throw new ArgumentOutOfRangeException();
				        }
			        }
		        }

		        object? ret = null;

		        switch (type)
		        {
			        case ValueType.Invalid:
				        break;
			        case ValueType.Void:
				        vm.CallVoid(funcAddress);
				        break;
			        case ValueType.Bool:
				        ret = vm.CallBool(funcAddress);
				        break;
			        case ValueType.Char8:
				        ret = vm.CallChar8(funcAddress);
				        break;
			        case ValueType.Char16:
				        ret = vm.CallChar16(funcAddress);
				        break;
			        case ValueType.Int8:
				        ret = vm.CallInt8(funcAddress);
				        break;
			        case ValueType.Int16:
				        ret = vm.CallInt16(funcAddress);
				        break;
			        case ValueType.Int32:
				        ret = vm.CallInt32(funcAddress);
				        break;
			        case ValueType.Int64:
				        ret = vm.CallInt64(funcAddress);
				        break;
			        case ValueType.UInt8:
				        ret = vm.CallUInt8(funcAddress);
				        break;
			        case ValueType.UInt16:
				        ret = vm.CallUInt16(funcAddress);
				        break;
			        case ValueType.UInt32:
				        ret = vm.CallUInt32(funcAddress);
				        break;
			        case ValueType.UInt64:
				        ret = vm.CallUInt64(funcAddress);
				        break;
			        case ValueType.Pointer:
				        ret = vm.CallPointer(funcAddress);
				        break;
			        case ValueType.Float:
				        ret = vm.CallFloat(funcAddress);
				        break;
			        case ValueType.Double:
				        ret = vm.CallDouble(funcAddress);
				        break;
			        /*case ValueType.Function: {
				        void* val = vm.CallPointer(funcAddress);
				        ret->SetReturnPtr(g_monolm.CreateDelegate(val, *method->retType.prototype));
				        break;
			        }*/
			        case ValueType.Vector2:
			        {
				        DCaggr ag = new DCaggr(2, 8);
				        for (int i = 0; i < 2; ++i)
					        ag.AddField(SignatureChars.Float, 4 * i, 1);
				        ag.Reset();
				        Vector2 source;
				        unsafe
				        {
					        vm.CallAggr(funcAddress, ag, &source);
				        }

				        ret = source;
				        break;
			        }
			        case ValueType.Vector3:
			        {
				        DCaggr ag = new DCaggr(3, 12);
				        for (int i = 0; i < 3; ++i)
					        ag.AddField(SignatureChars.Float, 4 * i, 1);
				        ag.Reset();
				        Vector3 source;
				        unsafe
				        {
					        vm.CallAggr(funcAddress, ag, &source);
				        }

				        ret = source;
				        break;
			        }
			        case ValueType.Vector4:
			        {
				        DCaggr ag = new DCaggr(4, 16);
				        for (int i = 0; i < 4; ++i)
					        ag.AddField(SignatureChars.Float, 4 * i, 1);
				        ag.Reset();
				        Vector4 source;
				        unsafe
				        {
					        vm.CallAggr(funcAddress, ag, &source);
				        }

				        ret = source;
				        break;
			        }
			        case ValueType.Matrix4x4:
			        {
				        DCaggr ag = new DCaggr(16, 64);
				        for (int i = 0; i < 16; ++i)
					        ag.AddField(SignatureChars.Float, 4 * i, 1);
				        ag.Reset();
				        Matrix4x4 source;
				        unsafe
				        {
					        vm.CallAggr(funcAddress, ag, &source);
				        }

				        ret = source;
				        break;
			        }
			        case ValueType.String:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        ret = NativeMethods.GetStringData(ptr);
				        break;
			        }
			        case ValueType.ArrayBool:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new bool[NativeMethods.GetVectorSizeBool(ptr)];
				        NativeMethods.GetVectorDataBool(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayChar8:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new char[NativeMethods.GetVectorSizeChar8(ptr)];
				        NativeMethods.GetVectorDataChar8(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayChar16:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new char[NativeMethods.GetVectorSizeChar16(ptr)];
				        NativeMethods.GetVectorDataChar16(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayInt8:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new sbyte[NativeMethods.GetVectorSizeInt8(ptr)];
				        NativeMethods.GetVectorDataInt8(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayInt16:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new short[NativeMethods.GetVectorSizeInt16(ptr)];
				        NativeMethods.GetVectorDataInt16(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayInt32:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new int[NativeMethods.GetVectorSizeInt32(ptr)];
				        NativeMethods.GetVectorDataInt32(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayInt64:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new long[NativeMethods.GetVectorSizeInt64(ptr)];
				        NativeMethods.GetVectorDataInt64(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayUInt8:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new byte[NativeMethods.GetVectorSizeUInt8(ptr)];
				        NativeMethods.GetVectorDataUInt8(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayUInt16:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new ushort[NativeMethods.GetVectorSizeUInt16(ptr)];
				        NativeMethods.GetVectorDataUInt16(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayUInt32:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new uint[NativeMethods.GetVectorSizeUInt32(ptr)];
				        NativeMethods.GetVectorDataUInt32(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayUInt64:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new ulong[NativeMethods.GetVectorSizeUInt64(ptr)];
				        NativeMethods.GetVectorDataUInt64(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayPointer:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new nint[NativeMethods.GetVectorSizeIntPtr(ptr)];
				        NativeMethods.GetVectorDataIntPtr(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayFloat:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new float[NativeMethods.GetVectorSizeFloat(ptr)];
				        NativeMethods.GetVectorDataFloat(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayDouble:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new double[NativeMethods.GetVectorSizeDouble(ptr)];
				        NativeMethods.GetVectorDataDouble(ptr, arr);
				        ret = arr;
				        break;
			        }
			        case ValueType.ArrayString:
			        {
				        vm.CallVoid(funcAddress);
				        nint ptr = handlers[0].Item1;
				        var arr = new string[NativeMethods.GetVectorSizeString(ptr)];
				        NativeMethods.GetVectorDataString(ptr, arr);
				        ret = arr;
				        break;
			        }
			        default:
				        throw new ArgumentOutOfRangeException();
		        }

		        if (hasRefs)
		        {
			        int j = hasRet ? 1 : 0; // skip first param if has return

			        if (j < handlers.Count)
			        {
				        for (int i = 0; i < parameters.Length; i++)
				        {
					        //object paramValue = parameters[i];
					        ManagedType paramType = parameterTypes[i];
					        if (paramType.IsByRef)
					        {
						        switch (paramType.ValueType)
						        {
							        case ValueType.String:
							        {
								        nint ptr = handlers[j++].Item1;
								        parameters[i] = NativeMethods.GetStringData(ptr);
								        break;
							        }
							        case ValueType.ArrayBool:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new bool[NativeMethods.GetVectorSizeBool(ptr)];
								        NativeMethods.GetVectorDataBool(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayChar8:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new char[NativeMethods.GetVectorSizeChar8(ptr)];
								        NativeMethods.GetVectorDataChar8(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayChar16:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new char[NativeMethods.GetVectorSizeChar16(ptr)];
								        NativeMethods.GetVectorDataChar16(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayInt8:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new sbyte[NativeMethods.GetVectorSizeInt8(ptr)];
								        NativeMethods.GetVectorDataInt8(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayInt16:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new short[NativeMethods.GetVectorSizeInt16(ptr)];
								        NativeMethods.GetVectorDataInt16(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayInt32:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new int[NativeMethods.GetVectorSizeInt32(ptr)];
								        NativeMethods.GetVectorDataInt32(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayInt64:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new long[NativeMethods.GetVectorSizeInt64(ptr)];
								        NativeMethods.GetVectorDataInt64(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayUInt8:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new byte[NativeMethods.GetVectorSizeUInt8(ptr)];
								        NativeMethods.GetVectorDataUInt8(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayUInt16:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new ushort[NativeMethods.GetVectorSizeUInt16(ptr)];
								        NativeMethods.GetVectorDataUInt16(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayUInt32:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new uint[NativeMethods.GetVectorSizeUInt32(ptr)];
								        NativeMethods.GetVectorDataUInt32(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayUInt64:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new ulong[NativeMethods.GetVectorSizeUInt64(ptr)];
								        NativeMethods.GetVectorDataUInt64(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayPointer:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new nint[NativeMethods.GetVectorSizeIntPtr(ptr)];
								        NativeMethods.GetVectorDataIntPtr(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayFloat:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new float[NativeMethods.GetVectorSizeFloat(ptr)];
								        NativeMethods.GetVectorDataFloat(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayDouble:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new double[NativeMethods.GetVectorSizeDouble(ptr)];
								        NativeMethods.GetVectorDataDouble(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
							        case ValueType.ArrayString:
							        {
								        nint ptr = handlers[j++].Item1;
								        var arr = new string[NativeMethods.GetVectorSizeString(ptr)];
								        NativeMethods.GetVectorDataString(ptr, arr);
								        parameters[i] = arr;
								        break;
							        }
						        }
					        }

					        if (j >= handlers.Count)
						        break;
				        }
			        }
		        }

		        if (hasRet)
		        {
			        DeleteReturn(handlers);
		        }

		        DeleteParams(handlers, hasRet);

		        foreach (var pin in pins)
		        {
			        pin.Free();
		        }

		        return ret!;
	        }
        };
    }

    private static nint Pin<T>(object paramValue, List<GCHandle> pins)
    {
        var handle = GCHandle.Alloc((T)paramValue, GCHandleType.Pinned);
        pins.Add(handle);
        return handle.AddrOfPinnedObject();
    }

    private static void DeleteReturn(List<(nint, ValueType)> handlers)
    {
        var (ptr, type) = handlers[0];
        switch (type)
        {
            case ValueType.String:
                NativeMethods.FreeString(ptr);
                break;
            case ValueType.ArrayBool:
                NativeMethods.FreeVectorBool(ptr);
                break;
            case ValueType.ArrayChar8:
                NativeMethods.FreeVectorChar8(ptr);
                break;
            case ValueType.ArrayChar16:
                NativeMethods.FreeVectorChar16(ptr);
                break;
            case ValueType.ArrayInt8:
                NativeMethods.FreeVectorInt8(ptr);
                break;
            case ValueType.ArrayInt16:
                NativeMethods.FreeVectorInt16(ptr);
                break;
            case ValueType.ArrayInt32:
                NativeMethods.FreeVectorInt32(ptr);
                break;
            case ValueType.ArrayInt64:
                NativeMethods.FreeVectorInt64(ptr);
                break;
            case ValueType.ArrayUInt8:
                NativeMethods.FreeVectorUInt8(ptr);
                break;
            case ValueType.ArrayUInt16:
                NativeMethods.FreeVectorUInt16(ptr);
                break;
            case ValueType.ArrayUInt32:
                NativeMethods.FreeVectorUInt32(ptr);
                break;
            case ValueType.ArrayUInt64:
                NativeMethods.FreeVectorUInt64(ptr);
                break;
            case ValueType.ArrayPointer:
                NativeMethods.FreeVectorIntPtr(ptr);
                break;
            case ValueType.ArrayFloat:
                NativeMethods.FreeVectorFloat(ptr);
                break;
            case ValueType.ArrayDouble:
                NativeMethods.FreeVectorDouble(ptr);
                break;
            case ValueType.ArrayString:
                NativeMethods.FreeVectorString(ptr);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private static void DeleteParams(List<(nint, ValueType)> handlers, bool hasRet)
    {
	    int j = hasRet ? 1 : 0;
        for (int i = j; i < handlers.Count; i++)
        {
            var (ptr, type) = handlers[i];
            switch (type)
            {
                case ValueType.String:
                    NativeMethods.DeleteString(ptr);
                    break;
                case ValueType.ArrayBool:
                    NativeMethods.DeleteVectorBool(ptr);
                    break;
                case ValueType.ArrayChar8:
                    NativeMethods.DeleteVectorChar8(ptr);
                    break;
                case ValueType.ArrayChar16:
                    NativeMethods.DeleteVectorChar16(ptr);
                    break;
                case ValueType.ArrayInt8:
                    NativeMethods.DeleteVectorInt8(ptr);
                    break;
                case ValueType.ArrayInt16:
                    NativeMethods.DeleteVectorInt16(ptr);
                    break;
                case ValueType.ArrayInt32:
                    NativeMethods.DeleteVectorInt32(ptr);
                    break;
                case ValueType.ArrayInt64:
                    NativeMethods.DeleteVectorInt64(ptr);
                    break;
                case ValueType.ArrayUInt8:
                    NativeMethods.DeleteVectorUInt8(ptr);
                    break;
                case ValueType.ArrayUInt16:
                    NativeMethods.DeleteVectorUInt16(ptr);
                    break;
                case ValueType.ArrayUInt32:
                    NativeMethods.DeleteVectorUInt32(ptr);
                    break;
                case ValueType.ArrayUInt64:
                    NativeMethods.DeleteVectorUInt64(ptr);
                    break;
                case ValueType.ArrayPointer:
                    NativeMethods.DeleteVectorIntPtr(ptr);
                    break;
                case ValueType.ArrayFloat:
                    NativeMethods.DeleteVectorFloat(ptr);
                    break;
                case ValueType.ArrayDouble:
                    NativeMethods.DeleteVectorDouble(ptr);
                    break;
                case ValueType.ArrayString:
                    NativeMethods.DeleteVectorString(ptr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

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
