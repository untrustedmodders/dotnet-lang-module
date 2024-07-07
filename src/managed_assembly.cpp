#include "managed_assembly.h"
#include "managed_functions.h"
#include "utils.h"

using namespace netlm;

ManagedAssembly& AssemblyLoadContext::LoadAssembly(const fs::path& assemblyPath, int64_t assemblyKey) {
	std::error_code ec;
	auto absolutePath= fs::absolute(assemblyPath, ec);
	if (ec) {
		_error = std::format("Invalid file path: {}", assemblyPath.string());
		return InvalidAssembly;
	}

	auto path = String::New(absolutePath.c_str());
	auto assemblyId = Managed.LoadManagedAssemblyFptr(_contextId, path);
	auto loadStatus = Managed.GetLastLoadStatusFptr();
	String::Free(path);

	switch (loadStatus) {
		case AssemblyLoadStatus::FileNotFound:
			_error = "File not found";
			return InvalidAssembly;
		case AssemblyLoadStatus::FileLoadFailure:
			_error = "File load failure";
			return InvalidAssembly;
		case AssemblyLoadStatus::InvalidFilePath:
			_error = "Invalid file path";
			return InvalidAssembly;
		case AssemblyLoadStatus::InvalidAssembly:
			_error = "Invalid assembly";
			return InvalidAssembly;
		case AssemblyLoadStatus::UnknownError:
			_error = "Unknown error";
			return InvalidAssembly;
		case AssemblyLoadStatus::Success:
			break;
	}

	if (assemblyKey == -1) {
		assemblyKey = static_cast<int64_t>(assemblyId);
	}

	auto [it, result] = _loadedAssemblies.try_emplace(assemblyKey);
	if (!result) {
		_error = "Assembly key duplicate";
		return InvalidAssembly;
	}

	auto& assembly = std::get<ManagedAssembly>(*it);

	assembly._assemblyId = assemblyId;
	assembly._loadStatus = loadStatus;

	auto name = Managed.GetAssemblyNameFptr(assembly._assemblyId);
	assembly._name = name;
	String::Free(name);

	int32_t typeCount = 0;
	Managed.GetAssemblyTypesFptr(assembly._assemblyId, nullptr, &typeCount);
	std::vector<TypeId> typeIds(static_cast<size_t>(typeCount));
	Managed.GetAssemblyTypesFptr(assembly._assemblyId, typeIds.data(), &typeCount);

	for (auto typeId : typeIds) {
		assembly._types.emplace_back(typeId);
	}

	return assembly;
}

ManagedAssembly& AssemblyLoadContext::FindAssembly(int64_t assemblyId) {
	auto it = _loadedAssemblies.find(assemblyId);

	if (it != _loadedAssemblies.end()) {
		return std::get<ManagedAssembly>(*it);
	}

	return InvalidAssembly;
}

void ManagedAssembly::AddInternalCall(std::string_view className, std::string_view variableName, void* functionPtr) {
	assert(functionPtr != nullptr);

	std::string assemblyQualifiedName(std::format("{}@{}, {}", className, variableName, _name));

	const auto& name = _internalCallNameStorage.emplace_back(NETLM_WIDE(assemblyQualifiedName));

	_internalCalls.emplace_back(name.c_str(), functionPtr);
}

void ManagedAssembly::UploadInternalCalls() {
	Managed.SetInternalCallsFptr(_internalCalls.data(), static_cast<int32_t>(_internalCalls.size()));

	_internalCalls.clear();
	_internalCallNameStorage.clear();
}

Type& ManagedAssembly::GetType(std::string_view className) {
	for (auto& type : _types) {
		if (type.GetFullName() == className) {
			return type;
		}
	}
	return InvalidType;
}

Type& ManagedAssembly::GetTypeByBaseType(std::string_view baseName) {
	for (auto& type : _types) {
		if (type.GetBaseType().GetFullName() == baseName) {
			return type;
		}
	}
	return InvalidType;
}
