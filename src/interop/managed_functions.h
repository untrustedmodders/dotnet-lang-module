#pragma once

#include "../core.h"

namespace netlm {
	class String;
	class ClassHolder;
	class Class;
	struct ManagedGuid;
	struct ManagedType;
	struct ManagedObject;
	struct ManagedMethod;
	enum class AssemblyLoadStatus;
	enum class GCCollectionMode;

	using InitializeAssemblyFn = AssemblyLoadStatus(*)(String, ManagedGuid *, ClassHolder *);
	using UnloadAssemblyFn = Bool32(*)(ManagedGuid *);
	using GetAssemblyNameFn = String(*)(ManagedGuid *);

	//using AddMethodToCacheFn = void(*)(ManagedGuid *, ManagedGuid *, void *);
	//using AddObjectToCacheFn = void(*)(ManagedGuid *, ManagedGuid *, void *, ManagedObject *);

	using CollectGarbageFn = void(*)(int32_t, GCCollectionMode, Bool32, Bool32);
	using WaitForPendingFinalizersFn = void(*)();

#pragma region TypeInterface

	using GetAssemblyTypesFn = void(*)(ManagedGuid*, TypeId*, int32_t*);
	using GetTypeIdFn = void (*)(String, TypeId*);
	using GetFullTypeNameFn = String(*)(TypeId);
	using GetAssemblyQualifiedNameFn = String(*)(TypeId);
	using GetBaseTypeFn = void(*)(TypeId, TypeId*);
	using GetTypeSizeFn = int32_t(*)(TypeId);
	using IsTypeSubclassOfFn = Bool32(*)(TypeId, TypeId);
	using IsTypeAssignableToFn = Bool32(*)(TypeId, TypeId);
	using IsTypeAssignableFromFn = Bool32(*)(TypeId, TypeId);
	using IsTypeSZArrayFn = Bool32(*)(TypeId);
	using IsTypeByRefFn = Bool32(*)(TypeId);
	using GetElementTypeFn = void(*)(TypeId, TypeId*);
	using GetTypeMethodsFn = void(*)(TypeId, ManagedHandle*, int32_t*);
	using GetTypeFieldsFn = void(*)(TypeId, ManagedHandle*, int32_t*);
	using GetTypePropertiesFn = void(*)(TypeId, ManagedHandle*, int32_t*);
	using GetTypeMethodFn = void(*)(TypeId, String, ManagedHandle*);
	using GetTypeFieldFn = void(*)(TypeId, String, ManagedHandle*);
	using GetTypePropertyFn = void(*)(TypeId, String, ManagedHandle*);
	using HasTypeAttributeFn = Bool32(*)(TypeId, TypeId);
	using GetTypeAttributesFn = void (*)(ManagedHandle, TypeId*, int32_t*);
	using GetTypeManagedTypeFn = ManagedType(*)(TypeId);

#pragma endregion

#pragma region MethodInfo
	using GetMethodInfoNameFn = String(*)(ManagedHandle);
	using GetMethodInfoReturnTypeFn = void(*)(ManagedHandle, TypeId*);
	using GetMethodInfoParameterTypesFn = void(*)(ManagedHandle, TypeId*, int32_t*);
	using GetMethodInfoAccessibilityFn = TypeAccessibility(*)(ManagedHandle);
	using GetMethodInfoAttributesFn = void(*)(ManagedHandle, TypeId*, int32_t*);
#pragma endregion

#pragma region FieldInfo
	using GetFieldInfoNameFn = String(*)(ManagedHandle);
	using GetFieldInfoTypeFn = void(*)(ManagedHandle, TypeId*);
	using GetFieldInfoAccessibilityFn = TypeAccessibility(*)(ManagedHandle);
	using GetFieldInfoAttributesFn = void(*)(ManagedHandle, TypeId*, int32_t*);
#pragma endregion

#pragma region PropertyInfo
	using GetPropertyInfoNameFn = String(*)(ManagedHandle);
	using GetPropertyInfoTypeFn = void(*)(ManagedHandle, TypeId*);
	using GetPropertyInfoAttributesFn = void(*)(ManagedHandle, TypeId*, int32_t*);
#pragma endregion

#pragma region Attribute
	using GetAttributeFieldValueFn = void(*)(ManagedHandle, String, void*);
	using GetAttributeTypeFn = void(*)(ManagedHandle, TypeId*);
#pragma endregion

	using IsClassFn = bool(*)(TypeId);
	using IsEnumFn = bool(*)(TypeId);
	using IsValueTypeFn = bool(*)(TypeId);
	using GetEnumNamesFn = void(*)(TypeId, String*, int32_t*);
	using GetEnumValuesFn = void(*)(TypeId, int32_t*, int32_t*);

	struct ManagedFunctions {
		InitializeAssemblyFn InitializeAssemblyFptr;
		UnloadAssemblyFn UnloadAssemblyFptr;
		GetAssemblyNameFn GetAssemblyNameFptr;

		//AddMethodToCacheFn AddMethodToCacheFptr;
		//AddObjectToCacheFn AddObjectToCacheFptr;

		CollectGarbageFn CollectGarbageFptr;
		WaitForPendingFinalizersFn WaitForPendingFinalizersFptr;

#pragma region TypeInterface
		GetAssemblyTypesFn GetAssemblyTypesFptr;
		GetTypeIdFn GetTypeIdFptr;
		GetFullTypeNameFn GetFullTypeNameFptr;
		GetAssemblyQualifiedNameFn GetAssemblyQualifiedNameFptr;
		GetBaseTypeFn GetBaseTypeFptr;
		GetTypeSizeFn GetTypeSizeFptr;
		IsTypeSubclassOfFn IsTypeSubclassOfFptr;
		IsTypeAssignableToFn IsTypeAssignableToFptr;
		IsTypeAssignableFromFn IsTypeAssignableFromFptr;
		IsTypeSZArrayFn IsTypeSZArrayFptr;
		IsTypeByRefFn IsTypeByRefFptr;
		GetElementTypeFn GetElementTypeFptr;
		GetTypeMethodsFn GetTypeMethodsFptr;
		GetTypeFieldsFn GetTypeFieldsFptr;
		GetTypePropertiesFn GetTypePropertiesFptr;
		GetTypeMethodFn GetTypeMethodFptr;
		GetTypeFieldFn GetTypeFieldFptr;
		GetTypePropertyFn GetTypePropertyFptr;
		HasTypeAttributeFn HasTypeAttributeFptr;
		GetTypeAttributesFn GetTypeAttributesFptr;
		GetTypeManagedTypeFn GetTypeManagedTypeFptr;

#pragma endregion

#pragma region MethodInfo
		GetMethodInfoNameFn GetMethodInfoNameFptr;
		GetMethodInfoReturnTypeFn GetMethodInfoReturnTypeFptr;
		GetMethodInfoParameterTypesFn GetMethodInfoParameterTypesFptr;
		GetMethodInfoAccessibilityFn GetMethodInfoAccessibilityFptr;
		GetMethodInfoAttributesFn GetMethodInfoAttributesFptr;
#pragma endregion

#pragma region FieldInfo
		GetFieldInfoNameFn GetFieldInfoNameFptr;
		GetFieldInfoTypeFn GetFieldInfoTypeFptr;
		GetFieldInfoAccessibilityFn GetFieldInfoAccessibilityFptr;
		GetFieldInfoAttributesFn GetFieldInfoAttributesFptr;
#pragma endregion

#pragma region PropertyInfo
		GetPropertyInfoNameFn GetPropertyInfoNameFptr;
		GetPropertyInfoTypeFn GetPropertyInfoTypeFptr;
		GetPropertyInfoAttributesFn GetPropertyInfoAttributesFptr;
#pragma endregion

#pragma region Attribute
		GetAttributeFieldValueFn GetAttributeFieldValueFptr;
		GetAttributeTypeFn GetAttributeTypeFptr;
#pragma endregion

		IsClassFn IsClassFptr;
		IsEnumFn IsEnumFptr;
		IsValueTypeFn IsValueTypeFptr;
		GetEnumNamesFn GetEnumNamesFptr;
		GetEnumValuesFn GetEnumValuesFptr;
	};

	inline ManagedFunctions Managed;
}
