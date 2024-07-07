using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Plugify;

//generated with https://github.com/untrustedmodders/dotnet-lang-module/blob/main/generator/generator.py from cpp_test 

namespace cpp_test
{

	internal static unsafe class cpp_test
	{
		private static Dictionary<Delegate, Delegate> s_DelegateHolder = new();
		private static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) where TKey : notnull
		{
			if (dict.TryGetValue(key, out TValue? val))
			{
				return val;
			}
			dict.Add(key, value);
			return value;
		}

		internal static delegate* <void> NoParamReturnVoid = &___NoParamReturnVoid;
		internal static delegate* unmanaged[Cdecl]<void> __NoParamReturnVoid;
		private static void ___NoParamReturnVoid()
		{
			__NoParamReturnVoid();
		}
		internal static delegate* <bool> NoParamReturnBool = &___NoParamReturnBool;
		internal static delegate* unmanaged[Cdecl]<bool> __NoParamReturnBool;
		private static bool ___NoParamReturnBool()
		{
			var __result = __NoParamReturnBool();
			return __result;
		}
		internal static delegate* <char> NoParamReturnChar8 = &___NoParamReturnChar8;
		internal static delegate* unmanaged[Cdecl]<sbyte> __NoParamReturnChar8;
		private static char ___NoParamReturnChar8()
		{
			var __result = __NoParamReturnChar8();
			return (char)__result;
		}
		internal static delegate* <char> NoParamReturnChar16 = &___NoParamReturnChar16;
		internal static delegate* unmanaged[Cdecl]<ushort> __NoParamReturnChar16;
		private static char ___NoParamReturnChar16()
		{
			var __result = __NoParamReturnChar16();
			return (char)__result;
		}
		internal static delegate* <sbyte> NoParamReturnInt8 = &___NoParamReturnInt8;
		internal static delegate* unmanaged[Cdecl]<sbyte> __NoParamReturnInt8;
		private static sbyte ___NoParamReturnInt8()
		{
			var __result = __NoParamReturnInt8();
			return __result;
		}
		internal static delegate* <short> NoParamReturnInt16 = &___NoParamReturnInt16;
		internal static delegate* unmanaged[Cdecl]<short> __NoParamReturnInt16;
		private static short ___NoParamReturnInt16()
		{
			var __result = __NoParamReturnInt16();
			return __result;
		}
		internal static delegate* <int> NoParamReturnInt32 = &___NoParamReturnInt32;
		internal static delegate* unmanaged[Cdecl]<int> __NoParamReturnInt32;
		private static int ___NoParamReturnInt32()
		{
			var __result = __NoParamReturnInt32();
			return __result;
		}
		internal static delegate* <long> NoParamReturnInt64 = &___NoParamReturnInt64;
		internal static delegate* unmanaged[Cdecl]<long> __NoParamReturnInt64;
		private static long ___NoParamReturnInt64()
		{
			var __result = __NoParamReturnInt64();
			return __result;
		}
		internal static delegate* <byte> NoParamReturnUInt8 = &___NoParamReturnUInt8;
		internal static delegate* unmanaged[Cdecl]<byte> __NoParamReturnUInt8;
		private static byte ___NoParamReturnUInt8()
		{
			var __result = __NoParamReturnUInt8();
			return __result;
		}
		internal static delegate* <ushort> NoParamReturnUInt16 = &___NoParamReturnUInt16;
		internal static delegate* unmanaged[Cdecl]<ushort> __NoParamReturnUInt16;
		private static ushort ___NoParamReturnUInt16()
		{
			var __result = __NoParamReturnUInt16();
			return __result;
		}
		internal static delegate* <uint> NoParamReturnUInt32 = &___NoParamReturnUInt32;
		internal static delegate* unmanaged[Cdecl]<uint> __NoParamReturnUInt32;
		private static uint ___NoParamReturnUInt32()
		{
			var __result = __NoParamReturnUInt32();
			return __result;
		}
		internal static delegate* <ulong> NoParamReturnUInt64 = &___NoParamReturnUInt64;
		internal static delegate* unmanaged[Cdecl]<ulong> __NoParamReturnUInt64;
		private static ulong ___NoParamReturnUInt64()
		{
			var __result = __NoParamReturnUInt64();
			return __result;
		}
		internal static delegate* <nint> NoParamReturnPtr64 = &___NoParamReturnPtr64;
		internal static delegate* unmanaged[Cdecl]<nint> __NoParamReturnPtr64;
		private static nint ___NoParamReturnPtr64()
		{
			var __result = __NoParamReturnPtr64();
			return __result;
		}
		internal static delegate* <float> NoParamReturnFloat = &___NoParamReturnFloat;
		internal static delegate* unmanaged[Cdecl]<float> __NoParamReturnFloat;
		private static float ___NoParamReturnFloat()
		{
			var __result = __NoParamReturnFloat();
			return __result;
		}
		internal static delegate* <double> NoParamReturnDouble = &___NoParamReturnDouble;
		internal static delegate* unmanaged[Cdecl]<double> __NoParamReturnDouble;
		private static double ___NoParamReturnDouble()
		{
			var __result = __NoParamReturnDouble();
			return __result;
		}
		internal static delegate* <nint> NoParamReturnFunction = &___NoParamReturnFunction;
		internal static delegate* unmanaged[Cdecl]<nint> __NoParamReturnFunction;
		private static nint ___NoParamReturnFunction()
		{
			var __result = __NoParamReturnFunction();
			return __result;
		}
		internal static delegate* <string> NoParamReturnString = &___NoParamReturnString;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnString;
		private static string ___NoParamReturnString()
		{
			var __output = NativeMethods.AllocateString();

			__NoParamReturnString(__output);

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <bool[]> NoParamReturnArrayBool = &___NoParamReturnArrayBool;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayBool;
		private static bool[] ___NoParamReturnArrayBool()
		{
			var __output = NativeMethods.AllocateVectorBool();

			__NoParamReturnArrayBool(__output);

			var output = new bool[NativeMethods.GetVectorSizeBool(__output)];
			NativeMethods.GetVectorDataBool(__output, output);

			NativeMethods.FreeVectorBool(__output);

			return output;
		}
		internal static delegate* <char[]> NoParamReturnArrayChar8 = &___NoParamReturnArrayChar8;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayChar8;
		private static char[] ___NoParamReturnArrayChar8()
		{
			var __output = NativeMethods.AllocateVectorChar8();

			__NoParamReturnArrayChar8(__output);

			var output = new char[NativeMethods.GetVectorSizeChar8(__output)];
			NativeMethods.GetVectorDataChar8(__output, output);

			NativeMethods.FreeVectorChar8(__output);

			return output;
		}
		internal static delegate* <char[]> NoParamReturnArrayChar16 = &___NoParamReturnArrayChar16;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayChar16;
		private static char[] ___NoParamReturnArrayChar16()
		{
			var __output = NativeMethods.AllocateVectorChar16();

			__NoParamReturnArrayChar16(__output);

			var output = new char[NativeMethods.GetVectorSizeChar16(__output)];
			NativeMethods.GetVectorDataChar16(__output, output);

			NativeMethods.FreeVectorChar16(__output);

			return output;
		}
		internal static delegate* <sbyte[]> NoParamReturnArrayInt8 = &___NoParamReturnArrayInt8;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayInt8;
		private static sbyte[] ___NoParamReturnArrayInt8()
		{
			var __output = NativeMethods.AllocateVectorInt8();

			__NoParamReturnArrayInt8(__output);

			var output = new sbyte[NativeMethods.GetVectorSizeInt8(__output)];
			NativeMethods.GetVectorDataInt8(__output, output);

			NativeMethods.FreeVectorInt8(__output);

			return output;
		}
		internal static delegate* <short[]> NoParamReturnArrayInt16 = &___NoParamReturnArrayInt16;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayInt16;
		private static short[] ___NoParamReturnArrayInt16()
		{
			var __output = NativeMethods.AllocateVectorInt16();

			__NoParamReturnArrayInt16(__output);

			var output = new short[NativeMethods.GetVectorSizeInt16(__output)];
			NativeMethods.GetVectorDataInt16(__output, output);

			NativeMethods.FreeVectorInt16(__output);

			return output;
		}
		internal static delegate* <int[]> NoParamReturnArrayInt32 = &___NoParamReturnArrayInt32;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayInt32;
		private static int[] ___NoParamReturnArrayInt32()
		{
			var __output = NativeMethods.AllocateVectorInt32();

			__NoParamReturnArrayInt32(__output);

			var output = new int[NativeMethods.GetVectorSizeInt32(__output)];
			NativeMethods.GetVectorDataInt32(__output, output);

			NativeMethods.FreeVectorInt32(__output);

			return output;
		}
		internal static delegate* <long[]> NoParamReturnArrayInt64 = &___NoParamReturnArrayInt64;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayInt64;
		private static long[] ___NoParamReturnArrayInt64()
		{
			var __output = NativeMethods.AllocateVectorInt64();

			__NoParamReturnArrayInt64(__output);

			var output = new long[NativeMethods.GetVectorSizeInt64(__output)];
			NativeMethods.GetVectorDataInt64(__output, output);

			NativeMethods.FreeVectorInt64(__output);

			return output;
		}
		internal static delegate* <byte[]> NoParamReturnArrayUInt8 = &___NoParamReturnArrayUInt8;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayUInt8;
		private static byte[] ___NoParamReturnArrayUInt8()
		{
			var __output = NativeMethods.AllocateVectorUInt8();

			__NoParamReturnArrayUInt8(__output);

			var output = new byte[NativeMethods.GetVectorSizeUInt8(__output)];
			NativeMethods.GetVectorDataUInt8(__output, output);

			NativeMethods.FreeVectorUInt8(__output);

			return output;
		}
		internal static delegate* <ushort[]> NoParamReturnArrayUInt16 = &___NoParamReturnArrayUInt16;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayUInt16;
		private static ushort[] ___NoParamReturnArrayUInt16()
		{
			var __output = NativeMethods.AllocateVectorUInt16();

			__NoParamReturnArrayUInt16(__output);

			var output = new ushort[NativeMethods.GetVectorSizeUInt16(__output)];
			NativeMethods.GetVectorDataUInt16(__output, output);

			NativeMethods.FreeVectorUInt16(__output);

			return output;
		}
		internal static delegate* <uint[]> NoParamReturnArrayUInt32 = &___NoParamReturnArrayUInt32;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayUInt32;
		private static uint[] ___NoParamReturnArrayUInt32()
		{
			var __output = NativeMethods.AllocateVectorUInt32();

			__NoParamReturnArrayUInt32(__output);

			var output = new uint[NativeMethods.GetVectorSizeUInt32(__output)];
			NativeMethods.GetVectorDataUInt32(__output, output);

			NativeMethods.FreeVectorUInt32(__output);

			return output;
		}
		internal static delegate* <ulong[]> NoParamReturnArrayUInt64 = &___NoParamReturnArrayUInt64;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayUInt64;
		private static ulong[] ___NoParamReturnArrayUInt64()
		{
			var __output = NativeMethods.AllocateVectorUInt64();

			__NoParamReturnArrayUInt64(__output);

			var output = new ulong[NativeMethods.GetVectorSizeUInt64(__output)];
			NativeMethods.GetVectorDataUInt64(__output, output);

			NativeMethods.FreeVectorUInt64(__output);

			return output;
		}
		internal static delegate* <nint[]> NoParamReturnArrayPtr64 = &___NoParamReturnArrayPtr64;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayPtr64;
		private static nint[] ___NoParamReturnArrayPtr64()
		{
			var __output = NativeMethods.AllocateVectorIntPtr();

			__NoParamReturnArrayPtr64(__output);

			var output = new nint[NativeMethods.GetVectorSizeIntPtr(__output)];
			NativeMethods.GetVectorDataIntPtr(__output, output);

			NativeMethods.FreeVectorIntPtr(__output);

			return output;
		}
		internal static delegate* <float[]> NoParamReturnArrayFloat = &___NoParamReturnArrayFloat;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayFloat;
		private static float[] ___NoParamReturnArrayFloat()
		{
			var __output = NativeMethods.AllocateVectorFloat();

			__NoParamReturnArrayFloat(__output);

			var output = new float[NativeMethods.GetVectorSizeFloat(__output)];
			NativeMethods.GetVectorDataFloat(__output, output);

			NativeMethods.FreeVectorFloat(__output);

			return output;
		}
		internal static delegate* <double[]> NoParamReturnArrayDouble = &___NoParamReturnArrayDouble;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayDouble;
		private static double[] ___NoParamReturnArrayDouble()
		{
			var __output = NativeMethods.AllocateVectorDouble();

			__NoParamReturnArrayDouble(__output);

			var output = new double[NativeMethods.GetVectorSizeDouble(__output)];
			NativeMethods.GetVectorDataDouble(__output, output);

			NativeMethods.FreeVectorDouble(__output);

			return output;
		}
		internal static delegate* <string[]> NoParamReturnArrayString = &___NoParamReturnArrayString;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayString;
		private static string[] ___NoParamReturnArrayString()
		{
			var __output = NativeMethods.AllocateVectorString();

			__NoParamReturnArrayString(__output);

			var output = new string[NativeMethods.GetVectorSizeString(__output)];
			NativeMethods.GetVectorDataString(__output, output);

			NativeMethods.FreeVectorString(__output);

			return output;
		}
		internal static delegate* <Vector2> NoParamReturnVector2 = &___NoParamReturnVector2;
		internal static delegate* unmanaged[Cdecl]<Vector2> __NoParamReturnVector2;
		private static Vector2 ___NoParamReturnVector2()
		{
			var __result = __NoParamReturnVector2();
			return __result;
		}
		internal static delegate* <Vector3> NoParamReturnVector3 = &___NoParamReturnVector3;
		internal static delegate* unmanaged[Cdecl]<Vector3> __NoParamReturnVector3;
		private static Vector3 ___NoParamReturnVector3()
		{
			var __result = __NoParamReturnVector3();
			return __result;
		}
		internal static delegate* <Vector4> NoParamReturnVector4 = &___NoParamReturnVector4;
		internal static delegate* unmanaged[Cdecl]<Vector4> __NoParamReturnVector4;
		private static Vector4 ___NoParamReturnVector4()
		{
			var __result = __NoParamReturnVector4();
			return __result;
		}
		internal static delegate* <Matrix4x4> NoParamReturnMatrix4x4 = &___NoParamReturnMatrix4x4;
		internal static delegate* unmanaged[Cdecl]<Matrix4x4> __NoParamReturnMatrix4x4;
		private static Matrix4x4 ___NoParamReturnMatrix4x4()
		{
			var __result = __NoParamReturnMatrix4x4();
			return __result;
		}
		internal static delegate* <int, void> Param1 = &___Param1;
		internal static delegate* unmanaged[Cdecl]<int, void> __Param1;
		private static void ___Param1(int a)
		{
			__Param1(a);
		}
		internal static delegate* <int, float, void> Param2 = &___Param2;
		internal static delegate* unmanaged[Cdecl]<int, float, void> __Param2;
		private static void ___Param2(int a, float b)
		{
			__Param2(a, b);
		}
		internal static delegate* <int, float, double, void> Param3 = &___Param3;
		internal static delegate* unmanaged[Cdecl]<int, float, double, void> __Param3;
		private static void ___Param3(int a, float b, double c)
		{
			__Param3(a, b, c);
		}
		internal static delegate* <int, float, double, Vector4, void> Param4 = &___Param4;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4, void> __Param4;
		private static void ___Param4(int a, float b, double c, Vector4 d)
		{
			__Param4(a, b, c, d);
		}
		internal static delegate* <int, float, double, Vector4, long[], void> Param5 = &___Param5;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, void> __Param5;
		private static void ___Param5(int a, float b, double c, Vector4 d, long[] e)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);

			__Param5(a, b, c, d, __e);

			NativeMethods.DeleteVectorInt64(__e);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, void> Param6 = &___Param6;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, void> __Param6;
		private static void ___Param6(int a, float b, double c, Vector4 d, long[] e, char f)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);

			__Param6(a, b, c, d, __e, __f);

			NativeMethods.DeleteVectorInt64(__e);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, string, void> Param7 = &___Param7;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, nint, void> __Param7;
		private static void ___Param7(int a, float b, double c, Vector4 d, long[] e, char f, string g)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__Param7(a, b, c, d, __e, __f, __g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, string, float, void> Param8 = &___Param8;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, nint, float, void> __Param8;
		private static void ___Param8(int a, float b, double c, Vector4 d, long[] e, char f, string g, float h)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__Param8(a, b, c, d, __e, __f, __g, h);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, string, float, short, void> Param9 = &___Param9;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, nint, float, short, void> __Param9;
		private static void ___Param9(int a, float b, double c, Vector4 d, long[] e, char f, string g, float h, short k)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__Param9(a, b, c, d, __e, __f, __g, h, k);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, string, float, short, nint, void> Param10 = &___Param10;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, nint, float, short, nint, void> __Param10;
		private static void ___Param10(int a, float b, double c, Vector4 d, long[] e, char f, string g, float h, short k, nint l)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__Param10(a, b, c, d, __e, __f, __g, h, k, l);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <ref int, void> ParamRef1 = &___ParamRef1;
		internal static delegate* unmanaged[Cdecl]<ref int, void> __ParamRef1;
		private static void ___ParamRef1(ref int a)
		{
			__ParamRef1(ref a);
		}
		internal static delegate* <ref int, ref float, void> ParamRef2 = &___ParamRef2;
		internal static delegate* unmanaged[Cdecl]<ref int, ref float, void> __ParamRef2;
		private static void ___ParamRef2(ref int a, ref float b)
		{
			__ParamRef2(ref a, ref b);
		}
		internal static delegate* <ref int, ref float, ref double, void> ParamRef3 = &___ParamRef3;
		internal static delegate* unmanaged[Cdecl]<ref int, ref float, ref double, void> __ParamRef3;
		private static void ___ParamRef3(ref int a, ref float b, ref double c)
		{
			__ParamRef3(ref a, ref b, ref c);
		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, void> ParamRef4 = &___ParamRef4;
		internal static delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, void> __ParamRef4;
		private static void ___ParamRef4(ref int a, ref float b, ref double c, ref Vector4 d)
		{
			__ParamRef4(ref a, ref b, ref c, ref d);
		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], void> ParamRef5 = &___ParamRef5;
		internal static delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, void> __ParamRef5;
		private static void ___ParamRef5(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);

			__ParamRef5(ref a, ref b, ref c, ref d, __e);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);

			NativeMethods.DeleteVectorInt64(__e);

		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, void> ParamRef6 = &___ParamRef6;
		internal static delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, void> __ParamRef6;
		private static void ___ParamRef6(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);

			__ParamRef6(ref a, ref b, ref c, ref d, __e, ref __f);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);

			NativeMethods.DeleteVectorInt64(__e);

		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, ref string, void> ParamRef7 = &___ParamRef7;
		internal static delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, nint, void> __ParamRef7;
		private static void ___ParamRef7(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__ParamRef7(ref a, ref b, ref c, ref d, __e, ref __f, __g);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, ref string, ref float, void> ParamRef8 = &___ParamRef8;
		internal static delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, nint, ref float, void> __ParamRef8;
		private static void ___ParamRef8(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref float h)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__ParamRef8(ref a, ref b, ref c, ref d, __e, ref __f, __g, ref h);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, ref string, ref float, ref short, void> ParamRef9 = &___ParamRef9;
		internal static delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, nint, ref float, ref short, void> __ParamRef9;
		private static void ___ParamRef9(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref float h, ref short k)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__ParamRef9(ref a, ref b, ref c, ref d, __e, ref __f, __g, ref h, ref k);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, ref string, ref float, ref short, ref nint, void> ParamRef10 = &___ParamRef10;
		internal static delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, nint, ref float, ref short, ref nint, void> __ParamRef10;
		private static void ___ParamRef10(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref float h, ref short k, ref nint l)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__ParamRef10(ref a, ref b, ref c, ref d, __e, ref __f, __g, ref h, ref k, ref l);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <ref bool[], ref char[], ref char[], ref sbyte[], ref short[], ref int[], ref long[], ref byte[], ref ushort[], ref uint[], ref ulong[], ref nint[], ref float[], ref double[], ref string[], void> ParamRefVectors = &___ParamRefVectors;
		internal static delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void> __ParamRefVectors;
		private static void ___ParamRefVectors(ref bool[] p1, ref char[] p2, ref char[] p3, ref sbyte[] p4, ref short[] p5, ref int[] p6, ref long[] p7, ref byte[] p8, ref ushort[] p9, ref uint[] p10, ref ulong[] p11, ref nint[] p12, ref float[] p13, ref double[] p14, ref string[] p15)
		{
			var __p1 = NativeMethods.CreateVectorBool(p1, p1.Length);
			var __p2 = NativeMethods.CreateVectorChar8(p2, p2.Length);
			var __p3 = NativeMethods.CreateVectorChar16(p3, p3.Length);
			var __p4 = NativeMethods.CreateVectorInt8(p4, p4.Length);
			var __p5 = NativeMethods.CreateVectorInt16(p5, p5.Length);
			var __p6 = NativeMethods.CreateVectorInt32(p6, p6.Length);
			var __p7 = NativeMethods.CreateVectorInt64(p7, p7.Length);
			var __p8 = NativeMethods.CreateVectorUInt8(p8, p8.Length);
			var __p9 = NativeMethods.CreateVectorUInt16(p9, p9.Length);
			var __p10 = NativeMethods.CreateVectorUInt32(p10, p10.Length);
			var __p11 = NativeMethods.CreateVectorUInt64(p11, p11.Length);
			var __p12 = NativeMethods.CreateVectorIntPtr(p12, p12.Length);
			var __p13 = NativeMethods.CreateVectorFloat(p13, p13.Length);
			var __p14 = NativeMethods.CreateVectorDouble(p14, p14.Length);
			var __p15 = NativeMethods.CreateVectorString(p15, p15.Length);

			__ParamRefVectors(__p1, __p2, __p3, __p4, __p5, __p6, __p7, __p8, __p9, __p10, __p11, __p12, __p13, __p14, __p15);

			Array.Resize(ref p1, NativeMethods.GetVectorSizeBool(__p1));
			NativeMethods.GetVectorDataBool(__p1, p1);
			Array.Resize(ref p2, NativeMethods.GetVectorSizeChar8(__p2));
			NativeMethods.GetVectorDataChar8(__p2, p2);
			Array.Resize(ref p3, NativeMethods.GetVectorSizeChar16(__p3));
			NativeMethods.GetVectorDataChar16(__p3, p3);
			Array.Resize(ref p4, NativeMethods.GetVectorSizeInt8(__p4));
			NativeMethods.GetVectorDataInt8(__p4, p4);
			Array.Resize(ref p5, NativeMethods.GetVectorSizeInt16(__p5));
			NativeMethods.GetVectorDataInt16(__p5, p5);
			Array.Resize(ref p6, NativeMethods.GetVectorSizeInt32(__p6));
			NativeMethods.GetVectorDataInt32(__p6, p6);
			Array.Resize(ref p7, NativeMethods.GetVectorSizeInt64(__p7));
			NativeMethods.GetVectorDataInt64(__p7, p7);
			Array.Resize(ref p8, NativeMethods.GetVectorSizeUInt8(__p8));
			NativeMethods.GetVectorDataUInt8(__p8, p8);
			Array.Resize(ref p9, NativeMethods.GetVectorSizeUInt16(__p9));
			NativeMethods.GetVectorDataUInt16(__p9, p9);
			Array.Resize(ref p10, NativeMethods.GetVectorSizeUInt32(__p10));
			NativeMethods.GetVectorDataUInt32(__p10, p10);
			Array.Resize(ref p11, NativeMethods.GetVectorSizeUInt64(__p11));
			NativeMethods.GetVectorDataUInt64(__p11, p11);
			Array.Resize(ref p12, NativeMethods.GetVectorSizeIntPtr(__p12));
			NativeMethods.GetVectorDataIntPtr(__p12, p12);
			Array.Resize(ref p13, NativeMethods.GetVectorSizeFloat(__p13));
			NativeMethods.GetVectorDataFloat(__p13, p13);
			Array.Resize(ref p14, NativeMethods.GetVectorSizeDouble(__p14));
			NativeMethods.GetVectorDataDouble(__p14, p14);
			Array.Resize(ref p15, NativeMethods.GetVectorSizeString(__p15));
			NativeMethods.GetVectorDataString(__p15, p15);

			NativeMethods.DeleteVectorBool(__p1);
			NativeMethods.DeleteVectorChar8(__p2);
			NativeMethods.DeleteVectorChar16(__p3);
			NativeMethods.DeleteVectorInt8(__p4);
			NativeMethods.DeleteVectorInt16(__p5);
			NativeMethods.DeleteVectorInt32(__p6);
			NativeMethods.DeleteVectorInt64(__p7);
			NativeMethods.DeleteVectorUInt8(__p8);
			NativeMethods.DeleteVectorUInt16(__p9);
			NativeMethods.DeleteVectorUInt32(__p10);
			NativeMethods.DeleteVectorUInt64(__p11);
			NativeMethods.DeleteVectorIntPtr(__p12);
			NativeMethods.DeleteVectorFloat(__p13);
			NativeMethods.DeleteVectorDouble(__p14);
			NativeMethods.DeleteVectorString(__p15);

		}
		internal static delegate* <bool, char, sbyte, short, int, long, byte, ushort, uint, ulong, nint, float, double, long> ParamAllPrimitives = &___ParamAllPrimitives;
		internal static delegate* unmanaged[Cdecl]<bool, ushort, sbyte, short, int, long, byte, ushort, uint, ulong, nint, float, double, long> __ParamAllPrimitives;
		private static long ___ParamAllPrimitives(bool p1, char p2, sbyte p3, short p4, int p5, long p6, byte p7, ushort p8, uint p9, ulong p10, nint p11, float p12, double p13)
		{
			var __p2 = Convert.ToUInt16(p2);

			var __result = __ParamAllPrimitives(p1, __p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
			return __result;
		}
	}
}
