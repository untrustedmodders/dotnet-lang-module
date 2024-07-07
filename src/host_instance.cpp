#include "host_instance.h"
#include "assembly.h"
#include "gc.h"
#include "managed_functions.h"
#include "utils.h"

#include <dotnet/coreclr_delegates.h>
#include <dotnet/error_codes.h>
#include <dotnet/hostfxr.h>

#if NETLM_PLATFORM_WINDOWS
#include <shlobj_core.h>
#endif

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

MessageCallbackFn MessageCallback;
MessageLevel MessageFilter;
ExceptionCallbackFn ExceptionCallback;

void HandleDeleter::operator()(void* handle) const noexcept {
	hostfxr_close(handle);
}

bool HostInstance::Initialize(HostSettings settings) {
	if (_ctx)
		return false;

	// Setup settings
	_settings = std::move(settings);

	MessageCallback = _settings.messageCallback;
	MessageFilter = _settings.messageFilter;

	if (!LoadHostFXR()) {
		MessageCallback("Failed to initialize hostfxr", MessageLevel::Error);
		return false;
	}

	return InitializeRuntimeHost();
}

void HostInstance::Shutdown() {
	MessageCallback("Shutting down .NET runtime", MessageLevel::Info);
	_ctx.reset();
	_dll.reset();
	MessageCallback("Shut down .NET runtime", MessageLevel::Info);
}

fs::path GetHostFXRPath() {
#if NETLM_PLATFORM_WINDOWS
	// Find the Program Files folder
	TCHAR pf[MAX_PATH];
	SHGetSpecialFolderPath(nullptr, pf, CSIDL_PROGRAM_FILES, FALSE);

	fs::path basePath(pf);
	basePath /= "dotnet/host/fxr/";

	auto searchPaths = std::array {
		basePath
	};
#elif NETLM_PLATFORM_LINUX
	auto searchPaths = std::array {
		fs::path("/usr/lib/dotnet/host/fxr/"),
		fs::path("/usr/share/dotnet/host/fxr/"),
	};
#endif

	auto name = fs::path(NETLM_LIBRARY_PREFIX "hostfxr" NETLM_LIBRARY_SUFFIX);

	for (const auto& path : searchPaths) {
		if (!fs::exists(path))
			continue;

		for (const auto& dir : fs::recursive_directory_iterator(path)) {
			if (!dir.is_directory())
				continue;

			auto dirPath = dir.path().string();

			if (dirPath.find(NETLM_DOTNET_TARGET_VERSION_MAJOR_STR) == std::string::npos)
				continue;

			auto res = dir / name;
			assert(fs::exists(res));
			return res;
		}
	}

	return {};
}

bool HostInstance::LoadHostFXR() {
	auto hostfxrPath = _settings.hostfxrPath;
	if (!fs::exists(hostfxrPath)) {
		hostfxrPath = GetHostFXRPath();
		if (!fs::exists(hostfxrPath)) {
			return false;
		}
	}
	
	MessageCallback(std::format("Loading hostfxr from: {}", hostfxrPath.string()), MessageLevel::Info);

	_dll = Assembly::LoadFromPath(hostfxrPath);
	if (!_dll) {
		return false;
	}

	hostfxr_initialize_for_runtime_config = _dll->GetFunction<hostfxr_initialize_for_runtime_config_fn>("hostfxr_initialize_for_runtime_config");
	hostfxr_get_runtime_delegate = _dll->GetFunction<hostfxr_get_runtime_delegate_fn>("hostfxr_get_runtime_delegate");
	hostfxr_set_runtime_property_value = _dll->GetFunction<hostfxr_set_runtime_property_value_fn>("hostfxr_set_runtime_property_value");
	hostfxr_close = _dll->GetFunction<hostfxr_close_fn>("hostfxr_close");
	hostfxr_set_error_writer = _dll->GetFunction<hostfxr_set_error_writer_fn>("hostfxr_set_error_writer");

	MessageCallback("Loaded hostfxr functions", MessageLevel::Info);

	return hostfxr_initialize_for_runtime_config &&
		   hostfxr_get_runtime_delegate &&
		   hostfxr_set_runtime_property_value &&
		   hostfxr_close &&
		   hostfxr_set_error_writer;
}

bool HostInstance::InitializeRuntimeHost() {
	auto rootAssemblyPath = _settings.rootDirectory / "Plugify.dll";
	if (!fs::exists(rootAssemblyPath)) {
		MessageCallback("Failed to find Plugify.dll", MessageLevel::Error);
		return false;
	}
	
	auto runtimeConfigPath = _settings.rootDirectory / "Plugify.runtimeconfig.json";
	if (!fs::exists(runtimeConfigPath)) {
		MessageCallback("Failed to find Plugify.runtimeconfig.json", MessageLevel::Error);
		return false;
	}
	
	hostfxr_handle cxt = nullptr;
	int32_t result = hostfxr_initialize_for_runtime_config(runtimeConfigPath.c_str(), nullptr, &cxt);
	std::unique_ptr<void, HandleDeleter> context(cxt);

	if ((result < 0 || result > 2) || cxt == nullptr) {
		MessageCallback(std::format("Failed to initialize hostfxr: {:x} ({})", uint32_t(result), hostfxr_str_error(result)), MessageLevel::Error);
		return false;
	}

	result = hostfxr_get_runtime_delegate(cxt, hdt_load_assembly_and_get_function_pointer, reinterpret_cast<void**>(&load_assembly_and_get_function_pointer));
	if (result != 0 || load_assembly_and_get_function_pointer == nullptr) {
		MessageCallback(std::format("hdt_load_assembly_and_get_function_pointer failed: {:x} ({})", uint32_t(result), hostfxr_str_error(result)), MessageLevel::Error);
		return false;
	}
	/*result = hostfxr_get_runtime_delegate(cxt, hdt_get_function_pointer, reinterpret_cast<void**>(&get_function_pointer));
	if (result != 0 || get_function_pointer == nullptr) {
		MessageCallback(std::format("hdt_get_function_pointer failed: {:x} ({})", uint32_t(result), hostfxr_str_error(result), MessageLevel::Error);
		return false;
	}
	result = hostfxr_get_runtime_delegate(cxt, hdt_load_assembly, reinterpret_cast<void**>(&load_assembly));
	if (result != 0 || load_assembly == nullptr) {
		MessageCallback(std::format("hdt_load_assembly failed: {:x} ({})", uint32_t(result), hostfxr_str_error(result), MessageLevel::Error);
		return false;
	}*/

	_ctx = std::move(context);

	hostfxr_set_error_writer([](const char_t* message) {
		MessageCallback(NETLM_UTF8(message), MessageLevel::Error);
	});

	using InitializeFn = void(*)(void(*)(String, MessageLevel), void(*)(String));
	InitializeFn managedEntryPoint = GetDelegate<InitializeFn>(rootAssemblyPath.c_str(), NETLM_STR("Plugify.ManagedHost, Plugify"), NETLM_STR("Initialize"));

	LoadManagedFunctions(rootAssemblyPath);

	managedEntryPoint([](String msg, MessageLevel level)
	{
		if (int(MessageFilter) & int(level)) {
			std::string message = msg;
			MessageCallback(message, level);
		}
	},
	[](String msg)
	{
		std::string message = msg;
		if (!ExceptionCallback) {
			MessageCallback(message, MessageLevel::Error);
			return;
		}
		ExceptionCallback(message);
	});

	return true;
}

AssemblyLoadContext HostInstance::CreateAssemblyLoadContext(std::string_view contextName) {
	assert(_ctx);
	auto name = String::New(contextName);
	AssemblyLoadContext alc;
	alc._contextId = Managed.CreateAssemblyLoadContextFptr(name);
	String::Free(name);
	return alc;
}

void HostInstance::UnloadAssemblyLoadContext(AssemblyLoadContext& loadContext) {
	assert(_ctx);
	Managed.UnloadAssemblyLoadContextFptr(loadContext._contextId);
	loadContext._contextId = -1;
	loadContext._loadedAssemblies.clear();
}

void* HostInstance::GetDelegate(const char_t* assemblyPath, const char_t* typeName, const char_t* methodName, const char_t* delegateTypeName) const {
	void* delegatePtr = nullptr;

	int32_t result = load_assembly_and_get_function_pointer(
			assemblyPath,
			typeName,
			methodName,
			delegateTypeName,
			nullptr,
			&delegatePtr);

	if (result != 0) {
		MessageCallback(std::format("Failed to get function pointer for .NET assembly: {}\\tType Name: {}\\tMethod Name: {} - {:x} ({})", NETLM_UTF8(assemblyPath), NETLM_UTF8(typeName), NETLM_UTF8(methodName), uint32_t(result), hostfxr_str_error(result)), MessageLevel::Error);
		return nullptr;
	}

	return delegatePtr;
}

void HostInstance::LoadManagedFunctions(const fs::path& assemblyPath) {
	Managed.CreateAssemblyLoadContextFptr = GetDelegate<CreateAssemblyLoadContextFn>(assemblyPath.c_str(), NETLM_STR("Plugify.AssemblyLoader, Plugify"), NETLM_STR("CreateAssemblyLoadContext"));
	Managed.UnloadAssemblyLoadContextFptr = GetDelegate<UnloadAssemblyLoadContextFn>(assemblyPath.c_str(), NETLM_STR("Plugify.AssemblyLoader, Plugify"), NETLM_STR("UnloadAssemblyLoadContext"));
	Managed.LoadManagedAssemblyFptr = GetDelegate<LoadManagedAssemblyFn>(assemblyPath.c_str(), NETLM_STR("Plugify.AssemblyLoader, Plugify"), NETLM_STR("LoadAssembly"));
	Managed.GetLastLoadStatusFptr = GetDelegate<GetLastLoadStatusFn>(assemblyPath.c_str(), NETLM_STR("Plugify.AssemblyLoader, Plugify"), NETLM_STR("GetLastLoadStatus"));
	Managed.GetAssemblyNameFptr = GetDelegate<GetAssemblyNameFn>(assemblyPath.c_str(), NETLM_STR("Plugify.AssemblyLoader, Plugify"), NETLM_STR("GetAssemblyName"));

	Managed.GetAssemblyTypesFptr = GetDelegate<GetAssemblyTypesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetAssemblyTypes"));
	Managed.GetTypeIdFptr = GetDelegate<GetTypeIdFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeId"));
	Managed.GetFullTypeNameFptr = GetDelegate<GetFullTypeNameFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetFullTypeName"));
	Managed.GetAssemblyQualifiedNameFptr = GetDelegate<GetAssemblyQualifiedNameFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetAssemblyQualifiedName"));
	Managed.GetBaseTypeFptr = GetDelegate<GetBaseTypeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetBaseType"));
	Managed.GetTypeSizeFptr = GetDelegate<GetTypeSizeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeSize"));
	Managed.IsTypeSubclassOfFptr = GetDelegate<IsTypeSubclassOfFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("IsTypeSubclassOf"));
	Managed.IsTypeAssignableToFptr = GetDelegate<IsTypeAssignableToFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("IsTypeAssignableTo"));
	Managed.IsTypeAssignableFromFptr = GetDelegate<IsTypeAssignableFromFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("IsTypeAssignableFrom"));
	Managed.IsTypeSZArrayFptr = GetDelegate<IsTypeSZArrayFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("IsTypeSZArray"));
	Managed.IsTypeByRefFptr = GetDelegate<IsTypeByRefFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("IsTypeByRef"));
	Managed.GetElementTypeFptr = GetDelegate<GetElementTypeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetElementType"));
	Managed.GetTypeMethodsFptr = GetDelegate<GetTypeMethodsFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeMethods"));
	Managed.GetTypeFieldsFptr = GetDelegate<GetTypeFieldsFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeFields"));
	Managed.GetTypePropertiesFptr = GetDelegate<GetTypePropertiesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeProperties"));
	Managed.GetTypeMethodFptr = GetDelegate<GetTypeMethodFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeMethod"));
	Managed.GetTypeFieldFptr = GetDelegate<GetTypeFieldFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeField"));
	Managed.GetTypePropertyFptr = GetDelegate<GetTypePropertyFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeProperty"));
	Managed.HasTypeAttributeFptr = GetDelegate<HasTypeAttributeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("HasTypeAttribute"));
	Managed.GetTypeAttributesFptr = GetDelegate<GetTypeAttributesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeAttributes"));
	Managed.GetTypeManagedTypeFptr = GetDelegate<GetTypeManagedTypeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetTypeManagedType"));
	Managed.InvokeStaticMethodFptr = GetDelegate<InvokeStaticMethodFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("InvokeStaticMethod"));
	Managed.InvokeStaticMethodRetFptr = GetDelegate<InvokeStaticMethodRetFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("InvokeStaticMethodRet"));

	Managed.GetMethodInfoNameFptr = GetDelegate<GetMethodInfoNameFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetMethodInfoName"));
	Managed.GetMethodInfoFunctionAddressFptr = GetDelegate<GetMethodInfoFunctionAddressFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetMethodInfoFunctionAddress"));
	Managed.GetMethodInfoReturnTypeFptr = GetDelegate<GetMethodInfoReturnTypeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetMethodInfoReturnType"));
	Managed.GetMethodInfoParameterTypesFptr = GetDelegate<GetMethodInfoParameterTypesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetMethodInfoParameterTypes"));
	Managed.GetMethodInfoAccessibilityFptr = GetDelegate<GetMethodInfoAccessibilityFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetMethodInfoAccessibility"));
	Managed.GetMethodInfoAttributesFptr = GetDelegate<GetMethodInfoAttributesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetMethodInfoAttributes"));
	Managed.GetMethodInfoReturnAttributesFptr = GetDelegate<GetMethodInfoReturnAttributesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetMethodInfoReturnAttributes"));

	Managed.GetFieldInfoNameFptr = GetDelegate<GetFieldInfoNameFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetFieldInfoName"));
	Managed.GetFieldInfoTypeFptr = GetDelegate<GetFieldInfoTypeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetFieldInfoType"));
	Managed.GetFieldInfoAccessibilityFptr = GetDelegate<GetFieldInfoAccessibilityFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetFieldInfoAccessibility"));
	Managed.GetFieldInfoAttributesFptr = GetDelegate<GetFieldInfoAttributesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetFieldInfoAttributes"));

	Managed.GetPropertyInfoNameFptr = GetDelegate<GetPropertyInfoNameFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetPropertyInfoName"));
	Managed.GetPropertyInfoTypeFptr = GetDelegate<GetPropertyInfoTypeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetPropertyInfoType"));
	Managed.GetPropertyInfoAttributesFptr = GetDelegate<GetPropertyInfoAttributesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetPropertyInfoAttributes"));

	Managed.GetAttributeFieldValueFptr = GetDelegate<GetAttributeFieldValueFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetAttributeFieldValue"));
	Managed.GetAttributeTypeFptr = GetDelegate<GetAttributeTypeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetAttributeType"));

	Managed.SetInternalCallsFptr = GetDelegate<SetInternalCallsFn>(assemblyPath.c_str(), NETLM_STR("Plugify.Interop.InternalCallsManager, Plugify"), NETLM_STR("SetInternalCalls"));
	Managed.CreateObjectFptr = GetDelegate<CreateObjectFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("CreateObject"));
	Managed.InvokeMethodFptr = GetDelegate<InvokeMethodFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("InvokeMethod"));
	Managed.InvokeMethodRetFptr = GetDelegate<InvokeMethodRetFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("InvokeMethodRet"));
	Managed.SetFieldValueFptr = GetDelegate<SetFieldValueFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("SetFieldValue"));
	Managed.GetFieldValueFptr = GetDelegate<GetFieldValueFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("GetFieldValue"));
	Managed.GetFieldPointerFptr = GetDelegate<GetFieldPointerFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("GetFieldPointer"));
	Managed.SetPropertyValueFptr = GetDelegate<SetFieldValueFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("SetPropertyValue"));
	Managed.GetPropertyValueFptr = GetDelegate<GetFieldValueFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("GetPropertyValue"));
	Managed.DestroyObjectFptr = GetDelegate<DestroyObjectFn>(assemblyPath.c_str(), NETLM_STR("Plugify.ManagedObject, Plugify"), NETLM_STR("DestroyObject"));
	Managed.CollectGarbageFptr = GetDelegate<CollectGarbageFn>(assemblyPath.c_str(), NETLM_STR("Plugify.GarbageCollector, Plugify"), NETLM_STR("CollectGarbage"));
	Managed.WaitForPendingFinalizersFptr = GetDelegate<WaitForPendingFinalizersFn>(assemblyPath.c_str(), NETLM_STR("Plugify.GarbageCollector, Plugify"), NETLM_STR("WaitForPendingFinalizers"));

	Managed.IsClassFptr = GetDelegate<IsClassFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("IsClass"));
	Managed.IsEnumFptr = GetDelegate<IsEnumFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("IsEnum"));
	Managed.IsValueTypeFptr = GetDelegate<IsValueTypeFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("IsValueType"));
	Managed.GetEnumNamesFptr = GetDelegate<GetEnumNamesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetEnumNames"));
	Managed.GetEnumValuesFptr = GetDelegate<GetEnumValuesFn>(assemblyPath.c_str(), NETLM_STR("Plugify.TypeInterface, Plugify"), NETLM_STR("GetEnumValues"));
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