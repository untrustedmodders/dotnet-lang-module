#include "memory.h"

#if NETLM_PLATFORM_WINDOWS
#include <strsafe.h>
#include <objbase.h>
#endif

using namespace netlm;

void* Memory::AllocHGlobal(size_t size) {
#if NETLM_PLATFORM_WINDOWS
	return LocalAlloc(LMEM_FIXED | LMEM_ZEROINIT, size);
#else
	return malloc(size);
#endif
}

void Memory::FreeHGlobal(void* ptr) {
#if NETLM_PLATFORM_WINDOWS
	LocalFree(ptr);
#else
	free(ptr);
#endif
}

char_t* Memory::StringToCoTaskMemAuto(string_view_t string) {
	size_t length = string.length() + 1;
	size_t size = length * sizeof(char_t);

#if NETLM_PLATFORM_WINDOWS
	auto* buffer = static_cast<char_t*>(CoTaskMemAlloc(size));
#else
	auto* buffer = static_cast<char_t*>(AllocHGlobal(size));
#endif

	if (buffer != nullptr) {
		memcpy(buffer, string.data(), size);
	}

	return buffer;
}

char* Memory::StringToHGlobalAnsi(std::string_view string) {
	size_t length = string.length() + 1;
	size_t size = length * sizeof(char);

#if NETLM_PLATFORM_WINDOWS
	auto* buffer = static_cast<char*>(CoTaskMemAlloc(size));
#else
	auto* buffer = static_cast<char*>(AllocHGlobal(size));
#endif

	if (buffer != nullptr) {
		memcpy(buffer, string.data(), size);
	}

	return buffer;
}

void Memory::FreeCoTaskMem(void* memory) {
#if NETLM_PLATFORM_WINDOWS
	CoTaskMemFree(memory);
#else
	FreeHGlobal(memory);
#endif
}
