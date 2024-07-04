#include "type.h"
#include "attribute.h"

#include "interop/managed_functions.h"

using namespace netlm;

std::string Type::GetFullName() const {
	auto name = Managed.GetFullTypeNameFptr(_id);
	std::string str(name);
	String::Free(name);
	return str;
}

std::string Type::GetAssemblyQualifiedName() const {
	auto name = Managed.GetAssemblyQualifiedNameFptr(_id);
	std::string str(name);
	String::Free(name);
	return str;
}

Type& Type::GetBaseType() {
	if (!_baseType) {
		_baseType = std::make_unique<Type>();
		Managed.GetBaseTypeFptr(_id, &_baseType->_id);
	}

	return *_baseType;
}

int32_t Type::GetSize() const {
	return Managed.GetTypeSizeFptr(_id);
}

bool Type::IsSubclassOf(const Type& other) const {
	return Managed.IsTypeSubclassOfFptr(_id, other._id);
}

bool Type::IsAssignableTo(const Type& other) const {
	return Managed.IsTypeAssignableToFptr(_id, other._id);
}

bool Type::IsAssignableFrom(const Type& other) const {
	return Managed.IsTypeAssignableFromFptr(_id, other._id);
}

std::vector<MethodInfo> Type::GetMethods() const {
	int32_t methodCount = 0;
	Managed.GetTypeMethodsFptr(_id, nullptr, &methodCount);
	std::vector<ManagedHandle> handles(static_cast<size_t>(methodCount));
	Managed.GetTypeMethodsFptr(_id, handles.data(), &methodCount);

	std::vector<MethodInfo> methods(handles.size());
	for (size_t i = 0; i < handles.size(); i++)
		methods[i]._handle = handles[i];

	return methods;
}

std::vector<FieldInfo> Type::GetFields() const {
	int32_t fieldCount = 0;
	Managed.GetTypeFieldsFptr(_id, nullptr, &fieldCount);
	std::vector<ManagedHandle> handles(static_cast<size_t>(fieldCount));
	Managed.GetTypeFieldsFptr(_id, handles.data(), &fieldCount);

	std::vector<FieldInfo> fields(handles.size());
	for (size_t i = 0; i < handles.size(); i++)
		fields[i]._handle = handles[i];

	return fields;
}

std::vector<PropertyInfo> Type::GetProperties() const {
	int32_t propertyCount = 0;
	Managed.GetTypePropertiesFptr(_id, nullptr, &propertyCount);
	std::vector<ManagedHandle> handles(static_cast<size_t>(propertyCount));
	Managed.GetTypePropertiesFptr(_id, handles.data(), &propertyCount);

	std::vector<PropertyInfo> properties(handles.size());
	for (size_t i = 0; i < handles.size(); i++)
		properties[i]._handle = handles[i];

	return properties;
}

MethodInfo Type::GetMethod(std::string_view methodName) const {
	auto name = String::New(methodName);
	ManagedHandle handle;
	Managed.GetTypeMethodFptr(_id, name, &handle);
	String::Free(name);

	MethodInfo result;
	result._handle = handle;

	return result;
}

FieldInfo Type::GetField(std::string_view fieldName) const {
	auto name = String::New(fieldName);
	ManagedHandle handle;
	Managed.GetTypeFieldFptr(_id, name, &handle);
	String::Free(name);

	FieldInfo result;
	result._handle = handle;

	return result;
}

PropertyInfo Type::GetProperty(std::string_view propertyName) const {
	auto name = String::New(propertyName);
	ManagedHandle handle;
	Managed.GetTypePropertyFptr(_id, name, &handle);
	String::Free(name);

	PropertyInfo result;
	result._handle = handle;

	return result;
}

bool Type::HasAttribute(const Type& attributeType) const {
	return Managed.HasTypeAttributeFptr(_id, attributeType._id);
}

std::vector<Attribute> Type::GetAttributes() const {
	int32_t attributeCount;
	Managed.GetTypeAttributesFptr(_id, nullptr, &attributeCount);
	std::vector<ManagedHandle> attributeHandles(static_cast<size_t>(attributeCount));
	Managed.GetTypeAttributesFptr(_id, attributeHandles.data(), &attributeCount);

	std::vector<Attribute> result(attributeHandles.size());
	for (size_t i = 0; i < attributeHandles.size(); i++)
		result[i]._handle = attributeHandles[i];

	return result;
}

ManagedType Type::GetManagedType() const {
	return Managed.GetTypeManagedTypeFptr(_id);
}

bool Type::IsClass() const {
	return Managed.IsClassFptr(_id);
}

bool Type::IsEnum() const {
	return Managed.IsEnumFptr(_id);
}

bool Type::IsValueType() const {
	return Managed.IsValueTypeFptr(_id);
}

bool Type::IsSZArray() const {
	return Managed.IsTypeSZArrayFptr(_id);
}

bool Type::IsByRef() const {
	return Managed.IsTypeByRefFptr(_id);
}

std::vector<std::string> Type::GetEnumNames() const {
	int size;
	Managed.GetEnumNamesFptr(_id, nullptr, &size);
	std::vector<String> names(static_cast<size_t>(size));
	Managed.GetEnumNamesFptr(_id, names.data(), &size);
	std::vector<std::string> namesStr;
	namesStr.reserve(static_cast<size_t>(size));
	for (auto& name : names) {
		namesStr.emplace_back(name);
		String::Free(name);
	}
	return namesStr;
}

std::vector<int> Type::GetEnumValues() const {
	int size;
	Managed.GetEnumValuesFptr(_id, nullptr, &size);
	std::vector<int> values(static_cast<size_t>(size));
	Managed.GetEnumValuesFptr(_id, values.data(), &size);
	return values;
}

Type& Type::GetElementType() {
	if (!_elementType) {
		_elementType = std::make_unique<Type>();
		Managed.GetElementTypeFptr(_id, &_elementType->_id);
	}

	return *_elementType;
}

bool Type::operator==(const Type& other) const {
	return _id == other._id;
}
