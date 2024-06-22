using System;
using System.Runtime.InteropServices;

namespace Plugify;

[StructLayout(LayoutKind.Sequential, Size = 24)]
public struct ManagedObject
{
	public Guid guid;
	public nint ptr;
}