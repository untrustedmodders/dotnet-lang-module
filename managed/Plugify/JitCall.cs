using System.Runtime.InteropServices;

namespace Plugify;

public class JitCall : SafeHandle
{
	public JitCall(nint target, ManagedType[] parameters, ManagedType ret) : base(nint.Zero, true)
	{
		handle = CallMethods.NewCall(target, parameters, parameters.Length, ret);
	}

	public override bool IsInvalid => handle == nint.Zero;

	public unsafe delegate*<nint*, nint*, void> Function => (delegate*<nint*, nint*, void>) CallMethods.GetCallFunction(handle);
	
	public string Error => CallMethods.GetCallError(handle);

	protected override bool ReleaseHandle()
	{
		CallMethods.DeleteCall(handle);
		return true;
	}
}

internal static class CallMethods
{
	[DllImport(NativeMethods.DllName)]
	public static extern nint NewCall(nint target, [In] ManagedType[] parameters, int count, ManagedType ret);

	[DllImport(NativeMethods.DllName)]
	public static extern void DeleteCall(nint call);

	[DllImport(NativeMethods.DllName)]
	public static extern nint GetCallFunction(nint call);

	[DllImport(NativeMethods.DllName)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetCallError(nint call);
}
