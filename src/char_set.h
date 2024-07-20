#pragma once

namespace netlm {
	enum class CharSet {
		/// <summary>This value is obsolete and has the same behavior as <see cref="F:System.Runtime.InteropServices.CharSet.Ansi" />.</summary>
		None = 1,
		/// <summary>Marshal strings as multiple-byte character strings: the system default Windows (ANSI) code page on Windows, and UTF-8 on Unix.</summary>
		Ansi = 2,
		/// <summary>Marshal strings as Unicode 2-byte character strings.</summary>
		Unicode = 3,
		/// <summary>Automatically marshal strings appropriately for the target operating system. See Charsets and marshaling for details. Although the common language runtime default is <see cref="F:System.Runtime.InteropServices.CharSet.Auto" />, languages may override this default. For example, by default C# and Visual Basic mark all methods and types as <see cref="F:System.Runtime.InteropServices.CharSet.Ansi" />.</summary>
		Auto = 4,
	};
}