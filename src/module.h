#include <plugify/language_module.h>
#include <plugify/method.h>
#include <plugify/function.h>

#include <asmjit/asmjit.h>
#include <dotnet/coreclr_delegates.h>
#include <dotnet/error_codes.h>
#include <dotnet/hostfxr.h>
#include <dyncall/dyncall.h>

namespace netlm {
	constexpr int kApiVersion = 1;

	class ClassHolder;
	struct ManagedGuid;
	struct ManagedObject;

	using InitializeAssemblyDelegate = void(*)(char*, ManagedGuid *, ClassHolder *, const char *);
	using UnloadAssemblyDelegate = void(*)(ManagedGuid *, int32_t *);

	class Assembly;
	class Library;

	using ErrorString = std::optional<std::string>;

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

		const std::shared_ptr<plugify::IPlugifyProvider>& GetProvider() { return _provider; }
		void* GetNativeMethod(const std::string& methodName) const;

		std::unique_ptr<Assembly> LoadAssembly(const fs::path& assemblyPath) const;
		bool UnloadAssembly(ManagedGuid guid) const;

	private:
		bool LoadHostFXR(const fs::path& hostPath);
		ErrorString InitializeRuntimeHost(const fs::path& configPath);

		template<class T>
		T GetDelegate(const char_t* assemblyPath, const char_t* typeName, const char_t* methodName, const char_t* delegateTypeName) const;

		static void ErrorWriter(const char_t* message);

		static void InternalCall(const plugify::Method* method, void* addr, const plugify::Parameters* p, uint8_t count, const plugify::ReturnValue* ret);

	private:
		std::shared_ptr<asmjit::JitRuntime> _rt;
		std::shared_ptr<plugify::IPlugifyProvider> _provider;

		std::unique_ptr<Library> _dll;
		std::deleted_unique_ptr<void> _ctx;
		std::unique_ptr<Assembly> _rootAssembly;
		std::vector<std::unique_ptr<Assembly>> _loadedAssemblies;
		std::unordered_map<std::string, void*> _nativesMap;
		std::unordered_map<void*, plugify::Function> _functions;

		std::deleted_unique_ptr<DCCallVM> _callVirtMachine;
		std::mutex _mutex;

		InitializeAssemblyDelegate _initializeAssembly;
		UnloadAssemblyDelegate _unloadAssembly;
	};

	extern DotnetLanguageModule g_netlm;
}