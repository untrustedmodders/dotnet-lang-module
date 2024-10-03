#pragma once

#include "core.h"
#include "type.h"

namespace netlm {

	enum class AssemblyLoadStatus {
		Success,
		FileNotFound,
		FileLoadFailure,
		InvalidFilePath,
		InvalidAssembly,
		UnknownError
	};

	class ManagedAssembly {
	public:
		ManagedGuid GetAssemblyID() const { return _assemblyId; }
		AssemblyLoadStatus GetLoadStatus() const { return _loadStatus; }
		std::string_view GetFullName() const { return _name; }

		void AddInternalCall(std::string_view className, std::string_view variableName, void* functionPtr);
		void UploadInternalCalls(bool warnOnMissing = true);

		Type& GetType(std::string_view className);
		const std::vector<Type>& GetTypes() const { return _types; }
		Type& GetTypeByBaseType(std::string_view baseName);

		bool operator==(const ManagedAssembly& other) const { return _assemblyId == other._assemblyId; }
		operator bool() const { return _assemblyId; }

	private:
		ManagedGuid _assemblyId{};
		AssemblyLoadStatus _loadStatus = AssemblyLoadStatus::UnknownError;
		std::string _name;
		std::vector<string_t> _internalCallNameStorage;
		std::vector<InternalCall> _internalCalls;
		std::vector<Type> _types;

		static inline Type InvalidType;

		friend class HostInstance;
		friend class AssemblyLoadContext;
	};

	using AssemblyMap = std::unordered_map<ManagedGuid, ManagedAssembly>;

	class AssemblyLoadContext {
	public:
		ManagedAssembly& LoadAssembly(const fs::path& assemblyPath);
		ManagedAssembly& FindAssembly(ManagedGuid assemblyId);
		AssemblyMap& GetLoadedAssemblies() { return _loadedAssemblies; }
		const std::string& GetError() { return _error; }

		bool operator==(const AssemblyLoadContext& other) const { return _contextId == other._contextId; }
		operator bool() const { return _contextId; }

	private:
		ManagedGuid _contextId{};
		AssemblyMap _loadedAssemblies;
		std::string _error;

		static inline ManagedAssembly InvalidAssembly;

		friend class HostInstance;
	};
}