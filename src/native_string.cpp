#include "memory.h"
#include "native_string.h"
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
	string._length = 0;
}

void String::Assign(std::string_view string) {
	if (_string != nullptr)
		Memory::FreeCoTaskMem(_string);

#if NETLM_PLATFORM_WINDOWS
	_string = Memory::StringToCoTaskMemAuto(Utils::ConvertUtf8ToWide(string));
#else
	_string = Memory::StringToCoTaskMemAuto(string);
#endif
	_length = static_cast<int32_t>(string.size());
}

String::operator std::string() const {
	string_view_t string(_string);

#if NETLM_PLATFORM_WINDOWS
	return Utils::ConvertWideToUtf8(string);
#else
	return std::string(string);
#endif
}

bool String::operator==(std::string_view other) const {
#if NETLM_PLATFORM_WINDOWS
	return _length == static_cast<int32_t>(other.size()) && wcscmp(_string, Utils::ConvertUtf8ToWide(other).data()) == 0;
#else
	return _length == static_cast<int32_t>(other.size()) && strcmp(_string, other.data()) == 0;
#endif
}

#if NETLM_PLATFORM_WINDOWS

// Not used on Unix

String String::New(std::wstring_view string) {
	String result;
	result.Assign(string);
	return result;
}

void String::Assign(std::wstring_view string) {
	if (_string != nullptr)
		Memory::FreeCoTaskMem(_string);

	_string = Memory::StringToCoTaskMemAuto(string);
	_length = static_cast<int32_t>(string.size());
}

String::operator std::wstring() const {
	string_view_t string(_string);

#if NETLM_PLATFORM_WINDOWS
	return std::wstring(string);
#else
	return Utils::ConvertUtf8ToWide(string);
#endif
}

bool String::operator==(std::wstring_view other) const {
#if NETLM_PLATFORM_WINDOWS
	return _length == static_cast<int32_t>(other.size()) && wcscmp(_string, other.data()) == 0;
#else
	return _length == static_cast<int32_t>(other.size()) && strcmp(_string, Utils::ConvertWideToUtf8(other).data()) == 0;
#endif
}

#endif

bool String::operator==(const String& other) const {
	if (_string == other._string)
		return true;

	if (_string == nullptr || other._string == nullptr)
		return false;

#if NETLM_PLATFORM_WINDOWS
	return _length == other._length && wcscmp(_string, other._string) == 0;
#else
	return _length == other._length && strcmp(_string, other._string) == 0;
#endif
}