#pragma once

namespace netlm {
#if NETLM_PLATFORM_WINDOWS
	#ifdef _WCHAR_T_DEFINED
		typedef wchar_t char_t;
		using string_view_t = std::wstring_view;
	#else
		typedef unsigned short char_t;
		using string_view_t = std::u16string_view;
	#endif
#else
	typedef char char_t;
	using string_view_t = std::string_view;
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

	/*struct InternalCall {
		const char_t* name;
		void* nativeFunctionVPtr;
	};*/
}
