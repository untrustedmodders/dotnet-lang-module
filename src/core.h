#pragma once

namespace netlm {
#if NETLM_PLATFORM_WINDOWS
	#ifdef _WCHAR_T_DEFINED
		typedef wchar_t char_t;
		using string_view_t = std::wstring_view;
		using string_t = std::wstring;
	#else
		typedef unsigned short char_t;
		using string_view_t = std::u16string_view;
		using string_t = std::u16string;
	#endif
#else
	typedef char char_t;
	using string_view_t = std::string_view;
	using string_t = std::string;
#endif

	using Bool32 = uint32_t;

	enum class TypeAccessibility {
		Public,
		Private,
		Protected,
		Internal,
		ProtectedPublic,
		PrivateProtected
	};

	using TypeId = int32_t;
	using ManagedHandle = int32_t;

	struct InternalCall {
		const char_t* name;
		void* nativeFunctionPtr;
	};
}

#define NETLM_DOTNET_TARGET_VERSION_MAJOR 8
#define NETLM_DOTNET_TARGET_VERSION_MAJOR_STR '8'
#define NETLM_UNMANAGED_CALLERS_ONLY (reinterpret_cast<const char_t*>(-1))
