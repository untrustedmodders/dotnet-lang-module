#include "method_info.h"
#include "type.h"
#include "attribute.h"

#include "interop/managed_functions.h"

using namespace netlm;

std::string MethodInfo::GetName() const {
	auto name = Managed.GetMethodInfoNameFptr(_handle);
	std::string str(name);
	String::Free(name);
	return str;
}

Type& MethodInfo::GetReturnType() {
	if (!_returnType) {
		_returnType = std::make_unique<Type>();
		Managed.GetMethodInfoReturnTypeFptr(_handle, &_returnType->_id);
	}

	return *_returnType;
}

const std::vector<std::unique_ptr<Type>>& MethodInfo::GetParameterTypes() {
	if (_parameterTypes.empty()) {
		int32_t parameterCount;
		Managed.GetMethodInfoParameterTypesFptr(_handle, nullptr, &parameterCount);
		std::vector<TypeId> parameterTypes(static_cast<size_t>(parameterCount));
		Managed.GetMethodInfoParameterTypesFptr(_handle, parameterTypes.data(), &parameterCount);

		_parameterTypes.reserve(parameterTypes.size());

		for (TypeId parameterType : parameterTypes) {
			std::unique_ptr<Type>& type = _parameterTypes.emplace_back();
			type->_id = parameterType;
		}
	}

	return _parameterTypes;
}

TypeAccessibility MethodInfo::GetAccessibility() const {
	return Managed.GetMethodInfoAccessibilityFptr(_handle);
}

std::vector<Attribute> MethodInfo::GetAttributes() const {
	int32_t attributeCount;
	Managed.GetMethodInfoAttributesFptr(_handle, nullptr, &attributeCount);
	std::vector<ManagedHandle> attributeHandles(static_cast<size_t>(attributeCount));
	Managed.GetMethodInfoAttributesFptr(_handle, attributeHandles.data(), &attributeCount);

	std::vector<Attribute> result(attributeHandles.size());
	for (size_t i = 0; i < attributeHandles.size(); i++)
		result[i]._handle = attributeHandles[i];

	return result;
}
