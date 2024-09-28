#include <plugify/language_module.h>
#include <plugify/method.h>
#include <plugify/plugin.h>
#include <plugify/jit/call.h>
#include <plugify/jit/callback.h>

#include "host_instance.h"

#include <asmjit/asmjit.h>
#include <cpptrace/cpptrace.hpp>

namespace netlm {
	class ScriptInstance {
	public:
		ScriptInstance(plugify::PluginRef plugin, Type& type);
		~ScriptInstance();

		plugify::PluginRef GetPlugin() const { return _plugin; }
		const ManagedObject& GetManagedObject() const { return _instance; }

		bool operator==(const ScriptInstance& other) const { return _instance == other._instance; }
		operator bool() const { return _instance; }

	private:
		void InvokeOnStart() const;
		void InvokeOnEnd() const;

	private:
		plugify::PluginRef _plugin;
		ManagedObject _instance;

		friend class DotnetLanguageModule;
	};

	using ScriptMap = std::map<plugify::UniqueId, ScriptInstance>;
	using FunctionList = std::vector<plugify::JitCallback>;

	class DotnetLanguageModule final : public plugify::ILanguageModule {
	public:
		DotnetLanguageModule() = default;

		// ILanguageModule
		plugify::InitResult Initialize(std::weak_ptr<plugify::IPlugifyProvider> provider, plugify::ModuleRef module) override;
		void Shutdown() override;
		plugify::LoadResult OnPluginLoad(plugify::PluginRef plugin) override;
		void OnPluginStart(plugify::PluginRef plugin) override;
		void OnPluginEnd(plugify::PluginRef plugin) override;
		void OnMethodExport(plugify::PluginRef plugin) override;
		bool IsDebugBuild() override;

		const ScriptMap& GetScripts() const { return _scripts; }
		ScriptInstance* FindScript(plugify::UniqueId pluginId);

		const std::shared_ptr<plugify::IPlugifyProvider>& GetProvider() { return _provider; }
		const std::shared_ptr<asmjit::JitRuntime>& GetRuntime() { return _rt; }

	private:
		static void ExceptionCallback(std::string_view message);
		static void MessageCallback(std::string_view message, MessageLevel level);
		static void DetectLeaks();

		static void InternalCall(plugify::MethodRef method, plugify::MemAddr data, const plugify::JitCallback::Parameters* p, uint8_t count, const plugify::JitCallback::ReturnValue* ret);

	private:
		std::shared_ptr<plugify::IPlugifyProvider> _provider;
		std::shared_ptr<asmjit::JitRuntime> _rt;

		HostInstance _host;
		AssemblyLoadContext _alc;

		ScriptMap _scripts;
		FunctionList _functions;
	};

	extern DotnetLanguageModule g_netlm;
}
