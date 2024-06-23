#include "assembly.h"
#include "class.h"
#include "module.h"

namespace netlm {
	Assembly::Assembly()
		: _guid{0, 0},
		  _classObjectHolder(*this) {
	}

	Assembly::~Assembly() {
		if (!Unload()) {
			assert("Failed to unload assembly");
		}
	}

	bool Assembly::Unload() {
		if (!IsLoaded()) {
			return true;
		}

#ifndef NDEBUG
		// Sanity check - ensure all owned classes, methods, objects etc will not be dangling

		for (const auto& [_, classObject] : _classObjectHolder._classObjects) {
			if (!classObject) {
				continue;
			}
		}
#endif

		return g_netlm.UnloadAssembly(_guid);
	}

	ClassHolder::ClassHolder(const Assembly& ownerAssembly)
		: _ownerAssembly(ownerAssembly),
		  _invokeMethodFunction(nullptr) {
	}

	bool ClassHolder::CheckAssemblyLoaded() const {
		return _ownerAssembly.IsLoaded();
	}

	Class* ClassHolder::GetOrCreateClassObject(int32_t typeHash, const char* typeName) {
		auto it = _classObjects.find(typeHash);

		if (it != _classObjects.end()) {
			return it->second.get();
		}

		it = _classObjects.emplace(typeHash, std::make_unique<Class>(*this, typeName)).first;

		return it->second.get();
	}

	Class* ClassHolder::FindClassByName(const char* typeName) {
		for (const auto& [_, classPtr]: _classObjects) {
			if (classPtr->GetName() == typeName) {
				return classPtr.get();
			}
		}

		return nullptr;
	}
}