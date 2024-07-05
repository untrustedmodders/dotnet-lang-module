#include "utils.h"

#include <dotnet/error_codes.h>

using namespace netlm;

#if NETLM_PLATFORM_WINDOWS

#include <Windows.h>

std::wstring Utils::UTF8StringToWideString(std::string_view str){
	std::wstring ret;
	if (!UTF8StringToWideString(ret, str))
		return {};
	return ret;
}

bool Utils::UTF8StringToWideString(std::wstring& dest, std::string_view str) {
	int wlen = MultiByteToWideChar(CP_UTF8, 0, str.data(), static_cast<int>(str.length()), nullptr, 0);
	if (wlen < 0)
		return false;

	dest.resize(static_cast<size_t>(wlen));
	if (wlen > 0 && MultiByteToWideChar(CP_UTF8, 0, str.data(), static_cast<int>(str.length()), dest.data(), wlen) < 0)
		return false;

	return true;
}

std::string Utils::WideStringToUTF8String(std::wstring_view str) {
	std::string ret;
	if (!WideStringToUTF8String(ret, str))
		return {};
	return ret;
}

bool Utils::WideStringToUTF8String(std::string& dest, std::wstring_view str) {
	int mblen = WideCharToMultiByte(CP_UTF8, 0, str.data(), static_cast<int>(str.length()), nullptr, 0, nullptr, nullptr);
	if (mblen < 0)
		return false;

	dest.resize(static_cast<size_t>(mblen));
	if (mblen > 0 && WideCharToMultiByte(CP_UTF8, 0, str.data(), static_cast<int>(str.length()), dest.data(), mblen, nullptr, nullptr) < 0)
		return false;

	return true;
}
#endif

std::vector<std::string_view> Utils::Split(std::string_view strv, std::string_view delims) {
	std::vector<std::string_view> output;
	size_t first = 0;

	while (first < strv.size()) {
		const size_t second = strv.find_first_of(delims, first);

		if (first != second)
			output.emplace_back(strv.substr(first, second-first));

		if (second == std::string_view::npos)
			break;

		first = second + 1;
	}

	return output;
}
