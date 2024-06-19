#include "module.h"
#include "assembly.h"
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
	hostfxr_close_fn hostfxr_close;
	hostfxr_set_error_writer_fn hostfxr_set_error_writer;
	load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer;
	get_function_pointer_fn get_function_pointer;
	load_assembly_fn load_assembly;
}

namespace ErrorCode {
	constexpr int32_t Success                           = 0x00000000;
	constexpr int32_t SuccessHostAlreadyInitialized     = 0x00000001;
	constexpr int32_t SuccessDifferentRuntimeProperties = 0x00000002;
	constexpr int32_t InvalidArgFailure                 = static_cast<int32_t>(0x80008081);
	constexpr int32_t CoreHostLibLoadFailure            = static_cast<int32_t>(0x80008082);
	constexpr int32_t CoreHostLibMissingFailure         = static_cast<int32_t>(0x80008083);
	constexpr int32_t CoreHostEntryPointFailure         = static_cast<int32_t>(0x80008084);
	constexpr int32_t CoreHostCurHostFindFailure        = static_cast<int32_t>(0x80008085);
	constexpr int32_t CoreClrResolveFailure             = static_cast<int32_t>(0x80008087);
	constexpr int32_t CoreClrBindFailure                = static_cast<int32_t>(0x80008088);
	constexpr int32_t CoreClrInitFailure                = static_cast<int32_t>(0x80008089);
	constexpr int32_t CoreClrExeFailure                 = static_cast<int32_t>(0x8000808a);
	constexpr int32_t ResolverInitFailure               = static_cast<int32_t>(0x8000808b);
	constexpr int32_t ResolverResolveFailure            = static_cast<int32_t>(0x8000808c);
	constexpr int32_t LibHostCurExeFindFailure          = static_cast<int32_t>(0x8000808d);
	constexpr int32_t LibHostInitFailure                = static_cast<int32_t>(0x8000808e);
	constexpr int32_t LibHostSdkFindFailure             = static_cast<int32_t>(0x80008091);
	constexpr int32_t LibHostInvalidArgs                = static_cast<int32_t>(0x80008092);
	constexpr int32_t InvalidConfigFile                 = static_cast<int32_t>(0x80008093);
	constexpr int32_t AppArgNotRunnable                 = static_cast<int32_t>(0x80008094);
	constexpr int32_t AppHostExeNotBoundFailure         = static_cast<int32_t>(0x80008095);
	constexpr int32_t FrameworkMissingFailure           = static_cast<int32_t>(0x80008096);
	constexpr int32_t HostApiFailed                     = static_cast<int32_t>(0x80008097);
	constexpr int32_t HostApiBufferTooSmall             = static_cast<int32_t>(0x80008098);
	constexpr int32_t LibHostUnknownCommand             = static_cast<int32_t>(0x80008099);
	constexpr int32_t LibHostAppRootFindFailure         = static_cast<int32_t>(0x8000809a);
	constexpr int32_t SdkResolverResolveFailure         = static_cast<int32_t>(0x8000809b);
	constexpr int32_t FrameworkCompatFailure            = static_cast<int32_t>(0x8000809c);
	constexpr int32_t FrameworkCompatRetry              = static_cast<int32_t>(0x8000809d);
	constexpr int32_t AppHostExeNotBundle               = static_cast<int32_t>(0x8000809e);
	constexpr int32_t BundleExtractionFailure           = static_cast<int32_t>(0x8000809f);
	constexpr int32_t BundleExtractionIOError           = static_cast<int32_t>(0x800080a0);
	constexpr int32_t LibHostDuplicateProperty          = static_cast<int32_t>(0x800080a1);
	constexpr int32_t HostApiUnsupportedVersion         = static_cast<int32_t>(0x800080a2);
	constexpr int32_t HostInvalidState                  = static_cast<int32_t>(0x800080a3);
	constexpr int32_t HostPropertyNotFound              = static_cast<int32_t>(0x800080a4);
	constexpr int32_t CoreHostIncompatibleConfig        = static_cast<int32_t>(0x800080a5);
	constexpr int32_t HostApiUnsupportedScenario        = static_cast<int32_t>(0x800080a6);
	constexpr int32_t HostFeatureDisabled               = static_cast<int32_t>(0x800080a7);
}

std::string_view GetError(int32_t code) {
    switch (code) {
		case ErrorCode::Success:                           return "Success";
        case ErrorCode::SuccessHostAlreadyInitialized:     return "SuccessHostAlreadyInitialized";
        case ErrorCode::SuccessDifferentRuntimeProperties: return "SuccessDifferentRuntimeProperties";
        case ErrorCode::InvalidArgFailure:                 return "InvalidArgFailure";
        case ErrorCode::CoreHostLibLoadFailure:            return "CoreHostLibLoadFailure";
        case ErrorCode::CoreHostLibMissingFailure:         return "CoreHostLibMissingFailure";
        case ErrorCode::CoreHostEntryPointFailure:         return "CoreHostEntryPointFailure";
        case ErrorCode::CoreHostCurHostFindFailure:        return "CoreHostCurHostFindFailure";
        case ErrorCode::CoreClrResolveFailure:             return "CoreClrResolveFailure";
        case ErrorCode::CoreClrBindFailure:                return "CoreClrBindFailure";
        case ErrorCode::CoreClrInitFailure:                return "CoreClrInitFailure";
        case ErrorCode::CoreClrExeFailure:                 return "CoreClrExeFailure";
        case ErrorCode::ResolverInitFailure:               return "ResolverInitFailure";
        case ErrorCode::ResolverResolveFailure:            return "ResolverResolveFailure";
        case ErrorCode::LibHostCurExeFindFailure:          return "LibHostCurExeFindFailure";
        case ErrorCode::LibHostInitFailure:                return "LibHostInitFailure";
        case ErrorCode::LibHostSdkFindFailure:             return "LibHostSdkFindFailure";
        case ErrorCode::LibHostInvalidArgs:                return "LibHostInvalidArgs";
        case ErrorCode::InvalidConfigFile:                 return "InvalidConfigFile";
        case ErrorCode::AppArgNotRunnable:                 return "AppArgNotRunnable";
        case ErrorCode::AppHostExeNotBoundFailure:         return "AppHostExeNotBoundFailure";
        case ErrorCode::FrameworkMissingFailure:           return "FrameworkMissingFailure";
        case ErrorCode::HostApiFailed:                     return "HostApiFailed";
        case ErrorCode::HostApiBufferTooSmall:             return "HostApiBufferTooSmall";
        case ErrorCode::LibHostUnknownCommand:             return "LibHostUnknownCommand";
        case ErrorCode::LibHostAppRootFindFailure:         return "LibHostAppRootFindFailure";
        case ErrorCode::SdkResolverResolveFailure:         return "SdkResolverResolveFailure";
        case ErrorCode::FrameworkCompatFailure:            return "FrameworkCompatFailure";
        case ErrorCode::FrameworkCompatRetry:              return "FrameworkCompatRetry";
        case ErrorCode::AppHostExeNotBundle:               return "AppHostExeNotBundle";
        case ErrorCode::BundleExtractionFailure:           return "BundleExtractionFailure";
        case ErrorCode::BundleExtractionIOError:           return "BundleExtractionIOError";
        case ErrorCode::LibHostDuplicateProperty:          return "LibHostDuplicateProperty";
        case ErrorCode::HostApiUnsupportedVersion:         return "HostApiUnsupportedVersion";
        case ErrorCode::HostInvalidState:                  return "HostInvalidState";
        case ErrorCode::HostPropertyNotFound:              return "HostPropertyNotFound";
        case ErrorCode::CoreHostIncompatibleConfig:        return "CoreHostIncompatibleConfig";
        case ErrorCode::HostApiUnsupportedScenario:        return "HostApiUnsupportedScenario";
        case ErrorCode::HostFeatureDisabled:               return "HostFeatureDisabled";
        default:                                           return "UnknownStatusCode";
    }
}

InitResult DotnetLanguageModule::Initialize(std::weak_ptr<IPlugifyProvider> provider, const IModule& module) {
	if (!(_provider = provider.lock())) {
		return ErrorData{ "Provider not exposed" };
	}

	fs::path hostPath(module.GetBaseDir() / "dotnet/host/fxr/8.0.3/" NETLM_LIBRARY_PREFIX "hostfxr" NETLM_LIBRARY_SUFFIX);

	// Load hostfxr and get desired exports
	auto hostFxr = Assembly::LoadFromPath(hostPath);
	if (!hostFxr) {
		return ErrorData{ std::format("Failed to load assembly: {}", Assembly::GetError()) };
	}

	std::vector<std::string_view> funcErrors;

	hostfxr_initialize_for_runtime_config = hostFxr->GetFunction<hostfxr_initialize_for_runtime_config_fn>("hostfxr_initialize_for_runtime_config");
	if (!hostfxr_initialize_for_runtime_config) {
		funcErrors.emplace_back("hostfxr_initialize_for_runtime_config");
	}
	hostfxr_get_runtime_delegate = hostFxr->GetFunction<hostfxr_get_runtime_delegate_fn>("hostfxr_get_runtime_delegate");
	if (!hostfxr_get_runtime_delegate) {
		funcErrors.emplace_back("hostfxr_get_runtime_delegate");
	}
	hostfxr_close = hostFxr->GetFunction<hostfxr_close_fn>("hostfxr_close");
	if (!hostfxr_close) {
		funcErrors.emplace_back("hostfxr_close");
	}
	hostfxr_set_error_writer = hostFxr->GetFunction<hostfxr_set_error_writer_fn>("hostfxr_set_error_writer");
	if (!hostfxr_set_error_writer) {
		funcErrors.emplace_back("hostfxr_set_error_writer");
	}

	if (!funcErrors.empty()) {
		std::string funcs(funcErrors[0]);
		for (auto it = std::next(funcErrors.begin()); it != funcErrors.end(); ++it) {
			std::format_to(std::back_inserter(funcs), ", {}", *it);
		}
		return ErrorData{ std::format("Not found {} function(s)", funcs) };
	}

	funcErrors.clear();

	hostfxr_set_error_writer(ErrorWriter);
	std::error_code ec;
	
	fs::path configPath(module.GetBaseDir() / "api/Plugify.runtimeconfig.json");
	if (!fs::exists(configPath, ec)) {
		return ErrorData{std::format("Config is missing: {}", configPath.string())};
	}
	
	hostfxr_handle ctx = nullptr;
	int32_t result = hostfxr_initialize_for_runtime_config(configPath.c_str(), nullptr, &ctx);
	if (result > ErrorCode::SuccessDifferentRuntimeProperties || ctx == nullptr) {
		return ErrorData{std::format("hostfxr_initialize_for_runtime_config failed: {}", GetError(result))};
	}

	std::deleted_unique_ptr<void> context(ctx, [](hostfxr_handle handle) {
		hostfxr_close(handle);
	});

	result = hostfxr_get_runtime_delegate(ctx, hdt_load_assembly_and_get_function_pointer, reinterpret_cast<void**>(&load_assembly_and_get_function_pointer));
	if (result != ErrorCode::Success || load_assembly_and_get_function_pointer == nullptr) {
		return ErrorData{std::format("hostfxr_get_runtime_delegate::hdt_load_assembly_and_get_function_pointer failed: {}", GetError(result))};
	}
	result = hostfxr_get_runtime_delegate(ctx, hdt_get_function_pointer, reinterpret_cast<void**>(&get_function_pointer));
	if (result != ErrorCode::Success || get_function_pointer == nullptr) {
		return ErrorData{std::format("hostfxr_get_runtime_delegate::hdt_get_function_pointer failed: {}", GetError(result))};
	}
	result = hostfxr_get_runtime_delegate(ctx, hdt_load_assembly, reinterpret_cast<void**>(&load_assembly));
	if (result != ErrorCode::Success || load_assembly == nullptr) {
		return ErrorData{std::format("hostfxr_get_runtime_delegate::hdt_load_assembly failed: {}", GetError(result))};
	}

	fs::path assemblyPath(module.GetBaseDir() / "api/Plugify.dll");
	if (!fs::exists(assemblyPath, ec)) {
		return ErrorData{std::format("Assembly is missing: {}", assemblyPath.string())};
	}

	EntryPoint entryPoint = nullptr;
	result = load_assembly_and_get_function_pointer(
			assemblyPath.c_str(),
			STRING("Plugify.Library, Plugify"),
			STRING("Initialize"),
			UNMANAGEDCALLERSONLY_METHOD,
			nullptr,
			reinterpret_cast<void**>(&entryPoint)
	);
	if (result != ErrorCode::Success || entryPoint == nullptr) {
		return ErrorData{std::format("load_assembly_and_get_function_pointer failed: {}", GetError(result))};
	}

	entryPoint();

	_hostFxr = std::move(hostFxr);
	_context = std::move(context);

	_provider->Log(LOG_PREFIX "Inited!", Severity::Debug);

	return InitResultData{};
}

void DotnetLanguageModule::Shutdown() {
	_nativesMap.clear();
	_context.reset();
	_hostFxr.reset();
	_provider.reset();
}

LoadResult DotnetLanguageModule::OnPluginLoad(const IPlugin& plugin) {
	fs::path entryPath(plugin.GetDescriptor().entryPoint);
	fs::path assemblyPath(plugin.GetBaseDir() / entryPath);
	fs::path fileName(entryPath.filename().replace_extension());

	int32_t result = load_assembly(assemblyPath.c_str(), nullptr, nullptr);
	if (result != ErrorCode::Success) {
		return ErrorData{std::format("Failed to load assembly: {}", GetError(result))};
	}

	std::vector<std::string_view> funcErrors;

	auto mainType = std::format(STRING("{}, {}"), STRING("Plugify.Main"), fileName.c_str());

	InitFunc initFunc = nullptr;
	result = get_function_pointer(
			mainType.data(),
			STRING("OnInit"),
			UNMANAGEDCALLERSONLY_METHOD,
			nullptr,
			nullptr,
			reinterpret_cast<void**>(&initFunc)
	);
	if (result != ErrorCode::Success || initFunc == nullptr) {
		funcErrors.emplace_back("OnInit");
	}

	StartFunc startFunc = nullptr;
	result = get_function_pointer(
			mainType.data(),
			STRING("OnStart"),
			UNMANAGEDCALLERSONLY_METHOD,
			nullptr,
			nullptr,
			reinterpret_cast<void**>(&startFunc)
	);
	if (result != ErrorCode::Success || startFunc == nullptr) {
		funcErrors.emplace_back("OnStart");
	}

	EndFunc endFunc = nullptr;
	result = get_function_pointer(
			mainType.data(),
			STRING("OnEnd"),
			UNMANAGEDCALLERSONLY_METHOD,
			nullptr,
			nullptr,
			reinterpret_cast<void**>(&endFunc)
	);
	if (result != ErrorCode::Success || endFunc == nullptr) {
		funcErrors.emplace_back("OnEnd");
	}

	if (!funcErrors.empty()) {
		std::string funcs(funcErrors[0]);
		for (auto it = std::next(funcErrors.begin()); it != funcErrors.end(); ++it) {
			std::format_to(std::back_inserter(funcs), ", {}", *it);
		}
		return ErrorData{ std::format("Not found {} entry function(s)", funcs) };
	}

	const auto& exportedMethods = plugin.GetDescriptor().exportedMethods;
	std::vector<MethodData> methods;
	methods.reserve(exportedMethods.size());

	for (const auto& method : exportedMethods) {
		auto seperated = String::Split(method.funcName, ".");
		if (seperated.size() != 3) {
			funcErrors.emplace_back(std::format("Invalid function name: '{}'. Please provide name in that format: 'Namespace.Class.Method'", method.funcName));
			continue;
		}

		auto typeName = std::format(STRING("{}.{}, {}"), STR(seperated[0]), STR(seperated[1]), fileName.c_str());
		auto methodName = STR(seperated[3]);

		void* addr = nullptr;
		result = get_function_pointer(
				typeName.data(),
				methodName.data(),
				UNMANAGEDCALLERSONLY_METHOD,
				nullptr,
				nullptr,
				&addr
		);

		if (result != ErrorCode::Success || addr == nullptr) {
			funcErrors.emplace_back(method.name);
		} else {
			methods.emplace_back(method.name, addr);
		}
	}
	if (!funcErrors.empty()) {
		std::string funcs(funcErrors[0]);
		for (auto it = std::next(funcErrors.begin()); it != funcErrors.end(); ++it) {
			std::format_to(std::back_inserter(funcs), ", {}", *it);
		}
		return ErrorData{ std::format("Not found {} method function(s)", funcs) };
	}

	const int resultVersion = initFunc(&plugin);
	if (resultVersion != ErrorCode::Success) {
		return ErrorData{ std::format("Not supported plugin api {}, max supported {}", resultVersion, kApiVersion) };
	}

	const auto [_, status] = _scripts.try_emplace(plugin.GetName(), startFunc, endFunc);
	if (!status) {
		return ErrorData{ std::format("Plugin name duplicate") };
	}

	return LoadResultData{ std::move(methods) };
}

void DotnetLanguageModule::OnPluginStart(const IPlugin& plugin) {
	if (const auto it = _scripts.find(plugin.GetName()); it != _scripts.end()) {
		const auto& script = std::get<Script>(*it);
		script.startFunc();
	}
}

void DotnetLanguageModule::OnPluginEnd(const IPlugin& plugin) {
	if (const auto it = _scripts.find(plugin.GetName()); it != _scripts.end()) {
		const auto& script = std::get<Script>(*it);
		script.endFunc();
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