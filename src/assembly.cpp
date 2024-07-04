#include "assembly.h"
#include "class.h"
#include "module.h"

#include "interop/managed_functions.h"

using namespace netlm;

std::string lastError;

std::unique_ptr<Assembly> Assembly::LoadFromPath(const std::filesystem::path& assemblyPath) {
	std::error_code ec;
	auto absolutePath= fs::absolute(assemblyPath, ec);
	if (ec) {
		lastError = std::format("Invalid file path: {}", assemblyPath.string());
		return nullptr;
	}

	auto assembly = std::unique_ptr<Assembly>(new Assembly);

	auto path = String::New(absolutePath.c_str());
	auto status = Managed.InitializeAssemblyFptr(path, &assembly->_guid, &assembly->_classObjectHolder);
	String::Free(path);

	if (status != AssemblyLoadStatus::Success) {
		switch (status) {
			case AssemblyLoadStatus::FileNotFound:
				lastError = "File not found";
				break;
			case AssemblyLoadStatus::FileLoadFailure:
				lastError = "File load failure";
				break;
			case AssemblyLoadStatus::InvalidFilePath:
				lastError = "Invalid file path";
				break;
			case AssemblyLoadStatus::InvalidAssembly:
				lastError = "Invalid assembly";
				break;
			case AssemblyLoadStatus::UnknownError:
				lastError = "Unknown error";
				break;
		}
		return nullptr;
	}

	//auto name = Managed.GetAssemblyNameFptr(&assembly->_guid);
	//assembly->_name = name;
	//String::Free(name);

	assembly->_loadStatus = status;

	// MOVE TO CLASS

	int32_t typeCount = 0;
	Managed.GetAssemblyTypesFptr(&assembly->_guid, nullptr, &typeCount);
	std::vector<TypeId> typeIds(static_cast<size_t>(typeCount));
	Managed.GetAssemblyTypesFptr(&assembly->_guid, typeIds.data(), &typeCount);

	for (auto typeId : typeIds) {
		assembly->_classObjectHolder._types.emplace_back(typeId);
	}

	return assembly;
}

const std::string& Assembly::GetError() {
	return lastError;
}

Assembly::Assembly()
	: _guid{0, 0},
	  _classObjectHolder(this) {
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

	Bool32 result = Managed.UnloadAssemblyFptr(&_guid);
	return bool(result);
}

ClassHolder::ClassHolder(Assembly* ownerAssembly)
	: _ownerAssembly(ownerAssembly),
	  _invokeMethodFunction(nullptr) {
}

bool ClassHolder::CheckAssemblyLoaded() const {
	if (_ownerAssembly) {
		return _ownerAssembly->IsLoaded();
	}

	return false;
}

Class* ClassHolder::GetOrCreateClassObject(int32_t typeHash, const char* typeName) {
	auto it = _classObjects.find(typeHash);

	if (it != _classObjects.end()) {
		return std::get<std::unique_ptr<Class>>(*it).get();
	}

	it = _classObjects.emplace(typeHash, std::make_unique<Class>(this, typeHash, typeName)).first;

	return std::get<std::unique_ptr<Class>>(*it).get();
}

Class* ClassHolder::FindClassByName(std::string_view typeName) const {
	for (const auto& [_, classPtr] : _classObjects) {
		if (classPtr->GetName() == typeName) {
			return classPtr.get();
		}
	}

	return nullptr;
}

Class* ClassHolder::FindClassBySubclass(Class* subClass) const {
	for (const auto& [_, classPtr] : _classObjects) {
		if (classPtr->GetType().IsSubclassOf(subClass->GetType())) {
			return classPtr.get();
		}
	}

	return nullptr;
}
