#include "assembly.h"
#include "class.h"

namespace netlm {
	Assembly::Assembly()
		: m_guid{0, 0},
		  m_class_object_holder(this) {
	}

	Assembly::~Assembly() {
		if (!Unload()) {
			//HYP_LOG(DotNET, LogLevel::WARNING, "Failed to unload assembly");
		}
	}

	bool Assembly::Unload() {
		if (!IsLoaded()) {
			return true;
		}

#ifdef _DEBUG
		// Sanity check - ensure all owned classes, methods, objects etc will not be dangling

		for (auto& it: m_class_object_holder.m_class_objects) {
			if (!it.second) {
				continue;
			}
		}
#endif

		return false;//DotNetSystem::GetInstance().UnloadAssembly(m_guid);
	}

	ClassHolder::ClassHolder(Assembly* owner_assembly)
		: m_owner_assembly(owner_assembly),
		  m_invoke_method_fptr(nullptr) {
	}

	bool ClassHolder::CheckAssemblyLoaded() const {
		if (m_owner_assembly) {
			return m_owner_assembly->IsLoaded();
		}

		return false;
	}

	Class* ClassHolder::GetOrCreateClassObject(int32_t type_hash, const char* type_name) {
		auto it = m_class_objects.find(type_hash);

		if (it != m_class_objects.end()) {
			return it->second.get();
		}

		it = m_class_objects.emplace(type_hash, std::make_unique<Class>(this, type_name)).first;

		return it->second.get();
	}

	Class* ClassHolder::FindClassByName(const char* type_name) {
		for (auto& pair: m_class_objects) {
			if (pair.second->GetName() == type_name) {
				return pair.second.get();
			}
		}

		return nullptr;
	}
}