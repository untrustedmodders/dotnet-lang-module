using System.Numerics;
using System.Runtime.InteropServices;
using Plugify;
using static cross_call_master.cross_call_master;

namespace cross_call_worker;

public class ExportClass
{
    public static void NoParamReturnVoid()
    {
        Console.WriteLine("NoParamReturnVoid");
    }

    public static bool NoParamReturnBool()
    {
        Console.WriteLine("NoParamReturnBool");
        return true;
    }

    [return: CharSet(CharSet.Ansi)]
    public static char NoParamReturnChar8()
    {
        Console.WriteLine("NoParamReturnChar8");
        return (char)127;
    }
        
    public static char NoParamReturnChar16()
    {
        Console.WriteLine("NoParamReturnChar16");
        return char.MaxValue;
    }
        
    public static sbyte NoParamReturnInt8()
    {
        Console.WriteLine("NoParamReturnInt8");
        return sbyte.MaxValue;
    }

    public static short NoParamReturnInt16()
    {
        Console.WriteLine("NoParamReturnInt16");
        return short.MaxValue;
    }

    public static int NoParamReturnInt32()
    {
        Console.WriteLine("NoParamReturnInt32");
        return int.MaxValue;
    }

    public static long NoParamReturnInt64()
    {
        Console.WriteLine("NoParamReturnInt64");
        return long.MaxValue;
    }

    public static byte NoParamReturnUInt8()
    {
        Console.WriteLine("NoParamReturnUInt8");
        return byte.MaxValue;
    }

    public static ushort NoParamReturnUInt16()
    {
        Console.WriteLine("NoParamReturnUInt16");
        return ushort.MaxValue;
    }

    public static uint NoParamReturnUInt32()
    {
        Console.WriteLine("NoParamReturnUInt32");
        return uint.MaxValue;
    }

    public static ulong NoParamReturnUInt64()
    {
        Console.WriteLine("NoParamReturnUInt64");
        return ulong.MaxValue;
    }

    public static IntPtr NoParamReturnPointer()
    {
        Console.WriteLine("NoParamReturnPointer");
        return IntPtr.Zero + 1;
    }

    public static float NoParamReturnFloat()
    {
        Console.WriteLine("NoParamReturnFloat");
        return float.MaxValue;
    }

    public static double NoParamReturnDouble()
    {
        Console.WriteLine("NoParamReturnDouble");
        return double.MaxValue;
    }

    public delegate void NoParamReturnFunctionDelegate();

    public static NoParamReturnFunctionDelegate? NoParamReturnFunction()
    {
        Console.WriteLine("NoParamReturnFunction");
        return null;
    }

    public static string NoParamReturnString()
    {
        Console.WriteLine("NoParamReturnString");
        return "Hello World";
    }

    public static bool[] NoParamReturnArrayBool()
    {
        Console.WriteLine("NoParamReturnArrayBool");
        return new bool[] { true, false };
    }

    [return: CharSet(CharSet.Ansi)]
    public static char[] NoParamReturnArrayChar8()
    {
        Console.WriteLine("NoParamReturnArrayChar8");
        return new char[] { 'a', 'b', 'c', 'd' };
    }

    public static char[] NoParamReturnArrayChar16()
    {
        Console.WriteLine("NoParamReturnArrayChar16");
        return new char[] { 'a', 'b', 'c', 'd' };
    }

    public static sbyte[] NoParamReturnArrayInt8()
    {
        Console.WriteLine("NoParamReturnArrayInt8");
        return new sbyte[] { -3, -2, -1, 0, 1 };
    }

    public static short[] NoParamReturnArrayInt16()
    {
        Console.WriteLine("NoParamReturnArrayInt16");
        return new short[] { -4, -3, -2, -1, 0, 1 };
    }

    public static int[] NoParamReturnArrayInt32()
    {
        Console.WriteLine("NoParamReturnArrayInt32");
        return new int[] { -5, -4, -3, -2, -1, 0, 1 };
    }

    public static long[] NoParamReturnArrayInt64()
    {
        Console.WriteLine("NoParamReturnArrayInt64");
        return new long[] { -6, -5, -4, -3, -2, -1, 0, 1 };
    }

    public static byte[] NoParamReturnArrayUInt8()
    {
        Console.WriteLine("NoParamReturnArrayUInt8");
        return new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    }

    public static ushort[] NoParamReturnArrayUInt16()
    {
        Console.WriteLine("NoParamReturnArrayUInt16");
        return new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    }

    public static uint[] NoParamReturnArrayUInt32()
    {
        Console.WriteLine("NoParamReturnArrayUInt32");
        return new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    }

    public static ulong[] NoParamReturnArrayUInt64()
    {
        Console.WriteLine("NoParamReturnArrayUInt64");
        return new ulong[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    }

    public static IntPtr[] NoParamReturnArrayPointer()
    {
        Console.WriteLine("NoParamReturnArrayPointer");
        return new IntPtr[] { IntPtr.Zero, IntPtr.Zero + 1, IntPtr.Zero + 2, IntPtr.Zero + 3 };
    }

    public static float[] NoParamReturnArrayFloat()
    {
        Console.WriteLine("NoParamReturnArrayFloat");
        return new float[] { -12.34f, 0.0f, 12.34f };
    }

    public static double[] NoParamReturnArrayDouble()
    {
        Console.WriteLine("NoParamReturnArrayDouble");
        return new double[] { -12.345, 0.0, 12.345 };
    }

    public static string[] NoParamReturnArrayString()
    {
        Console.WriteLine("NoParamReturnArrayString");
        return new []
        {
            "1st string", "2nd string",
            "3rd element string (Should be big enough to avoid small string optimization)"
        };
    }

    public static Vector2 NoParamReturnVector2()
    {
        Console.WriteLine("NoParamReturnVector2");
        return new Vector2(1, 2);
    }

    public static Vector3 NoParamReturnVector3()
    {
        Console.WriteLine("NoParamReturnVector3");
        return new Vector3(1, 2, 3);
    }

    public static Vector4 NoParamReturnVector4()
    {
        Console.WriteLine("NoParamReturnVector4");
        return new Vector4(1, 2, 3, 4);
    }

    public static Matrix4x4 NoParamReturnMatrix4x4()
    {
        Console.WriteLine("NoParamReturnMatrix4x4");
        return new Matrix4x4(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16);
    }
        
    // Params (no refs)
        
    public static void Param1(int a)
    {
        Console.WriteLine($"Param1: a = {a}");
    }

    public static void Param2(int a, float b)
    {
        Console.WriteLine($"Param2: a = {a}, b = {b}");
    }

    public static void Param3(int a, float b, double c)
    {
        Console.WriteLine($"Param3: a = {a}, b = {b}, c = {c}");
    }

    public static void Param4(int a, float b, double c, Vector4 d)
    {
        Console.WriteLine($"Param4: a = {a}, b = {b}, c = {c}, d = [{d.X},{d.Y},{d.Z},{d.W}]");
    }

    public static void Param5(int a, float b, double c, Vector4 d, long[] e)
    {
        Console.Write($"Param5: a = {a}, b = {b}, c = {c}, d = [{d.X},{d.Y},{d.Z},{d.W}], e.size() = {e.Length}, e = [");
        foreach (var elem in e)
        {
            Console.Write($"{elem}, ");
        }
        Console.WriteLine("]");
    }

    public static void Param6(int a, float b, double c, Vector4 d, long[] e, [CharSet(CharSet.Ansi)] char f)
    {
        Console.Write($"Param6: a = {a}, b = {b}, c = {c}, d = [{d.X},{d.Y},{d.Z},{d.W}], e.size() = {e.Length}, e = [");
        foreach (var elem in e)
        {
            Console.Write($"{elem}, ");
        }
        Console.WriteLine($"], f = {f}");
    }

    public static void Param7(int a, float b, double c, Vector4 d, long[] e, [CharSet(CharSet.Ansi)] char f, string g)
    {
        Console.Write($"Param7: a = {a}, b = {b}, c = {c}, d = [{d.X},{d.Y},{d.Z},{d.W}], e.size() = {e.Length}, e = [");
        foreach (var elem in e)
        {
            Console.Write($"{elem}, ");
        }
        Console.WriteLine($"], f = {f}, g = {g}");
    }

    public static void Param8(int a, float b, double c, Vector4 d, long[] e, [CharSet(CharSet.Ansi)] char f, string g, float h)
    {
        Console.Write($"Param8: a = {a}, b = {b}, c = {c}, d = [{d.X},{d.Y},{d.Z},{d.W}], e.size() = {e.Length}, e = [");
        foreach (var elem in e)
        {
            Console.Write($"{elem}, ");
        }
        Console.WriteLine($"], f = {f}, g = {g}, h = {h}");
    }

    public static void Param9(int a, float b, double c, Vector4 d, long[] e, [CharSet(CharSet.Ansi)] char f, string g, float h, short k)
    {
        Console.Write($"Param9: a = {a}, b = {b}, c = {c}, d = [{d.X},{d.Y},{d.Z},{d.W}], e.size() = {e.Length}, e = [");
        foreach (var elem in e)
        {
            Console.Write($"{elem}, ");
        }
        Console.WriteLine($"], f = {f}, g = {g}, h = {h}, k = {k}");
    }

    public static void Param10(int a, float b, double c, Vector4 d, long[] e, [CharSet(CharSet.Ansi)] char f, string g, float h, short k, IntPtr l)
    {
        Console.Write($"Param10: a = {a}, b = {b}, c = {c}, d = [{d.X},{d.Y},{d.Z},{d.W}], e.size() = {e.Length}, e = [");
        foreach (var elem in e)
        {
            Console.Write($"{elem}, ");
        }
        Console.WriteLine($"], f = {f}, g = {g}, h = {h}, k = {k}, l = {l}");
    }
        
    // Params (with refs)
        
    public static void ParamRef1(ref int a)
    {
        a = 42;
    }

    public static void ParamRef2(ref int a, ref float b)
    {
        a = 10;
        b = 3.14f;
    }

    public static void ParamRef3(ref int a, ref float b, ref double c)
    {
        a = -20;
        b = 2.718f;
        c = 3.14159;
    }

    public static void ParamRef4(ref int a, ref float b, ref double c, ref Vector4 d)
    {
        a = 100;
        b = -5.55f;
        c = 1.618;
        d = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
    }

    public static void ParamRef5(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e)
    {
        a = 500;
        b = -10.5f;
        c = 2.71828;
        d = new Vector4(-1.0f, -2.0f, -3.0f, -4.0f);
        e = new long[]{ -6, -5, -4, -3, -2, -1, 0, 1 };
    }

    public static void ParamRef6(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, [CharSet(CharSet.Ansi)] ref char f)
    {
        a = 750;
        b = 20.0f;
        c = 1.23456;
        d = new Vector4(10.0f, 20.0f, 30.0f, 40.0f);
        e = new long[]{ -6, -5, -4 };
        f = 'Z';
    }

    public static void ParamRef7(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, [CharSet(CharSet.Ansi)] ref char f, ref string g)
    {
        a = -1000;
        b = 3.0f;
        c = -1.0;
        d = new Vector4(100.0f, 200.0f, 300.0f, 400.0f);
        e = new long[]{ -6, -5, -4, -3 };
        f = 'Y';
        g = "Hello, World!";
    }

    public static void ParamRef8(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, [CharSet(CharSet.Ansi)] ref char f, ref string g, ref char h)
    {
        a = 999;
        b = -7.5f;
        c = 0.123456;
        d = new Vector4(-100.0f, -200.0f, -300.0f, -400.0f);
        e = new long[]{ -6, -5, -4, -3, -2, -1 };
        f = 'X';
        g = "Goodbye, World!";
        h = 'A';
    }

    public static void ParamRef9(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, [CharSet(CharSet.Ansi)] ref char f, ref string g, ref char h, ref short k)
    {
        a = -1234;
        b = 123.45f;
        c = -678.9;
        d = new Vector4(987.65f, 432.1f, 123.456f, 789.123f);
        e = new long[]{ -6, -5, -4, -3, -2, -1, 0, 1, 5, 9 };
        f = 'W';
        g = "Testing, 1 2 3";
        h = 'B';
        k = 42;
    }

    public static void ParamRef10(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, [CharSet(CharSet.Ansi)] ref char f, ref string g, ref char h, ref short k, ref IntPtr l)
    {
        a = 987;
        b = -0.123f;
        c = 456.789;
        d = new Vector4(-123.456f, 0.987f, 654.321f, -789.123f);
        e = new long[]{ -6, -5, -4, -3, -2, -1, 0, 1, 5, 9, 4, -7 };
        f = 'V';
        g = "Another string";
        h = 'C';
        k = -444;
        l = 0x12345678;
    }
        
    // Params (array refs only)
        
    public static void ParamRefVectors(ref bool[] p1, [CharSet(CharSet.Ansi)] ref char[] p2, ref char[] p3, ref sbyte[] p4, ref short[] p5, ref int[] p6, ref long[] p7, ref byte[] p8, ref ushort[] p9, ref uint[] p10, ref ulong[] p11, ref IntPtr[] p12, ref float[] p13, ref double[] p14, ref string[] p15)
    {
        p1 = new bool[] { true };
        p2 = new char[] { 'a', 'b', 'c' };
        p3 = new char[] { 'd', 'e', 'f' };
        p4 = new sbyte[] { -3, -2, -1, 0, 1, 2, 3 };
        p5 = new short[] { -4, -3, -2, -1, 0, 1, 2, 3, 4 };
        p6 = new int[] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5 };
        p7 = new long[] { -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6 };
        p8 = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        p9 = new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        p10 = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        p11 = new ulong[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        p12 = new IntPtr[] { IntPtr.Zero, new IntPtr(1), new IntPtr(2) };
        p13 = new float[] { -12.34f, 0.0f, 12.34f };
        p14 = new double[] { -12.345, 0.0, 12.345 };
        p15 = new [] { "1", "12", "123", "1234", "12345", "123456" };
    }

    // Parameters and Return (all primitive types)
        
    public static long ParamAllPrimitives(bool p1, [CharSet(CharSet.Ansi)] char p2, char p3, sbyte p4, short p5, int p6, long p7, byte p8, ushort p9, uint p10, ulong p11, IntPtr p12, float p13, double p14)
    {
        string buffer = $"{p1}{p2}{p3}{p4}{p5}{p6}{p7}{p8}{p9}{p10}{p11}{p12}{p13}{p14}";
        return 56;
    }

    static unsafe void ReverseCall(string test)
    {
        if (ReverseClass.ReverseTest.TryGetValue(test, out var method))
        {
            string result = method();
            if (!string.IsNullOrEmpty(result))
            {
                ReverseReturn(result);
            }
        }
        else
        {
            Console.WriteLine($"Method '{test}' not found.");
        }
    }
}