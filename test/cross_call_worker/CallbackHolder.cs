using System.Numerics;

namespace cross_call_worker;

public static unsafe class CallbackHolder
{
    public static void MockVoid()
    {
        // Console.WriteLine("Void function called");
    }
    
    // Functions returning primitive types
    public static bool MockBool() => true;
    public static char MockChar8() => 'A';
    public static char MockChar16() => 'Z';  // C# doesn't have a distinct char16_t; char is Unicode
    public static sbyte MockInt8() => 10;
    public static short MockInt16() => 100;
    public static int MockInt32() => 1000;
    public static long MockInt64() => 10000;
    public static byte MockUInt8() => 20;
    public static ushort MockUInt16() => 200;
    public static uint MockUInt32() => 2000;
    public static ulong MockUInt64() => 20000;
    public static IntPtr MockPtr() => new(0);  // Equivalent to `reinterpret_cast<void*>(1)` in C++
    public static float MockFloat() => 3.14f;
    public static double MockDouble() => 6.28;
    public static IntPtr MockFunction() => new(2);  // Equivalent to `reinterpret_cast<void*>(2)`
    public static string MockString() => "Test string";

    // Functions returning arrays (instead of std::vector)
    public static bool[] MockBoolArray() => [true, false];
    public static char[] MockChar8Array() => ['A', 'B'];
    public static char[] MockChar16Array() => ['A', 'B'];  // C# treats `char` as Unicode
    public static sbyte[] MockInt8Array() => [10, 20];
    public static short[] MockInt16Array() => [100, 200];
    public static int[] MockInt32Array() => [1000, 2000];
    public static long[] MockInt64Array() => [10000, 20000];
    public static byte[] MockUInt8Array() => [20, 30];
    public static ushort[] MockUInt16Array() => [200, 300];
    public static uint[] MockUInt32Array() => [2000, 3000];
    public static ulong[] MockUInt64Array() => [20000, 30000];
    public static IntPtr[] MockPtrArray() => [new(0), new(1)];
    public static float[] MockFloatArray() => [1.1f, 2.2f];
    public static double[] MockDoubleArray() => [3.3, 4.4];
    public static string[] MockStringArray() => ["Hello", "World"];

    // Functions returning vectors and matrices
    public static Vector2 MockVec2() => new(1.0f, 2.0f);
    public static Vector3 MockVec3() => new(1.0f, 2.0f, 3.0f);
    public static Vector4 MockVec4() => new(1.0f, 2.0f, 3.0f, 4.0f);

    public static Matrix4x4 MockMat4x4()
    {
        return new Matrix4x4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f
        );
    }
    
    public static int MockFunc1(ref Vector3 v)
    {
        string buffer = string.Format("{0}{1}{2}", v.X, v.Y, v.Z);
        return (int)(v.X + v.Y + v.Z);
    }

    // Mock implementation for 2 parameter function
    public static char MockFunc2(float a, long b)
    {
        string buffer = string.Format("{0}{1}", a, b);
        return '&';
    }

    // Mock implementation for 3 parameter function
    public static void MockFunc3(IntPtr p, ref Vector4 v, string s)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}", p, v.X, v.Y, v.Z, v.W, s);
    }

    // Mock implementation for 4 parameter function
    public static Vector4 MockFunc4(bool flag, int u, char c, ref Matrix4x4 m)
    {
        string buffer = string.Format("{0}{1}{2}{3}", flag, u, (ushort)c, m.M11);
        return new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
    }

    // Mock implementation for 5 parameter function
    public static bool MockFunc5(sbyte i, ref Vector2 v, IntPtr p, double d, ulong[] vec)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}", i, v.X, v.Y, p, d, vec.Length);
        return true;
    }

    // Mock implementation for 6 parameter function
    public static long MockFunc6(string s, float f, float[] vec, short i, byte[] uVec, IntPtr p)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}", s, f, vec.Length, i, uVec.Length, p);
        return (long)(f + i);
    }

    // Mock implementation for 7 parameter function
    public static double MockFunc7(char[] vec, ushort u, char c, uint[] uVec, ref Vector4 v, bool flag, ulong l)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", vec.Length, u, (ushort)c, uVec.Length, v.X, v.Y, v.Z, v.W);
        return 3.14;
    }

    // Mock implementation for 8 parameter function
    public static Matrix4x4 MockFunc8(ref Vector3 v, uint[] uVec, short i, bool flag, ref Vector4 v4, char[] cVec, char c, int a)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", v.X, v.Y, v.Z, uVec.Length, i, flag, v4.W, cVec.Length, (ushort)c);
        return new Matrix4x4(); // Return a dummy matrix
    }

    // Mock implementation for 9 parameter function
    public static void MockFunc9(float f, ref Vector2 v, sbyte[] iVec, ulong l, bool flag, string s, ref Vector4 v4, short i, IntPtr p)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", f, v.X, v.Y, iVec.Length, l, flag, s, v4.W, i, p);
    }

    // Mock implementation for 10 parameter function
    public static uint MockFunc10(ref Vector4 v4, ref Matrix4x4 m, uint[] uVec, ulong l, char[] cVec, int a, bool flag, ref Vector2 v, long i, double d)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", v4.X, v4.Y, v4.Z, v4.W, m.M22, uVec.Length, l, cVec.Length, a, flag);
        return 42; // Return a dummy value
    }

    // Mock implementation for 11 parameter function
    public static IntPtr MockFunc11(bool[] bVec, char c, byte u, double d, ref Vector3 v3, sbyte[] iVec, long i, ushort u16, float f, ref Vector2 v, uint u32)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", bVec.Length, (ushort)c, u, d, v3.X, iVec.Length, i, u16, f, v.X, u32);
        return new IntPtr(0); // Return a dummy non-null pointer
    }

    // Mock implementation for 12 parameter function
    public static bool MockFunc12(IntPtr p, double[] dVec, uint u, double d, bool flag, int a, sbyte i, ulong l, float f, IntPtr[] pVec, long i64, char c)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", p, dVec.Length, u, d, flag, a, i, l, f, pVec.Length, i64, c);
        return false;
    }

    // Mock implementation for 13 parameter function
    public static string MockFunc13(long i64, char[] cVec, ushort u16, float f, bool[] bVec, ref Vector4 v4, string s, int a, ref Vector3 v3, IntPtr p, ref Vector2 v2, byte[] u8Vec, short i16)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", i64, cVec.Length, u16, f, bVec.Length, v4.Z, s, a, v3.X, p, v2.X, u8Vec.Length, i16);
        return "Dummy String"; // Return a dummy string
    }

    // Mock implementation for 14 parameter function
    public static string[] MockFunc14(char[] cVec, uint[] uVec, ref Matrix4x4 m, bool flag, char c, int a, float[] fVec, ushort u16, byte[] u8Vec, sbyte i8, ref Vector3 v3, ref Vector4 v4, double d, IntPtr p)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}", cVec.Length, uVec.Length, m.M33, flag, (ushort)c, a, fVec.Length, u16, u8Vec.Length, i8, v3.X, v4.X, d, p);
        return ["String1", "String2"]; // Return dummy strings
    }

    // Mock implementation for 15 parameter function
    public static short MockFunc15(short[] iVec, ref Matrix4x4 m, ref Vector4 v4, IntPtr p, ulong l, uint[] uVec, bool flag, float f, char[] cVec, byte u, int a, ref Vector2 v2, ushort u16, double d, byte[] u8Vec)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}", iVec.Length, m.M21, v4.X, p, l, uVec.Length, flag, f, cVec.Length, u, a, v2.X, u16, d, u8Vec.Length);
        return 257; // Return a dummy value
    }

    // Mock implementation for 16 parameter function
    public static IntPtr MockFunc16(bool[] bVec, short i16, sbyte[] iVec, ref Vector4 v4, ref Matrix4x4 m, ref Vector2 v2, ulong[] uVec, char[] cVec, string s, long i64, uint[] u32Vec, ref Vector3 v3, float f, double d, sbyte i8, ushort u16)
    {
        string buffer = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}", bVec.Length, i16, iVec.Length, v4.X, v4.Y, v4.Z, v4.W, m.M34, v2.X, uVec.Length, cVec.Length, s, i64, u32Vec.Length, v3.X, f, d, i8, u16);
        return new IntPtr(0); // Return a different non-null pointer
    }
    
    // 17-parameter function
    public static void MockFunc17(ref int refVal)
    {
        refVal += 10; // Modified value change
    }

    // 18-parameter function
    public static Vector2 MockFunc18(ref sbyte i8, ref short i16)
    {
        i8 = 5; // Changed sbyte value
        i16 = 10; // Changed short value
        return new Vector2(i8, i16); // Return as Vector2
    }

    // 19-parameter function
    public static void MockFunc19(ref uint u32, ref Vector3 v3, ref uint[] uVec)
    {
        u32 = 42; // Changed uint32 value
        v3 = new Vector3(1.0f, 2.0f, 3.0f); // Update Vector3 reference
        uVec = [1, 2, 3]; // Change uint[] values
    }

    // 20-parameter function
    public static int MockFunc20(ref char c, ref Vector4 v4, ref ulong[] uVec, ref char ch)
    {
        c = 't'; // Changed char16_t equivalent
        v4 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f); // Update Vector4 reference
        uVec = [100, 200]; // Change ulong[] values
        ch = 'F'; // Modified char value
        return 0; // Return value
    }

    // 21-parameter function
    public static float MockFunc21(ref Matrix4x4 m, ref int[] iVec, ref Vector2 v2, ref bool flag, ref double d)
    {
        flag = true; // Changed boolean reference
        d = 3.14; // Updated double reference
        v2 = new Vector2(1.0f, 2.0f); // Changed Vector2 reference

        // Updated Matrix4x4 reference
        m = new Matrix4x4(
            1.3f, 0.6f, 0.8f, 0.5f,
            0.7f, 1.1f, 0.2f, 0.4f,
            0.9f, 0.3f, 1.2f, 0.7f,
            0.2f, 0.8f, 0.5f, 1.0f
        );
        iVec = [1, 2, 3]; // Change int[] values
        return 0.0f; // Return value
    }

    // 22-parameter function
    public static ulong MockFunc22(ref IntPtr p, ref uint u32, ref double[] dVec, ref short i16, ref string s, ref Vector4 v4)
    {
        p = new IntPtr(0x0); // Updated IntPtr reference
        u32 = 99; // Changed uint32 value
        i16 = 123; // Changed int16 value
        s = "Hello"; // Changed string reference
        v4 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f); // Updated Vector4 reference
        dVec = [1.1, 2.2, 3.3]; // Change double[] values
        return 0; // Return value
    }

    // 23-parameter function
    public static void MockFunc23(ref ulong u64, ref Vector2 v2, ref short[] iVec, ref char c, ref float f, ref sbyte i8, ref byte[] u8Vec)
    {
        u64 = 50; // Changed ulong reference
        f = 1.5f; // Updated float reference
        i8 = -1; // Changed sbyte reference
        v2 = new Vector2(3.0f, 4.0f); // Updated Vector2 reference
        u8Vec = [1, 2, 3]; // Change byte[] values
        c = 'Ⅴ'; // Updated char reference
        iVec = [1, 2, 3, 4]; // Change short[] values
    }

    // 24-parameter function
    public static Matrix4x4 MockFunc24(ref char[] cVec, ref long i64, ref byte[] u8Vec, ref Vector4 v4, ref ulong u64, ref IntPtr[] pVec, ref double d, ref IntPtr[] vVec)
    {
        i64 = 64; // Changed long reference
        d = 2.71; // Updated double reference
        v4 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f); // Updated Vector4 reference
        cVec = ['a', 'b', 'c']; // Change char[] values
        u8Vec = [5, 6, 7]; // Change byte[] values
        pVec = [new(0)]; // Updated IntPtr array
        vVec = [new(1), new(1), new(2), new(2)]; // Updated IntPtr array
        u64 = 0xffffffff; // Change ulong reference
        return new Matrix4x4(); // Return Matrix4x4
    }

    // 25-parameter function
    public static double MockFunc25(ref int i32, ref IntPtr[] pVec, ref bool flag, ref byte u8, ref string s, ref Vector3 v3, ref long i64, ref Vector4 v4, ref ushort u16)
    {
        flag = false; // Changed bool reference
        i32 = 100; // Updated int32 reference
        u8 = 250; // Changed byte reference
        v3 = new Vector3(1.0f, 2.0f, 3.0f); // Updated Vector3 reference
        v4 = new Vector4(4.0f, 5.0f, 6.0f, 7.0f); // Changed Vector4 reference
        s = "MockFunc25"; // Changed string reference
        pVec = [new(0)]; // Updated IntPtr array
        i64 = 1337; // Changed long reference
        u16 = 64222; // Changed ushort reference
        return 0.0; // Return value
    }

    // 26-parameter function
    public static char MockFunc26(ref char c, ref Vector2 v2, ref Matrix4x4 m, ref float[] fVec, ref short i16, ref ulong u64, ref uint u32, ref ushort[] u16Vec, ref IntPtr p, ref bool flag)
    {
        c = 'Z'; // Updated char16_t reference
        flag = true; // Changed boolean reference
        v2 = new Vector2(2.0f, 3.0f); // Updated Vector2 reference

        // Updated Matrix4x4 reference
        m = new Matrix4x4(
            0.9f, 0.2f, 0.4f, 0.8f,
            0.1f, 1.0f, 0.6f, 0.3f,
            0.7f, 0.5f, 0.2f, 0.9f,
            0.3f, 0.4f, 1.5f, 0.1f
        );
        fVec = [1.1f, 2.2f]; // Change float[] values
        u64 = 64; // Updated ulong reference
        u32 = 32; // Updated uint reference
        u16Vec = [100, 200]; // Change ushort[] values
        i16 = 332; // Changed short reference
        p = new IntPtr((void*)0xDEADBEAFDEADBEAF); // Updated IntPtr reference
        return 'A'; // Return value
    }

    // 27-parameter function
    public static byte MockFunc27(ref float f, ref Vector3 v3, ref IntPtr p, ref Vector2 v2, ref short[] i16Vec, ref Matrix4x4 m, ref bool flag, ref Vector4 v4, ref sbyte i8, ref int i32, ref byte[] u8Vec)
    {
        f = 1.0f;
        v3 = new Vector3(-1.0f, -2.0f, -3.0f);
        p = new IntPtr((void*)0xDEADBEAFDEADBEAF);
        v2 = new Vector2(-111.0f, 111.0f);
        i16Vec = [1, 2, 3, 4];
        m = new Matrix4x4(
            1.0f, 0.5f, 0.3f, 0.7f,
            0.8f, 1.2f, 0.6f, 0.9f,
            1.5f, 1.1f, 0.4f, 0.2f,
            0.3f, 0.9f, 0.7f, 1.0f
        );
        flag = true;
        v4 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
        i8 = 111;
        i32 = 30;
        u8Vec = [0, 0, 0, 0, 0, 0, 1, 0];
        return 0; // Returning dummy byte
    }
    
    // Mock implementation for a function with 28 parameters
    public static string MockFunc28(ref IntPtr ptr, ref ushort u16, ref uint[] u32Vec, ref Matrix4x4 m, ref float f, ref Vector4 v4, ref string str, ref ulong[] u64Vec, ref long i64, ref bool b, ref Vector3 vec3, ref float[] fVec)
    {
        ptr = IntPtr.Zero;
        u16 = 65500;
        u32Vec = [1, 2, 3, 4, 5, 7];
        m = new Matrix4x4(
            1.4f, 0.7f, 0.2f, 0.5f,
            0.3f, 1.1f, 0.6f, 0.8f,
            0.9f, 0.4f, 1.3f, 0.1f,
            0.6f, 0.2f, 0.7f, 1.0f
        );
        f = 5.5f; // Setting a value for float reference
        v4 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f); // Setting a value for Vector4 reference
        u64Vec = [1, 2]; // Setting values for ulong array
        i64 = 834748377834;
        b = true;
        vec3 = new Vector3(10, 20, 30); // Setting values for Vector3
        str = "MockFunc28"; // Setting a value for string reference
        fVec = [1.0f, -1000.0f, 2000.0f];
        return str; // Returning dummy string
    }

    // Mock implementation for a function with 29 parameters
    public static string[] MockFunc29(ref Vector4 v4, ref int i32, ref sbyte[] iVec, ref double d, ref bool flag, ref sbyte i8, ref ushort[] u16Vec, ref float f, ref string s, ref Matrix4x4 m, ref ulong u64, ref Vector3 v3, ref long[] i64Vec)
    {
        i32 = 30; // Setting a value for int32 reference
        flag = true; // Setting a value for bool reference
        v4 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f); // Setting a value for Vector4 reference
        d = 3.14; // Setting a value for double reference
        i8 = 8; // Setting a value for sbyte reference
        u16Vec = [100, 200]; // Setting values for ushort array
        f = 1.5f; // Setting a value for float reference
        s = "MockFunc29"; // Setting a value for string reference
        m = new Matrix4x4(
            0.4f, 1.0f, 0.6f, 0.3f,
            1.2f, 0.8f, 0.5f, 0.9f,
            0.7f, 0.3f, 1.4f, 0.6f,
            0.1f, 0.9f, 0.8f, 1.3f
        ); // Setting a value for Matrix4x4 reference
        u64 = 64; // Setting a value for ulong reference
        v3 = new Vector3(1.0f, 2.0f, 3.0f); // Setting a value for Vector3 reference
        i64Vec = [1, 2, 3]; // Setting values for long array
        iVec = [127, 126, 125]; // Setting values for sbyte array
        return ["Example", "MockFunc29"]; // Returning dummy array of strings
    }

    // Mock implementation for a function with 30 parameters
    public static int MockFunc30(ref IntPtr p, ref Vector4 v4, ref long i64, ref uint[] uVec, ref bool flag, ref string s, ref Vector3 v3, ref byte[] u8Vec, ref float f, ref Vector2 v2, ref Matrix4x4 m, ref sbyte i8, ref float[] vVec, ref double d)
    {
        flag = false; // Setting a value for bool reference
        f = 1.1f;    // Setting a value for float reference
        i64 = 1000;  // Setting a value for long reference
        v2 = new Vector2(3.0f, 4.0f); // Setting a value for Vector2 reference
        v4 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f); // Setting a value for Vector4 reference
        s = "MockFunc30"; // Setting a value for string reference
        p = IntPtr.Zero; // Setting a value for IntPtr reference
        uVec = [100, 200]; // Setting values for uint array
        m = new Matrix4x4(
            0.5f, 0.3f, 1.0f, 0.7f,
            1.1f, 0.9f, 0.6f, 0.4f,
            0.2f, 0.8f, 1.5f, 0.3f,
            0.7f, 0.4f, 0.9f, 1.0f
        ); // Setting a value for Matrix4x4 reference
        i8 = 8; // Setting a value for sbyte reference
        vVec = [1.0f, 1.0f, 2.0f, 2.0f]; // Setting values for float array
        d = 2.718; // Setting a value for double reference
        v3 = new Vector3(1, 2, 3); // Setting a value for Vector3 reference
        u8Vec = [255, 0, 255, 200, 100, 200]; // Setting values for byte array
        return 42; // Returning dummy int
    }

    // Mock implementation for a function with 31 parameters
    public static Vector3 MockFunc31(ref char c, ref uint u32, ref ulong[] uVec, ref Vector4 v4, ref string s, ref bool flag, ref long i64, ref Vector2 v2, ref sbyte i8, ref ushort u16, ref short[] iVec, ref Matrix4x4 m, ref Vector3 v3, ref float f, ref double[] v4Vec)
    {
        u32 = 12345; // Setting a value for uint reference
        flag = true; // Setting a value for bool reference
        v3 = new Vector3(1.0f, 2.0f, 3.0f); // Setting a value for Vector3 reference
        s = "MockFunc31"; // Setting a value for string reference
        v2 = new Vector2(5.0f, 6.0f); // Setting a value for Vector2 reference
        i8 = 7; // Setting a value for sbyte reference
        u16 = 255; // Setting a value for ushort reference
        m = new Matrix4x4(
            0.8f, 0.5f, 1.2f, 0.3f,  // Row 0
            1.0f, 0.7f, 0.4f, 0.6f,  // Row 1
            0.9f, 0.2f, 0.5f, 1.4f,  // Row 2
            0.6f, 0.8f, 1.1f, 0.7f   // Row 3
        ); // Setting a value for Matrix4x4 reference
        iVec = [1, 2]; // Setting values for short array
        v4 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f); // Setting a value for Vector4 reference
        i64 = 123456789; // Setting a value for long reference
        c = 'C'; // Setting a value for char reference
        v4Vec = [1.0, 1.0, 1.0, 1.0, 2.0, 2.0, 2.0, 2.0]; // Setting values for double array
        uVec = [1, 2, 3, 4, 5]; // Setting values for ulong array
        f = -1.0f;
        return new Vector3(1.0f, 2.0f, 3.0f); // Returning Vector3 reference
    }
    
    // Mock implementation for a function with 32 parameters
    public static double MockFunc32(ref int i32, ref ushort u16, ref sbyte[] iVec, ref Vector4 v4, ref IntPtr p, ref uint[] uVec, ref Matrix4x4 m, ref ulong u64, ref string s, ref long i64, ref Vector2 v2, ref sbyte[] u8Vec, ref bool flag, ref Vector3 v3, ref byte u8, ref char[] cVec)
    {
        i32 = 42; // Updated value for int32 reference
        u16 = 255; // Updated value for ushort reference
        flag = false; // Updated boolean value
        v2 = new Vector2(2.5f, 3.5f); // Updated value
        u8Vec = [1, 2, 3, 4, 5, 9]; // Updated values
        v4 = new Vector4(4.0f, 5.0f, 6.0f, 7.0f); // Updated value
        s = "MockFunc32"; // Updated string reference
        p = new IntPtr(0); // Updated IntPtr reference
        m = new Matrix4x4(
            1.0f, 0.4f, 0.3f, 0.9f,
            0.7f, 1.2f, 0.5f, 0.8f,
            0.2f, 0.6f, 1.1f, 0.4f,
            0.9f, 0.3f, 0.8f, 1.5f
        );
        u64 = 123456789; // Updated value for ulong reference
        uVec = [100, 200]; // Updated values
        i64 = 1000; // Updated value for long reference
        v3 = new Vector3(0.0f, 0.0f, 0.0f); // Updated value
        u8 = 8; // Updated value for byte reference
        cVec = ['a', 'b', 'c']; // Updated values
        iVec = [0, 1]; // Updated values
        return 1.0; // Updated return value
    }
}