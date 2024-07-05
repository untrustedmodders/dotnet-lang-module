#include "managed_object.h"
#include "managed_assembly.h"
#include "managed_functions.h"
#include "strings.h"
#include "type.h"

using namespace netlm;

void ManagedObject::InvokeMethodInternal(ManagedHandle methodId, const void** parameters, size_t length) const {
	// NOTE: If you get an exception in this function it's most likely because you're using a Native only debugger type in Visual Studio
	//		 and it's catching a C# exception even though it shouldn't. I recommend switching the debugger type to Mixed (.NET Core)
	//		 which should be the default for Hazelnut, or simply press "Continue" until it works.
	//		 This is a problem with the Visual Studio debugger and nothing we can change.
	Managed.InvokeMethodFptr(_handle, methodId, parameters, static_cast<int32_t>(length));
}

void ManagedObject::InvokeMethodRetInternal(ManagedHandle methodId, const void** parameters, size_t length, void* resultStorage) const {
	Managed.InvokeMethodRetFptr(_handle, methodId, parameters, static_cast<int32_t>(length), resultStorage);
}

void ManagedObject::SetFieldValueRaw(std::string_view fieldName, void* inValue) const {
	auto name = String::New(fieldName);
	Managed.SetFieldValueFptr(_handle, name, inValue);
	String::Free(name);
}

void ManagedObject::GetFieldValueRaw(std::string_view fieldName, void* outValue) const {
	auto name = String::New(fieldName);
	Managed.GetFieldValueFptr(_handle, name, outValue);
	String::Free(name);
}

void ManagedObject::GetFieldPointerRaw(std::string_view fieldName, void** outPointer) const {
	auto name = String::New(fieldName);
	Managed.GetFieldPointerFptr(_handle, name, outPointer);
	String::Free(name);
}

void ManagedObject::SetPropertyValueRaw(std::string_view propertyName, void* inValue) const {
	auto name = String::New(propertyName);
	Managed.SetPropertyValueFptr(_handle, name, inValue);
	String::Free(name);
}

void ManagedObject::GetPropertyValueRaw(std::string_view propertyName, void* outValue) const {
	auto name = String::New(propertyName);
	Managed.GetPropertyValueFptr(_handle, name, outValue);
	String::Free(name);
}

const Type& ManagedObject::GetType() const {
	return *_type;
}

void ManagedObject::Destroy() {
	if (!_handle)
		return;

	Managed.DestroyObjectFptr(_handle);
	
	_handle = nullptr;
	_type = nullptr;
}
