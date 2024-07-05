#include "module.h"
#include "assembly.h"
#include "managed_assembly.h"
#include "managed_functions.h"
#include "method_info.h"
#include "attribute.h"
#include "type.h"
#include "memory.h"
#include "utils.h"

#include <module_export.h>

#include <plugify/compat_format.h>
#include <plugify/plugin_descriptor.h>
#include <plugify/plugin.h>
#include <plugify/module.h>
#include <plugify/plugify_provider.h>
#include <plugify/language_module.h>
#include <plugify/log.h>

#if NETLM_PLATFORM_WINDOWS
#undef FindResource
#endif

#define LOG_PREFIX "[NETLM] "

using namespace plugify;
using namespace netlm;

InitResult DotnetLanguageModule::Initialize(std::weak_ptr<IPlugifyProvider> provider, const IModule& module) {
	if (!(_provider = provider.lock())) {
		return ErrorData{ "Provider not exposed" };
	}

	if (!_host.Initialize({
		.hostfxrPath = module.GetBaseDir() / "dotnet/host/fxr/8.0.3/" NETLM_LIBRARY_PREFIX "hostfxr" NETLM_LIBRARY_SUFFIX,
		.rootDirectory = module.GetBaseDir() / "api",
		.messageCallback = MessageCallback,
		.exceptionCallback = ExceptionCallback,
	})) {
		return ErrorData{ "Could not initialize hostfxr environment" };
	}

	_alc = _host.CreateAssemblyLoadContext("PlugifyContext");

	_provider->Log(LOG_PREFIX "Inited!", Severity::Debug);

	_rt = std::make_shared<asmjit::JitRuntime>();

	return InitResultData{};
}

void DotnetLanguageModule::Shutdown() {
	_scripts.clear();
	_nativesMap.clear();
	_functions.clear();

	_host.UnloadAssemblyLoadContext(_alc);
	_host.Shutdown();
	_rt.reset();
	_provider.reset();
}

LoadResult DotnetLanguageModule::OnPluginLoad(const IPlugin& plugin) {
	fs::path assemblyPath(plugin.GetBaseDir() / plugin.GetDescriptor().entryPoint);

	ManagedAssembly& assembly = _alc.LoadAssembly(assemblyPath);
	if (!assembly) {
		return ErrorData{ _alc.GetError() };
	}

	Type& pluginClassType = assembly.GetTypeByBaseType("Plugify.Plugin");
	if (!pluginClassType) {
		return ErrorData{"Failed to find 'Plugify.Plugin' class implementation"};
	}

	std::vector<std::string> methodErrors;

	const auto& exportedMethods = plugin.GetDescriptor().exportedMethods;
	std::vector<MethodData> methods;
	methods.reserve(exportedMethods.size());

	for (const auto& method : exportedMethods) {
		auto separated = Utils::Split(method.funcName, ".");
		size_t size = separated.size();
		bool noNamespace = (size == 2);
		if (size != 3 && noNamespace) {
			methodErrors.emplace_back(std::format("Invalid function format: '{}'. Please provide name in that format: 'Namespace.Class.Method' or 'Namespace.MyParentClass+MyNestedClass.Method' or 'Class.Method'", method.funcName));
			continue;
		}

		Type& type = assembly.GetType(noNamespace ? separated[size-2] : std::string_view(separated[0].data(), separated[size-1].data() - 1));
		if (!type) {
			methodErrors.emplace_back(std::format("Failed to find class '{}'", method.funcName));
			continue;
		}

		MethodInfo methodInfo = type.GetMethod(separated[size-1].data());
		if (!methodInfo) {
			methodErrors.emplace_back(std::format("Failed to find method '{}'", method.funcName));
			continue;
		}

		const ValueType& returnType = methodInfo.GetReturnType().GetManagedType().type;
		const ValueType& methodReturnType = method.retType.type;
		if (returnType != methodReturnType) {

			if (returnType == plugify::ValueType::Char16) {
				for (auto& attributes : methodInfo.GetReturnAttributes()) {
					if (attributes.GetFieldValue<ValueType>("Value") == ValueType::Char8) {
						goto next;
					}
				}
			} else if (returnType == plugify::ValueType::ArrayChar16) {
				for (auto& attributes : methodInfo.GetReturnAttributes()) {
					if (attributes.GetFieldValue<ValueType>("Value") == ValueType::ArrayChar8) {
						goto next;
					}
				}
			}

			methodErrors.emplace_back(std::format("Method '{}' has invalid return type '{}' when it should have '{}'", method.funcName, ValueTypeToString(methodReturnType), ValueTypeToString(returnType)));
			continue;
		}

	next:
		const auto& parameterTypes = methodInfo.GetParameterTypes();

		size_t paramCount = parameterTypes.size();
		if (paramCount != method.paramTypes.size()) {
			methodErrors.emplace_back(std::format("Method '{}' has invalid parameter count {} when it should have {}", method.funcName, method.paramTypes.size(), paramCount));
			continue;
		}

		bool methodFail = false;

		for (size_t i = 0; i < paramCount; ++i) {
			const ValueType& paramType = parameterTypes[i].GetManagedType().type;
			const ValueType& methodParamType = method.paramTypes[i].type;
			if (paramType != methodParamType) {
				methodFail = true;
				methodErrors.emplace_back(std::format("Method '{}' has invalid param type '{}' at index {} when it should have '{}'", method.funcName, ValueTypeToString(methodParamType), i, ValueTypeToString(paramType)));
				continue;
			}
		}

		if (methodFail)
			continue;

		auto packed = (static_cast<int64_t>(type.GetTypeId()) << 32) | (static_cast<int64_t>(methodInfo.GetHandle()) & 0xFFFFFFFF);

		auto function = std::make_unique<Function>(_rt);
		void* methodAddr = function->GetJitFunc(method, &InternalCall, reinterpret_cast<void*>(packed));
		if (!methodAddr) {
			methodErrors.emplace_back(std::format("Method JIT generation error: ", function->GetError()));
			continue;
		}
		_functions.emplace_back(std::move(function));

		methods.emplace_back(method.name, methodAddr);
	}

	if (!methodErrors.empty()) {
		std::string funcs(methodErrors[0]);
		for (auto it = std::next(methodErrors.begin()); it != methodErrors.end(); ++it) {
			std::format_to(std::back_inserter(funcs), ", {}", *it);
		}
		return ErrorData{ std::format("Not found {} method function(s)", funcs) };
	}

	const auto [_, result] = _scripts.try_emplace(plugin.GetName(), plugin, pluginClassType);
	if (!result) {
		return ErrorData{ std::format("Plugin name duplicate") };
	}

	return LoadResultData{ std::move(methods) };
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

ScriptInstance* DotnetLanguageModule::FindScript(const std::string& name) {
	auto it = _scripts.find(name);
	if (it != _scripts.end())
		return &std::get<ScriptInstance>(*it);
	return nullptr;
}

// C++ to C#
void DotnetLanguageModule::InternalCall(const plugify::Method* method, void* data, const plugify::Parameters* p, uint8_t count, const plugify::ReturnValue* ret) {
	auto combined = reinterpret_cast<int64_t>(data);
	auto typeId = static_cast<int32_t>((combined >> 32) & 0xFFFFFFFF);
	auto methodId = static_cast<int32_t>(combined & 0xFFFFFFFF);

	bool hasRet = ValueTypeIsHiddenObjectParam(method->retType.type);

	std::vector<const void*> args;
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
					std::puts(LOG_PREFIX "Unsupported types!\n");
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

	Type type(typeId);

	if (method->retType.type != ValueType::Void) {
		type.InvokeStaticMethodRetInternal(methodId, args.data(), args.size(), retPtr);
	} else {
		type.InvokeStaticMethodInternal(methodId, args.data(), args.size());
	}

	if (hasRet) {
		ret->SetReturnPtr(retPtr);
	}
}

/*_________________________________________________*/

void DotnetLanguageModule::ExceptionCallback(const std::string& message) {
	if (!g_netlm._provider)
		return;

	g_netlm._provider->Log(std::format(LOG_PREFIX "[Exception] {}", message), Severity::Error);

	std::stringstream stream;
	cpptrace::generate_trace().print(stream);
	g_netlm._provider->Log(stream.str(), Severity::Debug);
}

void DotnetLanguageModule::MessageCallback(const std::string& message, MessageLevel level) {
	if (!g_netlm._provider)
		return;

	plugify::Severity severity;
	switch (level) {
		case MessageLevel::Info:
			severity = Severity::Info;
			break;
		case MessageLevel::Warning:
			severity = Severity::Warning;
			break;
		case MessageLevel::Error:
			severity = Severity::Error;
			break;
		default:
			return;
	}

	g_netlm._provider->Log(std::format(LOG_PREFIX "{}", message), severity);
}

/*_________________________________________________*/

ScriptInstance::ScriptInstance(const IPlugin& plugin, Type& type) : _plugin{plugin}, _instance{type.CreateInstance()} {
	const auto& desc = plugin.GetDescriptor();
	std::vector<std::string> deps;
	deps.reserve(desc.dependencies.size());
	for (const auto& dependency : desc.dependencies) {
		deps.emplace_back(dependency.name);
	}

	_instance.SetPropertyValue("Id", plugin.GetId());
	_instance.SetPropertyValue("Name", plugin.GetName());
	_instance.SetPropertyValue("FullName", plugin.GetFriendlyName());
	_instance.SetPropertyValue("Description", desc.description);
	_instance.SetPropertyValue("Version", desc.versionName);
	_instance.SetPropertyValue("Author", desc.createdBy);
	_instance.SetPropertyValue("Website", desc.createdByURL);
	_instance.SetPropertyValue("BaseDir", plugin.GetBaseDir().string());
	_instance.SetPropertyValue("Dependencies", deps);
}

ScriptInstance::~ScriptInstance() {
	_instance.Destroy();
};

void ScriptInstance::InvokeOnStart() const {
	ManagedHandle onStartMethod = _instance.GetType().GetMethod("OnStart");
	if (onStartMethod) {
		_instance.InvokeMethodInternal(onStartMethod, nullptr, 0);
	}
}

void ScriptInstance::InvokeOnEnd() const {
	ManagedHandle onEndMethod = _instance.GetType().GetMethod("OnEnd");
	if (onEndMethod) {
		_instance.InvokeMethodInternal(onEndMethod, nullptr, 0);
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
		return Memory::StringToHGlobalAnsi(g_netlm.GetProvider()->GetBaseDir().string());
	}

	NETLM_EXPORT bool IsModuleLoaded(const char* moduleName, int version, bool minimum) {
		auto requiredVersion = (version >= 0 && version != INT_MAX) ? std::make_optional(version) : std::nullopt;
		return g_netlm.GetProvider()->IsModuleLoaded(moduleName, requiredVersion, minimum);
	}

	NETLM_EXPORT bool IsPluginLoaded(const char* pluginName, int version, bool minimum) {
		auto requiredVersion = (version >= 0 && version != INT_MAX) ? std::make_optional(version) : std::nullopt;
		return g_netlm.GetProvider()->IsPluginLoaded(pluginName, requiredVersion, minimum);
	}

	NETLM_EXPORT const char* FindPluginResource(const char* pluginName, const char* path) {
		ScriptInstance* script = g_netlm.FindScript(pluginName);
		if (script) {
			auto resource = script->GetPlugin().FindResource(path);
			if (resource.has_value()) {
				return Memory::StringToHGlobalAnsi(resource->string());
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
		return Memory::StringToHGlobalAnsi(*string);
	}
	NETLM_EXPORT int GetStringLength(std::string* string) {
		return static_cast<int>(string->length());
	}
	NETLM_EXPORT void ConstructString(std::string* string, const char* source) {
		if (source == nullptr)
			std::construct_at(string, std::string());
		else
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
			Memory::FreeCoTaskMem(arr[i]);
			arr[i] = Memory::StringToHGlobalAnsi((*vector)[i]);
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