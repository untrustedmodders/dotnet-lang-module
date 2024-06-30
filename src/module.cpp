#include "module.h"
#include "library.h"
#include "assembly.h"
#include "class.h"
#include "object.h"
#include "strings.h"

#include "interop/managed_guid.h"
#include "interop/managed_type.h"

#include <module_export.h>

#include <plugify/compat_format.h>
#include <plugify/plugin_descriptor.h>
#include <plugify/plugin.h>
#include <plugify/module.h>
#include <plugify/plugify_provider.h>
#include <plugify/language_module.h>
#include <plugify/log.h>

#if NETLM_PLATFORM_WINDOWS
#include <strsafe.h>
#include <objbase.h>
#define memAlloc(t) ::CoTaskMemAlloc(t)
#define memFree(t) ::CoTaskMemFree(t)
#undef FindResource
#else
#define memAlloc(t) std::malloc(t)
#define memFree(t) std::free(t)
#endif

#define LOG_PREFIX "[NETLM] "

using namespace plugify;
using namespace netlm;

namespace {
	hostfxr_initialize_for_runtime_config_fn hostfxr_initialize_for_runtime_config;
	hostfxr_get_runtime_delegate_fn hostfxr_get_runtime_delegate;
	hostfxr_set_runtime_property_value_fn hostfxr_set_runtime_property_value;
	hostfxr_close_fn hostfxr_close;
	hostfxr_set_error_writer_fn hostfxr_set_error_writer;

	load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer;
	//get_function_pointer_fn get_function_pointer;
	//load_assembly_fn load_assembly;
}

extern const char* hostfxr_str_error(int32_t error);

InitResult DotnetLanguageModule::Initialize(std::weak_ptr<IPlugifyProvider> provider, const IModule& module) {
	if (!(_provider = provider.lock())) {
		return ErrorData{ "Provider not exposed" };
	}

	std::error_code ec;

	fs::path hostPath(module.GetBaseDir() / "dotnet/host/fxr/8.0.3/" NETLM_LIBRARY_PREFIX "hostfxr" NETLM_LIBRARY_SUFFIX);
	if (!fs::exists(hostPath, ec)) {
		return ErrorData{std::format("Host '{}' has not been found", hostPath.string())};
	}

	if (!LoadHostFXR(hostPath)) {
		return ErrorData{ std::format("Failed to load host assembly: {}", Library::GetError()) };
	}

	fs::path configPath(module.GetBaseDir() / "api/Plugify.runtimeconfig.json");
	if (!fs::exists(configPath, ec)) {
		return ErrorData{ std::format("Config '{}' has not been found", configPath.string()) };
	}

	auto error = InitializeRuntimeHost(configPath);
	if (!error.empty()) {
		return ErrorData{ std::move(error) };
	}

	fs::path assemblyPath(module.GetBaseDir() / "api/Plugify.dll");
	if (!fs::exists(assemblyPath, ec)) {
		return ErrorData{ std::format("Assembly '{}' has not been found", assemblyPath.string()) };
	}

	const char_t* className = STRING("Plugify.NativeInterop, Plugify");

	_initializeAssembly = GetDelegate<InitializeAssemblyDelegate>(
			assemblyPath.c_str(),
			className,
			STRING("InitializeAssembly"),
			UNMANAGEDCALLERSONLY_METHOD
	);
	if (_initializeAssembly == nullptr) {
		return ErrorData{ "InitializeAssembly could not be found in Plugify.dll! Ensure .NET libraries are properly compiled." };
	}

	_unloadAssembly = GetDelegate<UnloadAssemblyDelegate>(
			assemblyPath.c_str(),
			className,
			STRING("UnloadAssembly"),
			UNMANAGEDCALLERSONLY_METHOD
	);
	if (_unloadAssembly == nullptr) {
		return ErrorData{ "UnloadAssembly could not be found in Plugify.dll! Ensure .NET libraries are properly compiled." };
	}

	// Call the Initialize method in the NativeInterop class directly,
	// to load all the classes and methods into the class object holder

	auto rootAssembly = std::make_unique<Assembly>();
	auto absolutePath= fs::absolute(assemblyPath, ec).string();
	if (ec) {
		return ErrorData{ "Failed to get main assembly path" };
	}

	std::string errorStr;
	errorStr.resize(256);
	_initializeAssembly(
			errorStr.data(),
			&rootAssembly->GetGuid(),
			&rootAssembly->GetClassObjectHolder(),
			absolutePath.c_str());
	if (errorStr.empty()) {
		return ErrorData{std::move(errorStr)};
	}

	_rootAssembly = std::move(rootAssembly);

	_provider->Log(LOG_PREFIX "Inited!", Severity::Debug);

	_rt = std::make_shared<asmjit::JitRuntime>();

	return InitResultData{};
}

void DotnetLanguageModule::Shutdown() {
	_provider->Log(LOG_PREFIX "Shutting down .NET runtime", Severity::Debug);

	_scripts.clear();
	_nativesMap.clear();
	_exportMethods.clear();
	_functions.clear();
	_rootAssembly.reset();
	_ctx.reset();
	_dll.reset();
	_rt.reset();

	_provider->Log(LOG_PREFIX "Shut down .NET runtime", Severity::Debug);
	_provider.reset();
}

bool DotnetLanguageModule::LoadHostFXR(const fs::path& hostPath) {
	_provider->Log(std::format("Loading hostfxr from: {}", hostPath.string()), Severity::Debug);

	_dll = Library::LoadFromPath(hostPath);
	if (!_dll) {
		return false;
	}

	hostfxr_initialize_for_runtime_config = _dll->GetFunction<hostfxr_initialize_for_runtime_config_fn>("hostfxr_initialize_for_runtime_config");
	hostfxr_get_runtime_delegate = _dll->GetFunction<hostfxr_get_runtime_delegate_fn>("hostfxr_get_runtime_delegate");
	hostfxr_set_runtime_property_value = _dll->GetFunction<hostfxr_set_runtime_property_value_fn>("hostfxr_set_runtime_property_value");
	hostfxr_close = _dll->GetFunction<hostfxr_close_fn>("hostfxr_close");
	hostfxr_set_error_writer = _dll->GetFunction<hostfxr_set_error_writer_fn>("hostfxr_set_error_writer");

	_provider->Log("Loaded hostfxr functions", Severity::Debug);

	return hostfxr_initialize_for_runtime_config &&
		   hostfxr_get_runtime_delegate &&
		   hostfxr_set_runtime_property_value &&
		   hostfxr_close &&
		   hostfxr_set_error_writer;
}

std::string DotnetLanguageModule::InitializeRuntimeHost(const fs::path& configPath) {
	hostfxr_handle cxt = nullptr;
	int32_t result = hostfxr_initialize_for_runtime_config(configPath.c_str(), nullptr, &cxt);
	std::deleted_unique_ptr<void> context(cxt, hostfxr_close);

	if ((result < 0 || result > 2) || cxt == nullptr) {
		return std::format("Failed to initialize hostfxr: {:x} ({})", uint32_t(result), hostfxr_str_error(result));
	}

	// @TODO Add all necessary properties here
	//hostfxr_set_runtime_property_value(cxt, STRING("APP_CONTEXT_BASE_DIRECTORY"), basePath.c_str());

	result = hostfxr_get_runtime_delegate(cxt, hdt_load_assembly_and_get_function_pointer, reinterpret_cast<void**>(&load_assembly_and_get_function_pointer));
	if (result != 0 || load_assembly_and_get_function_pointer == nullptr) {
		return std::format("hostfxr_get_runtime_delegate::hdt_load_assembly_and_get_function_pointer failed: {:x} ({})", uint32_t(result), hostfxr_str_error(result));
	}
	/*result = hostfxr_get_runtime_delegate(cxt, hdt_get_function_pointer, reinterpret_cast<void**>(&get_function_pointer));
	if (result != 0 || get_function_pointer == nullptr) {
		return std::format("hostfxr_get_runtime_delegate::hdt_get_function_pointer failed: {:x} ({})", uint32_t(result), hostfxr_str_error(result));
	}
	result = hostfxr_get_runtime_delegate(cxt, hdt_load_assembly, reinterpret_cast<void**>(&load_assembly));
	if (result != 0 || load_assembly == nullptr) {
		return std::format("hostfxr_get_runtime_delegate::hdt_load_assembly failed: {:x} ({})", uint32_t(result), hostfxr_str_error(result));
	}*/

	_ctx = std::move(context);

	hostfxr_set_error_writer(ErrorWriter);

	return {};
}

LoadResult DotnetLanguageModule::OnPluginLoad(const IPlugin& plugin) {
	fs::path assemblyPath(plugin.GetBaseDir() / plugin.GetDescriptor().entryPoint);

	std::error_code ec;
	auto absolutePath= fs::absolute(assemblyPath, ec).string();
	if (ec) {
		return ErrorData{ "Failed to get assembly path" };
	}

	auto assembly = std::make_unique<Assembly>();

	std::string errorStr;
	errorStr.resize(256);
	_initializeAssembly(
			errorStr.data(),
			&assembly->GetGuid(),
			&assembly->GetClassObjectHolder(),
			absolutePath.c_str()
	);
	if (errorStr.empty()) {
		return ErrorData{ std::move(errorStr) };
	}

	Class* pluginClassPtr = assembly->GetClassObjectHolder().FindClassBySubClass("Plugin");
	if (!pluginClassPtr) {
		return ErrorData{"Failed to find 'Plugin' class implementation"};
	}

	std::vector<std::string> methodErrors;

	const auto& exportedMethods = plugin.GetDescriptor().exportedMethods;
	std::vector<MethodData> methods;
	methods.reserve(exportedMethods.size());

	for (const auto& method : exportedMethods) {
		auto separated = String::Split(method.funcName, ".");
		if (separated.size() != 3) {
			methodErrors.emplace_back(std::format("Invalid function name: '{}'. Please provide name in that format: 'Namespace.Class.Method'", method.funcName));
			continue;
		}

		Class* classPtr = assembly->GetClassObjectHolder().FindClassByName(separated[1]);
		if (!classPtr) {
			methodErrors.emplace_back(std::format("Failed to find class '{}'", method.funcName));
			continue;
		}

		ManagedMethod* methodPtr = classPtr->GetMethod(separated[2].data());
		if (!methodPtr) {
			methodErrors.emplace_back(std::format("Failed to find method '{}'", method.funcName));
			continue;
		}

		const ValueType& returnType = methodPtr->returnType.type;
		const ValueType& methodReturnType = method.retType.type;
		if (returnType != methodReturnType) {
			methodErrors.emplace_back(std::format("Method '{}' has invalid return type '{}' when it should have '{}'", method.funcName, ValueTypeToString(methodReturnType), ValueTypeToString(returnType)));
			continue;
		}

		size_t paramCount = methodPtr->parameterTypes.size();
		if (paramCount != method.paramTypes.size()) {
			methodErrors.emplace_back(std::format("Method '{}' has invalid parameter count {} when it should have {}", method.funcName, method.paramTypes.size(), paramCount));
			continue;
		}

		bool methodFail = false;

		for (size_t i = 0; i < paramCount; ++i) {
			const ValueType& paramType = methodPtr->parameterTypes[i].type;
			const ValueType& methodParamType = method.paramTypes[i].type;
			if (paramType != methodParamType) {
				methodFail = true;
				methodErrors.emplace_back(std::format("Method '{}' has invalid param type '{}' at index {} when it should have '{}'", method.funcName, ValueTypeToString(methodParamType), i, ValueTypeToString(paramType)));
				continue;
			}

		}

		if (methodFail)
			continue;

		auto exportMethod = std::make_unique<ExportMethod>(classPtr, methodPtr);

		Function function(_rt);
		void* methodAddr = function.GetJitFunc(method, &InternalCall, exportMethod.get());
		if (!methodAddr) {
			methodErrors.emplace_back(std::format("Method JIT generation error: ", function.GetError()));
			continue;
		}
		_functions.emplace(exportMethod.get(), std::move(function));
		_exportMethods.emplace_back(std::move(exportMethod));

		methods.emplace_back(method.name, methodAddr);
	}

	if (!methodErrors.empty()) {
		std::string funcs(methodErrors[0]);
		for (auto it = std::next(methodErrors.begin()); it != methodErrors.end(); ++it) {
			std::format_to(std::back_inserter(funcs), ", {}", *it);
		}
		return ErrorData{ std::format("Not found {} method function(s)", funcs) };
	}

	const auto [_, result] = _scripts.try_emplace(plugin.GetName(), plugin, std::move(assembly), pluginClassPtr->NewObject());
	if (!result) {
		return ErrorData{ std::format("Plugin name duplicate") };
	}

	return LoadResultData{ std::move(methods) };
}

template<typename T>
T DotnetLanguageModule::GetDelegate(const char_t* assemblyPath, const char_t* typeName, const char_t* methodName, const char_t* delegateTypeName) const {
	_provider->Log(std::format("Loading .NET assembly: {}\tType Name: {}\tMethod Name: {}", STR(assemblyPath), STR(typeName), STR(methodName)), Severity::Info);

	T delegatePtr = nullptr;

	int32_t result = load_assembly_and_get_function_pointer(
			assemblyPath,
			typeName,
			methodName,
			delegateTypeName,
			nullptr,
			reinterpret_cast<void**>(&delegatePtr));

	if (result != 0) {
		_provider->Log(std::format("Failed to load assembly and get function pointer: {:x} ({})", uint32_t(result), hostfxr_str_error(result)), Severity::Error);
		return nullptr;
	}

	return delegatePtr;
}

void DotnetLanguageModule::OnPluginStart(const IPlugin& plugin) {
	ScriptInstance* script = FindScript(plugin.GetName());
	if (script) {
		script->InvokeOnStart();
	}
}

void DotnetLanguageModule::OnPluginEnd(const IPlugin& plugin) {
	ScriptInstance* script = FindScript(plugin.GetName());
	if (script) {
		script->InvokeOnEnd();
	}
}

void* DotnetLanguageModule::GetNativeMethod(const std::string& methodName) const {
	if (const auto it = _nativesMap.find(methodName); it != _nativesMap.end()) {
		return std::get<void*>(*it);
	}
	_provider->Log(std::format(LOG_PREFIX "GetNativeMethod failed to find: '{}'", methodName), Severity::Fatal);
	return nullptr;
}

void DotnetLanguageModule::OnMethodExport(const IPlugin& plugin) {
	const auto& pluginName = plugin.GetName();
	for (const auto& [name, addr] : plugin.GetMethods()) {
		_nativesMap.try_emplace(std::format("{}.{}", pluginName, name), addr);
	}
}

std::unique_ptr<Assembly> DotnetLanguageModule::LoadAssembly(const fs::path& assemblyPath) const {
	auto assembly = std::make_unique<Assembly>();

	assert(_rootAssembly != nullptr);

	_initializeAssembly(
			nullptr,
			&assembly->GetGuid(),
			&assembly->GetClassObjectHolder(),
			assemblyPath.string().c_str()
	);

	return assembly;
}

bool DotnetLanguageModule::UnloadAssembly(ManagedGuid assemblyGuid) const {
	int32_t result;
	_unloadAssembly(&assemblyGuid, &result);
	return bool(result);
}

ScriptInstance* DotnetLanguageModule::FindScript(const std::string& name) {
	auto it = _scripts.find(name);
	if (it != _scripts.end())
		return &std::get<ScriptInstance>(*it);
	return nullptr;
}

// C++ to C#
void DotnetLanguageModule::InternalCall(const plugify::Method* method, void* data, const plugify::Parameters* p, uint8_t count, const plugify::ReturnValue* ret) {
	const auto& [classPtr, methodPtr] = *reinterpret_cast<ExportMethod*>(data);

	bool hasRet = ValueTypeIsHiddenObjectParam(method->retType.type);

	std::vector<void*> args;
	args.reserve(hasRet ? count - 1 : count);

	for (uint8_t i = hasRet, j = 0; i < count; ++i, ++j) {
		void* arg;
		const auto& param = method->paramTypes[j];
		if (param.ref) {
			arg = p->GetArgument<void*>(i);
		} else {
			switch (param.type) {
				// Value types
				case ValueType::Bool:
				case ValueType::Char8:
				case ValueType::Char16:
				case ValueType::Int8:
				case ValueType::Int16:
				case ValueType::Int32:
				case ValueType::Int64:
				case ValueType::UInt8:
				case ValueType::UInt16:
				case ValueType::UInt32:
				case ValueType::UInt64:
				case ValueType::Pointer:
				case ValueType::Float:
				case ValueType::Double:
					arg = p->GetArgumentPtr(i);
					break;
				// Ref types
				case ValueType::Function:
				case ValueType::Vector2:
				case ValueType::Vector3:
				case ValueType::Vector4:
				case ValueType::Matrix4x4:
				case ValueType::String:
				case ValueType::ArrayBool:
				case ValueType::ArrayChar8:
				case ValueType::ArrayChar16:
				case ValueType::ArrayInt8:
				case ValueType::ArrayInt16:
				case ValueType::ArrayInt32:
				case ValueType::ArrayInt64:
				case ValueType::ArrayUInt8:
				case ValueType::ArrayUInt16:
				case ValueType::ArrayUInt32:
				case ValueType::ArrayUInt64:
				case ValueType::ArrayPointer:
				case ValueType::ArrayFloat:
				case ValueType::ArrayDouble:
				case ValueType::ArrayString:
					arg = p->GetArgument<void*>(i);
					break;
				default:
					std::puts("Unsupported types!\n");
					std::terminate();
					break;
			}
		}
		args.push_back(arg);
	}

	void* retPtr = hasRet ? p->GetArgument<void*>(0) : ret->GetReturnPtr();

	if (hasRet) {
		switch (method->retType.type) {
			case ValueType::String:
				std::construct_at(reinterpret_cast<std::string*>(retPtr), std::string());
				break;
			case ValueType::ArrayBool:
				std::construct_at(reinterpret_cast<std::vector<bool>*>(retPtr), std::vector<bool>());
				break;
			case ValueType::ArrayChar8:
				std::construct_at(reinterpret_cast<std::vector<char>*>(retPtr), std::vector<char>());
				break;
			case ValueType::ArrayChar16:
				std::construct_at(reinterpret_cast<std::vector<char16_t>*>(retPtr), std::vector<char16_t>());
				break;
			case ValueType::ArrayInt8:
				std::construct_at(reinterpret_cast<std::vector<int8_t>*>(retPtr), std::vector<int8_t>());
				break;
			case ValueType::ArrayInt16:
				std::construct_at(reinterpret_cast<std::vector<int16_t>*>(retPtr), std::vector<int16_t>());
				break;
			case ValueType::ArrayInt32:
				std::construct_at(reinterpret_cast<std::vector<int32_t>*>(retPtr), std::vector<int32_t>());
				break;
			case ValueType::ArrayInt64:
				std::construct_at(reinterpret_cast<std::vector<int64_t>*>(retPtr), std::vector<int64_t>());
				break;
			case ValueType::ArrayUInt8:
				std::construct_at(reinterpret_cast<std::vector<uint8_t>*>(retPtr), std::vector<uint8_t>());
				break;
			case ValueType::ArrayUInt16:
				std::construct_at(reinterpret_cast<std::vector<uint16_t>*>(retPtr), std::vector<uint16_t>());
				break;
			case ValueType::ArrayUInt32:
				std::construct_at(reinterpret_cast<std::vector<uint32_t>*>(retPtr), std::vector<uint32_t>());
				break;
			case ValueType::ArrayUInt64:
				std::construct_at(reinterpret_cast<std::vector<uint64_t>*>(retPtr), std::vector<uint64_t>());
				break;
			case ValueType::ArrayPointer:
				std::construct_at(reinterpret_cast<std::vector<uintptr_t>*>(retPtr), std::vector<uintptr_t>());
				break;
			case ValueType::ArrayFloat:
				std::construct_at(reinterpret_cast<std::vector<float>*>(retPtr), std::vector<float>());
				break;
			case ValueType::ArrayDouble:
				std::construct_at(reinterpret_cast<std::vector<double>*>(retPtr), std::vector<double>());
				break;
			case ValueType::ArrayString:
				std::construct_at(reinterpret_cast<std::vector<std::string>*>(retPtr), std::vector<std::string>());
				break;
			default:
				break;
		}
	}

	classPtr->InvokeStaticMethod(methodPtr, args.data(), retPtr);

	if (hasRet) {
		ret->SetReturnPtr(retPtr);
	}
}

/*_________________________________________________*/

ScriptInstance::ScriptInstance(const IPlugin& plugin, std::unique_ptr<Assembly> assembly, std::unique_ptr<Object> instance)
	: _plugin{plugin},
	  _assembly{std::move(assembly)},
	  _instance{std::move(instance)} {

	const auto& desc = plugin.GetDescriptor();
	std::vector<std::string> deps;
	deps.reserve(desc.dependencies.size());
	for (const auto& dependency : desc.dependencies) {
		deps.emplace_back(dependency.name);
	}
	_instance->InvokeMethodByName<void>("set_Id", plugin.GetId());
	_instance->InvokeMethodByName<void>("set_Name", plugin.GetName());
	_instance->InvokeMethodByName<void>("set_FullName", plugin.GetFriendlyName());
	_instance->InvokeMethodByName<void>("set_Description", desc.description);
	_instance->InvokeMethodByName<void>("set_Version", desc.versionName);
	_instance->InvokeMethodByName<void>("set_Author", desc.createdBy);
	_instance->InvokeMethodByName<void>("set_Website", desc.createdByURL);
	_instance->InvokeMethodByName<void>("set_BaseDir", plugin.GetBaseDir().string());
	_instance->InvokeMethodByName<void>("set_Dependencies", deps);
}

void ScriptInstance::InvokeOnStart() const {
	ManagedMethod* onStartMethod = _instance->GetClass()->GetMethod("OnStart");
	if (onStartMethod) {
		_instance->InvokeMethod<void>(onStartMethod);
	}
}

void ScriptInstance::InvokeOnEnd() const {
	ManagedMethod* onEndMethod = _instance->GetClass()->GetMethod("OnEnd");
	if (onEndMethod) {
		_instance->InvokeMethod<void>(onEndMethod);
	}
}

namespace netlm {
	DotnetLanguageModule g_netlm;
}

extern "C" {
	NETLM_EXPORT void* GetMethodPtr(const char* methodName) {
		return g_netlm.GetNativeMethod(methodName);
	}

	NETLM_EXPORT const char* GetBaseDir() {
		auto source = g_netlm.GetProvider()->GetBaseDir().string();
		size_t size = source.length() + 1;
		char* dest = reinterpret_cast<char*>(memAlloc(size));
		std::memcpy(dest, source.c_str(), size);
		return dest;
	}

	NETLM_EXPORT bool IsModuleLoaded(const char* moduleName, int version, bool minimum) {
		auto requiredVersion = (version >= 0 && version != INT_MAX) ? std::make_optional(version) : std::nullopt;
		return g_netlm.GetProvider()->IsModuleLoaded(moduleName, requiredVersion, minimum);
	}

	NETLM_EXPORT bool IsPluginLoaded(const char* pluginName, int version, bool minimum) {
		auto requiredVersion = (version >= 0 && version != INT_MAX) ? std::make_optional(version) : std::nullopt;
		return g_netlm.GetProvider()->IsPluginLoaded(pluginName, requiredVersion, minimum);
	}

	NETLM_EXPORT void Log(Severity severity, const char* funcName, uint32_t line, const char* message) {
		g_netlm.GetProvider()->Log(std::format(LOG_PREFIX "{}:{}: {}", funcName, line, message), severity);
	}

	NETLM_EXPORT const char* FindPluginResource(const char* pluginName, const char* path) {
		ScriptInstance* script = g_netlm.FindScript(pluginName);
		if (script) {
			auto resource = script->GetPlugin().FindResource(path);
			if (resource.has_value()) {
				auto source= resource->string();
				size_t size = source.length() + 1;
				char* dest = reinterpret_cast<char*>(memAlloc(size));
				std::memcpy(dest, source.c_str(), size);
				return dest;
			}
		}
		return nullptr;
	}

	// String Functions

	NETLM_EXPORT std::string* AllocateString() {
		return static_cast<std::string*>(malloc(sizeof(std::string)));
	}
	NETLM_EXPORT void* CreateString(const char* source) {
		return source == nullptr ? new std::string() : new std::string(source);
	}
	NETLM_EXPORT const char* GetStringData(std::string* string) {
		size_t size = string->length() + 1;
		char* str = reinterpret_cast<char*>(memAlloc(size));
		std::memcpy(str, string->c_str(), size);
		return str;
	}
	NETLM_EXPORT int GetStringLength(std::string* string) {
		return static_cast<int>(string->length());
	}
	NETLM_EXPORT void ConstructString(std::string* string, const char* source) {
		std::construct_at(string, source);
	}
	NETLM_EXPORT void AssignString(std::string* string, const char* source) {
		if (source == nullptr)
			string->clear();
		else
			string->assign(source);
	}
	NETLM_EXPORT void FreeString(std::string* string) {
		string->~basic_string();
		free(string);
	}
	NETLM_EXPORT void DeleteString(std::string* string) {
		delete string;
	}

	// CreateVector Functions

	NETLM_EXPORT std::vector<bool>* CreateVectorBool(bool* arr, int len) {
		return len == 0 ? new std::vector<bool>() : new std::vector<bool>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<char>* CreateVectorChar8(char* arr, int len) {
		return len == 0 ? new std::vector<char>() : new std::vector<char>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<char16_t>* CreateVectorChar16(char16_t* arr, int len) {
		return len == 0 ? new std::vector<char16_t>() : new std::vector<char16_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<int8_t>* CreateVectorInt8(int8_t* arr, int len) {
		return len == 0 ? new std::vector<int8_t>() : new std::vector<int8_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<int16_t>* CreateVectorInt16(int16_t* arr, int len) {
		return len == 0 ? new std::vector<int16_t>() : new std::vector<int16_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<int32_t>* CreateVectorInt32(int32_t* arr, int len) {
		return len == 0 ? new std::vector<int32_t>() : new std::vector<int32_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<int64_t>* CreateVectorInt64(int64_t* arr, int len) {
		return len == 0 ? new std::vector<int64_t>() : new std::vector<int64_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<uint8_t>* CreateVectorUInt8(uint8_t* arr, int len) {
		return len == 0 ? new std::vector<uint8_t>() : new std::vector<uint8_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<uint16_t>* CreateVectorUInt16(uint16_t* arr, int len) {
		return len == 0 ? new std::vector<uint16_t>() : new std::vector<uint16_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<uint32_t>* CreateVectorUInt32(uint32_t* arr, int len) {
		return len == 0 ? new std::vector<uint32_t>() : new std::vector<uint32_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<uint64_t>* CreateVectorUInt64(uint64_t* arr, int len) {
		return len == 0 ? new std::vector<uint64_t>() : new std::vector<uint64_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<uintptr_t>* CreateVectorIntPtr(uintptr_t* arr, int len) {
		return len == 0 ? new std::vector<uintptr_t>() : new std::vector<uintptr_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<float>* CreateVectorFloat(float* arr, int len) {
		return len == 0 ? new std::vector<float>() : new std::vector<float>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<double>* CreateVectorDouble(double* arr, int len) {
		return len == 0 ? new std::vector<double>() : new std::vector<double>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<std::string>* CreateVectorString(char* arr[], int len) {
		return len == 0 ? new std::vector<std::string>() : new std::vector<std::string>(arr, arr + len);
	}

	// AllocateVector Functions

	NETLM_EXPORT std::vector<bool>* AllocateVectorBool() {
		return static_cast<std::vector<bool>*>(malloc(sizeof(std::vector<bool>)));
	}

	NETLM_EXPORT std::vector<char>* AllocateVectorChar8() {
		return static_cast<std::vector<char>*>(malloc(sizeof(std::vector<char>)));
	}

	NETLM_EXPORT std::vector<char16_t>* AllocateVectorChar16() {
		return static_cast<std::vector<char16_t>*>(malloc(sizeof(std::vector<char16_t>)));
	}

	NETLM_EXPORT std::vector<int8_t>* AllocateVectorInt8() {
		return static_cast<std::vector<int8_t>*>(malloc(sizeof(std::vector<int8_t>)));
	}

	NETLM_EXPORT std::vector<int16_t>* AllocateVectorInt16() {
		return static_cast<std::vector<int16_t>*>(malloc(sizeof(std::vector<int16_t>)));
	}

	NETLM_EXPORT std::vector<int32_t>* AllocateVectorInt32() {
		return static_cast<std::vector<int32_t>*>(malloc(sizeof(std::vector<int32_t>)));
	}

	NETLM_EXPORT std::vector<int64_t>* AllocateVectorInt64() {
		return static_cast<std::vector<int64_t>*>(malloc(sizeof(std::vector<int64_t>)));
	}

	NETLM_EXPORT std::vector<uint8_t>* AllocateVectorUInt8() {
		return static_cast<std::vector<uint8_t>*>(malloc(sizeof(std::vector<uint8_t>)));
	}

	NETLM_EXPORT std::vector<uint16_t>* AllocateVectorUInt16() {
		return static_cast<std::vector<uint16_t>*>(malloc(sizeof(std::vector<uint16_t>)));
	}

	NETLM_EXPORT std::vector<uint32_t>* AllocateVectorUInt32() {
		return static_cast<std::vector<uint32_t>*>(malloc(sizeof(std::vector<uint32_t>)));
	}

	NETLM_EXPORT std::vector<uint64_t>* AllocateVectorUInt64() {
		return static_cast<std::vector<uint64_t>*>(malloc(sizeof(std::vector<uint64_t>)));
	}

	NETLM_EXPORT std::vector<uintptr_t>* AllocateVectorIntPtr() {
		return static_cast<std::vector<uintptr_t>*>(malloc(sizeof(std::vector<uintptr_t>)));
	}

	NETLM_EXPORT std::vector<float>* AllocateVectorFloat() {
		return static_cast<std::vector<float>*>(malloc(sizeof(std::vector<float>)));
	}

	NETLM_EXPORT std::vector<double>* AllocateVectorDouble() {
		return static_cast<std::vector<double>*>(malloc(sizeof(std::vector<double>)));
	}

	NETLM_EXPORT std::vector<std::string>* AllocateVectorString() {
		return static_cast<std::vector<std::string>*>(malloc(sizeof(std::vector<std::string>)));
	}

	// GetVectorSize Functions

	NETLM_EXPORT int GetVectorSizeBool(std::vector<bool>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeChar8(std::vector<char>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeChar16(std::vector<char16_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeInt8(std::vector<int8_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeInt16(std::vector<int16_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeInt32(std::vector<int32_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeInt64(std::vector<int64_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeUInt8(std::vector<uint8_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeUInt16(std::vector<uint16_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeUInt32(std::vector<uint32_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeUInt64(std::vector<uint64_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeIntPtr(std::vector<uintptr_t>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeFloat(std::vector<float>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeDouble(std::vector<double>* vector) {
		return static_cast<int>(vector->size());
	}

	NETLM_EXPORT int GetVectorSizeString(std::vector<std::string>* vector) {
		return static_cast<int>(vector->size());
	}

	// GetVectorData Functions

	NETLM_EXPORT void GetVectorDataBool(std::vector<bool>* vector, bool* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataChar8(std::vector<char>* vector, char* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataChar16(std::vector<char16_t>* vector, char16_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataInt8(std::vector<int8_t>* vector, int8_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataInt16(std::vector<int16_t>* vector, int16_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataInt32(std::vector<int32_t>* vector, int32_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataInt64(std::vector<int64_t>* vector, int64_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataUInt8(std::vector<uint8_t>* vector, uint8_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataUInt16(std::vector<uint16_t>* vector, uint16_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataUInt32(std::vector<uint32_t>* vector, uint32_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataUInt64(std::vector<uint64_t>* vector, uint64_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataIntPtr(std::vector<uintptr_t>* vector, uintptr_t* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataFloat(std::vector<float>* vector, float* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataDouble(std::vector<double>* vector, double* arr) {
		for (size_t i = 0; i < vector->size(); ++i) {
			arr[i] = (*vector)[i];
		}
	}

	NETLM_EXPORT void GetVectorDataString(std::vector<std::string>* vector, char* arr[]) {
		for (size_t i = 0; i < vector->size(); ++i) {
			const auto& source = (*vector)[i];
			size_t size = source.size() + 1;
			char* str = reinterpret_cast<char*>(memAlloc(size));
			std::memcpy(str, source.c_str(), size);
			memFree(arr[i]);
			arr[i] = str;
		}
	}

	// Construct Functions

	NETLM_EXPORT void ConstructVectorDataBool(std::vector<bool>* vector, bool* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<bool>() : std::vector<bool>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataChar8(std::vector<char>* vector, char* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<char>() : std::vector<char>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataChar16(std::vector<char16_t>* vector, char16_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<char16_t>() : std::vector<char16_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataInt8(std::vector<int8_t>* vector, int8_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<int8_t>() : std::vector<int8_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataInt16(std::vector<int16_t>* vector, int16_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<int16_t>() : std::vector<int16_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataInt32(std::vector<int32_t>* vector, int32_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<int32_t>() : std::vector<int32_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataInt64(std::vector<int64_t>* vector, int64_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<int64_t>() : std::vector<int64_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataUInt8(std::vector<uint8_t>* vector, uint8_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<uint8_t>() : std::vector<uint8_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataUInt16(std::vector<uint16_t>* vector, uint16_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<uint16_t>() : std::vector<uint16_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataUInt32(std::vector<uint32_t>* vector, uint32_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<uint32_t>() : std::vector<uint32_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataUInt64(std::vector<uint64_t>* vector, uint64_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<uint64_t>() : std::vector<uint64_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataIntPtr(std::vector<uintptr_t>* vector, uintptr_t* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<uintptr_t>() : std::vector<uintptr_t>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataFloat(std::vector<float>* vector, float* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<float>() : std::vector<float>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataDouble(std::vector<double>* vector, double* arr, int len) {
		std::construct_at(vector, len == 0 ? std::vector<double>() : std::vector<double>(arr, arr + len));
	}

	NETLM_EXPORT void ConstructVectorDataString(std::vector<std::string>* vector, char* arr[], int len) {
		std::construct_at(vector, len == 0 ? std::vector<std::string>() : std::vector<std::string>(arr, arr + len));
	}

	// AssignVector Functions

	NETLM_EXPORT void AssignVectorBool(std::vector<bool>* vector, bool* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorChar8(std::vector<char>* vector, char* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorChar16(std::vector<char16_t>* vector, char16_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorInt8(std::vector<int8_t>* vector, int8_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorInt16(std::vector<int16_t>* vector, int16_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorInt32(std::vector<int32_t>* vector, int32_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorInt64(std::vector<int64_t>* vector, int64_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorUInt8(std::vector<uint8_t>* vector, uint8_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorUInt16(std::vector<uint16_t>* vector, uint16_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorUInt32(std::vector<uint32_t>* vector, uint32_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorUInt64(std::vector<uint64_t>* vector, uint64_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorIntPtr(std::vector<uintptr_t>* vector, uintptr_t* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorFloat(std::vector<float>* vector, float* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorDouble(std::vector<double>* vector, double* arr, int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	NETLM_EXPORT void AssignVectorString(std::vector<std::string>* vector, char* arr[], int len) {
		if (arr == nullptr || len == 0)
			vector->clear();
		else
			vector->assign(arr, arr + len);
	}

	// DeleteVector Functions

	NETLM_EXPORT void DeleteVectorBool(std::vector<bool>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorChar8(std::vector<char>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorChar16(std::vector<char16_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorInt8(std::vector<int8_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorInt16(std::vector<int16_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorInt32(std::vector<int32_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorInt64(std::vector<int64_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorUInt8(std::vector<uint8_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorUInt16(std::vector<uint16_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorUInt32(std::vector<uint32_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorUInt64(std::vector<uint64_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorIntPtr(std::vector<uintptr_t>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorFloat(std::vector<float>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorDouble(std::vector<double>* vector) {
		delete vector;
	}

	NETLM_EXPORT void DeleteVectorString(std::vector<std::string>* vector) {
		delete vector;
	}

	// FreeVectorData functions

	NETLM_EXPORT void FreeVectorBool(std::vector<bool>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorChar8(std::vector<char>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorChar16(std::vector<char16_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorInt8(std::vector<int8_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorInt16(std::vector<int16_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorInt32(std::vector<int32_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorInt64(std::vector<int64_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorUInt8(std::vector<uint8_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorUInt16(std::vector<uint16_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorUInt32(std::vector<uint32_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorUInt64(std::vector<uint64_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorIntPtr(std::vector<uintptr_t>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorFloat(std::vector<float>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorDouble(std::vector<double>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT void FreeVectorString(std::vector<std::string>* vector) {
		vector->~vector();
		free(vector);
	}

	NETLM_EXPORT ILanguageModule* GetLanguageModule() {
		return &g_netlm;
	}
}

void DotnetLanguageModule::ErrorWriter(const char_t* message) {
#if NETLM_PLATFORM_WINDOWS
	g_netlm._provider->Log(std::format(LOG_PREFIX "{}", String::WideStringToUTF8String(message)), Severity::Error);
#else
	g_netlm._provider->Log(std::format(LOG_PREFIX "{}", message), Severity::Error);
#endif
}

// https://github.com/dotnet/runtime/blob/main/docs/design/features/host-error-codes.md
const char* hostfxr_str_error(int32_t error) {
	switch (static_cast<StatusCode>(error)) {
		case Success:
			return "Operation was successful";
		case Success_HostAlreadyInitialized:
			return "Initialization was successful, but another host context is already initialized.";
		case Success_DifferentRuntimeProperties:
			return "Initialization was successful, but another host context is already initialized and the requested context specified some runtime properties which are not the same to the already initialized context";
		case InvalidArgFailure:
			return "One of the specified arguments for the operation is invalid";
		case CoreHostLibLoadFailure:
			return "There was a failure loading a dependent library";
		case CoreHostLibMissingFailure:
			return "One of the dependent libraries is missing";
		case CoreHostEntryPointFailure:
			return "One of the dependent libraries is missing a required entry point.";
		case CoreHostCurHostFindFailure:
			return "Either the location of the current module could not be determined or the location is not in the right place relative to other expected components";
		case CoreClrResolveFailure:
			return "Failed to resolve the coreclr library";
		case CoreClrBindFailure:
			return "The loaded coreclr library does not have one of the required entry points";
		case CoreClrInitFailure:
			return "The call to coreclr_initialize failed";
		case CoreClrExeFailure:
			return "The call to coreclr_execute_assembly failed";
		case ResolverInitFailure:
			return "Initialization of the hostpolicy dependency resolver failed";
		case ResolverResolveFailure:
			return "Resolution of dependencies in hostpolicy failed";
		case LibHostCurExeFindFailure:
			return "Failure to determine the location of the current executable";
		case LibHostInitFailure:
			return "Initialization of the hostpolicy library failed";
		case LibHostExecModeFailure:
			return "Execution of the hostpolicy mode failed";
		case LibHostSdkFindFailure:
			return "Failure to find the requested SDK";
		case LibHostInvalidArgs:
			return "Arguments to hostpolicy are invalid";
		case InvalidConfigFile:
			return "The .runtimeconfig.json file is invalid";
		case AppArgNotRunnable:
			return "Used internally when the command line for dotnet.exe does not contain path to the application to run";
		case AppHostExeNotBoundFailure:
			return "apphost failed to determine which application to run";
		case FrameworkMissingFailure:
			return "It was not possible to find a compatible framework version";
		case HostApiFailed:
			return "hostpolicy could not calculate NATIVE_DLL_SEARCH_DIRECTORIES";
		case HostApiBufferTooSmall:
			return "Buffer specified to an API is not big enough to fit the requested value";
		case LibHostUnknownCommand:
			return "Returned by hostpolicy if corehost_main_with_output_buffer is called with unsupported host commands";
		case LibHostAppRootFindFailure:
			return "Returned by apphost if the imprinted application path does not exist";
		case SdkResolverResolveFailure:
			return "Returned from hostfxr_resolve_sdk2 when it failed to find matching SDK";
		case FrameworkCompatFailure:
			return "During processing of .runtimeconfig.json there were two framework references to the same framework which were not compatible";
		case FrameworkCompatRetry:
			return "Error used internally if the processing of framework references from .runtimeconfig.json reached a point where it needs to reprocess another already processed framework reference";
		//case AppHostExeNotBundle:
		//	return "Error reading the bundle footer metadata from a single-file apphost";
		case BundleExtractionFailure:
			return "Error extracting single-file apphost bundle";
		case BundleExtractionIOError:
			return "Error reading or writing files during single-file apphost bundle extraction";
		case LibHostDuplicateProperty:
			return "The .runtimeconfig.json specified by the app contains a runtime property which is also produced by the hosting layer";
		case HostApiUnsupportedVersion:
			return "Feature which requires certain version of the hosting layer binaries was used on a version which does not support it";
		case HostInvalidState:
			return "Error code returned by the hosting APIs in hostfxr if the current state is incompatible with the requested operation";
		case HostPropertyNotFound:
			return "Property requested by hostfxr_get_runtime_property_value does not exist";
		case CoreHostIncompatibleConfig:
			return "Error returned by hostfxr_initialize_for_runtime_config if the component being initialized requires framework which is not available or incompatible with the frameworks loaded by the runtime already in the process";
		case HostApiUnsupportedScenario:
			return "Error returned by hostfxr_get_runtime_delegate when hostfxr does not currently support requesting the given delegate type using the given context";
		case HostFeatureDisabled:
			return "Error returned by hostfxr_get_runtime_delegate when managed feature support for native host is disabled";
		default:
			return "Unknown error";
	}
}

extern "C" {
	NETLM_EXPORT void NativeInterop_SetInvokeMethodFunction(ManagedGuid* /*assemblyGuid*/, ClassHolder* classHolder, ClassHolder::InvokeMethodFunction invokeMethodFptr) {
		assert(classHolder != nullptr);

		classHolder->SetInvokeMethodFunction(invokeMethodFptr);

		// @TODO: Store the assembly guid somewhere
	}

	NETLM_EXPORT void ManagedClass_Create(ManagedGuid* assemblyGuid, ClassHolder* classHolder, int32_t typeHash, const char* typeName, uint32_t numBaseClasses, char** baseClasses, ManagedClass* outManagedClass) {
		assert(assemblyGuid != nullptr);
		assert(classHolder != nullptr);

		Class* classObject = classHolder->GetOrCreateClassObject(typeHash, typeName);
		if (numBaseClasses > 0) {
			classObject->SetBaseClasses(baseClasses, numBaseClasses);
		}

		*outManagedClass = ManagedClass{typeHash, classObject, *assemblyGuid};
	}

	NETLM_EXPORT void ManagedClass_AddMethod(ManagedClass managedClass, const char* methodName, ManagedGuid guid, ManagedType returnType, uint32_t numParameters, const ManagedType* parameterTypes, uint32_t numAttributes, const char** attributeNames) {
		if (!managedClass.classObject || !methodName) {
			return;
		}

		ManagedMethod methodObject;
		methodObject.guid = guid;
		methodObject.returnType = returnType;

		if (numParameters != 0) {
			methodObject.parameterTypes.assign(parameterTypes, parameterTypes + numParameters);
		}

		if (numAttributes != 0) {
			methodObject.attributeNames.assign(attributeNames, attributeNames + numAttributes);
		}

		if (managedClass.classObject->HasMethod(methodName)) {
			g_netlm.GetProvider()->Log(std::format("Class '{}' already has a method named '{}'!", managedClass.classObject->GetName(), methodName), Severity::Error);
			return;
		}

		managedClass.classObject->AddMethod(methodName, std::move(methodObject));
	}

	NETLM_EXPORT void ManagedClass_SetNewObjectFunction(ManagedClass managedClass, Class::NewObjectFunction newObjectFptr) {
		if (!managedClass.classObject) {
			return;
		}

		managedClass.classObject->SetNewObjectFunction(newObjectFptr);
	}

	NETLM_EXPORT void ManagedClass_SetFreeObjectFunction(ManagedClass managedClass, Class::FreeObjectFunction freeObjectFptr) {
		if (!managedClass.classObject) {
			return;
		}

		managedClass.classObject->SetFreeObjectFunction(freeObjectFptr);
	}
}