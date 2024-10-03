#pragma once

#include "core.h"

namespace netlm {
	class String;
	enum class AssemblyLoadStatus;
	enum class GCCollectionMode;
	struct ManagedType;
	class ManagedObject;

	using SetInternalCallsFn = void(*)(InternalCall*, int32_t, Bool32);
	using CreateAssemblyLoadContextFn = ManagedGuid(*)(String);
	using UnloadAssemblyLoadContextFn = void(*)(ManagedGuid);
	using LoadManagedAssemblyFn = ManagedGuid(*)(ManagedGuid, String);
	using GetLastLoadStatusFn = AssemblyLoadStatus(*)();
	using GetAssemblyNameFn = String(*)(ManagedGuid);

	using CollectGarbageFn = void(*)(int32_t, GCCollectionMode, Bool32, Bool32);
	using WaitForPendingFinalizersFn = void(*)();

	using CreateObjectFn = void*(*)(ManagedHandle, Bool32, const void**, int32_t);
	using InvokeMethodFn = void(*)(void*, ManagedHandle, const void**, int32_t);
	using InvokeMethodRetFn = void(*)(void*, ManagedHandle, const void**, int32_t, void*);
	using InvokeStaticMethodFn = void (*)(ManagedHandle, ManagedHandle, const void**, int32_t);
	using InvokeStaticMethodRetFn = void (*)(ManagedHandle, ManagedHandle, const void**, int32_t, void*);
	using SetFieldValueFn = void(*)(void*, String, void*);
	using GetFieldValueFn = void(*)(void*, String, void*);
	using GetFieldPointerFn = void(*)(void*, String, void**);
	using SetPropertyValueFn = void(*)(void*, String, void*);
	using GetPropertyValueFn = void(*)(void*, String, void*);
	using DestroyObjectFn = void(*)(void*);

#pragma region TypeInterface
	using GetAssemblyTypesFn = void(*)(ManagedGuid, ManagedHandle*, int32_t*);
	using GetTypeFn = void (*)(String, ManagedHandle*);
	using GetFullTypeNameFn = String(*)(ManagedHandle);
	using GetAssemblyQualifiedNameFn = String(*)(ManagedHandle);
	using GetBaseTypeFn = void(*)(ManagedHandle, ManagedHandle*);
	using GetTypeSizeFn = int32_t(*)(ManagedHandle);
	using IsTypeSubclassOfFn = Bool32(*)(ManagedHandle, ManagedHandle);
	using IsTypeAssignableToFn = Bool32(*)(ManagedHandle, ManagedHandle);
	using IsTypeAssignableFromFn = Bool32(*)(ManagedHandle, ManagedHandle);
	using IsTypeSZArrayFn = Bool32(*)(ManagedHandle);
	using IsTypeByRefFn = Bool32(*)(ManagedHandle);
	using GetElementTypeFn = void(*)(ManagedHandle, ManagedHandle*);
	using GetTypeMethodsFn = void(*)(ManagedHandle, ManagedHandle*, int32_t*);
	using GetTypeFieldsFn = void(*)(ManagedHandle, ManagedHandle*, int32_t*);
	using GetTypePropertiesFn = void(*)(ManagedHandle, ManagedHandle*, int32_t*);
	using GetTypeMethodFn = void(*)(ManagedHandle, String, ManagedHandle*);
	using GetTypeFieldFn = void(*)(ManagedHandle, String, ManagedHandle*);
	using GetTypePropertyFn = void(*)(ManagedHandle, String, ManagedHandle*);
	using HasTypeAttributeFn = Bool32(*)(ManagedHandle, ManagedHandle);
	using GetTypeAttributesFn = void (*)(ManagedHandle, ManagedHandle*, int32_t*);
	using GetTypeManagedTypeFn = ManagedType(*)(ManagedHandle);
#pragma endregion

#pragma region MethodInfo
	using GetMethodInfoNameFn = String(*)(ManagedHandle);
	using GetMethodInfoFunctionAddressFn = void*(*)(ManagedHandle);
	using GetMethodInfoReturnTypeFn = void(*)(ManagedHandle, ManagedHandle*);
	using GetMethodInfoParameterTypesFn = void(*)(ManagedHandle, ManagedHandle*, int32_t*);
	using GetMethodInfoAccessibilityFn = TypeAccessibility(*)(ManagedHandle);
	using GetMethodInfoAttributesFn = void(*)(ManagedHandle, ManagedHandle*, int32_t*);
	using GetMethodInfoParameterAttributesFn = void(*)(ManagedHandle, int32_t, ManagedHandle*, int32_t*);
	using GetMethodInfoReturnAttributesFn = void(*)(ManagedHandle, ManagedHandle*, int32_t*);
#pragma endregion

#pragma region FieldInfo
	using GetFieldInfoNameFn = String(*)(ManagedHandle);
	using GetFieldInfoTypeFn = void(*)(ManagedHandle, ManagedHandle*);
	using GetFieldInfoAccessibilityFn = TypeAccessibility(*)(ManagedHandle);
	using GetFieldInfoAttributesFn = void(*)(ManagedHandle, ManagedHandle*, int32_t*);
#pragma endregion

#pragma region PropertyInfo
	using GetPropertyInfoNameFn = String(*)(ManagedHandle);
	using GetPropertyInfoTypeFn = void(*)(ManagedHandle, ManagedHandle*);
	using GetPropertyInfoAttributesFn = void(*)(ManagedHandle, ManagedHandle*, int32_t*);
#pragma endregion

#pragma region Attribute
	using GetAttributeFieldValueFn = void(*)(ManagedHandle, String, void*);
	using GetAttributeTypeFn = void(*)(ManagedHandle, ManagedHandle*);
#pragma endregion

#pragma region Other
	using IsClassFn = bool(*)(ManagedHandle);
	using IsEnumFn = bool(*)(ManagedHandle);
	using IsValueTypeFn = bool(*)(ManagedHandle);
	using GetEnumNamesFn = void(*)(ManagedHandle, String*, int32_t*);
	using GetEnumValuesFn = void(*)(ManagedHandle, int32_t*, int32_t*);
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
		GetTypeFn GetTypeFptr;
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
		GetMethodInfoFunctionAddressFn GetMethodInfoFunctionAddressFptr;
		GetMethodInfoReturnTypeFn GetMethodInfoReturnTypeFptr;
		GetMethodInfoParameterTypesFn GetMethodInfoParameterTypesFptr;
		GetMethodInfoAccessibilityFn GetMethodInfoAccessibilityFptr;
		GetMethodInfoAttributesFn GetMethodInfoAttributesFptr;
		GetMethodInfoParameterAttributesFn GetMethodInfoParameterAttributesFptr;
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
