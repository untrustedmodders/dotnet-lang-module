using System.Runtime.InteropServices;

namespace Plugify;

public class DCCallVM : SafeHandle
{
    public DCCallVM(nuint size) : base(nint.Zero, true)
    {
        handle = DCMethods.NewVM(size);
    }

    public override bool IsInvalid => handle == nint.Zero;

    protected override bool ReleaseHandle()
    {
        DCMethods.Free(handle);
        return true;
    }

    public void Reset() => DCMethods.Reset(handle);
    public void SetMode(int mode) => DCMethods.Mode(handle, mode);
    
    public void ArgBool(bool value) => DCMethods.ArgBool(handle, value);
    public void ArgChar(char value) => DCMethods.ArgChar(handle, value);
    public void ArgInt8(sbyte value) => DCMethods.ArgInt8(handle, value);
    public void ArgUInt8(byte value) => DCMethods.ArgUInt8(handle, value);
    public void ArgInt16(short value) => DCMethods.ArgInt16(handle, value);
    public void ArgUInt16(ushort value) => DCMethods.ArgUInt16(handle, value);
    public void ArgInt32(int value) => DCMethods.ArgInt32(handle, value);
    public void ArgUInt32(uint value) => DCMethods.ArgUInt32(handle, value);
    public void ArgInt64(long value) => DCMethods.ArgInt64(handle, value);
    public void ArgUInt64(ulong value) => DCMethods.ArgUInt64(handle, value);
    public void ArgFloat(float value) => DCMethods.ArgFloat(handle, value);
    public void ArgDouble(double value) => DCMethods.ArgDouble(handle, value);
    public void ArgPointer(nint value) => DCMethods.ArgPointer(handle, value);
    public void ArgAggr(DCaggr ag, nint value) => DCMethods.ArgAggr(handle, ag.DangerousGetHandle(), value);

    public void CallVoid(nint funcptr) => DCMethods.CallVoid(handle, funcptr);
    public bool CallBool(nint funcptr) => DCMethods.CallBool(handle, funcptr);
    public char CallChar(nint funcptr) => DCMethods.CallChar(handle, funcptr);
    public sbyte CallInt8(nint funcptr) => DCMethods.CallInt8(handle, funcptr);
    public byte CallUInt8(nint funcptr) => DCMethods.CallUInt8(handle, funcptr);
    public short CallInt16(nint funcptr) => DCMethods.CallInt16(handle, funcptr);
    public ushort CallUInt16(nint funcptr) => DCMethods.CallUInt16(handle, funcptr);
    public int CallInt32(nint funcptr) => DCMethods.CallInt32(handle, funcptr);
    public uint CallUInt32(nint funcptr) => DCMethods.CallUInt32(handle, funcptr);
    public long CallInt64(nint funcptr) => DCMethods.CallInt64(handle, funcptr);
    public ulong CallUInt64(nint funcptr) => DCMethods.CallUInt64(handle, funcptr);
    public float CallFloat(nint funcptr) => DCMethods.CallFloat(handle, funcptr);
    public double CallDouble(nint funcptr) => DCMethods.CallDouble(handle, funcptr);
    public nint CallPointer(nint funcptr) => DCMethods.CallPointer(handle, funcptr);
    public void CallAggr(nint funcptr, DCaggr ag, nint returnValue) => DCMethods.CallAggr(handle, funcptr, ag.DangerousGetHandle(), returnValue);

    public int GetError() => DCMethods.GetError(handle);
}

public class DCaggr : SafeHandle
{
    public DCaggr(nuint fieldCount, nuint size) : base(nint.Zero, true)
    {
        handle = DCMethods.NewAggr(fieldCount, size);
    }

    public override bool IsInvalid => handle == nint.Zero;

    protected override bool ReleaseHandle()
    {
        DCMethods.FreeAggr(handle);
        return true;
    }

    public void AddField(char type, int offset, nuint arrayLength) => DCMethods.AggrField(handle, type, offset, arrayLength);
    public new void Close() => DCMethods.CloseAggr(handle);
}

internal static class DCMethods
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

    [DllImport(NativeMethods.DllName)]
    public static extern void ArgChar(nint vm, [MarshalAs(UnmanagedType.I1)] char value);

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
    public static extern char CallChar(nint vm, nint funcptr);

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
    public static extern int GetError(nint vm);

    [DllImport(NativeMethods.DllName)]
    public static extern nint NewAggr(nuint fieldCount, nuint size);

    [DllImport(NativeMethods.DllName)]
    public static extern void AggrField(nint ag, [MarshalAs(UnmanagedType.I1)] char type, int offset, nuint arrayLength);

    [DllImport(NativeMethods.DllName)]
    public static extern void CloseAggr(nint ag);

    [DllImport(NativeMethods.DllName)]
    public static extern void FreeAggr(nint ag);

    [DllImport(NativeMethods.DllName)]
    public static extern int GetModeFromCCSigChar([MarshalAs(UnmanagedType.I1)] char sigChar);

}