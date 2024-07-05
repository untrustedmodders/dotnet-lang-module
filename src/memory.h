#pragma once

#include "core.h"

namespace netlm {
	class Memory {
	public:
		Memory() = delete;

		static void* AllocHGlobal(size_t size);
		static void FreeHGlobal(void* ptr);

		static char_t* StringToCoTaskMemAuto(string_view_t string);
		static char* StringToHGlobalAnsi(std::string_view string);
		static void FreeCoTaskMem(void* memory);
	};
}
