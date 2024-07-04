using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Plugify;

public delegate void InvokeMethodDelegate(Guid managedMethodGuid, Guid thisObjectGuid, nint paramsPtr, nint outPtr);

internal static class NativeInterop
{
	//public static readonly int ApiVersion = 1;
    private static readonly InvokeMethodDelegate InvokeMethodDelegate = Marshalling.InvokeMethod;

    internal enum AssemblyLoadStatus
    {
        Success, FileNotFound, FileLoadFailure, InvalidFilePath, InvalidAssembly, UnknownError
    }
    
    private static readonly Dictionary<Type, AssemblyLoadStatus> AssemblyLoadErrorLookup = new();
    
    static NativeInterop()
    {
        AssemblyLoadErrorLookup.Add(typeof(BadImageFormatException), AssemblyLoadStatus.InvalidAssembly);
        AssemblyLoadErrorLookup.Add(typeof(FileNotFoundException), AssemblyLoadStatus.FileNotFound);
        AssemblyLoadErrorLookup.Add(typeof(FileLoadException), AssemblyLoadStatus.FileLoadFailure);
        AssemblyLoadErrorLookup.Add(typeof(ArgumentNullException), AssemblyLoadStatus.InvalidFilePath);
        AssemblyLoadErrorLookup.Add(typeof(ArgumentException), AssemblyLoadStatus.InvalidFilePath);
    }
    
    [UnmanagedCallersOnly]
    internal static AssemblyLoadStatus InitializeAssembly(NativeString assemblyPathString, nint outAssemblyGuid, nint classHolderPtr)
    {
        try
        {
            // Create a managed string from the pointer
            string? assemblyPath = assemblyPathString;
            if (assemblyPath == null)
            {
                return AssemblyLoadStatus.InvalidFilePath;
            }
        
            AssemblyInstance? assemblyInstance = AssemblyCache.Instance.Get(assemblyPath);
            if (assemblyInstance != null)
            {
                return AssemblyLoadStatus.Success;
            }

            Guid assemblyGuid = Guid.NewGuid();
            Marshal.StructureToPtr(assemblyGuid, outAssemblyGuid, false);

            Logger.Log(Severity.Info, "Loading assembly: {0}...", assemblyPath);

            assemblyInstance = AssemblyCache.Instance.Add(assemblyGuid, assemblyPath);
            Assembly? assembly = assemblyInstance.Assembly;

            if (assembly == null)
            {
                return AssemblyLoadStatus.InvalidAssembly;
            }

            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type is { IsClass: true, IsAbstract: false })
                {
                    InitManagedClass(assemblyGuid, classHolderPtr, type);
                }
            }
        
            NativeInterop_SetInvokeMethodFunction(ref assemblyGuid, classHolderPtr, Marshal.GetFunctionPointerForDelegate(InvokeMethodDelegate));

        }
        catch (Exception e)
        {
            var loadStatus = AssemblyLoadErrorLookup.GetValueOrDefault(e.GetType(), AssemblyLoadStatus.UnknownError);
            Logger.Log(Severity.Error, "Loading assembly error: {0}", e);
            return loadStatus;
        }
        
        return AssemblyLoadStatus.Success;
    }
    
    [UnmanagedCallersOnly]
    internal static Bool32 UnloadAssembly(nint assemblyGuidPtr)
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

                // TODO: Should I cleanup of it force unload ?
                ManagedMethodCache.Instance.RemoveMethodsForAssembly(assemblyGuid);
                ManagedObjectCache.Instance.RemoveObjectsForAssembly(assemblyGuid);

                AssemblyCache.Instance.Remove(assemblyGuid);
                
                TypeInterface.RemoveUnusedObjects();
            }
        }
        catch (Exception e)
        {
            Logger.Log(Severity.Error, "Error unloading assembly: {0}", e);

            result = false;
        }

        return result;
    }

    [UnmanagedCallersOnly]
    internal static unsafe void AddMethodToCache(nint assemblyGuidPtr, nint methodGuidPtr, nint methodInfoPtr)
    {
        Guid assemblyGuid = Marshal.PtrToStructure<Guid>(assemblyGuidPtr);
        Guid methodGuid = Marshal.PtrToStructure<Guid>(methodGuidPtr);

        ref MethodInfo methodInfo = ref Unsafe.AsRef<MethodInfo>(methodInfoPtr.ToPointer());

        ManagedMethodCache.Instance.AddMethod(assemblyGuid, methodGuid, methodInfo);
    }

    [UnmanagedCallersOnly]
    internal static unsafe void AddObjectToCache(nint assemblyGuidPtr, nint objectGuidPtr, nint objectPtr, nint outManagedObjectPtr)
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
        string typeName = type.FullName ?? type.Name;
        nint typeNamePtr = Marshal.StringToHGlobalAnsi(typeName);
     
        ManagedClass_Create(ref assemblyGuid, classHolderPtr, type.GetHashCode(), typeNamePtr, out ManagedClass managedClass);

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

    private static void FreeObject(ManagedObject obj)
    {
        if (!ManagedObjectCache.Instance.RemoveObject(obj.guid))
        {
            throw new Exception("Failed to remove object from cache: " + obj.guid);
        }
    }

    [DllImport(NativeMethods.DllName)]
    private static extern void ManagedClass_Create([In] ref Guid assemblyGuid, nint classHolderPtr, int typeHash, nint typeNamePtr, [Out] out ManagedClass result);

    [DllImport(NativeMethods.DllName)]
    private static extern void NativeInterop_SetInvokeMethodFunction([In] ref Guid assemblyGuid, nint classHolderPtr, nint invokeMethodPtr);
}
