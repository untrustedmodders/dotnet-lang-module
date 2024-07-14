#pragma once

#include "core.h"

namespace netlm {
	class String {
	public:
		static String New(std::string_view string);

		static void Free(String& string);
		
		void Assign(std::string_view string);

		operator std::string() const;

		bool operator==(std::string_view other) const;

#if NETLM_PLATFORM_WINDOWS
		static String New(std::wstring_view string);

		void Assign(std::wstring_view string);

		operator std::wstring() const;

		bool operator==(std::wstring_view other) const;
#endif

		bool operator==(const String& other) const;

		char_t* Data() { return _string; }
		const char_t* Data() const { return _string; }

		bool IsNull() const { return _string == nullptr; }
		bool IsEmpty() const { return Size() == 0; }
		size_t Size() const{ return static_cast<size_t>(_length); }

	private:
		char_t* _string{ nullptr };
		int32_t _length{ 0 };
		Bool32 _disposed{ false }; // unused in C++
	};
}
