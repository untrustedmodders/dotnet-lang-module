#pragma once

#include "interop/managed_guid.h"
#include "type.h"

namespace netlm {
	struct ManagedMethod;

	enum class AssemblyLoadStatus {
		Success,
		FileNotFound,
		FileLoadFailure,
		InvalidFilePath,
		InvalidAssembly,
		UnknownError,
	};

	class Class;
	class Assembly;

	using ClassMap = std::unordered_map<int32_t, std::unique_ptr<Class>>;

	class ClassHolder {
	public:
		friend class Assembly;

		using InvokeMethodFunction = void* (*) (ManagedGuid, ManagedGuid, void**, void*);

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
		Class* FindClassBySubclass(Class* subClass) const;
		InvokeMethodFunction GetInvokeMethodFunction() const { return _invokeMethodFunction; }
		void SetInvokeMethodFunction(InvokeMethodFunction invokeMethodFptr) { _invokeMethodFunction = invokeMethodFptr; }

	private:
		explicit ClassHolder(Assembly* ownerAssembly);

	private:
		Assembly* _ownerAssembly;

		ClassMap _classObjects;
		std::vector<Type> _types;

		// Function pointer to invoke a managed method
		InvokeMethodFunction _invokeMethodFunction;
	};

	class Assembly {
	public:
		static std::unique_ptr<Assembly> LoadFromPath(const std::filesystem::path& assemblyPath);
		static const std::string& GetError();

		Assembly(const Assembly&) = delete;
		Assembly& operator=(const Assembly&) = delete;
		Assembly(Assembly&&) noexcept = delete;
		Assembly& operator=(Assembly&&) noexcept = delete;
		~Assembly();

		const ManagedGuid& GetGuid() { return _guid; }
		const ManagedGuid& GetGuid() const { return _guid; }
		const std::string& GetName() const { return _name; }
		AssemblyLoadStatus GetLoadStatus() const { return _loadStatus; }
		const ClassHolder& GetClassObjectHolder() const { return _classObjectHolder; }
		bool IsLoaded() const { return _loadStatus == AssemblyLoadStatus::Success; }

		bool Unload();

	private:
		Assembly();

	private:
		ManagedGuid _guid;
		std::string _name;
		AssemblyLoadStatus _loadStatus{ AssemblyLoadStatus::UnknownError };

		ClassHolder _classObjectHolder;
	};
}
