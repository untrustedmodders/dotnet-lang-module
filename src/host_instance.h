#pragma once

#include "core.h"
#include "managed_assembly.h"

namespace netlm {
	enum class MessageLevel {
		Info = 1 << 0,
		Warning = 1 << 1,
		Error = 1 << 2,
		All = Info | Warning | Error
	};

	using MessageCallbackFn = std::function<void(const std::string&, MessageLevel)>;
	using ExceptionCallbackFn = std::function<void(const std::string&)>;

	struct HostSettings {
		fs::path hostfxrPath;
		fs::path rootDirectory;
		
		MessageCallbackFn messageCallback;
		MessageLevel messageFilter = MessageLevel::All;

		ExceptionCallbackFn exceptionCallback;
	};

	struct HandleDeleter {
		void operator()(void* handle) const noexcept;
	};

	class Assembly;

	class HostInstance {
	public:
		bool Initialize(HostSettings settings);
		void Shutdown();

		AssemblyLoadContext CreateAssemblyLoadContext(std::string_view contextName);
		void UnloadAssemblyLoadContext(AssemblyLoadContext& loadContext);

	private:
		bool LoadHostFXR();
		bool InitializeRuntimeHost();

		void LoadManagedFunctions(const fs::path& assemblyPath);

		void* GetDelegate(const char_t* assemblyPath, const char_t* typeName, const char_t* methodName, const char_t* delegateType = NETLM_UNMANAGED_CALLERS_ONLY) const;

		template<typename TFunc> requires(std::is_pointer_v<TFunc> && std::is_function_v<std::remove_pointer_t<TFunc>>)
		TFunc GetDelegate(const char_t* assemblyPath, const char_t* typeName, const char_t* methodName, const char_t* delegateType = NETLM_UNMANAGED_CALLERS_ONLY) const {
			return reinterpret_cast<TFunc>(GetDelegate(assemblyPath, typeName, methodName, delegateType));
		}

	private:
		HostSettings _settings;
		std::unique_ptr<void, HandleDeleter> _ctx;
		std::unique_ptr<Assembly> _dll;

		friend class AssemblyLoadContext;
	};
}