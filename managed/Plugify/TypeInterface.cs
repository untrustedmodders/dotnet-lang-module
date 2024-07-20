using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Plugify;

using static ManagedHost;

internal static class TypeInterface 
{
	internal static readonly UniqueIdList<Type> CachedTypes = new();
	internal static readonly UniqueIdList<MethodInfo> CachedMethods = new();
	internal static readonly UniqueIdList<FieldInfo> CachedFields = new();
	internal static readonly UniqueIdList<PropertyInfo> CachedProperties = new();
	internal static readonly UniqueIdList<Attribute> CachedAttributes = new();
	
	internal static Type? FindType(string? typeName)
	{
		var type = Type.GetType(typeName!,
			(name) => AssemblyLoader.ResolveAssembly(null, name),
			(assembly, name, ignore) => assembly != null ? assembly.GetType(name, false, ignore) : Type.GetType(name, false, ignore));

		return type;
	}

	internal static object? CreateInstance(Type type, params object?[]? arguments)
	{
		return type.Assembly.CreateInstance(type.FullName ?? string.Empty, false, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, arguments!, null, null);
	}

	/*internal static unsafe T? FindSuitableMethod<T>(string? methodName, ManagedType* parameterTypes, int parameterCount, ReadOnlySpan<T> methods) where T : MethodBase
	{
		if (methodName == null)
			return null;

		T? result = null;

		foreach (var methodInfo in methods)
		{
			var parameters = methodInfo.GetParameters();

			if (parameters.Length != parameterCount)
				continue;

			// Check if the method name matches the signature of methodInfo, if so we ignore the automatic type checking
			if (methodName == methodInfo.ToString())
			{
				result = methodInfo;
				break;
			}

			if (methodInfo.Name != methodName)
				continue;

			int matchingTypes = 0;

			for (int i = 0; i < parameters.Length; i++)
			{
				ManagedType paramType = new ManagedType(parameters[i].ParameterType);

				if (paramType == parameterTypes[i])
				{
					matchingTypes++;
				}
			}

			if (matchingTypes == parameterCount)
			{
				result = methodInfo;
				break;
			}
		}

		return result;
	}*/
	
	[UnmanagedCallersOnly]
	private static unsafe void GetAssemblyTypes(int assemblyId, int* outTypeArrayPtr, int* outTypeCount)
	{
		try
		{
			if (!AssemblyLoader.TryGetAssembly(assemblyId, out var assembly))
			{
				LogMessage($"Couldn't get types for assembly '{assemblyId}', assembly not found.", MessageLevel.Error);
				return;
			}

			if (assembly == null)
			{
				LogMessage($"Couldn't get types for assembly '{assemblyId}', assembly was null.", MessageLevel.Error);
				return;
			}
			
			Type[] assemblyTypes = assembly.GetTypes();

			if (assemblyTypes.Length == 0)
			{
				*outTypeCount = 0;
				return;
			}

			*outTypeCount = assemblyTypes.Length;

			if (outTypeArrayPtr == null)
				return;

			for (int i = 0; i < assemblyTypes.Length; i++)
			{
				outTypeArrayPtr[i] = CachedTypes.Add(assemblyTypes[i]);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetTypeId(NativeString name, int* outType)
	{
		try
		{
			var type = Type.GetType(name);

			if (type == null)
			{
				LogMessage($"Failed to find type with name '{name}'.", MessageLevel.Error);
				return;
			}

			*outType = CachedTypes.Add(type);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe NativeString GetFullTypeName(int typeId)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return NativeString.Null();

			return type.FullName;
		}
		catch (Exception e)
		{
			HandleException(e);
			return NativeString.Null();
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe NativeString GetAssemblyQualifiedName(int typeId)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return NativeString.Null();

			return type.AssemblyQualifiedName;
		}
		catch (Exception e)
		{
			HandleException(e);
			return NativeString.Null();
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetBaseType(int typeId, int* outBaseType)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type) || outBaseType == null)
				return;

			if (type.BaseType == null)
			{
				*outBaseType = 0;
				return;
			}

			*outBaseType = CachedTypes.Add(type.BaseType);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static int GetTypeSize(int typeId)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return -1;

			return Marshal.SizeOf(type);
		}
		catch (Exception e)
		{
			HandleException(e);
			return -1;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe Bool32 IsTypeSubclassOf(int typeId0, int typeId1)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId0, out var type0) || !CachedTypes.TryGetValue(typeId1, out var type1))
				return false;

			return type0.IsSubclassOf(type1);
		}
		catch (Exception e)
		{
			HandleException(e);
			return false;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe Bool32 IsTypeAssignableTo(int typeId0, int typeId1)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId0, out var type0) || !CachedTypes.TryGetValue(typeId1, out var type1))
				return false;

			return type0.IsAssignableTo(type1);
		}
		catch (Exception e)
		{
			HandleException(e);
			return false;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe Bool32 IsTypeAssignableFrom(int typeId0, int typeId1)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId0, out var type0) || !CachedTypes.TryGetValue(typeId1, out var type1))
				return false;

			return type0.IsAssignableFrom(type1);
		}
		catch (Exception e)
		{
			HandleException(e);
			return false;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe Bool32 IsTypeSZArray(int typeId)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return false;

			return type.IsSZArray;
		}
		catch (Exception e)
		{
			HandleException(e);
			return false;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe Bool32 IsTypeByRef(int typeId)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return false;

			return type.IsByRef;
		}
		catch (Exception e)
		{
			HandleException(e);
			return false;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetElementType(int typeId, int* outElementTypeId)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			var elementType = type.GetElementType();

			if (elementType == null)
				*outElementTypeId = 0;

			*outElementTypeId = CachedTypes.Add(elementType);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetTypeMethods(int typeId, int* outMethodArrayPtr, int* outMethodCount)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

			if (methods.Length == 0)
			{
				*outMethodCount = 0;
				return;
			}

			*outMethodCount = methods.Length;

			if (outMethodArrayPtr == null)
				return;

			for (int i = 0; i < methods.Length; i++)
			{
				outMethodArrayPtr[i] = CachedMethods.Add(methods[i]);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetTypeFields(int typeId, int* outFieldArrayPtr, int* outFieldCount)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

			if (fields.Length == 0)
			{
				*outFieldCount = 0;
				return;
			}

			*outFieldCount = fields.Length;

			if (outFieldArrayPtr == null)
				return;

			for (int i = 0; i < fields.Length; i++)
			{
				outFieldArrayPtr[i] = CachedFields.Add(fields[i]);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetTypeProperties(int typeId, int* outPropertyArrayPtr, int* outPropertyCount)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

			if (properties.Length == 0)
			{
				*outPropertyCount = 0;
				return;
			}

			*outPropertyCount = properties.Length;

			if (outPropertyArrayPtr == null)
				return;

			for (int i = 0; i < properties.Length; i++)
			{
				outPropertyArrayPtr[i] = CachedProperties.Add(properties[i]);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}
	
	[UnmanagedCallersOnly]
	private static unsafe void GetTypeMethod(int typeId, NativeString name, int* outMethod)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			MethodInfo method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

			if (method == null)
				return;

			if (outMethod == null)
				return;

			*outMethod = CachedMethods.Add(method);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetTypeField(int typeId, NativeString name, int* outField)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

			if (field == null)
				return;

			if (outField == null)
				return;

			*outField = CachedFields.Add(field);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetTypeProperty(int typeId, NativeString name, int* outProperty)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

			if (property == null)
				return;

			if (outProperty == null)
				return;

			*outProperty = CachedProperties.Add(property);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe Bool32 HasTypeAttribute(int typeId, int attributeTypeId)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type) || !CachedTypes.TryGetValue(attributeTypeId, out var attributeType))
				return false;

			return type.GetCustomAttribute(attributeType) != null;
		}
		catch (Exception e)
		{
			HandleException(e);
			return false;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetTypeAttributes(int typeId, int* outAttributeArrayPtr, int* outAttributeCount)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			var attributes = type.GetCustomAttributes().ToImmutableArray();

			if (attributes.Length == 0)
			{
				*outAttributeCount = 0;
				return;
			}

			*outAttributeCount = attributes.Length;

			if (outAttributeArrayPtr == null)
				return;

			for (int i = 0; i < attributes.Length; i++)
			{
				var attribute = attributes[i];
				outAttributeArrayPtr[i] = CachedAttributes.Add(attribute);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe ManagedType GetTypeManagedType(int typeId)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return ManagedType.Invalid;

			return new ManagedType(type, type.GetCustomAttributes(false));
		}
		catch (Exception e)
		{
			HandleException(e);
			return ManagedType.Invalid;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe NativeString GetMethodInfoName(int methodId)
	{
		try
		{
			if (!CachedMethods.TryGetValue(methodId, out var methodInfo))
				return NativeString.Null();

			return methodInfo.Name;
		}
		catch (Exception e)
		{
			HandleException(e);
			return NativeString.Null();
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe nint GetMethodInfoFunctionAddress(int methodId)
	{
		try
		{
			if (!CachedMethods.TryGetValue(methodId, out var methodInfo))
				return nint.Zero;

			return methodInfo.MethodHandle.GetFunctionPointer();
		}
		catch (Exception e)
		{
			HandleException(e);
			return nint.Zero;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetMethodInfoReturnType(int methodId, int* outReturnType)
	{
		try
		{
			if (!CachedMethods.TryGetValue(methodId, out var methodInfo) || outReturnType == null)
				return;

			*outReturnType = CachedTypes.Add(methodInfo.ReturnType);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetMethodInfoParameterTypes(int methodId, int* outParameterTypeArrayPtr, int* outParameterCount)
	{
		try
		{
			if (!CachedMethods.TryGetValue(methodId, out var methodInfo))
				return;

			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length == 0)
			{
				*outParameterCount = 0;
				return;
			}

			*outParameterCount = parameters.Length;

			if (outParameterTypeArrayPtr == null)
				return;

			for (int i = 0; i < parameters.Length; i++)
			{
				outParameterTypeArrayPtr[i] = CachedTypes.Add(parameters[i].ParameterType);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetMethodInfoAttributes(int methodId, int* outAttributeArrayPtr, int* outAttributeCount)
	{
		try
		{
			if (!CachedMethods.TryGetValue(methodId, out var methodInfo))
				return;

			var attributes = methodInfo.GetCustomAttributes().ToImmutableArray();

			if (attributes.Length == 0)
			{
				*outAttributeCount = 0;
				return;
			}

			*outAttributeCount = attributes.Length;

			if (outAttributeArrayPtr == null)
				return;

			for (int i = 0; i < attributes.Length; i++)
			{
				outAttributeArrayPtr[i] = CachedAttributes.Add(attributes[i]);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetMethodInfoParameterAttributes(int methodId, int parameterIndex, int* outAttributeArrayPtr, int* outAttributeCount)
	{
		try
		{
			if (!CachedMethods.TryGetValue(methodId, out var methodInfo))
				return;

			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length == 0 || parameterIndex >= parameters.Length)
			{
				*outAttributeCount = 0;
				return;
			}
			
			var attributes = parameters[parameterIndex].GetCustomAttributes(false).ToImmutableArray();

			if (attributes.Length == 0)
			{
				*outAttributeCount = 0;
				return;
			}

			*outAttributeCount = attributes.Length;

			if (outAttributeArrayPtr == null)
				return;

			for (int i = 0; i < attributes.Length; i++)
			{
				outAttributeArrayPtr[i] = CachedAttributes.Add((Attribute) attributes[i]);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetMethodInfoReturnAttributes(int methodId, int* outAttributeArrayPtr, int* outAttributeCount)
	{
		try
		{
			if (!CachedMethods.TryGetValue(methodId, out var methodInfo))
				return;

			var attributes = methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(false).ToImmutableArray();

			if (attributes.Length == 0)
			{
				*outAttributeCount = 0;
				return;
			}

			*outAttributeCount = attributes.Length;

			if (outAttributeArrayPtr == null)
				return;

			for (int i = 0; i < attributes.Length; i++)
			{
				outAttributeArrayPtr[i] = CachedAttributes.Add((Attribute) attributes[i]);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	internal enum TypeAccessibility
	{
		Public,
		Private,
		Protected,
		Internal,
		ProtectedPublic,
		PrivateProtected
	}

	private static TypeAccessibility GetTypeAccessibility(FieldInfo fieldInfo)
	{
		if (fieldInfo.IsPublic) return TypeAccessibility.Public;
		if (fieldInfo.IsPrivate) return TypeAccessibility.Private;
		if (fieldInfo.IsFamily) return TypeAccessibility.Protected;
		if (fieldInfo.IsAssembly) return TypeAccessibility.Internal;
		if (fieldInfo.IsFamilyOrAssembly) return TypeAccessibility.ProtectedPublic;
		if (fieldInfo.IsFamilyAndAssembly) return TypeAccessibility.PrivateProtected;
		return TypeAccessibility.Public;
	}

	private static TypeAccessibility GetTypeAccessibility(MethodInfo methodInfo)
	{
		if (methodInfo.IsPublic) return TypeAccessibility.Public;
		if (methodInfo.IsPrivate) return TypeAccessibility.Private;
		if (methodInfo.IsFamily) return TypeAccessibility.Protected;
		if (methodInfo.IsAssembly) return TypeAccessibility.Internal;
		if (methodInfo.IsFamilyOrAssembly) return TypeAccessibility.ProtectedPublic;
		if (methodInfo.IsFamilyAndAssembly) return TypeAccessibility.PrivateProtected;
		return TypeAccessibility.Public;
	}

	[UnmanagedCallersOnly]
	private static unsafe TypeAccessibility GetMethodInfoAccessibility(int methodId)
	{
		try
		{
			if (!CachedMethods.TryGetValue(methodId, out var methodInfo))
				return TypeAccessibility.Internal;

			return GetTypeAccessibility(methodInfo);
		}
		catch (Exception e)
		{
			HandleException(e);
			return TypeAccessibility.Public;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe NativeString GetFieldInfoName(int fieldId)
	{
		try
		{
			if (!CachedFields.TryGetValue(fieldId, out var fieldInfo))
				return NativeString.Null();

			return fieldInfo.Name;
		}
		catch (Exception e)
		{
			HandleException(e);
			return NativeString.Null();
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetFieldInfoType(int fieldId, int* outFieldType)
	{
		try
		{
			if (!CachedFields.TryGetValue(fieldId, out var fieldInfo))
				return;

			*outFieldType = CachedTypes.Add(fieldInfo.FieldType);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe TypeAccessibility GetFieldInfoAccessibility(int fieldId)
	{
		try
		{
			if (!CachedFields.TryGetValue(fieldId, out var fieldInfo))
				return TypeAccessibility.Public;

			return GetTypeAccessibility(fieldInfo);
		}
		catch (Exception e)
		{
			HandleException(e);
			return TypeAccessibility.Public;
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetFieldInfoAttributes(int fieldId, int* outAttributeArrayPtr, int* outAttributeCount)
	{
		try
		{
			if (!CachedFields.TryGetValue(fieldId, out var fieldInfo))
				return;

			var attributes = fieldInfo.GetCustomAttributes().ToImmutableArray();

			if (attributes.Length == 0)
			{
				*outAttributeCount = 0;
				return;
			}

			*outAttributeCount = attributes.Length;

			if (outAttributeArrayPtr == null)
				return;

			for (int i = 0; i < attributes.Length; i++)
			{
				outAttributeArrayPtr[i] = CachedAttributes.Add(attributes[i]);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe NativeString GetPropertyInfoName(int propertyId)
	{
		try
		{
			if (!CachedProperties.TryGetValue(propertyId, out var propertyInfo))
				return NativeString.Null();

			return propertyInfo.Name;
		}
		catch (Exception e)
		{
			HandleException(e);
			return NativeString.Null();
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetPropertyInfoType(int propertyId, int* outPropertyType)
	{
		try
		{
			if (!CachedProperties.TryGetValue(propertyId, out var propertyInfo))
				return;

			*outPropertyType = CachedTypes.Add(propertyInfo.PropertyType);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetPropertyInfoAttributes(int propertyId, int* outAttributeArrayPtr, int* outAttributeCount)
	{
		try
		{
			if (!CachedProperties.TryGetValue(propertyId, out var propertyInfo))
				return;

			var attributes = propertyInfo.GetCustomAttributes().ToImmutableArray();

			if (attributes.Length == 0)
			{
				*outAttributeCount = 0;
				return;
			}

			*outAttributeCount = attributes.Length;

			if (outAttributeArrayPtr == null)
				return;

			for (int i = 0; i < attributes.Length; i++)
			{
				outAttributeArrayPtr[i] = CachedAttributes.Add(attributes[i]);
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetAttributeFieldValue(int attributeId, NativeString fieldName, nint outValue)
	{
		try
		{
			if (!CachedAttributes.TryGetValue(attributeId, out var attribute))
				return;

			Type targetType = attribute.GetType();
			FieldInfo? fieldInfo = targetType.GetField(fieldName!, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (fieldInfo == null)
			{
				LogMessage($"Failed to find field with name '{fieldName}' in attribute {targetType.FullName}.", MessageLevel.Error);
				return;
			}
			
			Marshalling.MarshalReturnValue(fieldInfo.GetValue(attribute), fieldInfo.FieldType, outValue);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe void GetAttributeType(int attributeId, int* outType)
	{
		try
		{
			if (!CachedAttributes.TryGetValue(attributeId, out var attribute))
				return;

			*outType = CachedTypes.Add(attribute.GetType());
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe Bool32 IsClass(int typeId)
	{
		try
		{
			if (CachedTypes.TryGetValue(typeId, out var type))
				return type.IsClass;
		}
		catch (Exception e)
		{
			HandleException(e);
		}

		return false;
	}

	[UnmanagedCallersOnly]
	private static unsafe Bool32 IsEnum(int typeId)
	{
		try
		{
			if (CachedTypes.TryGetValue(typeId, out var type))
				return type.IsEnum;
		}
		catch (Exception e)
		{
			HandleException(e);
		}

		return false;
	}

	[UnmanagedCallersOnly]
	private static unsafe Bool32 IsValueType(int typeId)
	{
		try
		{
			if (CachedTypes.TryGetValue(typeId, out var type))
				return type.IsValueType;
		}
		catch (Exception e)
		{
			HandleException(e);
		}

		return false;
	}
	
	[UnmanagedCallersOnly]
	private static unsafe void GetEnumNames(int typeId, NativeString* outNameArrayPtr, int* outCount)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			string[] names = Enum.GetNames(type);

			*outCount = names.Length;

			if (outNameArrayPtr == null)
				return;

			for (int i = 0; i < *outCount; i++)
				outNameArrayPtr[i] = names[i];
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}
        
	[UnmanagedCallersOnly]
	private static unsafe void GetEnumValues(int typeId, int* outValueArrayPtr, int* outCount)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return;

			Array values = Enum.GetValues(type);

			*outCount = values.Length;

			if (outValueArrayPtr == null)
				return;

			for (int i = 0; i < *outCount; i++)
				outValueArrayPtr[i] = (int) values.GetValue(i);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}
}
