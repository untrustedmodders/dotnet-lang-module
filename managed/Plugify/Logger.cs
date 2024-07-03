using System;
using System.Runtime.InteropServices;

namespace Plugify;

public enum Severity : byte
{
	None = 0,
	Fatal = 1,
	Error = 2,
	Warning = 3,
	Info = 4,
	Debug = 5,
	Verbose = 6,
}
    
public static class Logger
{
	public static void Log(Severity severity, string message, params object[] args)
	{
		var frame = new System.Diagnostics.StackFrame(1, true);

		string formattedMessage;
		try
		{
			formattedMessage = string.Format(message, args);
		}
		catch (FormatException)
		{
			// Do nothing, just log as is
			formattedMessage = message;
		}

		string funcName = frame.GetMethod()?.Name ?? "<Unknown>";

		uint line = (uint)frame.GetFileLineNumber();

		Log((byte)severity, funcName, line, formattedMessage);
	}

	[DllImport(NativeMethods.DllName)]
	private static extern void Log(byte severity, [MarshalAs(UnmanagedType.LPStr)] string funcName, uint line, [MarshalAs(UnmanagedType.LPStr)] string message);
}
