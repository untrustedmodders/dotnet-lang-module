#pragma once

#include "core.h"
#include "strings.h"
#include "field_info.h"
#include "managed_object.h"
#include "managed_type.h"
#include "method_info.h"
#include "property_info.h"

namespace netlm {
	class Type {
	public:
		Type() = default;
		explicit Type(TypeId id) : _id{id} {}
		//Type(const Type& type) : _id{type._id} {}

		std::string GetFullName() const;
		std::string GetAssemblyQualifiedName() const;

		Type& GetBaseType();

		int32_t GetSize() const;

		bool IsSubclassOf(const Type& other) const;
		bool IsAssignableTo(const Type& other) const;
		bool IsAssignableFrom(const Type& other) const;

		std::vector<MethodInfo> GetMethods() const;
		std::vector<FieldInfo> GetFields() const;
		std::vector<PropertyInfo> GetProperties() const;

		MethodInfo GetMethod(std::string_view name) const;
		FieldInfo GetField(std::string_view name) const;
		PropertyInfo GetProperty(std::string_view name) const;

		bool HasAttribute(const Type& attributeType) const;
		std::vector<Attribute> GetAttributes() const;

		ManagedType GetManagedType() const;

		bool IsClass() const;
		bool IsEnum() const;
		bool IsValueType() const;

		std::vector<std::string> GetEnumNames() const;
		std::vector<int> GetEnumValues() const;

		bool IsSZArray() const;
		bool IsByRef() const;
		Type& GetElementType();

		bool operator==(const Type& other) const { return _id == other._id; }

		operator bool() const { return _id != -1; }

		TypeId GetTypeId() const { return _id; }

	public:
		template<typename... TArgs>
		ManagedObject CreateInstance(TArgs&&... arguments) {
			constexpr size_t argumentCount = sizeof...(arguments);

			ManagedObject result;

			if constexpr (argumentCount > 0) {
				const void* argumentsValues[] = { (void*) &arguments ... };
				result = CreateInstanceInternal(argumentsValues, argumentCount);
			} else {
				result = CreateInstanceInternal(nullptr, 0);
			}

			return result;
		}

		template<typename TReturn, typename... TArgs>
		TReturn InvokeStaticMethod(std::string_view methodName, TArgs&&... parameters) {
			constexpr size_t parameterCount = sizeof...(parameters);

			MethodInfo method = GetMethod(methodName);

			TReturn result;

			if constexpr (parameterCount > 0) {
				const void* parameterValues[] = { (void*) &parameters ... };
				InvokeStaticMethodRetInternal(method._handle, parameterValues, parameterCount, &result);
			} else {
				InvokeStaticMethodRetInternal(method._handle, nullptr, 0, &result);
			}

			return result;
		}

		template<typename... TArgs>
		void InvokeStaticMethod(std::string_view methodName, TArgs&&... parameters) {
			constexpr size_t parameterCount = sizeof...(parameters);

			MethodInfo method = GetMethod(methodName);

			if constexpr (parameterCount > 0) {
				const void* parameterValues[] = { (void*) &parameters ... };
				InvokeStaticMethodInternal(method._handle, parameterValues, parameterCount);
			} else {
				InvokeStaticMethodInternal(method._handle, nullptr, 0);
			}
		}

		ManagedObject CreateInstanceInternal(const void** arguments, size_t length);
		void InvokeStaticMethodInternal(ManagedHandle methodId, const void** parameters, size_t length) const;
		void InvokeStaticMethodRetInternal(ManagedHandle methodId, const void** parameters, size_t length, void* resultStorage) const;

	private:
		TypeId _id{ -1 };
		std::unique_ptr<Type> _baseType;
		std::unique_ptr<Type> _elementType;

		friend class ManagedAssembly;
		friend class MethodInfo;
		friend class FieldInfo;
		friend class PropertyInfo;
		friend class Attribute;
	};
}
