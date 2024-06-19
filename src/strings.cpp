#include "strings.h"

#include <dotnet/error_codes.h>

using namespace netlm;

#if NETLM_PLATFORM_WINDOWS
#include <windows.h>

std::wstring String::UTF8StringToWideString(std::string_view str){
	std::wstring ret;
	if (!UTF8StringToWideString(ret, str))
		return {};
	return ret;
}

bool String::UTF8StringToWideString(std::wstring& dest, std::string_view str) {
	int wlen = MultiByteToWideChar(CP_UTF8, 0, str.data(), static_cast<int>(str.length()), nullptr, 0);
	if (wlen < 0)
		return false;

	dest.resize(static_cast<size_t>(wlen));
	if (wlen > 0 && MultiByteToWideChar(CP_UTF8, 0, str.data(), static_cast<int>(str.length()), dest.data(), wlen) < 0)
		return false;

	return true;
}

std::string String::WideStringToUTF8String(std::wstring_view str) {
	std::string ret;
	if (!WideStringToUTF8String(ret, str))
		return {};
	return ret;
}

bool String::WideStringToUTF8String(std::string& dest, std::wstring_view str) {
	int mblen = WideCharToMultiByte(CP_UTF8, 0, str.data(), static_cast<int>(str.length()), nullptr, 0, nullptr, nullptr);
	if (mblen < 0)
		return false;

	dest.resize(static_cast<size_t>(mblen));
	if (mblen > 0 && WideCharToMultiByte(CP_UTF8, 0, str.data(), static_cast<int>(str.length()), dest.data(), mblen, nullptr, nullptr) < 0)
		return false;

	return true;
}
#endif

std::vector<std::string_view> String::Split(std::string_view strv, std::string_view delims) {
	std::vector<std::string_view> output;
	size_t first = 0;

	while (first < strv.size()) {
		const size_t second = strv.find_first_of(delims, first);

		if (first != second)
			output.emplace_back(strv.substr(first, second-first));

		if (second == std::string_view::npos)
			break;

		first = second + 1;
	}

	return output;
}

// https://github.com/dotnet/runtime/blob/main/docs/design/features/host-error-codes.md
std::string_view String::GetError(int32_t error) {
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