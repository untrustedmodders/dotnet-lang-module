using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
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
        return [true, false];
    }

    [return: CharSet(CharSet.Ansi)]
    public static char[] NoParamReturnArrayChar8()
    {
        Console.WriteLine("NoParamReturnArrayChar8");
        return ['a', 'b', 'c', 'd'];
    }

    public static char[] NoParamReturnArrayChar16()
    {
        Console.WriteLine("NoParamReturnArrayChar16");
        return ['a', 'b', 'c', 'd'];
    }

    public static sbyte[] NoParamReturnArrayInt8()
    {
        Console.WriteLine("NoParamReturnArrayInt8");
        return [-3, -2, -1, 0, 1];
    }

    public static short[] NoParamReturnArrayInt16()
    {
        Console.WriteLine("NoParamReturnArrayInt16");
        return [-4, -3, -2, -1, 0, 1];
    }

    public static int[] NoParamReturnArrayInt32()
    {
        Console.WriteLine("NoParamReturnArrayInt32");
        return [-5, -4, -3, -2, -1, 0, 1];
    }

    public static long[] NoParamReturnArrayInt64()
    {
        Console.WriteLine("NoParamReturnArrayInt64");
        return [-6, -5, -4, -3, -2, -1, 0, 1];
    }

    public static byte[] NoParamReturnArrayUInt8()
    {
        Console.WriteLine("NoParamReturnArrayUInt8");
        return [0, 1, 2, 3, 4, 5, 6, 7, 8];
    }

    public static ushort[] NoParamReturnArrayUInt16()
    {
        Console.WriteLine("NoParamReturnArrayUInt16");
        return [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    }

    public static uint[] NoParamReturnArrayUInt32()
    {
        Console.WriteLine("NoParamReturnArrayUInt32");
        return [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    }

    public static ulong[] NoParamReturnArrayUInt64()
    {
        Console.WriteLine("NoParamReturnArrayUInt64");
        return [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
    }

    public static IntPtr[] NoParamReturnArrayPointer()
    {
        Console.WriteLine("NoParamReturnArrayPointer");
        return [IntPtr.Zero, IntPtr.Zero + 1, IntPtr.Zero + 2, IntPtr.Zero + 3];
    }

    public static float[] NoParamReturnArrayFloat()
    {
        Console.WriteLine("NoParamReturnArrayFloat");
        return [-12.34f, 0.0f, 12.34f];
    }

    public static double[] NoParamReturnArrayDouble()
    {
        Console.WriteLine("NoParamReturnArrayDouble");
        return [-12.345, 0.0, 12.345];
    }

    public static string[] NoParamReturnArrayString()
    {
        Console.WriteLine("NoParamReturnArrayString");
        return
        [
            "1st string", "2nd string",
            "3rd element string (Should be big enough to avoid small string optimization)"
        ];
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
        e = [-6, -5, -4, -3, -2, -1, 0, 1];
    }

    public static void ParamRef6(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, [CharSet(CharSet.Ansi)] ref char f)
    {
        a = 750;
        b = 20.0f;
        c = 1.23456;
        d = new Vector4(10.0f, 20.0f, 30.0f, 40.0f);
        e = [-6, -5, -4];
        f = 'Z';
    }

    public static void ParamRef7(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, [CharSet(CharSet.Ansi)] ref char f, ref string g)
    {
        a = -1000;
        b = 3.0f;
        c = -1.0;
        d = new Vector4(100.0f, 200.0f, 300.0f, 400.0f);
        e = [-6, -5, -4, -3];
        f = 'Y';
        g = "Hello, World!";
    }

    public static void ParamRef8(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, [CharSet(CharSet.Ansi)] ref char f, ref string g, ref char h)
    {
        a = 999;
        b = -7.5f;
        c = 0.123456;
        d = new Vector4(-100.0f, -200.0f, -300.0f, -400.0f);
        e = [-6, -5, -4, -3, -2, -1];
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
        e = [-6, -5, -4, -3, -2, -1, 0, 1, 5, 9];
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
        e = [-6, -5, -4, -3, -2, -1, 0, 1, 5, 9, 4, -7];
        f = 'V';
        g = "Another string";
        h = 'C';
        k = -444;
        l = 0x12345678;
    }
        
    // Params (array refs only)
        
    public static void ParamRefVectors(ref bool[] p1, [CharSet(CharSet.Ansi)] ref char[] p2, ref char[] p3, ref sbyte[] p4, ref short[] p5, ref int[] p6, ref long[] p7, ref byte[] p8, ref ushort[] p9, ref uint[] p10, ref ulong[] p11, ref IntPtr[] p12, ref float[] p13, ref double[] p14, ref string[] p15)
    {
        p1 = [true];
        p2 = ['a', 'b', 'c'];
        p3 = ['d', 'e', 'f'];
        p4 = [-3, -2, -1, 0, 1, 2, 3];
        p5 = [-4, -3, -2, -1, 0, 1, 2, 3, 4];
        p6 = [-5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5];
        p7 = [-6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6];
        p8 = [0, 1, 2, 3, 4, 5, 6, 7];
        p9 = [0, 1, 2, 3, 4, 5, 6, 7, 8];
        p10 = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
        p11 = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        p12 = [IntPtr.Zero, new(1), new(2)];
        p13 = [-12.34f, 0.0f, 12.34f];
        p14 = [-12.345, 0.0, 12.345];
        p15 = ["1", "12", "123", "1234", "12345", "123456"];
    }

    // Parameters and Return (all primitive types)
        
    public static long ParamAllPrimitives(bool p1, [CharSet(CharSet.Ansi)] char p2, char p3, sbyte p4, short p5, int p6, long p7, byte p8, ushort p9, uint p10, ulong p11, IntPtr p12, float p13, double p14)
    {
        string buffer = $"{p1}{p2}{p3}{p4}{p5}{p6}{p7}{p8}{p9}{p10}{p11}{p12}{p13}{p14}";
        return 56;
    }
    
    
    // Call functions using the typedefs
    public static void CallFuncVoid(cross_call_master.FuncVoid func) {
        func();
    }

    public static bool CallFuncBool(cross_call_master.FuncBool func) {
        bool result = func();
        return result;
    }

    [return: CharSet(CharSet.Ansi)]
    public static char CallFuncChar8(cross_call_master.FuncChar8 func) {
        char result = func();
        return result;
    }

    public static char CallFuncChar16(cross_call_master.FuncChar16 func) {
        char result = func();
        return result;
    }

    public static sbyte CallFuncInt8(cross_call_master.FuncInt8 func) {
        sbyte result = func();
        return result;
    }

    public static short CallFuncInt16(cross_call_master.FuncInt16 func) {
        short result = func();
        return result;
    }

    public static int CallFuncInt32(cross_call_master.FuncInt32 func) {
        int result = func();
        return result;
    }

    public static long CallFuncInt64(cross_call_master.FuncInt64 func) {
        long result = func();
        return result;
    }

    public static byte CallFuncUInt8(cross_call_master.FuncUInt8 func) {
        byte result = func();
        return result;
    }

    public static ushort CallFuncUInt16(cross_call_master.FuncUInt16 func) {
        ushort result = func();
        return result;
    }

    public static uint CallFuncUInt32(cross_call_master.FuncUInt32 func) {
        uint result = func();
        return result;
    }

    public static ulong CallFuncUInt64(cross_call_master.FuncUInt64 func) {
        ulong result = func();
        return result;
    }

    public static nint CallFuncPtr(cross_call_master.FuncPtr func) {
        nint result = func();
        return result;
    }

    public static float CallFuncFloat(cross_call_master.FuncFloat func) {
        float result = func();
        return result;
    }

    public static double CallFuncDouble(cross_call_master.FuncDouble func) {
        double result = func();
        return result;
    }

    public static nint CallFuncFunction(cross_call_master.FuncFunction func) {
        nint result = func();
        return result;
    }

    public static string CallFuncString(cross_call_master.FuncString func) {
        string result = func();
        return result;
    }

    // Call functions for vector return types
    public static bool[] CallFuncBoolVector(cross_call_master.FuncBoolVector func) {
        var result = func();
        return result;
    }

    [return: CharSet(CharSet.Ansi)]
    public static char[] CallFuncChar8Vector(cross_call_master.FuncChar8Vector func) {
        var result = func();
        return result;
    }

    public static char[] CallFuncChar16Vector(cross_call_master.FuncChar16Vector func) {
        var result = func();
        return result;
    }

    public static sbyte[] CallFuncInt8Vector(cross_call_master.FuncInt8Vector func) {
        var result = func();
        return result;
    }

    public static short[] CallFuncInt16Vector(cross_call_master.FuncInt16Vector func) {
        var result = func();
        return result;
    }

    public static int[] CallFuncInt32Vector(cross_call_master.FuncInt32Vector func) {
        var result = func();
        return result;
    }

    public static long[] CallFuncInt64Vector(cross_call_master.FuncInt64Vector func) {
        var result = func();
        return result;
    }

    public static byte[] CallFuncUInt8Vector(cross_call_master.FuncUInt8Vector func) {
        var result = func();
        return result;
    }

    public static ushort[] CallFuncUInt16Vector(cross_call_master.FuncUInt16Vector func) {
        var result = func();
        return result;
    }

    public static uint[] CallFuncUInt32Vector(cross_call_master.FuncUInt32Vector func) {
        var result = func();
        return result;
    }

    public static ulong[] CallFuncUInt64Vector(cross_call_master.FuncUInt64Vector func) {
        var result = func();
        return result;
    }

    public static nint[] CallFuncPtrVector(cross_call_master.FuncPtrVector func) {
        var result = func();
        return result;
    }

    public static float[] CallFuncFloatVector(cross_call_master.FuncFloatVector func) {
        var result = func();
        return result;
    }

    public static double[] CallFuncDoubleVector(cross_call_master.FuncDoubleVector func) {
        var result = func();
        return result;
    }

    public static string[] CallFuncStringVector(cross_call_master.FuncStringVector func) {
        var result = func();
        return result;
    }

    // Call functions for vector return types
    public static Vector2 CallFuncVec2(cross_call_master.FuncVec2 func) {
        Vector2 result = func();
        return result;
    }

    public static Vector3 CallFuncVec3(cross_call_master.FuncVec3 func) {
        Vector3 result = func();
        return result;
    }

    public static Vector4 CallFuncVec4(cross_call_master.FuncVec4 func) {
        Vector4 result = func();
        return result;
    }

    public static Matrix4x4 CallFuncMat4x4(cross_call_master.FuncMat4x4 func) {
        Matrix4x4 result = func();
        return result;
    }
    
    // 1 parameter
    public static int CallFunc1(cross_call_master.Func1 func)
    {
        Vector3 vec = new Vector3(4.5f, 5.6f, 6.7f); // Random values
        return func(vec);
    }

    // 2 parameters
    [return: CharSet(CharSet.Ansi)]
    public static char CallFunc2(cross_call_master.Func2 func)
    {
        float f = 2.71f;
        long i64 = 200;
        return func(f, i64);
    }

    // 3 parameters
    public static void CallFunc3(cross_call_master.Func3 func)
    {
        IntPtr ptr = new IntPtr(12345);
        Vector4 vec4 = new Vector4(7.8f, 8.9f, 9.1f, 10.2f);
        string str = "RandomString";
        func(ptr, vec4, str);
    }

    // 4 parameters
    public static Vector4 CallFunc4(cross_call_master.Func4 func)
    {
        bool b = false;
        int u32 = 42;
        char ch16 = 'B';
        Matrix4x4 mat = Matrix4x4.Identity; // Assume it's initialized properly
        return func(b, u32, ch16, mat);
    }

    // 5 parameters
    public static bool CallFunc5(cross_call_master.Func5 func)
    {
        sbyte i8 = 10;
        Vector2 vec2 = new Vector2(3.4f, 5.6f);
        IntPtr ptr = new IntPtr(67890);
        double d = 1.618;
        ulong[] vec64 = [4, 5, 6];
        return func(i8, vec2, ptr, d, vec64);
    }

    // 6 parameters
    public static long CallFunc6(cross_call_master.Func6 func)
    {
        string str = "AnotherString";
        float f = 4.56f;
        float[] vecF = [4.0f, 5.0f, 6.0f];
        short i16 = 30;
        byte[] vecU8 = [3, 4, 5];
        IntPtr ptr = new IntPtr(24680);
        return func(str, f, vecF, i16, vecU8, ptr);
    }

    // 7 parameters
    public static double CallFunc7(cross_call_master.Func7 func)
    {
        char[] vecC = ['X', 'Y', 'Z'];
        ushort u16 = 20;
        char ch16 = 'C';
        uint[] vecU32 = [4, 5, 6];
        Vector4 vec4 = new Vector4(4.5f, 5.6f, 6.7f, 7.8f);
        bool b = false;
        ulong u64 = 200;
        return func(vecC, u16, ch16, vecU32, vec4, b, u64);
    }

    // 8 parameters
    public static Matrix4x4 CallFunc8(cross_call_master.Func8 func)
    {
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        uint[] vecU32 = [4, 5, 6];
        short i16 = 30;
        bool b = false;
        Vector4 vec4 = new Vector4(4.5f, 5.6f, 6.7f, 7.8f);
        char[] vecC16 = ['D', 'E'];
        char ch16 = 'B';
        int i32 = 50;
        return func(vec3, vecU32, i16, b, vec4, vecC16, ch16, i32);
    }

    // 9 parameters
    public static void CallFunc9(cross_call_master.Func9 func)
    {
        float f = 2.71f;
        Vector2 vec2 = new Vector2(3.4f, 5.6f);
        sbyte[] vecI8 = [4, 5, 6];
        ulong u64 = 250;
        bool b = false;
        string str = "Random";
        Vector4 vec4 = new Vector4(4.5f, 5.6f, 6.7f, 7.8f);
        short i16 = 30;
        IntPtr ptr = new IntPtr(13579);
        func(f, vec2, vecI8, u64, b, str, vec4, i16, ptr);
    }

    // 10 parameters
    public static uint CallFunc10(cross_call_master.Func10 func)
    {
        Vector4 vec4 = new Vector4(5.6f, 7.8f, 8.9f, 9.0f);
        Matrix4x4 mat = Matrix4x4.Identity;
        uint[] vecU32 = [4, 5, 6];
        ulong u64 = 150;
        char[] vecC = ['X', 'Y', 'Z'];
        int i32 = 60;
        bool b = false;
        Vector2 vec2 = new Vector2(3.4f, 5.6f);
        long i64 = 75;
        double d = 2.71;
        return func(vec4, mat, vecU32, u64, vecC, i32, b, vec2, i64, d);
    }

    // 11 parameters
    public static IntPtr CallFunc11(cross_call_master.Func11 func)
    {
        bool[] vecB = [false, true, false];
        char ch16 = 'C';
        byte u8 = 10;
        double d = 2.71;
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        sbyte[] vecI8 = [3, 4, 5];
        long i64 = 150;
        ushort u16 = 20;
        float f = 2.0f;
        Vector2 vec2 = new Vector2(4.5f, 6.7f);
        uint u32 = 30;
        return func(vecB, ch16, u8, d, vec3, vecI8, i64, u16, f, vec2, u32);
    }

    // 12 parameters
    public static bool CallFunc12(cross_call_master.Func12 func)
    {
        IntPtr ptr = new IntPtr(98765);
        double[] vecD = [4.0, 5.0, 6.0];
        uint u32 = 30;
        double d = 1.41;
        bool b = false;
        int i32 = 25;
        sbyte i8 = 10;
        ulong u64 = 300;
        float f = 2.72f;
        IntPtr[] vecPtr = [new(2), new(3), new(4)];
        long i64 = 200;
        char ch = 'B';
        return func(ptr, vecD, u32, d, b, i32, i8, u64, f, vecPtr, i64, ch);
    }

    // 13 parameters
    public static string CallFunc13(cross_call_master.Func13 func)
    {
        long i64 = 75;
        char[] vecC = ['D', 'E', 'F'];
        ushort u16 = 20;
        float f = 2.71f;
        bool[] vecB = [false, true, false];
        Vector4 vec4 = new Vector4(5.6f, 7.8f, 9.0f, 10.1f);
        string str = "RandomString";
        int i32 = 30;
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        IntPtr ptr = new IntPtr(13579);
        Vector2 vec2 = new Vector2(4.5f, 6.7f);
        byte[] vecU8 = [2, 3, 4];
        short i16 = 20;
        return func(i64, vecC, u16, f, vecB, vec4, str, i32, vec3, ptr, vec2, vecU8, i16);
    }

    // 14 parameters
    public static string[] CallFunc14(cross_call_master.Func14 func)
    {
        char[] vecC = ['D', 'E', 'F'];
        uint[] vecU32 = [4, 5, 6];
        Matrix4x4 mat = Matrix4x4.Identity;
        bool b = false;
        char ch16 = 'B';
        int i32 = 25;
        float[] vecF = [4.0f, 5.0f, 6.0f];
        ushort u16 = 30;
        byte[] vecU8 = [3, 4, 5];
        sbyte i8 = 10;
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        Vector4 vec4 = new Vector4(5.6f, 7.8f, 9.0f, 10.1f);
        double d = 2.72;
        IntPtr ptr = new IntPtr(54321);
        return func(vecC, vecU32, mat, b, ch16, i32, vecF, u16, vecU8, i8, vec3, vec4, d, ptr);
    }

    // 15 parameters
    public static short CallFunc15(cross_call_master.Func15 func)
    {
        short[] vecI16 = [4, 5, 6];
        Matrix4x4 mat = Matrix4x4.Identity;
        Vector4 vec4 = new Vector4(7.8f, 8.9f, 9.0f, 10.1f);
        IntPtr ptr = new IntPtr(12345);
        ulong u64 = 200;
        uint[] vecU32 = [5, 6, 7];
        bool b = false;
        float f = 3.14f;
        char[] vecC16 = ['D', 'E'];
        byte u8 = 6;
        int i32 = 25;
        Vector2 vec2 = new Vector2(5.6f, 7.8f);
        ushort u16 = 40;
        double d = 2.71;
        byte[] vecU8 = [1, 3, 5];
        return func(vecI16, mat, vec4, ptr, u64, vecU32, b, f, vecC16, u8, i32, vec2, u16, d, vecU8);
    }

    // 16 parameters
    public static IntPtr CallFunc16(cross_call_master.Func16 func)
    {
        bool[] vecB = [true, true, false];
        short i16 = 20;
        sbyte[] vecI8 = [2, 3, 4];
        Vector4 vec4 = new Vector4(7.8f, 8.9f, 9.0f, 10.1f);
        Matrix4x4 mat = Matrix4x4.Identity;
        Vector2 vec2 = new Vector2(5.6f, 7.8f);
        ulong[] vecU64 = [5, 6, 7];
        char[] vecC = ['D', 'E', 'F'];
        string str = "DifferentString";
        long i64 = 300;
        uint[] vecU32 = [6, 7, 8];
        Vector3 vec3 = new Vector3(5.0f, 6.0f, 7.0f);
        float f = 3.14f;
        double d = 2.718;
        sbyte i8 = 6;
        ushort u16 = 30;
        return func(vecB, i16, vecI8, vec4, mat, vec2, vecU64, vecC, str, i64, vecU32, vec3, f, d, i8, u16);
    }
    
    public static string VectorToString<T>(T[] array)
    {
        return $"{{{string.Join(", ", array)}}}";
    }
    
    public static string VectorToString(bool[] array)
    {
        if (array.Length == 0)
            return "{{{}}}";
        
        var result = new StringBuilder();
        result.Append($"{BStr(array[0])}");

        for (int i = 1; i < array.Length; i++)
        {
            result.Append($", {BStr(array[i])}");
        }

        return $"{{{result}}}";
    }    
    
    public static string VectorToString(nint[] array)
    {
        if (array.Length == 0)
            return "{{{}}}";
        
        var result = new StringBuilder();
        result.Append($"{"0x" + array[0].ToString("x")}");

        for (int i = 1; i < array.Length; i++)
        {
            result.Append($", {"0x" + array[i].ToString("x")}");
        }

        return $"{{{result}}}";
    }    
    
    public static string VectorToString(char[] array)
    {
        if (array.Length == 0)
            return "{{{}}}";

        var result = new StringBuilder();
        result.Append($"{(int)array[0]}");

        for (int i = 1; i < array.Length; i++)
        {
            result.Append($", {(int)array[i]}");
        }

        return $"{{{result}}}";
    }
    
    public static string VectorToString(string[] array)
    {
        if (array.Length == 0)
            return "{{{}}}";

        var result = new StringBuilder();
        result.Append($"'{array[0]}'");

        for (int i = 1; i < array.Length; i++)
        {
            result.Append($", '{array[i]}'");
        }

        return $"{{{result}}}";
    }

    public static string BStr(bool b)
    {
        return b ? "true" : "false";
    }

    // Overload for Vector2
    public static string PodToString(Vector2 t)
    {
        return $"{{{t.X}, {t.Y}}}";
    }

    // Overload for Vector3
    public static string PodToString(Vector3 t)
    {
        return $"{{{t.X}, {t.Y}, {t.Z}}}";
    }

    // Overload for Vector4
    public static string PodToString(Vector4 t)
    {
        return $"{{{t.X}, {t.Y}, {t.Z}, {t.W}}}";
    }

    // Overload for Matrix4x4
    public static string PodToString(Matrix4x4 t)
    {
        // Format matrix4x4 as a string
        string formattedRow1 = $"{{{t.M11}, {t.M12}, {t.M13}, {t.M14}}}";
        string formattedRow2 = $"{{{t.M21}, {t.M22}, {t.M23}, {t.M24}}}";
        string formattedRow3 = $"{{{t.M31}, {t.M32}, {t.M33}, {t.M34}}}";
        string formattedRow4 = $"{{{t.M41}, {t.M42}, {t.M43}, {t.M44}}}";
        return $"{{{formattedRow1}, {formattedRow2}, {formattedRow3}, {formattedRow4}}}";
    }
    
    // 1 parameter
    public static string CallFunc17(cross_call_master.Func17 func)
    {
        int i32 = 42;
        func(ref i32);
        return $"{i32}";
    }

    // 2 parameters
    public static string CallFunc18(cross_call_master.Func18 func)
    {
        sbyte i8 = 9;
        short i16 = 25;
        Vector2 ret = func(ref i8, ref i16);
        return $"{PodToString(ret)}|{i8}|{i16}";
    }

    // 3 parameters
    public static string CallFunc19(cross_call_master.Func19 func)
    {
        uint u32 = 75;
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        uint[] vecU32 = [4, 5, 6];
        func(ref u32, ref vec3, ref vecU32);
        return $"{u32}|{PodToString(vec3)}|{VectorToString(vecU32)}";
    }

    // 4 parameters
    public static string CallFunc20(cross_call_master.Func20 func)
    {
        char ch16 = 'Z';
        Vector4 vec4 = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);
        ulong[] vecU64 = [4, 5, 6];
        char ch = 'X';
        int ret = func(ref ch16, ref vec4, ref vecU64, ref ch);
        return $"{ret}|{(ushort)ch16}|{PodToString(vec4)}|{VectorToString(vecU64)}|{ch}";
    }

    // 5 parameters
    public static string CallFunc21(cross_call_master.Func21 func)
    {
        Matrix4x4 mat = Matrix4x4.Identity;
        int[] vecI32 = [4, 5, 6];
        Vector2 vec2 = new Vector2(3.0f, 4.0f);
        bool b = false;
        double d = 6.28;
        float ret = func(ref mat, ref vecI32, ref vec2, ref b, ref d);
        return $"{ret}|{PodToString(mat)}|{VectorToString(vecI32)}|{PodToString(vec2)}|{BStr(b)}|{d}";
    }

    // 6 parameters
    public static string CallFunc22(cross_call_master.Func22 func)
    {
        IntPtr ptr = new IntPtr(1);
        uint u32 = 20;
        double[] vecD = [4.0, 5.0, 6.0];
        short i16 = 15;
        string str = "Updated Test";
        Vector4 vec4 = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);
        ulong ret = func(ref ptr, ref u32, ref vecD, ref i16, ref str, ref vec4);
        return $"{ret}|{"0x" + ptr.ToString("x")}|{u32}|{VectorToString(vecD)}|{i16}|{str}|{PodToString(vec4)}";
    }

    // 7 parameters
    public static string CallFunc23(cross_call_master.Func23 func)
    {
        ulong u64 = 200;
        Vector2 vec2 = new Vector2(3.0f, 4.0f);
        short[] vecI16 = [4, 5, 6];
        char ch16 = 'Y';
        float f = 2.34f;
        sbyte i8 = 10;
        byte[] vecU8 = [3, 4, 5];
        func(ref u64, ref vec2, ref vecI16, ref ch16, ref f, ref i8, ref vecU8);
        return $"{u64}|{PodToString(vec2)}|{VectorToString(vecI16)}|{(ushort)ch16}|{f}|{i8}|{VectorToString(vecU8)}";
    }

    // 8 parameters
    public static string CallFunc24(cross_call_master.Func24 func)
    {
        char[] vecC = ['D', 'E', 'F'];
        long i64 = 100;
        byte[] vecU8 = [3, 4, 5];
        Vector4 vec4 = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);
        ulong u64 = 200;
        IntPtr[] vecPtr = [new(3), new(4), new(5)];
        double d = 6.28;
        IntPtr[] vecV2 = [new(4), new(5), new(6), new(7)];
        Matrix4x4 ret = func(ref vecC, ref i64, ref vecU8, ref vec4, ref u64, ref vecPtr, ref d, ref vecV2);
        return $"{PodToString(ret)}|{VectorToString(vecC)}|{i64}|{VectorToString(vecU8)}|{PodToString(vec4)}|{u64}|{VectorToString(vecPtr)}|{d}|{VectorToString(vecV2)}";
    }

    // 9 parameters
    public static string CallFunc25(cross_call_master.Func25 func)
    {
        int i32 = 50;
        IntPtr[] vecPtr = [new(3), new(4), new(5)];
        bool b = false;
        byte u8 = 10;
        string str = "Updated Test String";
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        long i64 = 100;
        Vector4 vec4 = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);
        ushort u16 = 20;
        double ret = func(ref i32, ref vecPtr, ref b, ref u8, ref str, ref vec3, ref i64, ref vec4, ref u16);
        return $"{ret}|{i32}|{VectorToString(vecPtr)}|{BStr(b)}|{u8}|{str}|{PodToString(vec3)}|{i64}|{PodToString(vec4)}|{u16}";
    }

    // 10 parameters
    public static unsafe string CallFunc26(cross_call_master.Func26 func)
    {
        char ch16 = 'B';
        Vector2 vec2 = new Vector2(3.0f, 4.0f);
        Matrix4x4 mat = Matrix4x4.Identity;
        float[] vecF = [4.0f, 5.0f, 6.0f];
        short i16 = 20;
        ulong u64 = 200;
        uint u32 = 20;
        ushort[] vecU16 = [3, 4, 5];
        IntPtr ptr = new IntPtr((void*)0xDEADBEAFDEADBEAF);
        bool b = false;
        char ret = func(ref ch16, ref vec2, ref mat, ref vecF, ref i16, ref u64, ref u32, ref vecU16, ref ptr, ref b);
        return $"{ret}|{(ushort)ch16}|{PodToString(vec2)}|{PodToString(mat)}|{VectorToString(vecF)}|{u64}|{u32}|{VectorToString(vecU16)}|{"0x" + ptr.ToString("x")}|{BStr(b)}";
    }
    
    // 11 parameters
    public static string CallFunc27(cross_call_master.Func27 func)
    {
        float f = 2.56f;
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        IntPtr ptr = IntPtr.Zero; // Example pointer
        Vector2 vec2 = new Vector2(3.0f, 4.0f);
        short[] vecI16 = [4, 5, 6];
        Matrix4x4 mat = new Matrix4x4(); // Assume initialized
        bool b = false;
        Vector4 vec4 = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);
        sbyte i8 = 10;
        int i32 = 40;
        byte[] vecU8 = [3, 4, 5];

        byte ret = func(ref f, ref vec3, ref ptr, ref vec2, ref vecI16, ref mat, ref b, ref vec4, ref i8, ref i32, ref vecU8);
        return $"{ret}|{f}|{PodToString(vec3)}|{"0x" + ptr.ToString("x")}|{PodToString(vec2)}|{VectorToString(vecI16)}|{PodToString(mat)}|{BStr(b)}|{PodToString(vec4)}|{i8}|{i32}|{VectorToString(vecU8)}";
    }

    // 12 parameters
    public static string CallFunc28(cross_call_master.Func28 func)
    {
        IntPtr ptr = new IntPtr(1);
        ushort u16 = 20;
        uint[] vecU32 = [4, 5, 6];
        Matrix4x4 mat = new Matrix4x4();
        float f = 2.71f;
        Vector4 vec4 = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);
        string str = "New example string";
        ulong[] vecU64 = [400, 500, 600];
        long i64 = 987654321;
        bool b = false;
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        float[] vecF = [4.0f, 5.0f, 6.0f];

        string ret = func(ref ptr, ref u16, ref vecU32, ref mat, ref f, ref vec4, ref str, ref vecU64, ref i64, ref b, ref vec3, ref vecF);
        return $"{ret}|{"0x" + ptr.ToString("x")}|{u16}|{VectorToString(vecU32)}|{PodToString(mat)}|{f}|{PodToString(vec4)}|{str}|{VectorToString(vecU64)}|{i64}|{BStr(b)}|{PodToString(vec3)}|{VectorToString(vecF)}";
    }

    // 13 parameters
    public static string CallFunc29(cross_call_master.Func29 func)
    {
        Vector4 vec4 = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
        int i32 = 99;
        sbyte[] vecI8 = [4, 5, 6];
        double d = 2.71;
        bool b = false;
        sbyte i8 = 10;
        ushort[] vecU16 = [4, 5, 6];
        float f = 3.21f;
        string str = "Yet another example string";
        Matrix4x4 mat = new Matrix4x4();
        ulong u64 = 200;
        Vector3 vec3 = new Vector3(5.0f, 6.0f, 7.0f);
        long[] vecI64 = [2000, 3000, 4000];

        string[] ret = func(ref vec4, ref i32, ref vecI8, ref d, ref b, ref i8, ref vecU16, ref f, ref str, ref mat, ref u64, ref vec3, ref vecI64);
        return $"{VectorToString(ret)}|{PodToString(vec4)}|{i32}|{VectorToString(vecI8)}|{d}|{BStr(b)}|{i8}|{VectorToString(vecU16)}|{f}|{str}|{PodToString(mat)}|{u64}|{PodToString(vec3)}|{VectorToString(vecI64)}";
    }

    // 14 parameters
    public static string CallFunc30(cross_call_master.Func30 func)
    {
        IntPtr ptr = new IntPtr(1);
        Vector4 vec4 = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
        long i64 = 987654321;
        uint[] vecU32 = [4, 5, 6];
        bool b = false;
        string str = "Updated String for Func30";
        Vector3 vec3 = new Vector3(5.0f, 6.0f, 7.0f);
        byte[] vecU8 = [1, 2, 3];
        float f = 5.67f;
        Vector2 vec2 = new Vector2(3.0f, 4.0f);
        Matrix4x4 mat = new Matrix4x4();
        sbyte i8 = 10;
        float[] vecF = [4.0f, 5.0f, 6.0f];
        double d = 8.90;

        int ret = func(ref ptr, ref vec4, ref i64, ref vecU32, ref b, ref str, ref vec3, ref vecU8, ref f, ref vec2, ref mat, ref i8, ref vecF, ref d);
        return $"{ret}|{"0x" + ptr.ToString("x")}|{PodToString(vec4)}|{i64}|{VectorToString(vecU32)}|{BStr(b)}|{str}|{PodToString(vec3)}|{VectorToString(vecU8)}|{f}|{PodToString(vec2)}|{PodToString(mat)}|{i8}|{VectorToString(vecF)}|{d}";
    }

    // 15 parameters
    public static string CallFunc31(cross_call_master.Func31 func)
    {
        char ch = 'B';
        uint u32 = 200;
        ulong[] vecU64 = [4, 5, 6];
        Vector4 vec4 = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
        string str = "Updated String for Func31";
        bool b = true;
        long i64 = 987654321;
        Vector2 vec2 = new Vector2(3.0f, 4.0f);
        sbyte i8 = 10;
        ushort u16 = 20;
        short[] vecI16 = [4, 5, 6];
        Matrix4x4 mat = new Matrix4x4();
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        float f = 5.67f;
        double[] vecD = [4.0, 5.0, 6.0];

        Vector3 ret = func(ref ch, ref u32, ref vecU64, ref vec4, ref str, ref b, ref i64, ref vec2, ref i8, ref u16, ref vecI16, ref mat, ref vec3, ref f, ref vecD);
        return $"{PodToString(ret)}|{ch}|{u32}|{VectorToString(vecU64)}|{PodToString(vec4)}|{str}|{BStr(b)}|{i64}|{PodToString(vec2)}|{i8}|{u16}|{VectorToString(vecI16)}|{PodToString(mat)}|{PodToString(vec3)}|{f}|{VectorToString(vecD)}";
    }

    // 16 parameters
    public static string CallFunc32(cross_call_master.Func32 func)
    {
        int i32 = 30;
        ushort u16 = 20;
        sbyte[] vecI8 = [4, 5, 6];
        Vector4 vec4 = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
        IntPtr ptr = new IntPtr(1);
        uint[] vecU32 = [4, 5, 6];
        Matrix4x4 mat = new Matrix4x4();
        ulong u64 = 200;
        string str = "Updated String for Func32";
        long i64 = 987654321;
        Vector2 vec2 = new Vector2(3.0f, 4.0f);
        sbyte[] vecI8_2 = [7, 8, 9];
        bool b = false;
        Vector3 vec3 = new Vector3(4.0f, 5.0f, 6.0f);
        byte u8 = 128;
        char[] vecC16 = ['D', 'E', 'F'];

        func(ref i32, ref u16, ref vecI8, ref vec4, ref ptr, ref vecU32, ref mat, ref u64, ref str, ref i64, ref vec2, ref vecI8_2, ref b, ref vec3, ref u8, ref vecC16);
        return $"{i32}|{u16}|{VectorToString(vecI8)}|{PodToString(vec4)}|{"0x" + ptr.ToString("x")}|{VectorToString(vecU32)}|{PodToString(mat)}|{u64}|{str}|{i64}|{PodToString(vec2)}|{VectorToString(vecI8_2)}|{BStr(b)}|{PodToString(vec3)}|{u8}|{VectorToString(vecC16)}";
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