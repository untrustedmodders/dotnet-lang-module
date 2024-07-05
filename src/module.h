#include <plugify/language_module.h>
#include <plugify/method.h>
#include <plugify/plugin.h>
#include <plugify/function.h>

#include "host_instance.h"

#include <asmjit/asmjit.h>
#include <dyncall/dyncall.h>
#include <cpptrace/cpptrace.hpp>

namespace netlm {
	class ScriptInstance {
	public:
		ScriptInstance(const plugify::IPlugin& plugin, Type& type);
		~ScriptInstance();

		const plugify::IPlugin& GetPlugin() const { return _plugin; }

	private:
		void InvokeOnStart() const;
		void InvokeOnEnd() const;

	private:
		const plugify::IPlugin& _plugin;
		ManagedObject _instance;

		friend class DotnetLanguageModule;
	};

	using ScriptMap = std::unordered_map<std::string, ScriptInstance>;

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
		ScriptInstance* FindScript(const std::string& name);

		const std::shared_ptr<plugify::IPlugifyProvider>& GetProvider() { return _provider; }
		void* GetNativeMethod(const std::string& methodName) const;

	private:
		static void ExceptionCallback(const std::string& message);
		static void MessageCallback(const std::string& message, MessageLevel level);

		static void InternalCall(const plugify::Method* method, void* data, const plugify::Parameters* p, uint8_t count, const plugify::ReturnValue* ret);

	private:
		std::shared_ptr<plugify::IPlugifyProvider> _provider;
		std::shared_ptr<asmjit::JitRuntime> _rt;
		HostInstance _host;
		AssemblyLoadContext _alc;

		ScriptMap _scripts;
		std::unordered_map<std::string, void*> _nativesMap;
		std::vector<std::unique_ptr<plugify::Function>> _functions;
	};

	extern DotnetLanguageModule g_netlm;
}
