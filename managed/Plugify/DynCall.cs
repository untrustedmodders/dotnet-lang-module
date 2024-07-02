using System.Runtime.InteropServices;

namespace Plugify;

public class DCCallVM : SafeHandle
{
    public DCCallVM(nuint size) : base(nint.Zero, true)
    {
        handle = DyncallMethods.NewVM(size);
        SetMode(CallingConventions.DC_CALL_C_DEFAULT);
    }

    public override bool IsInvalid => handle == nint.Zero;

    protected override bool ReleaseHandle()
    {
        DyncallMethods.Free(handle);
        return true;
    }

    public void Reset() => DyncallMethods.Reset(handle);
    public void SetMode(int mode) => DyncallMethods.Mode(handle, mode);
    
    public void ArgBool(bool value) => DyncallMethods.ArgBool(handle, value);
    public void ArgChar8(char value) => DyncallMethods.ArgChar8(handle, value);
    public void ArgChar16(char value) => DyncallMethods.ArgChar16(handle, value);
    public void ArgInt8(sbyte value) => DyncallMethods.ArgInt8(handle, value);
    public void ArgUInt8(byte value) => DyncallMethods.ArgUInt8(handle, value);
    public void ArgInt16(short value) => DyncallMethods.ArgInt16(handle, value);
    public void ArgUInt16(ushort value) => DyncallMethods.ArgUInt16(handle, value);
    public void ArgInt32(int value) => DyncallMethods.ArgInt32(handle, value);
    public void ArgUInt32(uint value) => DyncallMethods.ArgUInt32(handle, value);
    public void ArgInt64(long value) => DyncallMethods.ArgInt64(handle, value);
    public void ArgUInt64(ulong value) => DyncallMethods.ArgUInt64(handle, value);
    public void ArgFloat(float value) => DyncallMethods.ArgFloat(handle, value);
    public void ArgDouble(double value) => DyncallMethods.ArgDouble(handle, value);
    public void ArgPointer(nint value) => DyncallMethods.ArgPointer(handle, value);
    public void ArgAggr(DCaggr ag, nint value) => DyncallMethods.ArgAggr(handle, ag.DangerousGetHandle(), value);

    public void CallVoid(nint funcptr) => DyncallMethods.CallVoid(handle, funcptr);
    public bool CallBool(nint funcptr) => DyncallMethods.CallBool(handle, funcptr);
    public char CallChar8(nint funcptr) => DyncallMethods.CallChar8(handle, funcptr);
    public char CallChar16(nint funcptr) => DyncallMethods.CallChar16(handle, funcptr);
    public sbyte CallInt8(nint funcptr) => DyncallMethods.CallInt8(handle, funcptr);
    public byte CallUInt8(nint funcptr) => DyncallMethods.CallUInt8(handle, funcptr);
    public short CallInt16(nint funcptr) => DyncallMethods.CallInt16(handle, funcptr);
    public ushort CallUInt16(nint funcptr) => DyncallMethods.CallUInt16(handle, funcptr);
    public int CallInt32(nint funcptr) => DyncallMethods.CallInt32(handle, funcptr);
    public uint CallUInt32(nint funcptr) => DyncallMethods.CallUInt32(handle, funcptr);
    public long CallInt64(nint funcptr) => DyncallMethods.CallInt64(handle, funcptr);
    public ulong CallUInt64(nint funcptr) => DyncallMethods.CallUInt64(handle, funcptr);
    public float CallFloat(nint funcptr) => DyncallMethods.CallFloat(handle, funcptr);
    public double CallDouble(nint funcptr) => DyncallMethods.CallDouble(handle, funcptr);
    public nint CallPointer(nint funcptr) => DyncallMethods.CallPointer(handle, funcptr);
    public void CallAggr(nint funcptr, DCaggr ag, nint returnValue) => DyncallMethods.CallAggr(handle, funcptr, ag.DangerousGetHandle(), returnValue);
    public unsafe void CallAggr(nint funcptr, DCaggr ag, void* returnValue) => DyncallMethods.CallAggr(handle, funcptr, ag.DangerousGetHandle(), (nint)returnValue);
    public unsafe void BeginCallAggr(DCaggr ag) => DyncallMethods.BeginCallAggr(handle, ag.DangerousGetHandle());

    public int GetError() => DyncallMethods.GetError(handle);
}

public class DCaggr : SafeHandle
{
    public DCaggr(nuint fieldCount, nuint size) : base(nint.Zero, true)
    {
        handle = DyncallMethods.NewAggr(fieldCount, size);
    }

    public override bool IsInvalid => handle == nint.Zero;

    protected override bool ReleaseHandle()
    {
        DyncallMethods.FreeAggr(handle);
        return true;
    }

    public void AddField(SignatureChars type, int offset, nuint arrayLength) => DyncallMethods.AggrField(handle, (char)type, offset, arrayLength);
    public void Reset() => DyncallMethods.CloseAggr(handle);
}

internal static class CallingConventions
{
    public const int DC_CALL_C_DEFAULT = 0;
    public const int DC_CALL_C_ELLIPSIS = 100;
    public const int DC_CALL_C_ELLIPSIS_VARARGS = 101;
    public const int DC_CALL_C_X86_CDECL = 1;
    public const int DC_CALL_C_X86_WIN32_STD = 2;
    public const int DC_CALL_C_X86_WIN32_FAST_MS = 3;
    public const int DC_CALL_C_X86_WIN32_FAST_GNU = 4;
    public const int DC_CALL_C_X86_WIN32_THIS_MS = 5;
    public const int DC_CALL_C_X86_WIN32_THIS_GNU = 6;
    public const int DC_CALL_C_X64_WIN64 = 7;
    public const int DC_CALL_C_X64_SYSV = 8;
    public const int DC_CALL_C_PPC32_DARWIN = 9;
    public const int DC_CALL_C_PPC32_OSX = DC_CALL_C_PPC32_DARWIN; // alias
    public const int DC_CALL_C_ARM_ARM_EABI = 10;
    public const int DC_CALL_C_ARM_THUMB_EABI = 11;
    public const int DC_CALL_C_ARM_ARMHF = 30;
    public const int DC_CALL_C_MIPS32_EABI = 12;
    public const int DC_CALL_C_MIPS32_PSPSDK = DC_CALL_C_MIPS32_EABI; // alias - deprecated
    public const int DC_CALL_C_PPC32_SYSV = 13;
    public const int DC_CALL_C_PPC32_LINUX = DC_CALL_C_PPC32_SYSV; // alias
    public const int DC_CALL_C_ARM_ARM = 14;
    public const int DC_CALL_C_ARM_THUMB = 15;
    public const int DC_CALL_C_MIPS32_O32 = 16;
    public const int DC_CALL_C_MIPS64_N32 = 17;
    public const int DC_CALL_C_MIPS64_N64 = 18;
    public const int DC_CALL_C_X86_PLAN9 = 19;
    public const int DC_CALL_C_SPARC32 = 20;
    public const int DC_CALL_C_SPARC64 = 21;
    public const int DC_CALL_C_ARM64 = 22;
    public const int DC_CALL_C_PPC64 = 23;
    public const int DC_CALL_C_PPC64_LINUX = DC_CALL_C_PPC64; // alias
    public const int DC_CALL_SYS_DEFAULT = 200;
    public const int DC_CALL_SYS_X86_INT80H_LINUX = 201;
    public const int DC_CALL_SYS_X86_INT80H_BSD = 202;
    public const int DC_CALL_SYS_PPC32 = 210;

    // Error codes
    public const int DC_ERROR_NONE = 0;
    public const int DC_ERROR_UNSUPPORTED_MODE = -1;
}

public enum SignatureChars
{
    Void = 'v',
    Bool = 'B',
    Char = 'c',
    UChar = 'C',
    Short = 's',
    UShort = 'S',
    Int = 'i',
    UInt = 'I',
    Long = 'j',
    ULong = 'J',
    LongLong = 'l',
    ULongLong = 'L',
    Float = 'f',
    Double = 'd',
    Pointer = 'p', // also used for arrays, as such args decay to ptrs
    String = 'Z', // in theory same as 'p', but convenient to disambiguate
    Aggregate = 'A', // aggregate (struct/union described out-of-band via DCaggr)
    EndArg = ')',

    // Calling convention / mode signatures
    CcPrefix = '_', // announces next char to be one of the below calling convention mode chars
    CcDefault = ':', // default calling conv (platform native)
    CcThisCall = '*', // C++ this calls (platform native)
    CcEllipsis = 'e',
    CcEllipsisVarArgs = '.',
    CcCdecl = 'c', // x86 specific
    CcStdCall = 's', // x86 specific
    CcFastCallMs = 'F', // x86 specific
    CcFastCallGnu = 'f', // x86 specific
    CcThisCallMs = '+', // x86 specific, MS C++ this calls
    CcThisCallGnu = '#', // x86 specific, GNU C++ this calls are cdecl, but keep specific sig char for clarity
    CcArmArm = 'A',
    CcArmThumb = 'a',
    CcSysCall = '$',
}

internal static class DyncallMethods
{
    [DllImport(NativeMethods.DllName)]
    public static extern nint NewVM(nuint size);

    [DllImport(NativeMethods.DllName)]
    public static extern void Free(nint vm);

    [DllImport(NativeMethods.DllName)]
    public static extern void Reset(nint vm);

    [DllImport(NativeMethods.DllName)]
    public static extern void Mode(nint vm, int mode);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgBool(nint vm, bool value);

    [DllImport(NativeMethods.DllName, CharSet = CharSet.Ansi)]
    public static extern void ArgChar8(nint vm, char value);
    
    [DllImport(NativeMethods.DllName)]
    public static extern void ArgChar16(nint vm, char value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgInt8(nint vm, sbyte value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgUInt8(nint vm, byte value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgInt16(nint vm, short value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgUInt16(nint vm, ushort value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgInt32(nint vm, int value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgUInt32(nint vm, uint value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgInt64(nint vm, long value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgUInt64(nint vm, ulong value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgFloat(nint vm, float value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgDouble(nint vm, double value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgPointer(nint vm, nint value);

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgAggr(nint vm, nint ag, nint value);

    [DllImport(NativeMethods.DllName)]
    public static extern void CallVoid(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern bool CallBool(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern char CallChar8(nint vm, nint funcptr);
    
    [DllImport(NativeMethods.DllName)]
    public static extern char CallChar16(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern sbyte CallInt8(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern byte CallUInt8(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern short CallInt16(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern ushort CallUInt16(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern int CallInt32(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern uint CallUInt32(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern long CallInt64(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern ulong CallUInt64(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern float CallFloat(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern double CallDouble(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern nint CallPointer(nint vm, nint funcptr);

    [DllImport(NativeMethods.DllName)]
    public static extern void CallAggr(nint vm, nint funcptr, nint ag, nint returnValue);

    [DllImport(NativeMethods.DllName)]
    public static extern void BeginCallAggr(nint vm, nint ag);

    [DllImport(NativeMethods.DllName)]
    public static extern int GetError(nint vm);

    [DllImport(NativeMethods.DllName)]
    public static extern nint NewAggr(nuint fieldCount, nuint size);

    [DllImport(NativeMethods.DllName, CharSet = CharSet.Ansi)]
    public static extern void AggrField(nint ag, char type, int offset, nuint arrayLength);

    [DllImport(NativeMethods.DllName)]
    public static extern void CloseAggr(nint ag);

    [DllImport(NativeMethods.DllName)]
    public static extern void FreeAggr(nint ag);

    [DllImport(NativeMethods.DllName, CharSet = CharSet.Ansi)]
    public static extern int GetModeFromCCSigChar(char sigChar);

}