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

	if (buffer != nullptr)
	{
		memset(buffer, 0xCE, size);
		//wcscpy(buffer, string.data());
		wcscpy_s(buffer, length, string.data());
	}
#else
	auto* buffer = static_cast<char_t*>(AllocHGlobal(size));

	if (buffer != nullptr)
	{
		memset(buffer, 0, size);
		strncpy(buffer, string.data(), length);
	}
#endif

	return buffer;
}

void Memory::FreeCoTaskMem(void* memory) {
#if NETLM_PLATFORM_WINDOWS
	CoTaskMemFree(memory);
#else
	FreeHGlobal(memory);
#endif
}
