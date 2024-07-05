#pragma once

#include "core.h"

namespace netlm {
	class String;
	enum class AssemblyLoadStatus;
	enum class GCCollectionMode;
	struct ManagedType;
	class ManagedObject;

	using SetInternalCallsFn = void(*)(void*, int32_t);
	using CreateAssemblyLoadContextFn = int32_t(*)(String);
	using UnloadAssemblyLoadContextFn = void(*)(int32_t);
	using LoadManagedAssemblyFn = int32_t(*)(int32_t, String);
	using GetLastLoadStatusFn = AssemblyLoadStatus(*)();
	using GetAssemblyNameFn = String(*)(int32_t);

	using CollectGarbageFn = void(*)(int32_t, GCCollectionMode, Bool32, Bool32);
	using WaitForPendingFinalizersFn = void(*)();

	using CreateObjectFn = void*(*)(TypeId, Bool32, const void**, int32_t);
	using InvokeMethodFn = void(*)(void*, ManagedHandle, const void**, int32_t);
	using InvokeMethodRetFn = void(*)(void*, ManagedHandle, const void**, int32_t, void*);
	using InvokeStaticMethodFn = void (*)(TypeId, ManagedHandle, const void**, int32_t);
	using InvokeStaticMethodRetFn = void (*)(TypeId, ManagedHandle, const void**, int32_t, void*);
	using SetFieldValueFn = void(*)(void*, String, void*);
	using GetFieldValueFn = void(*)(void*, String, void*);
	using GetFieldPointerFn = void(*)(void*, String, void**);
	using SetPropertyValueFn = void(*)(void*, String, void*);
	using GetPropertyValueFn = void(*)(void*, String, void*);
	using DestroyObjectFn = void(*)(void*);

#pragma region TypeInterface

	using GetAssemblyTypesFn = void(*)(int32_t, TypeId*, int32_t*);
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
	using GetMethodInfoReturnAttributesFn = void(*)(ManagedHandle, TypeId*, int32_t*);
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

#pragma region Other
	using IsClassFn = bool(*)(TypeId);
	using IsEnumFn = bool(*)(TypeId);
	using IsValueTypeFn = bool(*)(TypeId);
	using GetEnumNamesFn = void(*)(TypeId, String*, int32_t*);
	using GetEnumValuesFn = void(*)(TypeId, int32_t*, int32_t*);
#pragma endregion
	
	struct ManagedFunctions {
		SetInternalCallsFn SetInternalCallsFptr;
		LoadManagedAssemblyFn LoadManagedAssemblyFptr;
		UnloadAssemblyLoadContextFn UnloadAssemblyLoadContextFptr;
		GetLastLoadStatusFn GetLastLoadStatusFptr;
		GetAssemblyNameFn GetAssemblyNameFptr;

		CollectGarbageFn CollectGarbageFptr;
		WaitForPendingFinalizersFn WaitForPendingFinalizersFptr;

		CreateObjectFn CreateObjectFptr;
		CreateAssemblyLoadContextFn CreateAssemblyLoadContextFptr;
		InvokeMethodFn InvokeMethodFptr;
		InvokeMethodRetFn InvokeMethodRetFptr;
		InvokeStaticMethodFn InvokeStaticMethodFptr;
		InvokeStaticMethodRetFn InvokeStaticMethodRetFptr;
		SetFieldValueFn SetFieldValueFptr;
		GetFieldValueFn GetFieldValueFptr;
		GetFieldPointerFn GetFieldPointerFptr;
		SetPropertyValueFn SetPropertyValueFptr;
		GetPropertyValueFn GetPropertyValueFptr;
		DestroyObjectFn DestroyObjectFptr;
		
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
		GetMethodInfoReturnAttributesFn GetMethodInfoReturnAttributesFptr;
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

#pragma region Other
		IsClassFn IsClassFptr;
		IsEnumFn IsEnumFptr;
		IsValueTypeFn IsValueTypeFptr;
		GetEnumNamesFn GetEnumNamesFptr;
		GetEnumValuesFn GetEnumValuesFptr;
#pragma endregion
	};

	inline ManagedFunctions Managed;
}
