#pragma once

#pragma once

#include "core.h"
#include "strings.h"
#include "object.h"
#include "method_info.h"
#include "field_info.h"
#include "property_info.h"

namespace netlm {
	class Type {
	public:
		Type() = default;
		explicit Type(TypeId id) : _id{id} {}

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

		bool operator==(const Type& other) const;

		operator bool() const { return _id != -1; }

		TypeId GetTypeId() const { return _id; }

	private:
		TypeId _id{ -1 };
		std::unique_ptr<Type> _baseType;
		std::unique_ptr<Type> _elementType;

		friend class Assembly;
		friend class MethodInfo;
		friend class FieldInfo;
		friend class PropertyInfo;
		friend class Attribute;
	};
}
