#include "module.h"
#include "assembly.h"
#include <module_export.h>

#include <plugify/compat_format.h>
#include <plugify/plugin_descriptor.h>
#include <plugify/plugin.h>
#include <plugify/plugify_provider.h>
#include <plugify/language_module.h>
#include <plugify/log.h>

#if NETLM_PLATFORM_WINDOWS
#include <windows.h>
#define STRING(str) L##str
#else
#define STRING(str) str
#endif

using namespace plugify;
using namespace netlm;

InitResult DotnetLanguageModule::Initialize(std::weak_ptr<IPlugifyProvider> provider, const IModule& module) {
	if (!(_provider = provider.lock())) {
		return ErrorData{ "Provider not exposed" };
	}

	fs::path hostPath(_provider->GetBaseDir() / "dotnet/host/fxr/8.0.3/" BINARY_MODULE_PREFIX "hostfxr" BINARY_MODULE_SUFFIX);

	// Load hostfxr and get desired exports
	_hostFxr = Assembly::LoadFromPath(hostPath);
	if (!_hostFxr) {
		return ErrorData{ std::format("Failed to load assembly: {}", Assembly::GetError()) };
	}

	std::vector<std::string_view> funcErrors;

	auto hostfxr_initialize_for_runtime_config = _hostFxr->GetFunction<hostfxr_initialize_for_runtime_config_fn>("hostfxr_initialize_for_runtime_config");
	if (!hostfxr_initialize_for_runtime_config) {
		funcErrors.emplace_back("hostfxr_initialize_for_runtime_config");
	}
	auto hostfxr_get_runtime_delegate = _hostFxr->GetFunction<hostfxr_get_runtime_delegate_fn>("hostfxr_get_runtime_delegate");
	if (!hostfxr_get_runtime_delegate) {
		funcErrors.emplace_back("hostfxr_get_runtime_delegate");
	}
	auto hostfxr_close = _hostFxr->GetFunction<hostfxr_close_fn>("hostfxr_close");
	if (!hostfxr_close) {
		funcErrors.emplace_back("hostfxr_close");
	}
	auto hostfxr_set_error_writer = _hostFxr->GetFunction<hostfxr_set_error_writer_fn>("hostfxr_set_error_writer");
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

	fs::path assemblyPath(_provider->GetBaseDir() / "api/AssemblyLoader.dll");
	fs::path configPath(_provider->GetBaseDir() / "api/AssemblyLoader.runtimeconfig.json");

	hostfxr_handle context = nullptr;
	scope_guard([=](void*) {
		hostfxr_close(context);
	});

	int32_t result = hostfxr_initialize_for_runtime_config(configPath.c_str(), nullptr, &context);
	if (result != 0 || context == nullptr) {
		return ErrorData{std::format("\"hostfxr_initialize_for_runtime_config\" failed: {0:x}", result)};
	}

	load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
	result = hostfxr_get_runtime_delegate(context, hdt_load_assembly_and_get_function_pointer, reinterpret_cast<void**>(&load_assembly_and_get_function_pointer));
	if (result != 0 || load_assembly_and_get_function_pointer == nullptr) {
		return ErrorData{std::format("\"hostfxr_get_runtime_delegate\" failed: {0:x}", result)};
	}

	auto dotnetType = STRING("Plugify.EntryPoint, AssemblyLoader");

	const char_t* dotnetTypeMethod = STRING("ExecuteScript");

	component_entry_point_fn entryPoint = nullptr;
	result = load_assembly_and_get_function_pointer(
			assemblyPath.c_str(),
			dotnetType,
			dotnetTypeMethod,
			nullptr,
			nullptr,
			reinterpret_cast<void**>(&entryPoint)
	);
	if (result != 0 || entryPoint == nullptr) {
		return ErrorData{std::format("\"load_assembly_and_get_function_pointer\" failed: {0:x}", result)};
	}

	hostfxr_set_error_writer(ErrorWriter);

	_provider->Log("[NETLM] Inited!", Severity::Debug);

	return InitResultData{};
}

void DotnetLanguageModule::Shutdown() {
	_hostFxr.reset();
	_provider.reset();
}

LoadResult DotnetLanguageModule::OnPluginLoad(const IPlugin& plugin) {
	// TODO: implement
	return ErrorData{ "Loading not implemented" };
}

void DotnetLanguageModule::OnPluginStart(const IPlugin& plugin) {
	// TODO: implement
}

void DotnetLanguageModule::OnPluginEnd(const IPlugin& plugin) {
	// TODO: implement
}

void DotnetLanguageModule::OnMethodExport(const IPlugin& plugin) {
	// TODO: implement
}

namespace netlm {
	DotnetLanguageModule g_netlm;
}

#if NETLM_PLATFORM_WINDOWS
std::string utf16ToUtf8(const wchar_t* wstr) {
	int mblen = WideCharToMultiByte(CP_UTF8, 0, wstr, -1, NULL, 0, NULL, NULL);
	if (mblen < 0)
		return {};

	std::string str(static_cast<size_t>(mblen), 0);
	WideCharToMultiByte(CP_UTF8, 0, wstr, -1, str.data(), mblen, NULL, NULL);
	return str;
}
#endif

void DotnetLanguageModule::ErrorWriter(const char_t* message) {
#if NETLM_PLATFORM_WINDOWS
	g_netlm._provider->Log(utf16ToUtf8(message), Severity::Error);
#else
	g_netlm._provider->Log(message, Severity::Error);
#endif
}

extern "C"
NETLM_EXPORT ILanguageModule* GetLanguageModule() {
	return &g_netlm;
}