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
		int32_t GetAssemblyID() const { return _assemblyId; }
		AssemblyLoadStatus GetLoadStatus() const { return _loadStatus; }
		std::string_view GetFullName() const { return _name; }

		void AddInternalCall(std::string_view className, std::string_view variableName, void* functionPtr);
		void UploadInternalCalls();

		Type& GetType(std::string_view className);
		const std::vector<Type>& GetTypes() const { return _types; }
		Type& GetTypeByBaseType(std::string_view baseName);

		bool operator==(const ManagedAssembly& other) const { return _assemblyId == other._assemblyId; }
		operator bool() const { return _assemblyId != -1; }

	private:
		int32_t _assemblyId{ -1 };
		AssemblyLoadStatus _loadStatus{ AssemblyLoadStatus::UnknownError };
		std::string _name;
		std::vector<string_t> _internalCallNameStorage;
		std::vector<InternalCall> _internalCalls;
		std::vector<Type> _types;

		static inline Type InvalidType;

		friend class HostInstance;
		friend class AssemblyLoadContext;
	};

	using AssemblyList = std::vector<std::unique_ptr<ManagedAssembly>>;

	class AssemblyLoadContext {
	public:
		ManagedAssembly& LoadAssembly(const std::filesystem::path& assemblyPath);
		ManagedAssembly& FindAssembly(const std::string& assemblyName);
		const AssemblyList& GetLoadedAssemblies() const { return _loadedAssemblies; }
		const std::string& GetError() { return _error; }

		bool operator==(const AssemblyLoadContext& other) const { return _contextId == other._contextId; }
		operator bool() const { return _contextId != -1; }

	private:
		int32_t _contextId;
		AssemblyList _loadedAssemblies;
		std::string _error;

		static inline ManagedAssembly InvalidAssembly;

		friend class HostInstance;
	};
}