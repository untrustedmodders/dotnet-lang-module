#include "attribute.h"
#include "type.h"

#include "interop/managed_functions.h"

using namespace netlm;

Type& Attribute::GetType() {
	if (!_type) {
		_type = std::make_unique<Type>();
		Managed.GetAttributeTypeFptr(_handle, &_type->_id);
	}

	return *_type;
}

void Attribute::GetFieldValue(std::string_view fieldName, void* valueVptr) const {
	auto field = String::New(fieldName);
	Managed.GetAttributeFieldValueFptr(_handle, field, valueVptr);
	String::Free(field);
}
