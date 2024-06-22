#include <plugify/language_module.h>
#include <plugify/method.h>

#include <dotnet/coreclr_delegates.h>
#include <dotnet/error_codes.h>
#include <dotnet/hostfxr.h>

namespace netlm {
	constexpr int kApiVersion = 1;

	struct MethodRaw;
	struct ManagedGuid;
	struct ManagedObject;

	using InitializeFunc = void (*)();
	using ShutdownFunc = void (*)();
	using LoadPluginFunc = void (*)(const char*, char*, const ManagedGuid*, const MethodRaw*, void**, int);
	using StartPluginFunc = void (*)();
	using EndPluginFunc = void (*)();

	class Assembly;
	class Library;

	struct PropertyRaw {
		plugify::ValueType type;
		bool ref;
		MethodRaw* prototype;
	};

	struct MethodRaw {
		const char* nameStringPtr;
		const char* funcNameStringPtr;
		const char* callConvStringPtr;
		const PropertyRaw* paramTypesArrayPtr;
		PropertyRaw* retType;
		uint8_t varIndex;
		int32_t paramCount;
	};

	using PropertyHolder = std::vector<std::unique_ptr<PropertyRaw[]>>;
	using MethodHolder =  std::vector<std::unique_ptr<MethodRaw>>;
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

		UniquePtr<Assembly> LoadAssembly(const char* path) const;
		bool UnloadAssembly(ManagedGuid guid) const;

		void AddMethodToCache(ManagedGuid assembly_guid, ManagedGuid method_guid, void* method_info_ptr) const;
		void AddObjectToCache(ManagedGuid assembly_guid, ManagedGuid object_guid, void* object_ptr, ManagedObject* out_managed_object) const;

		const std::shared_ptr<plugify::IPlugifyProvider>& GetProvider() { return _provider; }
		void* GetNativeMethod(const std::string& methodName) const;

	private:
		bool LoadHostFXR(const fs::path& hostPath);
		ErrorString InitializeRuntimeHost(const fs::path& configPath);

		static void ErrorWriter(const char_t* message);

	private:
		std::shared_ptr<plugify::IPlugifyProvider> _provider;
		std::unique_ptr<Library> _hostFxr;
		std::deleted_unique_ptr<void> _context;
		std::unordered_map<std::string, void*> _nativesMap;

		InitializeFunc _initialize;
		ShutdownFunc _shutdown;
		LoadPluginFunc _loadPlugin;
		StartPluginFunc _startPlugin;
		EndPluginFunc _endPlugin;
	};
}