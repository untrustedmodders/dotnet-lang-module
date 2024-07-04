#pragma once

#include "core.h"

namespace netlm {
	class String {
	public:
		static String New(std::string_view string);

		static void Free(String& string);
		
		void Assign(std::string_view string);

		operator std::string() const;

		bool operator==(const String& other) const;
		bool operator==(std::string_view other) const;

#if NETLM_PLATFORM_WINDOWS
		static String New(std::wstring_view string);

		void Assign(std::wstring_view string);

		operator std::wstring() const;

		bool operator==(std::wstring_view other) const;
#endif

		char_t* Data() { return _string; }
		const char_t* Data() const { return _string; }

		bool IsNull() const { return _string == nullptr; }
		bool IsEmpty() const { return Size() == 0; }
		size_t Size() const;

	private:
		char_t* _string{ nullptr };
		Bool32 _disposed{ false }; // Required for the layout to match the C# NativeString struct, unused in C++
	};
}
