#include "class.h"
#include "assembly.h"
#include "object.h"

using namespace netlm;

void Class::EnsureLoaded() const {
	if (!_parent || !_parent->CheckAssemblyLoaded()) {
		assert("Cannot use managed class: assembly has been unloaded");
	}
}

std::unique_ptr<Object> Class::NewObject() {
	EnsureLoaded();

	assert(_newObjectFunction != nullptr && "New object function pointer not set for managed class");

	ManagedObject managedObject = _newObjectFunction();

	return std::make_unique<Object>(this, managedObject);
}

void* Class::InvokeStaticMethod(const ManagedMethod* methodPtr, void** argsVptr, void* returnValueVptr) const {
	EnsureLoaded();

	auto invokeMethod = _parent->GetInvokeMethodFunction();

	assert(invokeMethod != nullptr && "Invoke method function pointer not set");

	return invokeMethod(methodPtr->guid, {}, argsVptr, returnValueVptr);
}

ManagedMethod* Class::GetMethod(const std::string& methodName) {
	auto it = _methods.find(methodName);
	if (it == _methods.end()) {
		return nullptr;
	}

	return &it->second;
}

const ManagedMethod* Class::GetMethod(const std::string& methodName) const {
	auto it = _methods.find(methodName);
	if (it == _methods.end()) {
		return nullptr;
	}

	return &it->second;
}