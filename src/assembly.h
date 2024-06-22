#pragma once

#include "interop/managed_guid.h"

namespace netlm {
	struct ManagedMethod;

	class Class;
	class Assembly;

	class ClassHolder {
	public:
		friend class Assembly;

		using InvokeMethodFunction = void* (*) (ManagedGuid, ManagedGuid, void**, void*);

		ClassHolder(Assembly* owner_assembly);
		ClassHolder(const ClassHolder&) = delete;
		ClassHolder& operator=(const ClassHolder&) = delete;
		ClassHolder(ClassHolder&&) noexcept = delete;
		ClassHolder& operator=(ClassHolder&&) noexcept = delete;
		~ClassHolder() = default;

		[[nodiscard]] bool CheckAssemblyLoaded() const;

		[[nodiscard]] Assembly* GetOwnerAssembly() const { return m_owner_assembly; }

		[[nodiscard]] Class* GetOrCreateClassObject(int32_t type_hash, const char* type_name);

		[[nodiscard]] Class* FindClassByName(const char* type_name);

		[[nodiscard]] InvokeMethodFunction GetInvokeMethodFunction() const { return m_invoke_method_fptr; }

		void SetInvokeMethodFunction(InvokeMethodFunction invoke_method_fptr) { m_invoke_method_fptr = invoke_method_fptr; }

	private:
		Assembly* m_owner_assembly;

		HashMap<int32_t, UniquePtr<Class>> m_class_objects;

		// Function pointer to invoke a managed method
		InvokeMethodFunction m_invoke_method_fptr;
	};

	class Assembly {
	public:
		Assembly();
		Assembly(const Assembly&) = delete;
		Assembly& operator=(const Assembly&) = delete;
		Assembly(Assembly&&) noexcept = delete;
		Assembly& operator=(Assembly&&) noexcept = delete;
		~Assembly();

		[[nodiscard]] ManagedGuid& GetGuid() { return m_guid; }
		[[nodiscard]] const ManagedGuid& GetGuid() const { return m_guid; }
		[[nodiscard]] ClassHolder& GetClassObjectHolder() { return m_class_object_holder; }
		[[nodiscard]] const ClassHolder& GetClassObjectHolder() const { return m_class_object_holder; }
		[[nodiscard]] bool IsLoaded() const { return m_guid.IsValid(); }

		bool Unload();

	private:
		ManagedGuid m_guid;
		ClassHolder m_class_object_holder;
	};
}