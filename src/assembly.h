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

		explicit ClassHolder(const Assembly& ownerAssembly);
		ClassHolder(const ClassHolder&) = delete;
		ClassHolder& operator=(const ClassHolder&) = delete;
		ClassHolder(ClassHolder&&) noexcept = delete;
		ClassHolder& operator=(ClassHolder&&) noexcept = delete;
		~ClassHolder() = default;

		[[nodiscard]] bool CheckAssemblyLoaded() const;
		[[nodiscard]] const Assembly& GetOwnerAssembly() const { return _ownerAssembly; }
		[[nodiscard]] Class* GetOrCreateClassObject(int32_t typeHash, const char* typeName);
		[[nodiscard]] Class* FindClassByName(const char* typeName);
		[[nodiscard]] InvokeMethodFunction GetInvokeMethodFunction() const { return _invokeMethodFunction; }

		void SetInvokeMethodFunction(InvokeMethodFunction invokeMethodFptr) { _invokeMethodFunction = invokeMethodFptr; }

	private:
		const Assembly& _ownerAssembly;

		std::unordered_map<int32_t, std::unique_ptr<Class>> _classObjects;

		// Function pointer to invoke a managed method
		InvokeMethodFunction _invokeMethodFunction;
	};

	class Assembly {
	public:
		Assembly();
		Assembly(const Assembly&) = delete;
		Assembly& operator=(const Assembly&) = delete;
		Assembly(Assembly&&) noexcept = delete;
		Assembly& operator=(Assembly&&) noexcept = delete;
		~Assembly();

		[[nodiscard]] ManagedGuid& GetGuid() { return _guid; }
		[[nodiscard]] const ManagedGuid& GetGuid() const { return _guid; }
		[[nodiscard]] ClassHolder& GetClassObjectHolder() { return _classObjectHolder; }
		[[nodiscard]] const ClassHolder& GetClassObjectHolder() const { return _classObjectHolder; }
		[[nodiscard]] bool IsLoaded() const { return _guid.IsValid(); }

		bool Unload();

	private:
		ManagedGuid _guid;
		ClassHolder _classObjectHolder;
	};
}