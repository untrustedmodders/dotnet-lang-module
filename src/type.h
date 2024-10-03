#pragma once

#include "core.h"
#include "field_info.h"
#include "managed_object.h"
#include "managed_type.h"
#include "method_info.h"
#include "native_string.h"
#include "property_info.h"

namespace netlm {
	class Type {
	public:
		Type() = default;
		explicit Type(ManagedHandle handle) : _handle{handle} {}
		//Type(const Type& type) : _handle{type._handle {}

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

		MethodInfo GetMethod(std::string_view methodName) const;
		FieldInfo GetField(std::string_view fieldName) const;
		PropertyInfo GetProperty(std::string_view propertyName) const;

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

		bool operator==(const Type& other) const { return _handle == other._handle; }
		operator bool() const { return _handle; }
		ManagedHandle GetHandle() const { return _handle; }

	public:
		template<typename... TArgs>
		ManagedObject CreateInstance(TArgs&&... arguments) {
			constexpr size_t argumentCount = sizeof...(arguments);

			ManagedObject result;

			if constexpr (argumentCount > 0) {
				const void* argumentsValues[] = { &arguments ... };
				result = CreateInstanceInternal(argumentsValues, argumentCount);
			} else {
				result = CreateInstanceInternal(nullptr, 0);
			}

			return result;
		}

		template<typename TReturn, typename... TArgs>
		TReturn InvokeStaticMethod(std::string_view methodName, TArgs&&... parameters) const {
			MethodInfo methodInfo = GetMethod(methodName);
			return InvokeStaticMethodRaw<TReturn, TArgs...>(methodInfo, std::forward<TArgs>(parameters)...);
		}

		template<typename... TArgs>
		void InvokeStaticMethod(std::string_view methodName, TArgs&&... parameters) const {
			MethodInfo methodInfo = GetMethod(methodName);
			InvokeStaticMethodRaw<TArgs...>(methodInfo, std::forward<TArgs>(parameters)...);
		}

		template<typename TReturn, typename... TArgs>
		TReturn InvokeStaticMethodRaw(const MethodInfo& methodInfo, TArgs&&... parameters) const {
			constexpr size_t parameterCount = sizeof...(parameters);

			TReturn result{};

			if constexpr (parameterCount > 0) {
				const void* parameterValues[] = { &parameters ... };
				InvokeStaticMethodRetInternal(methodInfo._handle, parameterValues, parameterCount, &result);
			} else {
				InvokeStaticMethodRetInternal(methodInfo._handle, nullptr, 0, &result);
			}

			return result;
		}

		template<typename... TArgs>
		void InvokeStaticMethodRaw(const MethodInfo& methodInfo, TArgs&&... parameters) const {
			constexpr size_t parameterCount = sizeof...(parameters);

			if constexpr (parameterCount > 0) {
				const void* parameterValues[] = { &parameters ... };
				InvokeStaticMethodInternal(methodInfo._handle, parameterValues, parameterCount);
			} else {
				InvokeStaticMethodInternal(methodInfo._handle, nullptr, 0);
			}
		}

	//private:
		ManagedObject CreateInstanceInternal(const void** arguments, size_t length) const;
		void InvokeStaticMethodInternal(ManagedHandle methodHandle, const void** parameters, size_t length) const;
		void InvokeStaticMethodRetInternal(ManagedHandle methodHandle, const void** parameters, size_t length, void* resultStorage) const;

	private:
		ManagedHandle _handle{};
		std::unique_ptr<Type> _baseType;
		std::unique_ptr<Type> _elementType;

		friend class ManagedAssembly;
		friend class MethodInfo;
		friend class FieldInfo;
		friend class PropertyInfo;
		friend class Attribute;
	};
}
