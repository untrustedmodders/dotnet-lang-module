#include "strings.h"
#include "memory.h"
#include "utils.h"

using namespace netlm;

String String::New(std::string_view string) {
	String result;
	result.Assign(string);
	return result;
}

void String::Free(String& string) {
	if (string._string == nullptr)
		return;

	Memory::FreeCoTaskMem(string._string);
	string._string = nullptr;
}

void String::Assign(std::string_view string) {
	if (_string != nullptr)
		Memory::FreeCoTaskMem(_string);

	_string = Memory::StringToCoTaskMemAuto(Utils::UTF8StringToWideString(string));
}

String::operator std::string() const {
	string_view_t string(_string);

#if NETLM_PLATFORM_WINDOWS
	return Utils::WideStringToUTF8String(string);
#else
	return std::string(string);
#endif
}

bool String::operator==(const String& other) const {
	if (_string == other._string)
		return true;

	if (_string == nullptr || other._string == nullptr)
		return false;

#if NETLM_PLATFORM_WINDOWS
	return wcscmp(_string, other._string) == 0;
#else
	return strcmp(_string, other._string) == 0;
#endif
}

bool String::operator==(std::string_view other) const {
#if NETLM_PLATFORM_WINDOWS
	auto str = Utils::UTF8StringToWideString(other);
	return wcscmp(_string, str.data()) == 0;
#else
	return strcmp(_string, other.data()) == 0;
#endif
}

size_t String::Size() const {
	if (IsNull())
		return 0;

#if NETLM_PLATFORM_WINDOWS
	return wcslen(_string);
#else
	return strlen(_string);
#endif
}

#if NETLM_PLATFORM_WINDOWS

String String::New(std::wstring_view string) {
	String result;
	result.Assign(string);
	return result;
}

void String::Assign(std::wstring_view string) {
	if (_string != nullptr)
		Memory::FreeCoTaskMem(_string);

	_string = Memory::StringToCoTaskMemAuto(string);
}

String::operator std::wstring() const {
	string_view_t string(_string);

#if NETLM_PLATFORM_WINDOWS
	return std::wstring(string);
#else
	return Utils::UTF8StringToWideString(string);
#endif
}

bool String::operator==(std::wstring_view other) const {
#if NETLM_PLATFORM_WINDOWS
	return wcscmp(_string, other.data()) == 0;
#else
	auto str = Utils::WideStringToUTF8String(other);
	return strcmp(_string, str.data()) == 0;
#endif
}

#endif
