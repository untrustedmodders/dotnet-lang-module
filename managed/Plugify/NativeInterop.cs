using System;
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

    private static void HandleParameters(nint paramsPtr, MethodInfo methodInfo, out object[] parameters)
    {
        int numParams = methodInfo.GetParameters().Length;

        if (numParams == 0 || paramsPtr == nint.Zero)
        {
            parameters = [];
            return;
        }

        parameters = new object[numParams];

        int paramsOffset = 0;

        for (int i = 0; i < numParams; i++)
        {
            Type paramType = methodInfo.GetParameters()[i].ParameterType;

            // params is stored as void**
            nint paramAddress = Marshal.ReadIntPtr(paramsPtr, paramsOffset);
            paramsOffset += nint.Size;

            if (paramType.IsPrimitive)
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                
                if (paramType == typeof(bool))
                {
                    parameters[i] = Marshal.ReadByte(ptr); 
                    continue;
                }
                
                if (paramType == typeof(char))
                {
                    parameters[i] = Marshal.ReadInt16(ptr); 
                    continue;
                }

                switch (Marshal.SizeOf(paramType))
                {
                    case 2: 
                        parameters[i] = Marshal.ReadInt16(ptr); 
                        break;
                    case 4: 
                        parameters[i] = Marshal.ReadInt32(ptr); 
                        break;
                    case 8: 
                        parameters[i] = Marshal.ReadInt64(ptr); 
                        break;
                    default:
                        throw new NotImplementedException("Parameter type not implemented");
                }
                
                continue;
            }
            
            if (paramType == typeof(string))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                parameters[i] = NativeMethods.GetStringData(ptr);

                continue;
            }

            if (paramType == typeof(bool[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new bool[NativeMethods.GetVectorSizeBool(ptr)];
                NativeMethods.GetVectorDataBool(ptr, array);
                
                parameters[i] = array;
                
                continue;
            }
            if (paramType == typeof(char[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new char[NativeMethods.GetVectorSizeChar16(ptr)];
                NativeMethods.GetVectorDataChar16(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(sbyte[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new sbyte[NativeMethods.GetVectorSizeInt8(ptr)];
                NativeMethods.GetVectorDataInt8(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(short[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new short[NativeMethods.GetVectorSizeInt16(ptr)];
                NativeMethods.GetVectorDataInt16(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(int[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new int[NativeMethods.GetVectorSizeInt32(ptr)];
                NativeMethods.GetVectorDataInt32(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(long[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new long[NativeMethods.GetVectorSizeInt64(ptr)];
                NativeMethods.GetVectorDataInt64(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(byte[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new byte[NativeMethods.GetVectorSizeUInt8(ptr)];
                NativeMethods.GetVectorDataUInt8(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(ushort[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new ushort[NativeMethods.GetVectorSizeUInt16(ptr)];
                NativeMethods.GetVectorDataUInt16(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(uint[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new uint[NativeMethods.GetVectorSizeUInt32(ptr)];
                NativeMethods.GetVectorDataUInt32(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(ulong[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new ulong[NativeMethods.GetVectorSizeUInt64(ptr)];
                NativeMethods.GetVectorDataUInt64(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(nint[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new nint[NativeMethods.GetVectorSizeIntPtr(ptr)];
                NativeMethods.GetVectorDataIntPtr(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(float[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new float[NativeMethods.GetVectorSizeFloat(ptr)];
                NativeMethods.GetVectorDataFloat(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(double[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new double[NativeMethods.GetVectorSizeDouble(ptr)];
                NativeMethods.GetVectorDataDouble(ptr, array);
                
                parameters[i] = array;
            }
            if (paramType == typeof(string[]))
            {
                nint ptr = Marshal.ReadIntPtr(paramAddress);
                var array = new string[NativeMethods.GetVectorSizeString(ptr)];
                NativeMethods.GetVectorDataString(ptr, array);
                
                parameters[i] = array;
            }
            
            if (paramType.IsValueType)
            {
                parameters[i] = Marshal.PtrToStructure(paramAddress, paramType);

                continue;
            }
            
            if (paramType.IsPointer)
            {
                throw new NotImplementedException("Pointer parameter type not implemented");
            }

            if (paramType.IsByRef)
            {
                throw new NotImplementedException("ByRef parameter type not implemented");
            }

            throw new NotImplementedException("Parameter type not implemented");
        }
    }

    public static void InvokeMethod(Guid managedMethodGuid, Guid thisObjectGuid, nint paramsPtr, nint outPtr)
    {
        MethodInfo? methodInfo = ManagedMethodCache.Instance.GetMethod(managedMethodGuid);
        if (methodInfo == null)
        {
            throw new Exception("Failed to get method from GUID: " + managedMethodGuid);
        }
        
        Type returnType = methodInfo.ReturnType;
        //Type thisType = methodInfo.DeclaringType;

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

        
        
        // If returnType is an enum we need to get the underlying type and cast the value to it
        if (returnType.IsEnum)
        {
            returnType = Enum.GetUnderlyingType(returnType);
            returnValue = Convert.ChangeType(returnValue, returnType);
        }

        if (returnType == typeof(void))
        {
            // No need to fill out the outPtr
            return;
        }

        if (returnType.IsArray)
        {
            var elementType = returnType.GetElementType();
            switch (Type.GetTypeCode(elementType))
            {
                case TypeCode.Boolean:
                    if (returnValue is bool[] arrBoolean) NativeMethods.AssignVectorBool(outPtr, arrBoolean, arrBoolean.Length);
                    return;
                case TypeCode.Char:
                    if (returnValue is char[] arrChar) NativeMethods.AssignVectorChar16(outPtr, arrChar, arrChar.Length);
                    return;
                case TypeCode.SByte:
                    if (returnValue is sbyte[] arrSByte) NativeMethods.AssignVectorInt8(outPtr, arrSByte, arrSByte.Length);
                    return;
                case TypeCode.Int16:
                    if (returnValue is short[] arrInt16) NativeMethods.AssignVectorInt16(outPtr, arrInt16, arrInt16.Length);
                    return;
                case TypeCode.Int32:
                    if (returnValue is int[] arrInt32) NativeMethods.AssignVectorInt32(outPtr, arrInt32, arrInt32.Length);
                    return;
                case TypeCode.Int64:
                    if (returnValue is long[] arrInt64) NativeMethods.AssignVectorInt64(outPtr, arrInt64, arrInt64.Length);
                    return;
                case TypeCode.Byte:
                    if (returnValue is byte[] arrByte) NativeMethods.AssignVectorUInt8(outPtr, arrByte, arrByte.Length);
                    return;
                case TypeCode.UInt16:
                    if (returnValue is ushort[] arrUInt16) NativeMethods.AssignVectorUInt16(outPtr, arrUInt16, arrUInt16.Length);
                    return;
                case TypeCode.UInt32:
                    if (returnValue is uint[] arrUInt32) NativeMethods.AssignVectorUInt32(outPtr, arrUInt32, arrUInt32.Length);
                    return;
                case TypeCode.UInt64:
                    if (returnValue is ulong[] arrUInt64) NativeMethods.AssignVectorUInt64(outPtr, arrUInt64, arrUInt64.Length);
                    return;
                case TypeCode.Single:
                    if (returnValue is float[] arrFloat) NativeMethods.AssignVectorFloat(outPtr, arrFloat, arrFloat.Length);
                    return;
                case TypeCode.Double:
                    if (returnValue is double[] arrDouble) NativeMethods.AssignVectorDouble(outPtr, arrDouble, arrDouble.Length);
                    return;
                default:
                    if (elementType == typeof(nint))
                    {
                        if (returnValue is IntPtr[] arrIntPtr) NativeMethods.AssignVectorIntPtr(outPtr, arrIntPtr, arrIntPtr.Length);
                    }
                    else
                        throw new NotImplementedException($"Unknown array type {returnType.Name}");
                    return;
            }
        }

        switch (Type.GetTypeCode(returnType))
        {
            case TypeCode.Boolean:
                Marshal.WriteByte(outPtr, (byte)((bool)returnValue ? 1 : 0));
                return;
            case TypeCode.Char:
                Marshal.WriteInt16(outPtr, (short)((char)returnValue));
                return;
            case TypeCode.SByte:
                Marshal.WriteByte(outPtr, (byte)((sbyte)returnValue));
                return;
            case TypeCode.Int16:
                Marshal.WriteInt16(outPtr, (short)returnValue);
                return;
            case TypeCode.Int32:
                Marshal.WriteInt32(outPtr, (int)returnValue);
                return;
            case TypeCode.Int64:
                Marshal.WriteInt64(outPtr, (long)returnValue);
                return;
            case TypeCode.Byte:
                Marshal.WriteByte(outPtr, (byte)returnValue);
                return;
            case TypeCode.UInt16:
                Marshal.WriteInt16(outPtr, (short)((ushort)returnValue));
                return;
            case TypeCode.UInt32:
                Marshal.WriteInt32(outPtr, (int)((uint)returnValue));
                return;
            case TypeCode.UInt64:
                Marshal.WriteInt64(outPtr, (long)((ulong)returnValue));
                return;
            case TypeCode.Single:
                unsafe
                {
                    *((float*)outPtr) = (float)returnValue;
                }
                return;
            case TypeCode.Double:
                unsafe
                {
                    *((double*)outPtr) = (double)returnValue;
                }
                return;
            case TypeCode.String:
                if (returnValue is string str) NativeMethods.AssignString(outPtr, str);
                return;
        }
        
        if (returnType == typeof(nint))
        {
            Marshal.WriteIntPtr(outPtr, (nint)returnValue);
            return;
        }
        
        if (returnType.IsValueType)
        {
            Marshal.StructureToPtr(returnValue, outPtr, false);

            return;
        }

        throw new NotImplementedException($"Return type {returnType.Name} not implemented");
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
