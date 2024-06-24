using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Plugify;

//generated with https://github.com/untrustedmodders/csharp-lang-module/blob/main/generator/generator.py from cpp_test 

namespace cpp_test
{

	internal static class cpp_test
	{
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void NoParamReturnVoid();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern bool NoParamReturnBool();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern char NoParamReturnChar8();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern char NoParamReturnChar16();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern sbyte NoParamReturnInt8();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern short NoParamReturnInt16();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern int NoParamReturnInt32();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern long NoParamReturnInt64();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern byte NoParamReturnUInt8();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern ushort NoParamReturnUInt16();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern uint NoParamReturnUInt32();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern ulong NoParamReturnUInt64();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern IntPtr NoParamReturnPtr64();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern float NoParamReturnFloat();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern double NoParamReturnDouble();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern IntPtr NoParamReturnFunction();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern string NoParamReturnString();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern bool[] NoParamReturnArrayBool();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern char[] NoParamReturnArrayChar8();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern char[] NoParamReturnArrayChar16();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern sbyte[] NoParamReturnArrayInt8();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern short[] NoParamReturnArrayInt16();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern int[] NoParamReturnArrayInt32();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern long[] NoParamReturnArrayInt64();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern byte[] NoParamReturnArrayUInt8();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern ushort[] NoParamReturnArrayUInt16();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern uint[] NoParamReturnArrayUInt32();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern ulong[] NoParamReturnArrayUInt64();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern IntPtr[] NoParamReturnArrayPtr64();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern float[] NoParamReturnArrayFloat();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern double[] NoParamReturnArrayDouble();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern string[] NoParamReturnArrayString();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern Vector2 NoParamReturnVector2();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern Vector3 NoParamReturnVector3();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern Vector4 NoParamReturnVector4();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern Matrix4x4 NoParamReturnMatrix4x4();
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern int Param1(int a);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void Param2(int a, float b);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void Param3(int a, float b, double c);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void Param4(int a, float b, double c, Vector4 d);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void Param5(int a, float b, double c, Vector4 d, long[] e);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void Param6(int a, float b, double c, Vector4 d, long[] e, char f);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void Param7(int a, float b, double c, Vector4 d, long[] e, char f, string g);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void Param8(int a, float b, double c, Vector4 d, long[] e, char f, string g, float h);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void Param9(int a, float b, double c, Vector4 d, long[] e, char f, string g, float h, short k);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void Param10(int a, float b, double c, Vector4 d, long[] e, char f, string g, float h, short k, IntPtr l);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef1(ref int a);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef2(ref int a, ref float b);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef3(ref int a, ref float b, ref double c);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef4(ref int a, ref float b, ref double c, ref Vector4 d);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef5(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef6(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef7(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef8(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref float h);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef9(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref float h, ref short k);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRef10(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref float h, ref short k, ref IntPtr l);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern void ParamRefVectors(ref bool[] p1, ref char[] p2, ref char[] p3, ref sbyte[] p4, ref short[] p5, ref int[] p6, ref long[] p7, ref byte[] p8, ref ushort[] p9, ref uint[] p10, ref ulong[] p11, ref IntPtr[] p12, ref float[] p13, ref double[] p14, ref string[] p15);
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		internal static extern long ParamAllPrimitives(bool p1, char p2, sbyte p3, short p4, int p5, long p6, byte p7, ushort p8, uint p9, ulong p10, IntPtr p11, float p12, double p13);
	}
}
