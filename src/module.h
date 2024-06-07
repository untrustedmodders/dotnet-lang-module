#include <plugify/language_module.h>

#include <dotnet/coreclr_delegates.h>
#include <dotnet/hostfxr.h>

namespace netlm {
	class Assembly;

	struct Script {
		std::string name;
		std::function<void()> entryPoint;
	};

	template<class F> auto scope_guard(F&& f) {
		return std::unique_ptr<void, typename std::decay<F>::type>{reinterpret_cast<void*>(1), std::forward<F>(f)};
	}

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

	private:
		static void ErrorWriter(const char_t* message);

	private:
		std::shared_ptr<plugify::IPlugifyProvider> _provider;
		std::unique_ptr<Assembly> _hostFxr;
		std::vector<Script> _scripts;
	};
}