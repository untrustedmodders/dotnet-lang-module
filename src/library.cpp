#include "library.h"

#if NETLM_PLATFORM_WINDOWS
#include <windows.h>
#elif NETLM_PLATFORM_LINUX
#include <dlfcn.h>
#elif NETLM_PLATFORM_APPLE
#include <dlfcn.h>
#else
#error "Platform is not supported!"
#endif

namespace netlm {
	thread_local static std::string lastError;

	std::unique_ptr<Library> Library::LoadFromPath(const std::filesystem::path& assemblyPath) {
#if NETLM_PLATFORM_WINDOWS
		void* handle = static_cast<void*>(LoadLibraryW(assemblyPath.c_str()));
#elif NETLM_PLATFORM_LINUX || NETLM_PLATFORM_APPLE
		void* handle = dlopen(assemblyPath.string().c_str(), RTLD_LAZY | RTLD_NODELETE);
#else
		void* handle = nullptr;
#endif
		if (handle) {
#if NETLM_PLATFORM_WINDOWS
			GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_PIN, assemblyPath.filename().c_str(), reinterpret_cast<HMODULE*>(&handle));
#endif
			return std::unique_ptr<Library>(new Library(handle));
		}
#if NETLM_PLATFORM_WINDOWS
		uint32_t errorCode = GetLastError();
		if (errorCode != 0) {
			LPSTR messageBuffer = nullptr;
			size_t size = FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
										 NULL, errorCode, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), reinterpret_cast<LPSTR>(&messageBuffer), 0, NULL);
			lastError = std::string(messageBuffer, size);
			LocalFree(messageBuffer);
		}
#elif NETLM_PLATFORM_LINUX || NETLM_PLATFORM_APPLE
		lastError = dlerror();
#endif
		return nullptr;
	}

	std::string Library::GetError() {
		return lastError;
	}

	Library::Library(void* handle) : _handle{handle} {
	}

	Library::~Library() {
#if NETLM_PLATFORM_WINDOWS
		FreeLibrary(static_cast<HMODULE>(_handle));
#elif NETLM_PLATFORM_LINUX || NETLM_PLATFORM_APPLE
		dlclose(_handle);
#endif
	}

	void* Library::GetFunction(const char* functionName) const {
#if NETLM_PLATFORM_WINDOWS
		return reinterpret_cast<void*>(GetProcAddress(static_cast<HMODULE>(_handle), functionName));
#elif NETLM_PLATFORM_LINUX || NETLM_PLATFORM_APPLE
		return dlsym(_handle, functionName);
#else
		return nullptr;
#endif
	}
}