using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;

namespace Plugify;

public static class NativeInterop
{
	public static readonly int ApiVersion = 1;
	
	[UnmanagedCallersOnly]
	public static void Initialize()
	{
	}
	
	[UnmanagedCallersOnly]
	public static void Shutdown()
	{
	}

	[UnmanagedCallersOnly]
	public static void LoadPlugin(nint assemblyPathStringPtr, nint outErrorStringPtr, nint outAssemblyGuid, nint exportMethodsPtr, nint outExportMethodPtr, int methodCount)
	{
        string? assemblyPath = Marshal.PtrToStringAnsi(assemblyPathStringPtr);
        if (assemblyPath == null)
        {
	        var message = "Assembly path is invalid";
	        byte[] bytes = Encoding.ASCII.GetBytes(message);
	        Marshal.Copy(bytes, 0, outErrorStringPtr, message.Length + 1);
	        return;
        }
        
        Guid assemblyGuid = Guid.NewGuid();
        Marshal.StructureToPtr(assemblyGuid, outAssemblyGuid, false);

        var assemblyInstance = AssemblyCache.Instance.Get(assemblyPath);
        if (assemblyInstance != null)
        {
	        var message = $"Assembly already loaded: '{assemblyPath}' ({assemblyInstance.Guid})"; 
	        byte[] bytes = Encoding.ASCII.GetBytes(message);
	        Marshal.Copy(bytes, 0, outErrorStringPtr, message.Length + 1);
	        return;
        }

        Logger.Log(Severity.Info, "Loading assembly: '{0}' ...", assemblyPath);

        assemblyInstance = AssemblyCache.Instance.Add(assemblyGuid, assemblyPath);
        
        Assembly? assembly = assemblyInstance.Assembly;
        if (assembly == null)
        {
	        var message = $"Failed to load assembly: '{assemblyPath}'";
	        byte[] bytes = Encoding.ASCII.GetBytes(message);
	        Marshal.Copy(bytes, 0, outErrorStringPtr, bytes.Length);
	        return;
        }
        
        Type[] types = assembly.GetExportedTypes();

        Type? pluginType = types.FirstOrDefault(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(Plugin)));
        if (pluginType == null)
        {
	        var message = "Unable to find plugin in assembly";
	        byte[] bytes = Encoding.ASCII.GetBytes(message);
	        Marshal.Copy(bytes, 0, outErrorStringPtr, bytes.Length);
	        return;
        }

        if (Attribute.GetCustomAttribute(pluginType, typeof(MinimumApiVersion)) is MinimumApiVersion versionAttribute && versionAttribute.Version > ApiVersion)
        {
	        var message = $"Requires a newer version of Plugify. The plugin expects version v{versionAttribute.Version} but the current version is v{ApiVersion}.";
	        byte[] bytes = Encoding.ASCII.GetBytes(message);
	        Marshal.Copy(bytes, 0, outErrorStringPtr, bytes.Length);
	        return;
        }

        // TODO: Export methods
        unsafe
        {
	        Method* exportMethods = (Method*)exportMethodsPtr.ToPointer();

	        for (int i = 0; i < methodCount; i++)
	        {
		        Method exportMethod = exportMethods[i];
		        string funcName = exportMethod.FunctionName;
		        Property returnType = exportMethod.ReturnType;
		        Property[] parameterTypes = exportMethod.ParameterTypes;
		        
		        string[] separated = funcName.Split('.');
		        if (separated.Length != 3) {
			        PrintError(outErrorStringPtr, $"Invalid function name: '{funcName}'. Please provide name in that format: 'Namespace.Class.Method'");
			        return;
		        }
		        
		        foreach (Type type in types.Where(t => t.IsClass && t.Namespace == separated[0] && t.Name == separated[1]))
		        {
			        BindingFlags bindingAttr = BindingFlags.Public;
			        bindingAttr |= (type == pluginType) ? BindingFlags.Instance : BindingFlags.Static;
			        
			        foreach (MethodInfo method in type.GetMethods(bindingAttr).Where(m => m is { IsPublic: true, IsConstructor: false } && m.Name == separated[2]))
			        {
				        var parameters = method.GetParameters();
				        
						if (exportMethod.ParamCount != parameters.Length) {
							PrintError(outErrorStringPtr, $"Invalid parameter count {parameters.Length} when it should have {exportMethod.ParamCount}");
							return;
						}
						
						ValueType retType = TypeMapper.MonoTypeToValueType(method.ReturnType.Name);

						if (retType == ValueType.Invalid) {
							if (method.ReturnType.IsClass) {
								retType = ValueType.Function;
							}
						}

						if (retType == ValueType.Invalid) {
							PrintError(outErrorStringPtr, $"Return of method '{funcName}' not supported '{method.ReturnType.Name}'");
							return;
						}
						
						ValueType methodReturnType = returnType.Type;

						if (methodReturnType == ValueType.Char8 && retType == ValueType.Char16) {
							retType = ValueType.Char8;
						}
		
						if (retType != methodReturnType) {
							PrintError(outErrorStringPtr, $"Method '{funcName}' has invalid return type '{methodReturnType.ToString()}' when it should have '{retType.ToString()}'");
							return;
						}

						for (int j = 0; j < parameters.Length; j++)
						{
							var param = parameters[j];
							
							ValueType paramType = TypeMapper.MonoTypeToValueType(param.ParameterType.Name);

							if (paramType == ValueType.Invalid) {
								if (param.ParameterType.IsClass) {
									paramType = ValueType.Function;
								}
							}

							if (paramType == ValueType.Invalid) {
								PrintError(outErrorStringPtr, $"Parameter '{param.Name}' ({j}) of method '{funcName}' not supported '{paramType.ToString()}'");
								return;
							}

							ValueType methodParamType = parameterTypes[j].Type;
			
							if (methodParamType == ValueType.Char8 && paramType == ValueType.Char16) {
								paramType = ValueType.Char8;
							}

							if (paramType != methodParamType) {
								PrintError(outErrorStringPtr, $"Method '{funcName}' has invalid param type '{methodParamType.ToString()}' at {param.Name} ({j}) when it should have '{paramType.ToString()}'");
								return;
							}
						}
						
						
						
			        }
		        }
	        }
        }
	}

	[UnmanagedCallersOnly]
	public static void StartPlugin(Guid assemblyGuid)
	{
	}
	
	[UnmanagedCallersOnly]
	public static void EndPlugin()
	{
		
	}

	private static void PrintError(nint outErrorStringPtr, string message)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(message);
		Marshal.Copy(bytes, 0, outErrorStringPtr, bytes.Length);
	}

	/*private static ManagedClass InitManagedClass(Guid assemblyGuid, IntPtr classHolderPtr, Type type)
    {
        string typeName = type.Name;

        IntPtr typeNamePtr = Marshal.StringToHGlobalAnsi(typeName);

        ManagedClass managedClass = new ManagedClass();
        ManagedClass_Create(ref assemblyGuid, classHolderPtr, type.GetHashCode(), typeNamePtr, out managedClass);

        Marshal.FreeHGlobal(typeNamePtr);

        Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        CollectMethods(type, methods);

        foreach (var item in methods)
        {
            MethodInfo methodInfo = item.Value;
            
            // Get all custom attributes for the method
            object[] attributes = methodInfo.GetCustomAttributes(false);

            List<string> attributeNames = new List<string>();

            foreach (object attribute in attributes)
            {
                // Add full qualified name of the attribute
                attributeNames.Add(attribute.GetType().FullName);
            }

            // Add the objects being pointed to to the delegate cache so they don't get GC'd
            Guid methodGuid = Guid.NewGuid();
            managedClass.AddMethod(item.Key, methodGuid, attributeNames.ToArray());

            ManagedMethodCache.Instance.AddMethod(assemblyGuid, methodGuid, methodInfo);
        }

        // Add new object, free object delegates
        managedClass.NewObjectFunction = new NewObjectDelegate(() =>
        {
            // Allocate the object
            object obj = RuntimeHelpers.GetUninitializedObject(type);
            // GCHandle objHandle = GCHandle.Alloc(obj);

            // Call the constructor
            ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

            if (constructorInfo == null)
            {
                throw new Exception("Failed to find empty constructor for type: " + type.Name);
            }

            constructorInfo.Invoke(obj, null);

            Guid objectGuid = Guid.NewGuid();

            // @TODO: Reduce complexity - this is a bit of a mess between C# adn C++ interop
            // ManagedObject managedObject = new ManagedObject();

            // NativeInterop_AddObjectToCache(ref assemblyGuid, ref objectGuid, GCHandle.ToIntPtr(objHandle), out managedObject);

            // objHandle.Free();

            // return managedObject;
            
            return ManagedObjectCache.Instance.AddObject(assemblyGuid, objectGuid, obj);
        });

        managedClass.FreeObjectFunction = new FreeObjectDelegate(FreeObject);

        return managedClass;
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
	}*/
}
