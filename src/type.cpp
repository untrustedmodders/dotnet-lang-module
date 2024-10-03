#include "type.h"
#include "attribute.h"
#include "managed_functions.h"

using namespace netlm;

std::string Type::GetFullName() const {
	auto name = Managed.GetFullTypeNameFptr(_handle);
	std::string str(name);
	String::Free(name);
	return str;
}

std::string Type::GetAssemblyQualifiedName() const {
	auto name = Managed.GetAssemblyQualifiedNameFptr(_handle);
	std::string str(name);
	String::Free(name);
	return str;
}

Type& Type::GetBaseType() {
	if (!_baseType) {
		_baseType = std::make_unique<Type>();
		Managed.GetBaseTypeFptr(_handle, &_baseType->_handle);
	}

	return *_baseType;
}

int32_t Type::GetSize() const {
	return Managed.GetTypeSizeFptr(_handle);
}

bool Type::IsSubclassOf(const Type& other) const {
	return Managed.IsTypeSubclassOfFptr(_handle, other._handle);
}

bool Type::IsAssignableTo(const Type& other) const {
	return Managed.IsTypeAssignableToFptr(_handle, other._handle);
}

bool Type::IsAssignableFrom(const Type& other) const {
	return Managed.IsTypeAssignableFromFptr(_handle, other._handle);
}

std::vector<MethodInfo> Type::GetMethods() const {
	int32_t methodCount = 0;
	Managed.GetTypeMethodsFptr(_handle, nullptr, &methodCount);
	std::vector<ManagedHandle> methodHandles(static_cast<size_t>(methodCount));
	Managed.GetTypeMethodsFptr(_handle, methodHandles.data(), &methodCount);

	std::vector<MethodInfo> methods(methodHandles.size());
	for (size_t i = 0; i < methodHandles.size(); i++)
		methods[i]._handle = methodHandles[i];

	return methods;
}

std::vector<FieldInfo> Type::GetFields() const {
	int32_t fieldCount = 0;
	Managed.GetTypeFieldsFptr(_handle, nullptr, &fieldCount);
	std::vector<ManagedHandle> fieldHandles(static_cast<size_t>(fieldCount));
	Managed.GetTypeFieldsFptr(_handle, fieldHandles.data(), &fieldCount);

	std::vector<FieldInfo> fields(fieldHandles.size());
	for (size_t i = 0; i < fieldHandles.size(); i++)
		fields[i]._handle = fieldHandles[i];

	return fields;
}

std::vector<PropertyInfo> Type::GetProperties() const {
	int32_t propertyCount = 0;
	Managed.GetTypePropertiesFptr(_handle, nullptr, &propertyCount);
	std::vector<ManagedHandle> propertyHandles(static_cast<size_t>(propertyCount));
	Managed.GetTypePropertiesFptr(_handle, propertyHandles.data(), &propertyCount);

	std::vector<PropertyInfo> properties(propertyHandles.size());
	for (size_t i = 0; i < propertyHandles.size(); i++)
		properties[i]._handle = propertyHandles[i];

	return properties;
}

MethodInfo Type::GetMethod(std::string_view methodName) const {
	auto name = String::New(methodName);
	ManagedHandle handle{};
	Managed.GetTypeMethodFptr(_handle, name, &handle);
	String::Free(name);

	MethodInfo result;
	result._handle = handle;

	return result;
}

FieldInfo Type::GetField(std::string_view fieldName) const {
	auto name = String::New(fieldName);
	ManagedHandle handle{};
	Managed.GetTypeFieldFptr(_handle, name, &handle);
	String::Free(name);

	FieldInfo result;
	result._handle = handle;

	return result;
}

PropertyInfo Type::GetProperty(std::string_view propertyName) const {
	auto name = String::New(propertyName);
	ManagedHandle handle{};
	Managed.GetTypePropertyFptr(_handle, name, &handle);
	String::Free(name);

	PropertyInfo result;
	result._handle = handle;

	return result;
}

bool Type::HasAttribute(const Type& attributeType) const {
	return Managed.HasTypeAttributeFptr(_handle, attributeType._handle);
}

std::vector<Attribute> Type::GetAttributes() const {
	int32_t attributeCount;
	Managed.GetTypeAttributesFptr(_handle, nullptr, &attributeCount);
	std::vector<ManagedHandle> attributeHandles(static_cast<size_t>(attributeCount));
	Managed.GetTypeAttributesFptr(_handle, attributeHandles.data(), &attributeCount);

	std::vector<Attribute> result(attributeHandles.size());
	for (size_t i = 0; i < attributeHandles.size(); i++)
		result[i]._handle = attributeHandles[i];

	return result;
}

ManagedType Type::GetManagedType() const {
	return Managed.GetTypeManagedTypeFptr(_handle);
}

bool Type::IsClass() const {
	return Managed.IsClassFptr(_handle);
}

bool Type::IsEnum() const {
	return Managed.IsEnumFptr(_handle);
}

bool Type::IsValueType() const {
	return Managed.IsValueTypeFptr(_handle);
}

bool Type::IsSZArray() const {
	return Managed.IsTypeSZArrayFptr(_handle);
}

bool Type::IsByRef() const {
	return Managed.IsTypeByRefFptr(_handle);
}

std::vector<std::string> Type::GetEnumNames() const {
	int size;
	Managed.GetEnumNamesFptr(_handle, nullptr, &size);
	std::vector<String> names(static_cast<size_t>(size));
	Managed.GetEnumNamesFptr(_handle, names.data(), &size);
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
	Managed.GetEnumValuesFptr(_handle, nullptr, &size);
	std::vector<int> values(static_cast<size_t>(size));
	Managed.GetEnumValuesFptr(_handle, values.data(), &size);
	return values;
}

Type& Type::GetElementType() {
	if (!_elementType) {
		_elementType = std::make_unique<Type>();
		Managed.GetElementTypeFptr(_handle, &_elementType->_handle);
	}

	return *_elementType;
}

// TODO: Cache methods

ManagedObject Type::CreateInstanceInternal(const void** parameters, size_t length) const {
	ManagedObject result;
	result._handle = Managed.CreateObjectFptr(_handle, false, parameters, static_cast<int32_t>(length));
	result._type = const_cast<Type*>(this);
	return result;
}

void Type::InvokeStaticMethodInternal(ManagedHandle methodHandle, const void** parameters, size_t length) const {
	Managed.InvokeStaticMethodFptr(_handle, methodHandle, parameters, static_cast<int32_t>(length));
}

void Type::InvokeStaticMethodRetInternal(ManagedHandle methodHandle, const void** parameters, size_t length, void* resultStorage) const {
	Managed.InvokeStaticMethodRetFptr(_handle, methodHandle, parameters, static_cast<int32_t>(length), resultStorage);
}