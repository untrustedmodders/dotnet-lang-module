#pragma once

#include "interop/managed_guid.h"

namespace netlm {
	struct ManagedMethod;

	class Class;
	class Assembly;

	using ClassMap = std::unordered_map<int32_t, std::unique_ptr<Class>>;

	class ClassHolder {
	public:
		friend class Assembly;

		using InvokeMethodFunction = void* (*) (ManagedGuid, ManagedGuid, void**, void*);

		explicit ClassHolder(Assembly* ownerAssembly);
		ClassHolder(const ClassHolder&) = delete;
		ClassHolder& operator=(const ClassHolder&) = delete;
		ClassHolder(ClassHolder&&) noexcept = delete;
		ClassHolder& operator=(ClassHolder&&) noexcept = delete;
		~ClassHolder() = default;

		bool CheckAssemblyLoaded() const;
		Assembly* GetOwnerAssembly() const { return _ownerAssembly; }
		const ClassMap& GetClasses() const { return _classObjects; }
		Class* GetOrCreateClassObject(int32_t typeHash, const char* typeName);
		Class* FindClassByName(std::string_view typeName) const;
		Class* FindClassBySubClass(std::string_view typeName) const;
		InvokeMethodFunction GetInvokeMethodFunction() const { return _invokeMethodFunction; }

		void SetInvokeMethodFunction(InvokeMethodFunction invokeMethodFptr) { _invokeMethodFunction = invokeMethodFptr; }

	private:
		Assembly* _ownerAssembly;

		ClassMap _classObjects;

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

		ManagedGuid& GetGuid() { return _guid; }
		const ManagedGuid& GetGuid() const { return _guid; }
		ClassHolder& GetClassObjectHolder() { return _classObjectHolder; }
		const ClassHolder& GetClassObjectHolder() const { return _classObjectHolder; }
		bool IsLoaded() const { return _guid.IsValid(); }

		bool Unload();

	private:
		ManagedGuid _guid;
		ClassHolder _classObjectHolder;
	};
}