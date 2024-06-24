#include "object.h"
#include "class.h"
#include "assembly.h"

using namespace netlm;

Object::Object(Class* classPtr, ManagedObject managedObject)
	: _classPtr{classPtr},
	  _managedObject{managedObject} {
	assert(_classPtr != nullptr && "Class pointer not set!");

	assert(_classPtr->GetParent() != nullptr && "Class parent not set!");
	assert(_classPtr->GetParent()->GetInvokeMethodFunction() != nullptr && "Invoke method function pointer not set on class parent!");

	assert(_classPtr->GetNewObjectFunction() != nullptr && "New object function pointer not set!");
	assert(_classPtr->GetFreeObjectFunction() != nullptr && "Free object function pointer not set!");
}

Object::~Object() {
	_classPtr->EnsureLoaded();

	_classPtr->GetFreeObjectFunction()(_managedObject);
}

void* Object::InvokeMethod(const ManagedMethod* methodPtr, void** argsVptr, void* returnValueVptr) const {
	_classPtr->EnsureLoaded();

	return _classPtr->GetParent()->GetInvokeMethodFunction()(methodPtr->guid, _managedObject.guid, argsVptr, returnValueVptr);
}

const ManagedMethod* Object::GetMethod(const std::string& methodName) const {
	_classPtr->EnsureLoaded();

	auto it = _classPtr->GetMethods().find(methodName);

	if (it == _classPtr->GetMethods().end()) {
		return nullptr;
	}

	return &it->second;
}