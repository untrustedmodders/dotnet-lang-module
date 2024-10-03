#include "field_info.h"
#include "attribute.h"
#include "type.h"
#include "managed_functions.h"

using namespace netlm;

std::string FieldInfo::GetName() const {
	auto name = Managed.GetFieldInfoNameFptr(_handle);
	std::string str(name);
	String::Free(name);
	return str;
}

Type& FieldInfo::GetType() {
	if (!_type) {
		_type = std::make_unique<Type>();
		Managed.GetFieldInfoTypeFptr(_handle, &_type->_handle);
	}

	return *_type;
}

TypeAccessibility FieldInfo::GetAccessibility() const {
	return Managed.GetFieldInfoAccessibilityFptr(_handle);
}

std::vector<Attribute> FieldInfo::GetAttributes() const {
	int32_t attributeCount;
	Managed.GetFieldInfoAttributesFptr(_handle, nullptr, &attributeCount);
	std::vector<ManagedHandle> attributeHandles(static_cast<size_t>(attributeCount));
	Managed.GetFieldInfoAttributesFptr(_handle, attributeHandles.data(), &attributeCount);

	std::vector<Attribute> result(attributeHandles.size());
	for (size_t i = 0; i < attributeHandles.size(); i++)
		result[i]._handle = attributeHandles[i];

	return result;
}
