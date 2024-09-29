using System.Reflection;
using System.Runtime.InteropServices;

namespace Plugify;

using static ManagedHost;

internal static class ManagedObject
{
	/*public readonly struct MethodKey : IEquatable<MethodKey>
	{
		public readonly string TypeName;
		public readonly string Name;
		public readonly ManagedType[] Types;
		public readonly int ParameterCount;

		public MethodKey(string typeIdName, string name, ManagedType[] typeIds, int parameterCount)
		{
			TypeName = typeIdName;
			Name = name;
			Types = typeIds;
			ParameterCount = parameterCount;
		}

		public override bool Equals([NotNullWhen(true)] object? obj) => obj is MethodKey other && Equals(other);

		bool IEquatable<MethodKey>.Equals(MethodKey other)
		{
			if (TypeName != other.TypeName || Name != other.Name)
				return false;

			for (int i = 0; i < Types.Length; i++)
			{
				if (Types[i] != other.Types[i])
					return false;
			}

			return ParameterCount == other.ParameterCount;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + TypeName.GetHashCode();
				hash = hash * 23 + Name.GetHashCode();
				foreach (var type in Types)
					hash = hash * 23 + type.GetHashCode();
				hash = hash * 23 + ParameterCount.GetHashCode();
				return hash;
			}
		}
	}

	internal static Dictionary<MethodKey, MethodInfo> CachedMethods = new Dictionary<MethodKey, MethodInfo>();*/

	[UnmanagedCallersOnly]
	private static unsafe nint CreateObject(int typeId, Bool32 weakRef, nint parameterPtr, ManagedType* parameterTypes, int parameterCount)
	{
		try
		{
			if (!TypeInterface.CachedTypes.TryGetValue(typeId, out var type))
			{
				LogMessage($"Failed to find type with id '{typeId}'.", MessageLevel.Error);
				return nint.Zero;
			}
			
			ConstructorInfo? constructor = null;

			var currentType = type;
			while (currentType != null)
			{
				ReadOnlySpan<ConstructorInfo> constructors = currentType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				
				//constructor = TypeInterface.FindSuitableMethod(".ctor", parameterTypes, parameterCount, constructors);

				// TODO: Rework
				
				constructor = constructors[0];

				if (constructor != null)
					break;

				currentType = currentType.BaseType;
			}

			if (constructor == null)
			{
				LogMessage($"Failed to find constructor for type {type.FullName} with {parameterCount} parameters.", MessageLevel.Error);
				return nint.Zero;
			}

			var parameters = Marshalling.MarshalParameterArray(parameterPtr, parameterCount, constructor);

			object? result;

			if (currentType != type || parameters == null)
			{
				result = TypeInterface.CreateInstance(type);

				if (currentType != type)
					constructor.Invoke(result, parameters);
			}
			else
			{
				result = TypeInterface.CreateInstance(type, parameters);
			}

			if (result == null)
			{
				LogMessage($"Failed to instantiate type {type.FullName}.", MessageLevel.Error);
			}

			var handle = GCHandle.Alloc(result, weakRef ? GCHandleType.Weak : GCHandleType.Normal);
			AssemblyLoader.RegisterHandle(type.Assembly, handle);
			return GCHandle.ToIntPtr(handle);
		}
		catch (Exception e)
		{
			HandleException(e);
			return nint.Zero;
		}
	}

	[UnmanagedCallersOnly]
	public static void DestroyObject(nint objectHandle)
	{
		try
		{
			GCHandle.FromIntPtr(objectHandle).Free();
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	/*private static unsafe MethodInfo? TryGetMethodInfo(Type typeId, string methodName, ManagedType* parameterTypes, int parameterCount, BindingFlags bindingFlags)
	{
		MethodInfo? methodInfo = null;

		var parameterTypes = new ManagedType[parameterCount];

		unsafe
		{
			fixed (ManagedType* parameterTypesPtr = parameterTypes)
			{
				ulong size = sizeof(ManagedType) * (ulong)parameterCount;
				Buffer.MemoryCopy(parameterTypes, parameterTypesPtr, size, size);
			}
		}

		var methodKey = new MethodKey(typeId.FullName, methodName, parameterTypes, parameterCount);

		if (!CachedMethods.TryGetValue(methodKey, out methodInfo))
		{
			List<MethodInfo> methods = [..typeId.GetMethods(bindingFlags)];

			Type? baseType = typeId.BaseType;
			while (baseType != null)
			{
				methods.AddRange(baseType.GetMethods(bindingFlags));
				baseType = baseType.BaseType;
			}

			methodInfo = TypeInterface.FindSuitableMethod<MethodInfo>(methodName, parameterTypes, parameterCount, CollectionsMarshal.AsSpan(methods));

			if (methodInfo == null)
			{
				LogMessage($"Failed to find method '{methodName}' for type {typeId.FullName} with {parameterCount} parameters.", Severity.Error);
				return null;
			}

			CachedMethods.Add(methodKey, methodInfo);
		}

		return methodInfo;
	}*/

	[UnmanagedCallersOnly]
	private static void InvokeStaticMethod(int typeId, int methodId, nint parameterPtr, int parameterCount)
	{
		try
		{
			if (!TypeInterface.CachedMethods.TryGetValue(methodId, out var methodInfo))
			{
				LogMessage($"Cannot find method {methodId}.", MessageLevel.Error);
				return;
			}
			
			/*if (!TypeInterface.CachedTypes.TryGetValue(typeId, out var type))
			{
				LogMessage($"Cannot invoke method {methodInfo.Name} on a null type.", Severity.Error);
				return;
			}*/

			//var methodInfo = TryGetMethodInfo(type, methodName, parameterTypes, parameterCount, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			var parameters = Marshalling.MarshalParameterArray(parameterPtr, parameterCount, methodInfo);

			methodInfo.Invoke(null, parameters);
			
			Marshalling.MarshalParameterRefs(parameterPtr, parameterCount, methodInfo, parameters);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static void InvokeStaticMethodRet(int typeId, int methodId, nint parameterPtr, int parameterCount, nint resultStorage)
	{
		try
		{
			if (!TypeInterface.CachedMethods.TryGetValue(methodId, out var methodInfo))
			{
				LogMessage($"Cannot find method {methodId}.", MessageLevel.Error);
				return;
			}
			
			/*if (!TypeInterface.CachedTypes.TryGetValue(typeId, out var type))
			{
				LogMessage($"Cannot invoke method {methodInfo.Name} on a null type.", Severity.Error);
				return;
			}*/

			//var methodInfo = TryGetMethodInfo(type, methodName, parameterTypes, parameterCount, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			var parameters = Marshalling.MarshalParameterArray(parameterPtr, parameterCount, methodInfo);

			object? returnValue = methodInfo.Invoke(null, parameters);

			Marshalling.MarshalParameterRefs(parameterPtr, parameterCount, methodInfo, parameters);
			
			/*if (returnValue == null)
			{
				return;
			}*/

			Type returnType = methodInfo.ReturnType;
			
			bool useAnsi = returnType.IsChar() && TypeUtils.IsUseAnsi(methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(false));

			Marshalling.MarshalReturnValue(returnValue, returnType, resultStorage, useAnsi);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static void InvokeMethod(nint objectHandle, int methodId, nint parameterPtr, int parameterCount)
	{
		try
		{
			if (!TypeInterface.CachedMethods.TryGetValue(methodId, out var methodInfo))
			{
				LogMessage($"Cannot find method {methodId}.", MessageLevel.Error);
				return;
			}

			var target = GCHandle.FromIntPtr(objectHandle).Target;
			
			if (target == null)
			{
				LogMessage($"Cannot invoke method {methodInfo.Name} on a null type.", MessageLevel.Error);
				return;
			}

			//var targetType = target.GetType();
			//var methodInfo = TryGetMethodInfo(targetType, methodName, parameterTypes, parameterCount, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var parameters = Marshalling.MarshalParameterArray(parameterPtr, parameterCount, methodInfo);

			methodInfo.Invoke(target, parameters);
			
			Marshalling.MarshalParameterRefs(parameterPtr, parameterCount, methodInfo, parameters);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static void InvokeMethodRet(nint objectHandle, int methodId, nint parameterPtr, int parameterCount, nint resultStorage)
	{
		try
		{
			if (!TypeInterface.CachedMethods.TryGetValue(methodId, out var methodInfo))
			{
				LogMessage($"Cannot find method {methodId}.", MessageLevel.Error);
				return;
			}

			var target = GCHandle.FromIntPtr(objectHandle).Target;

			if (target == null)
			{
				LogMessage($"Cannot invoke method {methodInfo.Name} on object with handle {objectHandle}. Target was null.", MessageLevel.Error);
				return;
			}

			//var targetType = target.GetType();
			//var methodInfo = TryGetMethodInfo(targetType, methodName, parameterTypes, parameterCount, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var parameters = Marshalling.MarshalParameterArray(parameterPtr, parameterCount, methodInfo);
			
			object? returnValue = methodInfo.Invoke(target, parameters);

			Marshalling.MarshalParameterRefs(parameterPtr, parameterCount, methodInfo, parameters);
			
			/*if (returnValue == null)
			{
				return;
			}*/
			
			Type returnType = methodInfo.ReturnType;
			
			bool useAnsi = returnType.IsChar() && TypeUtils.IsUseAnsi(methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(false));

			Marshalling.MarshalReturnValue(returnValue, returnType, resultStorage, useAnsi);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static void SetFieldValue(nint targetPtr, NativeString fieldName, nint inValue)
	{
		try
		{
			var target = GCHandle.FromIntPtr(targetPtr).Target;

			if (target == null)
			{
				LogMessage($"Cannot set value of field {fieldName} on object with handle {targetPtr}. Target was null.", MessageLevel.Error);
				return;
			}

			var targetType = target.GetType();
			var fieldInfo = targetType.GetField(fieldName!, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (fieldInfo == null)
			{
				LogMessage($"Failed to find field '{fieldName}' in type '{targetType.FullName}'.", MessageLevel.Error);
				return;
			}

			object value = Marshalling.MarshalPointer(inValue, fieldInfo.FieldType);
			fieldInfo.SetValue(target, value);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static void GetFieldValue(nint targetPtr, NativeString fieldName, nint outValue)
	{
		try
		{
			var target = GCHandle.FromIntPtr(targetPtr).Target;

			if (target == null)
			{
				LogMessage($"Cannot get value of field {fieldName} from object with handle {targetPtr}. Target was null.", MessageLevel.Error);
				return;
			}

			var targetType = target.GetType();
			var fieldInfo = targetType.GetField(fieldName!, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (fieldInfo == null)
			{
				LogMessage($"Failed to find field '{fieldName}' in type '{targetType.FullName}'.", MessageLevel.Error);
				return;
			}

			Marshalling.MarshalReturnValue(fieldInfo.GetValue(target), fieldInfo.FieldType, outValue);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static void GetFieldPointer(nint targetPtr, NativeString fieldName, nint outValue)
	{
		try
		{
			var target = GCHandle.FromIntPtr(targetPtr).Target;

			if (target == null)
			{
				LogMessage($"Cannot get pointer to field {fieldName} from object with handle {targetPtr}. Target was null.", MessageLevel.Error);
				return;
			}

			FieldInfo? fieldInfo = null;
			
			var baseTargetType = target.GetType();
			var targetType = baseTargetType;
			while (targetType != null)
			{
				fieldInfo = targetType.GetField(fieldName!, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				if (fieldInfo != null)
					break;

				targetType = targetType.BaseType;
			}

			if (fieldInfo == null)
			{
				LogMessage($"Failed to find field '{fieldName}' in type '{baseTargetType.FullName}'.", MessageLevel.Error);
				return;
			}

			Marshalling.MarshalFieldAddress(target, fieldInfo, outValue);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}
	
	[UnmanagedCallersOnly]
	private static void SetPropertyValue(nint targetPtr, NativeString propertyName, nint inValue)
	{
		try
		{
			var target = GCHandle.FromIntPtr(targetPtr).Target;

			if (target == null)
			{
				LogMessage($"Cannot set value of property {propertyName} on object with handle {targetPtr}. Target was null.", MessageLevel.Error);
				return;
			}

			var targetType = target.GetType();
			var propertyInfo = targetType.GetProperty(propertyName!, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		
			if (propertyInfo == null)
			{
				LogMessage($"Failed to find property '{propertyName}' in type '{targetType.FullName}'", MessageLevel.Error);
				return;
			}

			if (propertyInfo.SetMethod == null)
			{
				LogMessage($"Cannot set value of property '{propertyName}'. No setter was found.", MessageLevel.Error);
				return;
			}

			object? value = Marshalling.MarshalPointer(inValue, propertyInfo.PropertyType);
			propertyInfo.SetValue(target, value);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static void GetPropertyValue(nint targetPtr, NativeString propertyName, nint outValue)
	{
		try
		{
			var target = GCHandle.FromIntPtr(targetPtr).Target;

			if (target == null)
			{
				LogMessage($"Cannot get value of property '{propertyName}' from object with handle {targetPtr}. Target was null.", MessageLevel.Error);
				return;
			}

			var targetType = target.GetType();
			var propertyInfo = targetType.GetProperty(propertyName!, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (propertyInfo == null)
			{
				LogMessage($"Failed to find property '{propertyName}' in type '{targetType.FullName}'.", MessageLevel.Error);
				return;
			}

			if (propertyInfo.GetMethod == null)
			{
				LogMessage($"Cannot get value of property '{propertyName}'. No getter was found.", MessageLevel.Error);
				return;
			}

			Marshalling.MarshalReturnValue(propertyInfo.GetValue(target), propertyInfo.PropertyType, outValue);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}
}