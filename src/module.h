#include <plugify/language_module.h>

#include <dotnet/coreclr_delegates.h>
#include <dotnet/hostfxr.h>

namespace netlm {
	constexpr int kApiVersion = 1;

	using EntryPoint = void (*)();
	using InitFunc = int (*)(const void*);
	using StartFunc = void (*)();
	using EndFunc = void (*)();

	class Assembly;
	struct Script {
		StartFunc startFunc{ nullptr };
		EndFunc endFunc{ nullptr };
	};

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

	private:
		static void ErrorWriter(const char_t* message);

	private:
		std::shared_ptr<plugify::IPlugifyProvider> _provider;
		std::unique_ptr<Assembly> _hostFxr;
		std::deleted_unique_ptr<void> _context;
		std::unordered_map<std::string, Script> _scripts;
		std::unordered_map<std::string, void*> _nativesMap;
	};
}