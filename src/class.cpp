#include "class.h"
#include "assembly.h"
#include "object.h"

namespace netlm {

	void Class::EnsureLoaded() const {
		if (!m_parent || !m_parent->CheckAssemblyLoaded()) {
			//HYP_THROW("Cannot use managed class: assembly has been unloaded");
		}
	}

	UniquePtr<Object> Class::NewObject() {
		EnsureLoaded();

		//AssertThrowMsg(m_new_object_fptr != nullptr, "New object function pointer not set for managed class %s", m_name.data());

		ManagedObject managed_object = m_new_object_fptr();

		return std::make_unique<Object>(this, managed_object);
	}

	void* Class::InvokeStaticMethod(const ManagedMethod* method_ptr, void** args_vptr, void* return_value_vptr) {
		EnsureLoaded();

		//AssertThrowMsg(m_parent->GetInvokeMethodFunction() != nullptr, "Invoke method function pointer not set");

		return m_parent->GetInvokeMethodFunction()(method_ptr->guid, {}, args_vptr, return_value_vptr);
	}

}// namespace netln