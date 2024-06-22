#include "module.h"
#include "assembly.h"
#include "managed_guid.h"
#include "strings.h"
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
	//hostfxr_set_runtime_property_value_fn hostfxr_set_runtime_property_value;
	hostfxr_close_fn hostfxr_close;
	hostfxr_set_error_writer_fn hostfxr_set_error_writer;

	load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer;
	//get_function_pointer_fn get_function_pointer;
	//load_assembly_fn load_assembly;
}

InitResult DotnetLanguageModule::Initialize(std::weak_ptr<IPlugifyProvider> provider, const IModule& module) {
	if (!(_provider = provider.lock())) {
		return ErrorData{ "Provider not exposed" };
	}

	std::error_code ec;

	fs::path hostPath(module.GetBaseDir() / "dotnet/host/fxr/8.0.3/" NETLM_LIBRARY_PREFIX "hostfxr" NETLM_LIBRARY_SUFFIX);
	if (!fs::exists(hostPath, ec)) {
		return ErrorData{std::format("Host is missing: {}", hostPath.string())};
	}

	if (!LoadHostFXR(hostPath)) {
		return ErrorData{ std::format("Failed to load host assembly: {}", Assembly::GetError()) };
	}

	fs::path configPath(module.GetBaseDir() / "api/Plugify.runtimeconfig.json");
	if (!fs::exists(configPath, ec)) {
		return ErrorData{std::format("Config is missing: {}", configPath.string())};
	}

	auto error = InitializeRuntimeHost(configPath);
	if (error) {
		return ErrorData{ std::move(*error) };
	}

	fs::path assemblyPath(module.GetBaseDir() / "api/Plugify.dll");
	if (!fs::exists(assemblyPath, ec)) {
		return ErrorData{std::format("Assembly is missing: {}", assemblyPath.string())};
	}

	auto className = STRING("Plugify.NativeInterop, Plugify");

	std::vector<std::string_view> funcErrors;

	_initialize = nullptr;
	int32_t result = load_assembly_and_get_function_pointer(
			assemblyPath.c_str(),
			className,
			STRING("Initialize"),
			UNMANAGEDCALLERSONLY_METHOD,
			nullptr,
			reinterpret_cast<void**>(&_initialize)
	);
	if (result != StatusCode::Success || _initialize == nullptr) {
		funcErrors.emplace_back(std::format("Initialize: {:x} ({})", uint32_t(result), String::GetError(result)));
	}

	_shutdown = nullptr;
	result = load_assembly_and_get_function_pointer(
			assemblyPath.c_str(),
			className,
			STRING("Shutdown"),
			UNMANAGEDCALLERSONLY_METHOD,
			nullptr,
			reinterpret_cast<void**>(&_shutdown)
	);
	if (result != StatusCode::Success || _shutdown == nullptr) {
		funcErrors.emplace_back(std::format("Shutdown: {:x} ({})", uint32_t(result), String::GetError(result)));
	}

	_loadPlugin = nullptr;
	result = load_assembly_and_get_function_pointer(
			assemblyPath.c_str(),
			className,
			STRING("LoadPlugin"),
			UNMANAGEDCALLERSONLY_METHOD,
			nullptr,
			reinterpret_cast<void**>(&_loadPlugin)
	);
	if (result != StatusCode::Success || _loadPlugin == nullptr) {
		funcErrors.emplace_back(std::format("LoadPlugin: {:x} ({})", uint32_t(result), String::GetError(result)));
	}

	_startPlugin = nullptr;
	result = load_assembly_and_get_function_pointer(
			assemblyPath.c_str(),
			className,
			STRING("StartPlugin"),
			UNMANAGEDCALLERSONLY_METHOD,
			nullptr,
			reinterpret_cast<void**>(&_startPlugin)
	);
	if (result != StatusCode::Success || _startPlugin == nullptr) {
		funcErrors.emplace_back(std::format("StartPlugin: {:x} ({})", uint32_t(result), String::GetError(result)));
	}

	_endPlugin = nullptr;
	result = load_assembly_and_get_function_pointer(
			assemblyPath.c_str(),
			className,
			STRING("EndPlugin"),
			UNMANAGEDCALLERSONLY_METHOD,
			nullptr,
			reinterpret_cast<void**>(&_endPlugin)
	);
	if (result != StatusCode::Success || _endPlugin == nullptr) {
		funcErrors.emplace_back(std::format("EndPlugin: {:x} ({})", uint32_t(result), String::GetError(result)));
	}

	if (!funcErrors.empty()) {
		std::string funcs(funcErrors[0]);
		for (auto it = std::next(funcErrors.begin()); it != funcErrors.end(); ++it) {
			std::format_to(std::back_inserter(funcs), ", {}", *it);
		}
		return ErrorData{ std::format("Not found {} function(s)", funcs) };
	}

	hostfxr_set_error_writer(ErrorWriter);

	_initialize();

	_provider->Log(LOG_PREFIX "Inited!", Severity::Debug);

	return InitResultData{};
}

void DotnetLanguageModule::Shutdown() {
	_shutdown();

	_nativesMap.clear();
	_context.reset();
	_hostFxr.reset();
	_provider.reset();
}

bool DotnetLanguageModule::LoadHostFXR(const fs::path& hostPath) {
	_hostFxr = Assembly::LoadFromPath(hostPath);
	if (!_hostFxr) {
		return false;
	}

	hostfxr_initialize_for_runtime_config = _hostFxr->GetFunction<hostfxr_initialize_for_runtime_config_fn>("hostfxr_initialize_for_runtime_config");
	hostfxr_get_runtime_delegate = _hostFxr->GetFunction<hostfxr_get_runtime_delegate_fn>("hostfxr_get_runtime_delegate");
	//hostfxr_set_runtime_property_value = _hostFxr->GetFunction<hostfxr_set_runtime_property_value_fn>("hostfxr_set_runtime_property_value");
	hostfxr_close = _hostFxr->GetFunction<hostfxr_close_fn>("hostfxr_close");
	hostfxr_set_error_writer = _hostFxr->GetFunction<hostfxr_set_error_writer_fn>("hostfxr_set_error_writer");

	return hostfxr_initialize_for_runtime_config &&
		   hostfxr_get_runtime_delegate &&
		   //hostfxr_set_runtime_property_value &&
		   hostfxr_close &&
		   hostfxr_set_error_writer;
}

ErrorString DotnetLanguageModule::InitializeRuntimeHost(const fs::path& configPath) {
	hostfxr_handle ctx = nullptr;
	int32_t result = hostfxr_initialize_for_runtime_config(configPath.c_str(), nullptr, &ctx);
	std::deleted_unique_ptr<void> context(ctx, [](hostfxr_handle handle) {
		hostfxr_close(handle);
	});

	if ((result < StatusCode::Success || result > StatusCode::Success_DifferentRuntimeProperties) || ctx == nullptr) {
		return std::format("Failed to initialize hostfxr: {:x} ({})", uint32_t(result), String::GetError(result));
	}

	result = hostfxr_get_runtime_delegate(ctx, hdt_load_assembly_and_get_function_pointer, reinterpret_cast<void**>(&load_assembly_and_get_function_pointer));
	if (result != StatusCode::Success || load_assembly_and_get_function_pointer == nullptr) {
		return std::format("hostfxr_get_runtime_delegate::hdt_load_assembly_and_get_function_pointer failed: {:x} ({})", uint32_t(result), String::GetError(result));
	}
	/*result = hostfxr_get_runtime_delegate(ctx, hdt_get_function_pointer, reinterpret_cast<void**>(&get_function_pointer));
	if (result != StatusCode::Success || get_function_pointer == nullptr) {
		return std::format("hostfxr_get_runtime_delegate::hdt_get_function_pointer failed: {:x} ({})", uint32_t(result), String::GetError(result));
	}
	result = hostfxr_get_runtime_delegate(ctx, hdt_load_assembly, reinterpret_cast<void**>(&load_assembly));
	if (result != StatusCode::Success || load_assembly == nullptr) {
		return std::format("hostfxr_get_runtime_delegate::hdt_load_assembly failed: {:x} ({})", uint32_t(result), String::GetError(result));
	}*/

	_context = std::move(context);

	return std::nullopt;
}

MethodRaw MapToRawMethod(const Method& method, PropertyHolder& rawProperties, MethodHolder& rawPrototypes)  {
	auto paramCount = method.paramTypes.size();
	auto properties = std::make_unique<PropertyRaw[]>(paramCount + 1);

	for (size_t i = 0; i < paramCount; ++i) {
		const auto& param = method.paramTypes[i];

		MethodRaw* prototype = param.prototype ? rawPrototypes.emplace_back(std::make_unique<MethodRaw>(MapToRawMethod(*param.prototype, rawProperties, rawPrototypes))).get() : nullptr;

		properties[i] = PropertyRaw{
				param.type,
				param.ref,
				prototype
		};
	}

	MethodRaw* prototype = method.retType.prototype ? rawPrototypes.emplace_back(std::make_unique<MethodRaw>(MapToRawMethod(*method.retType.prototype, rawProperties, rawPrototypes))).get() : nullptr;

	properties[paramCount] = {
			method.retType.type,
			method.retType.ref,
			prototype
	};

	MethodRaw methodRaw{
		method.name.c_str(),
		method.funcName.c_str(),
		method.callConv.c_str(),
		properties.get(),
		&properties[paramCount],
		method.varIndex,
		static_cast<int32_t>(paramCount)
	};

	rawProperties.emplace_back(std::move(properties));

	return methodRaw;
}

LoadResult DotnetLanguageModule::OnPluginLoad(const IPlugin& plugin) {
	fs::path assemblyPath(plugin.GetBaseDir() / plugin.GetDescriptor().entryPoint);

	const auto& exportedMethods = plugin.GetDescriptor().exportedMethods;
	auto methodCount = exportedMethods.size();

	PropertyHolder rawProperties;
	MethodHolder rawPrototypes;

	auto rawMethods = std::make_unique<MethodRaw[]>(methodCount);
	auto outMethods = std::make_unique<void*[]>(methodCount);

	for (size_t i = 0; i < methodCount; ++i) {
		rawMethods[i] = MapToRawMethod(exportedMethods[i], rawProperties, rawPrototypes);
	}

	char error[256] = {'\0'};
	auto absolutePath= fs::absolute(assemblyPath).string();

	ManagedGuid assemblyGuid{};
	_loadPlugin(absolutePath.c_str(), error, &assemblyGuid, rawMethods.get(), outMethods.get(), static_cast<int>(methodCount));

	if (error[0] != '\0') {
		return ErrorData{ error };
	}

	std::vector<MethodData> methods;
	methods.reserve(methodCount);

	for (size_t i = 0; i < methodCount; ++i) {
		methods.emplace_back(exportedMethods[i].name, outMethods[i]);
	}

	return LoadResultData{ std::move(methods) };
}

void DotnetLanguageModule::OnPluginStart(const IPlugin& ) {
	//_startPlugin();
}

void DotnetLanguageModule::OnPluginEnd(const IPlugin& ) {
	//_endPlugin();
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
		g_netlm.GetProvider()->Log(std::format("{}:{}: {}", funcName, line, message), severity);
	}

	NETLM_EXPORT UniqueId GetPluginId(const plugify::IPlugin& plugin) {
		return plugin.GetId();
	}

	NETLM_EXPORT const char* GetPluginName(const plugify::IPlugin& plugin) {
		auto& source = plugin.GetName();
		size_t size = source.length() + 1;
		char* dest = reinterpret_cast<char*>(memAlloc(size));
		std::memcpy(dest, source.c_str(), size);
		return dest;
	}

	NETLM_EXPORT const char* GetPluginFullName(const plugify::IPlugin& plugin) {
		auto& source = plugin.GetFriendlyName();
		size_t size = source.length() + 1;
		char* dest = reinterpret_cast<char*>(memAlloc(size));
		std::memcpy(dest, source.c_str(), size);
		return dest;
	}

	NETLM_EXPORT const char* GetPluginDescription(const plugify::IPlugin& plugin) {
		auto& source = plugin.GetDescriptor().description;
		size_t size = source.length() + 1;
		char* dest = reinterpret_cast<char*>(memAlloc(size));
		std::memcpy(dest, source.c_str(), size);
		return dest;
	}

	NETLM_EXPORT const char* GetPluginVersion(const plugify::IPlugin& plugin) {
		auto& source = plugin.GetDescriptor().versionName;
		size_t size = source.length() + 1;
		char* dest = reinterpret_cast<char*>(memAlloc(size));
		std::memcpy(dest, source.c_str(), size);
		return dest;
	}

	NETLM_EXPORT const char* GetPluginAuthor(const plugify::IPlugin& plugin) {
		auto& source = plugin.GetDescriptor().createdBy;
		size_t size = source.length() + 1;
		char* dest = reinterpret_cast<char*>(memAlloc(size));
		std::memcpy(dest, source.c_str(), size);
		return dest;
	}

	NETLM_EXPORT const char* GetPluginWebsite(const plugify::IPlugin& plugin) {
		auto& source = plugin.GetDescriptor().createdByURL;
		size_t size = source.length() + 1;
		char* dest = reinterpret_cast<char*>(memAlloc(size));
		std::memcpy(dest, source.c_str(), size);
		return dest;
	}

	NETLM_EXPORT const char* GetPluginBaseDir(const plugify::IPlugin& plugin) {
		auto source = plugin.GetBaseDir().string();
		size_t size = source.length() + 1;
		char* dest = reinterpret_cast<char*>(memAlloc(size));
		std::memcpy(dest, source.c_str(), size);
		return dest;
	}

	NETLM_EXPORT void GetPluginDependencies(const plugify::IPlugin& plugin, char* deps[]) {
		auto& dependencies = plugin.GetDescriptor().dependencies;
		for (size_t i = 0; i < dependencies.size(); ++i) {
			const auto& source = dependencies[i].name;
			size_t size = source.size() + 1;
			char* str = reinterpret_cast<char*>(memAlloc(size));
			std::memcpy(str, source.c_str(), size);
			memFree(deps[i]);
			deps[i] = str;
		}
	}

	NETLM_EXPORT int GetPluginDependenciesSize(const plugify::IPlugin& plugin) {
		return static_cast<int>(plugin.GetDescriptor().dependencies.size());
	}

	NETLM_EXPORT const char* FindPluginResource(const plugify::IPlugin& plugin, const char* path) {
		auto resource = plugin.FindResource(path);
		if (resource.has_value()) {
			auto source= resource->string();
			size_t size = source.length() + 1;
			char* dest = reinterpret_cast<char*>(memAlloc(size));
			std::memcpy(dest, source.c_str(), size);
			return dest;
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

	NETLM_EXPORT std::vector<uintptr_t>* CreateVectorUIntPtr(uintptr_t* arr, int len) {
		return len == 0 ? new std::vector<uintptr_t>() : new std::vector<uintptr_t>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<float>* CreateVectorFloat(float* arr, int len) {
		return len == 0 ? new std::vector<float>() : new std::vector<float>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<double>* CreateVectorDouble(double* arr, int len) {
		return len == 0 ? new std::vector<double>() : new std::vector<double>(arr, arr + len);
	}

	NETLM_EXPORT std::vector<std::string>* CreateVectorString(char* arr[], int len) {
		auto* vector = new std::vector<std::string>();
		if (len != 0) {
			vector->reserve(len);
			for (int i = 0; i < len; ++i) {
				vector->emplace_back(arr[i]);
			}
		}
		return vector;
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

	NETLM_EXPORT std::vector<uintptr_t>* AllocateVectorUIntPtr() {
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

	NETLM_EXPORT int GetVectorSizeUIntPtr(std::vector<uintptr_t>* vector) {
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

	NETLM_EXPORT void GetVectorDataUIntPtr(std::vector<uintptr_t>* vector, uintptr_t* arr) {
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

	NETLM_EXPORT void AssignVectorUIntPtr(std::vector<uintptr_t>* vector, uintptr_t* arr, int len) {
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
		else {
			auto N = static_cast<size_t>(len);
			vector->resize(N);
			for (size_t i = 0; i < N; ++i) {
				(*vector)[i].assign(arr[i]);
			}
		}
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

	NETLM_EXPORT void DeleteVectorUIntPtr(std::vector<uintptr_t>* vector) {
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

	NETLM_EXPORT void FreeVectorUIntPtr(std::vector<uintptr_t>* vector) {
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
	g_netlm._provider->Log(String::WideStringToUTF8String(message), Severity::Error);
#else
	g_netlm._provider->Log(message, Severity::Error);
#endif
}