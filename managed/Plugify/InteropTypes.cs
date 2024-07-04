using System.Runtime.InteropServices;

namespace Plugify;

[StructLayout(LayoutKind.Sequential)]
public struct NativeString : IDisposable
{
	internal nint _string;
	private Bool32 _disposed;

	public void Dispose()
	{
		if (!_disposed)
		{
			if (_string != nint.Zero)
			{
				Marshal.FreeCoTaskMem(_string);
				_string = nint.Zero;
			}

			_disposed = true;
		}

		GC.SuppressFinalize(this);
	}

	public override string? ToString() => this;

	public static NativeString Null() => new(){ _string = nint.Zero };

	public static implicit operator NativeString(string? value) => new(){ _string = Marshal.StringToCoTaskMemAuto(value) };
	public static implicit operator string?(NativeString value) => Marshal.PtrToStringAuto(value._string);
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Bool32
{
	public uint Value { get; init; }

	public static implicit operator Bool32(bool value) => new() { Value = value ? 1u : 0u };
	public static implicit operator bool(Bool32 value) => value.Value > 0;
}
