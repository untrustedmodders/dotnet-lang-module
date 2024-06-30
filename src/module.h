#include <plugify/language_module.h>
#include <plugify/method.h>
#include <plugify/plugin.h>
#include <plugify/function.h>

#include <asmjit/asmjit.h>
#include <dotnet/coreclr_delegates.h>
#include <dotnet/error_codes.h>
#include <dotnet/hostfxr.h>
#include <dyncall/dyncall.h>

namespace netlm {
	constexpr int kApiVersion = 1;

	class ClassHolder;
	class Class;
	struct ManagedGuid;
	struct ManagedObject;
	struct ManagedMethod;

	using InitializeAssemblyDelegate = void(*)(char*, ManagedGuid *, ClassHolder *, const char *);
	using UnloadAssemblyDelegate = void(*)(ManagedGuid *, int32_t *);

	class Object;
	class Assembly;
	class Library;

	struct ExportMethod {
		Class* classPtr;
		ManagedMethod* methodPtr;
	};

	class ScriptInstance {
	public:
		ScriptInstance(const plugify::IPlugin& plugin, std::unique_ptr<Assembly> assembly, std::unique_ptr<Object> instance);
		~ScriptInstance() = default;

		const plugify::IPlugin& GetPlugin() const { return _plugin; }

	private:
		void InvokeOnStart() const;
		void InvokeOnEnd() const;

	private:
		const plugify::IPlugin& _plugin;
		std::unique_ptr<Assembly> _assembly;
		std::unique_ptr<Object> _instance;

		friend class DotnetLanguageModule;
	};

	using ScriptMap = std::unordered_map<std::string, ScriptInstance>;
	using ScriptOpt = std::optional<std::reference_wrapper<ScriptInstance>>;

	class DotnetLanguageModule final : public plugify::ILanguageModule {
	public:
		DotnetLanguageModule() = default;

		// ILanguageModule
		plugify::InitResult Initialize(std::weak_ptr<plugify::IPlugifyProvider> provider, const plugify::IModule& module) override;
		void Shutdown() override;
		plugify::LoadResult OnPluginLoad(const plugify::IPlugin& plugin) override;
		void OnPluginStart(const plugify::IPlugin& plugin) override;
		void OnPluginEnd(const plugify::IPlugin& plugin) override;
		void OnMethodExport(const plugify::IPlugin& plugin) override;

		const ScriptMap& GetScripts() const { return _scripts; }
		ScriptOpt FindScript(const std::string& name);

		const std::shared_ptr<plugify::IPlugifyProvider>& GetProvider() { return _provider; }
		void* GetNativeMethod(const std::string& methodName) const;

		std::unique_ptr<Assembly> LoadAssembly(const fs::path& assemblyPath) const;
		bool UnloadAssembly(ManagedGuid guid) const;

	private:
		bool LoadHostFXR(const fs::path& hostPath);
		std::string InitializeRuntimeHost(const fs::path& configPath);

		template<class T>
		T GetDelegate(const char_t* assemblyPath, const char_t* typeName, const char_t* methodName, const char_t* delegateTypeName) const;

		static void ErrorWriter(const char_t* message);

		static void InternalCall(const plugify::Method* method, void* data, const plugify::Parameters* p, uint8_t count, const plugify::ReturnValue* ret);

	private:
		std::shared_ptr<asmjit::JitRuntime> _rt;
		std::shared_ptr<plugify::IPlugifyProvider> _provider;

		std::unique_ptr<Library> _dll;
		std::deleted_unique_ptr<void> _ctx;
		std::unique_ptr<Assembly> _rootAssembly;
		ScriptMap _scripts;
		std::vector<std::unique_ptr<ExportMethod>> _exportMethods;
		std::unordered_map<std::string, void*> _nativesMap;
		std::unordered_map<void*, plugify::Function> _functions;

		InitializeAssemblyDelegate _initializeAssembly;
		UnloadAssemblyDelegate _unloadAssembly;
	};

	extern DotnetLanguageModule g_netlm;
}