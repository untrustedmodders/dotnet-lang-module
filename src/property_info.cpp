#include "property_info.h"
#include "attribute.h"
#include "type.h"

#include "interop/managed_functions.h"

using namespace netlm;

std::string PropertyInfo::GetName() const {
	auto name = Managed.GetPropertyInfoNameFptr(_handle);
	std::string str(name);
	String::Free(name);
	return str;
}

Type& PropertyInfo::GetType() {
	if (!_type) {
		_type = std::make_unique<Type>();
		Managed.GetPropertyInfoTypeFptr(_handle, &_type->_id);
	}

	return *_type;
}

std::vector<Attribute> PropertyInfo::GetAttributes() const {
	int32_t attributeCount;
	Managed.GetPropertyInfoAttributesFptr(_handle, nullptr, &attributeCount);
	std::vector<ManagedHandle> attributeHandles(static_cast<size_t>(attributeCount));
	Managed.GetPropertyInfoAttributesFptr(_handle, attributeHandles.data(), &attributeCount);

	std::vector<Attribute> result(attributeHandles.size());
	for (size_t i = 0; i < attributeHandles.size(); i++)
		result[i]._handle = attributeHandles[i];

	return result;
}
