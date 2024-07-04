using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Plugify;

internal static class TypeInterface 
{
	private static readonly UniqueIdList<Type> CachedTypes = new();
	private static readonly UniqueIdList<MethodInfo> CachedMethods = new();
	private static readonly UniqueIdList<FieldInfo> CachedFields = new();
	private static readonly UniqueIdList<PropertyInfo> CachedProperties = new();
	private static readonly UniqueIdList<Attribute> CachedAttributes = new();

	public static void RemoveUnusedObjects()
	{
		CachedTypes.RemoveUnusedObjects();
		CachedMethods.RemoveUnusedObjects();
		CachedFields.RemoveUnusedObjects();
		CachedProperties.RemoveUnusedObjects();
		CachedAttributes.RemoveUnusedObjects();
	}
	
	[UnmanagedCallersOnly]
	private static unsafe void GetAssemblyTypes(nint assemblyGuidPtr, int* outTypeArrayPtr, int* outTypeCount)
	{
		try
		{
			Guid assemblyGuid = Marshal.PtrToStructure<Guid>(assemblyGuidPtr);

			AssemblyInstance? assemblyInstance = AssemblyCache.Instance.Get(assemblyGuid);
            if (assemblyInstance == null)
            {
                Logger.Log(Severity.Warning, "Failed to get assembly types: {0} not found", assemblyGuid);
				return;
            }

			Type[]? assemblyTypes = assemblyInstance.Assembly?.GetTypes();

			if (assemblyTypes == null || assemblyTypes.Length == 0)
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
			Logger.Log(Severity.Error, e.ToString());
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
				Logger.Log(Severity.Error, "Failed to find type with name '{0}'.", name);
				return;
			}

			*outType = CachedTypes.Add(type);
		}
		catch (Exception e)
		{
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
		}
	}

	[UnmanagedCallersOnly]
	private static unsafe ManagedType GetTypeManagedType(int typeId)
	{
		try
		{
			if (!CachedTypes.TryGetValue(typeId, out var type))
				return ManagedType.Invalid;

			return new ManagedType(type);
		}
		catch (Exception e)
		{
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
			return NativeString.Null();
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
				Logger.Log(Severity.Error, "Failed to find field with name '{0}' in attribute {1}.", fieldName, targetType.FullName ?? targetType.Name);
				return;
			}
			
			Marshalling.Assign(fieldInfo.FieldType, fieldInfo.GetValue(attribute), outValue);
		}
		catch (Exception e)
		{
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
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
			Logger.Log(Severity.Error, e.ToString());
		}
	}
}
