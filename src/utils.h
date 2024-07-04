#pragma once

#include <cstring>

#if NETLM_PLATFORM_WINDOWS
#define NETLM_STR(str) L##str
#define NETLM_UTF8(str) Utils::WideStringToUTF8String(str)
#else
#define NETLM_STR(str) str
#define NETLM_UTF8(str) str
#endif

namespace netlm {
	class Utils {
	public:
		Utils() = delete;

#if NETLM_PLATFORM_WINDOWS
		/// Converts the specified UTF-8 string to a wide string.
		static std::wstring UTF8StringToWideString(std::string_view str);
		static bool UTF8StringToWideString(std::wstring& dest, std::string_view str);

		/// Converts the specified wide string to a UTF-8 string.
		static std::string WideStringToUTF8String(std::wstring_view str);
		static bool WideStringToUTF8String(std::string& dest, std::wstring_view str);
#endif

		static std::vector<std::string_view> Split(std::string_view strv, std::string_view delims = " ");
	};
}
