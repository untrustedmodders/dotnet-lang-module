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

		private static nint NoParamReturnVoidPtr = nint.Zero;
		internal static void NoParamReturnVoid()
		{
			if (NoParamReturnVoidPtr == nint.Zero) NoParamReturnVoidPtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnVoid");
			var NoParamReturnVoidFunc = (delegate* unmanaged[Cdecl]<void>)NoParamReturnVoidPtr;
			NoParamReturnVoidFunc();
		}
		private static nint NoParamReturnBoolPtr = nint.Zero;
		internal static bool NoParamReturnBool()
		{
			if (NoParamReturnBoolPtr == nint.Zero) NoParamReturnBoolPtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnBool");
			var NoParamReturnBoolFunc = (delegate* unmanaged[Cdecl]<bool>)NoParamReturnBoolPtr;
			var __result = NoParamReturnBoolFunc();
			return __result;
		}
		private static nint NoParamReturnChar8Ptr = nint.Zero;
		internal static char NoParamReturnChar8()
		{
			if (NoParamReturnChar8Ptr == nint.Zero) NoParamReturnChar8Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnChar8");
			var NoParamReturnChar8Func = (delegate* unmanaged[Cdecl]<sbyte>)NoParamReturnChar8Ptr;
			var __result = NoParamReturnChar8Func();
			return (char)__result;
		}
		private static nint NoParamReturnChar16Ptr = nint.Zero;
		internal static char NoParamReturnChar16()
		{
			if (NoParamReturnChar16Ptr == nint.Zero) NoParamReturnChar16Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnChar16");
			var NoParamReturnChar16Func = (delegate* unmanaged[Cdecl]<ushort>)NoParamReturnChar16Ptr;
			var __result = NoParamReturnChar16Func();
			return (char)__result;
		}
		private static nint NoParamReturnInt8Ptr = nint.Zero;
		internal static sbyte NoParamReturnInt8()
		{
			if (NoParamReturnInt8Ptr == nint.Zero) NoParamReturnInt8Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnInt8");
			var NoParamReturnInt8Func = (delegate* unmanaged[Cdecl]<sbyte>)NoParamReturnInt8Ptr;
			var __result = NoParamReturnInt8Func();
			return __result;
		}
		private static nint NoParamReturnInt16Ptr = nint.Zero;
		internal static short NoParamReturnInt16()
		{
			if (NoParamReturnInt16Ptr == nint.Zero) NoParamReturnInt16Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnInt16");
			var NoParamReturnInt16Func = (delegate* unmanaged[Cdecl]<short>)NoParamReturnInt16Ptr;
			var __result = NoParamReturnInt16Func();
			return __result;
		}
		private static nint NoParamReturnInt32Ptr = nint.Zero;
		internal static int NoParamReturnInt32()
		{
			if (NoParamReturnInt32Ptr == nint.Zero) NoParamReturnInt32Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnInt32");
			var NoParamReturnInt32Func = (delegate* unmanaged[Cdecl]<int>)NoParamReturnInt32Ptr;
			var __result = NoParamReturnInt32Func();
			return __result;
		}
		private static nint NoParamReturnInt64Ptr = nint.Zero;
		internal static long NoParamReturnInt64()
		{
			if (NoParamReturnInt64Ptr == nint.Zero) NoParamReturnInt64Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnInt64");
			var NoParamReturnInt64Func = (delegate* unmanaged[Cdecl]<long>)NoParamReturnInt64Ptr;
			var __result = NoParamReturnInt64Func();
			return __result;
		}
		private static nint NoParamReturnUInt8Ptr = nint.Zero;
		internal static byte NoParamReturnUInt8()
		{
			if (NoParamReturnUInt8Ptr == nint.Zero) NoParamReturnUInt8Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnUInt8");
			var NoParamReturnUInt8Func = (delegate* unmanaged[Cdecl]<byte>)NoParamReturnUInt8Ptr;
			var __result = NoParamReturnUInt8Func();
			return __result;
		}
		private static nint NoParamReturnUInt16Ptr = nint.Zero;
		internal static ushort NoParamReturnUInt16()
		{
			if (NoParamReturnUInt16Ptr == nint.Zero) NoParamReturnUInt16Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnUInt16");
			var NoParamReturnUInt16Func = (delegate* unmanaged[Cdecl]<ushort>)NoParamReturnUInt16Ptr;
			var __result = NoParamReturnUInt16Func();
			return __result;
		}
		private static nint NoParamReturnUInt32Ptr = nint.Zero;
		internal static uint NoParamReturnUInt32()
		{
			if (NoParamReturnUInt32Ptr == nint.Zero) NoParamReturnUInt32Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnUInt32");
			var NoParamReturnUInt32Func = (delegate* unmanaged[Cdecl]<uint>)NoParamReturnUInt32Ptr;
			var __result = NoParamReturnUInt32Func();
			return __result;
		}
		private static nint NoParamReturnUInt64Ptr = nint.Zero;
		internal static ulong NoParamReturnUInt64()
		{
			if (NoParamReturnUInt64Ptr == nint.Zero) NoParamReturnUInt64Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnUInt64");
			var NoParamReturnUInt64Func = (delegate* unmanaged[Cdecl]<ulong>)NoParamReturnUInt64Ptr;
			var __result = NoParamReturnUInt64Func();
			return __result;
		}
		private static nint NoParamReturnPtr64Ptr = nint.Zero;
		internal static nint NoParamReturnPtr64()
		{
			if (NoParamReturnPtr64Ptr == nint.Zero) NoParamReturnPtr64Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnPtr64");
			var NoParamReturnPtr64Func = (delegate* unmanaged[Cdecl]<nint>)NoParamReturnPtr64Ptr;
			var __result = NoParamReturnPtr64Func();
			return __result;
		}
		private static nint NoParamReturnFloatPtr = nint.Zero;
		internal static float NoParamReturnFloat()
		{
			if (NoParamReturnFloatPtr == nint.Zero) NoParamReturnFloatPtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnFloat");
			var NoParamReturnFloatFunc = (delegate* unmanaged[Cdecl]<float>)NoParamReturnFloatPtr;
			var __result = NoParamReturnFloatFunc();
			return __result;
		}
		private static nint NoParamReturnDoublePtr = nint.Zero;
		internal static double NoParamReturnDouble()
		{
			if (NoParamReturnDoublePtr == nint.Zero) NoParamReturnDoublePtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnDouble");
			var NoParamReturnDoubleFunc = (delegate* unmanaged[Cdecl]<double>)NoParamReturnDoublePtr;
			var __result = NoParamReturnDoubleFunc();
			return __result;
		}
		private static nint NoParamReturnFunctionPtr = nint.Zero;
		internal static nint NoParamReturnFunction()
		{
			if (NoParamReturnFunctionPtr == nint.Zero) NoParamReturnFunctionPtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnFunction");
			var NoParamReturnFunctionFunc = (delegate* unmanaged[Cdecl]<nint>)NoParamReturnFunctionPtr;
			var __result = NoParamReturnFunctionFunc();
			return __result;
		}
		private static nint NoParamReturnStringPtr = nint.Zero;
		internal static string NoParamReturnString()
		{
			var __output = NativeMethods.AllocateString();

			if (NoParamReturnStringPtr == nint.Zero) NoParamReturnStringPtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnString");
			var NoParamReturnStringFunc = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnStringPtr;
			NoParamReturnStringFunc(__output);

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		private static nint NoParamReturnArrayBoolPtr = nint.Zero;
		internal static bool[] NoParamReturnArrayBool()
		{
			var __output = NativeMethods.AllocateVectorBool();

			if (NoParamReturnArrayBoolPtr == nint.Zero) NoParamReturnArrayBoolPtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayBool");
			var NoParamReturnArrayBoolFunc = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayBoolPtr;
			NoParamReturnArrayBoolFunc(__output);

			var output = new bool[NativeMethods.GetVectorSizeBool(__output)];
			NativeMethods.GetVectorDataBool(__output, output);

			NativeMethods.FreeVectorBool(__output);

			return output;
		}
		private static nint NoParamReturnArrayChar8Ptr = nint.Zero;
		internal static char[] NoParamReturnArrayChar8()
		{
			var __output = NativeMethods.AllocateVectorChar8();

			if (NoParamReturnArrayChar8Ptr == nint.Zero) NoParamReturnArrayChar8Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayChar8");
			var NoParamReturnArrayChar8Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayChar8Ptr;
			NoParamReturnArrayChar8Func(__output);

			var output = new char[NativeMethods.GetVectorSizeChar8(__output)];
			NativeMethods.GetVectorDataChar8(__output, output);

			NativeMethods.FreeVectorChar8(__output);

			return output;
		}
		private static nint NoParamReturnArrayChar16Ptr = nint.Zero;
		internal static char[] NoParamReturnArrayChar16()
		{
			var __output = NativeMethods.AllocateVectorChar16();

			if (NoParamReturnArrayChar16Ptr == nint.Zero) NoParamReturnArrayChar16Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayChar16");
			var NoParamReturnArrayChar16Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayChar16Ptr;
			NoParamReturnArrayChar16Func(__output);

			var output = new char[NativeMethods.GetVectorSizeChar16(__output)];
			NativeMethods.GetVectorDataChar16(__output, output);

			NativeMethods.FreeVectorChar16(__output);

			return output;
		}
		private static nint NoParamReturnArrayInt8Ptr = nint.Zero;
		internal static sbyte[] NoParamReturnArrayInt8()
		{
			var __output = NativeMethods.AllocateVectorInt8();

			if (NoParamReturnArrayInt8Ptr == nint.Zero) NoParamReturnArrayInt8Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayInt8");
			var NoParamReturnArrayInt8Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayInt8Ptr;
			NoParamReturnArrayInt8Func(__output);

			var output = new sbyte[NativeMethods.GetVectorSizeInt8(__output)];
			NativeMethods.GetVectorDataInt8(__output, output);

			NativeMethods.FreeVectorInt8(__output);

			return output;
		}
		private static nint NoParamReturnArrayInt16Ptr = nint.Zero;
		internal static short[] NoParamReturnArrayInt16()
		{
			var __output = NativeMethods.AllocateVectorInt16();

			if (NoParamReturnArrayInt16Ptr == nint.Zero) NoParamReturnArrayInt16Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayInt16");
			var NoParamReturnArrayInt16Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayInt16Ptr;
			NoParamReturnArrayInt16Func(__output);

			var output = new short[NativeMethods.GetVectorSizeInt16(__output)];
			NativeMethods.GetVectorDataInt16(__output, output);

			NativeMethods.FreeVectorInt16(__output);

			return output;
		}
		private static nint NoParamReturnArrayInt32Ptr = nint.Zero;
		internal static int[] NoParamReturnArrayInt32()
		{
			var __output = NativeMethods.AllocateVectorInt32();

			if (NoParamReturnArrayInt32Ptr == nint.Zero) NoParamReturnArrayInt32Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayInt32");
			var NoParamReturnArrayInt32Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayInt32Ptr;
			NoParamReturnArrayInt32Func(__output);

			var output = new int[NativeMethods.GetVectorSizeInt32(__output)];
			NativeMethods.GetVectorDataInt32(__output, output);

			NativeMethods.FreeVectorInt32(__output);

			return output;
		}
		private static nint NoParamReturnArrayInt64Ptr = nint.Zero;
		internal static long[] NoParamReturnArrayInt64()
		{
			var __output = NativeMethods.AllocateVectorInt64();

			if (NoParamReturnArrayInt64Ptr == nint.Zero) NoParamReturnArrayInt64Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayInt64");
			var NoParamReturnArrayInt64Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayInt64Ptr;
			NoParamReturnArrayInt64Func(__output);

			var output = new long[NativeMethods.GetVectorSizeInt64(__output)];
			NativeMethods.GetVectorDataInt64(__output, output);

			NativeMethods.FreeVectorInt64(__output);

			return output;
		}
		private static nint NoParamReturnArrayUInt8Ptr = nint.Zero;
		internal static byte[] NoParamReturnArrayUInt8()
		{
			var __output = NativeMethods.AllocateVectorUInt8();

			if (NoParamReturnArrayUInt8Ptr == nint.Zero) NoParamReturnArrayUInt8Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayUInt8");
			var NoParamReturnArrayUInt8Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayUInt8Ptr;
			NoParamReturnArrayUInt8Func(__output);

			var output = new byte[NativeMethods.GetVectorSizeUInt8(__output)];
			NativeMethods.GetVectorDataUInt8(__output, output);

			NativeMethods.FreeVectorUInt8(__output);

			return output;
		}
		private static nint NoParamReturnArrayUInt16Ptr = nint.Zero;
		internal static ushort[] NoParamReturnArrayUInt16()
		{
			var __output = NativeMethods.AllocateVectorUInt16();

			if (NoParamReturnArrayUInt16Ptr == nint.Zero) NoParamReturnArrayUInt16Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayUInt16");
			var NoParamReturnArrayUInt16Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayUInt16Ptr;
			NoParamReturnArrayUInt16Func(__output);

			var output = new ushort[NativeMethods.GetVectorSizeUInt16(__output)];
			NativeMethods.GetVectorDataUInt16(__output, output);

			NativeMethods.FreeVectorUInt16(__output);

			return output;
		}
		private static nint NoParamReturnArrayUInt32Ptr = nint.Zero;
		internal static uint[] NoParamReturnArrayUInt32()
		{
			var __output = NativeMethods.AllocateVectorUInt32();

			if (NoParamReturnArrayUInt32Ptr == nint.Zero) NoParamReturnArrayUInt32Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayUInt32");
			var NoParamReturnArrayUInt32Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayUInt32Ptr;
			NoParamReturnArrayUInt32Func(__output);

			var output = new uint[NativeMethods.GetVectorSizeUInt32(__output)];
			NativeMethods.GetVectorDataUInt32(__output, output);

			NativeMethods.FreeVectorUInt32(__output);

			return output;
		}
		private static nint NoParamReturnArrayUInt64Ptr = nint.Zero;
		internal static ulong[] NoParamReturnArrayUInt64()
		{
			var __output = NativeMethods.AllocateVectorUInt64();

			if (NoParamReturnArrayUInt64Ptr == nint.Zero) NoParamReturnArrayUInt64Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayUInt64");
			var NoParamReturnArrayUInt64Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayUInt64Ptr;
			NoParamReturnArrayUInt64Func(__output);

			var output = new ulong[NativeMethods.GetVectorSizeUInt64(__output)];
			NativeMethods.GetVectorDataUInt64(__output, output);

			NativeMethods.FreeVectorUInt64(__output);

			return output;
		}
		private static nint NoParamReturnArrayPtr64Ptr = nint.Zero;
		internal static nint[] NoParamReturnArrayPtr64()
		{
			var __output = NativeMethods.AllocateVectorIntPtr();

			if (NoParamReturnArrayPtr64Ptr == nint.Zero) NoParamReturnArrayPtr64Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayPtr64");
			var NoParamReturnArrayPtr64Func = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayPtr64Ptr;
			NoParamReturnArrayPtr64Func(__output);

			var output = new nint[NativeMethods.GetVectorSizeIntPtr(__output)];
			NativeMethods.GetVectorDataIntPtr(__output, output);

			NativeMethods.FreeVectorIntPtr(__output);

			return output;
		}
		private static nint NoParamReturnArrayFloatPtr = nint.Zero;
		internal static float[] NoParamReturnArrayFloat()
		{
			var __output = NativeMethods.AllocateVectorFloat();

			if (NoParamReturnArrayFloatPtr == nint.Zero) NoParamReturnArrayFloatPtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayFloat");
			var NoParamReturnArrayFloatFunc = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayFloatPtr;
			NoParamReturnArrayFloatFunc(__output);

			var output = new float[NativeMethods.GetVectorSizeFloat(__output)];
			NativeMethods.GetVectorDataFloat(__output, output);

			NativeMethods.FreeVectorFloat(__output);

			return output;
		}
		private static nint NoParamReturnArrayDoublePtr = nint.Zero;
		internal static double[] NoParamReturnArrayDouble()
		{
			var __output = NativeMethods.AllocateVectorDouble();

			if (NoParamReturnArrayDoublePtr == nint.Zero) NoParamReturnArrayDoublePtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayDouble");
			var NoParamReturnArrayDoubleFunc = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayDoublePtr;
			NoParamReturnArrayDoubleFunc(__output);

			var output = new double[NativeMethods.GetVectorSizeDouble(__output)];
			NativeMethods.GetVectorDataDouble(__output, output);

			NativeMethods.FreeVectorDouble(__output);

			return output;
		}
		private static nint NoParamReturnArrayStringPtr = nint.Zero;
		internal static string[] NoParamReturnArrayString()
		{
			var __output = NativeMethods.AllocateVectorString();

			if (NoParamReturnArrayStringPtr == nint.Zero) NoParamReturnArrayStringPtr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnArrayString");
			var NoParamReturnArrayStringFunc = (delegate* unmanaged[Cdecl]<nint, void>)NoParamReturnArrayStringPtr;
			NoParamReturnArrayStringFunc(__output);

			var output = new string[NativeMethods.GetVectorSizeString(__output)];
			NativeMethods.GetVectorDataString(__output, output);

			NativeMethods.FreeVectorString(__output);

			return output;
		}
		private static nint NoParamReturnVector2Ptr = nint.Zero;
		internal static Vector2 NoParamReturnVector2()
		{
			if (NoParamReturnVector2Ptr == nint.Zero) NoParamReturnVector2Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnVector2");
			var NoParamReturnVector2Func = (delegate* unmanaged[Cdecl]<Vector2>)NoParamReturnVector2Ptr;
			var __result = NoParamReturnVector2Func();
			return __result;
		}
		private static nint NoParamReturnVector3Ptr = nint.Zero;
		internal static Vector3 NoParamReturnVector3()
		{
			if (NoParamReturnVector3Ptr == nint.Zero) NoParamReturnVector3Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnVector3");
			var NoParamReturnVector3Func = (delegate* unmanaged[Cdecl]<Vector3>)NoParamReturnVector3Ptr;
			var __result = NoParamReturnVector3Func();
			return __result;
		}
		private static nint NoParamReturnVector4Ptr = nint.Zero;
		internal static Vector4 NoParamReturnVector4()
		{
			if (NoParamReturnVector4Ptr == nint.Zero) NoParamReturnVector4Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnVector4");
			var NoParamReturnVector4Func = (delegate* unmanaged[Cdecl]<Vector4>)NoParamReturnVector4Ptr;
			var __result = NoParamReturnVector4Func();
			return __result;
		}
		private static nint NoParamReturnMatrix4x4Ptr = nint.Zero;
		internal static Matrix4x4 NoParamReturnMatrix4x4()
		{
			if (NoParamReturnMatrix4x4Ptr == nint.Zero) NoParamReturnMatrix4x4Ptr = NativeMethods.GetMethodPtr("cpp_test.NoParamReturnMatrix4x4");
			var NoParamReturnMatrix4x4Func = (delegate* unmanaged[Cdecl]<Matrix4x4>)NoParamReturnMatrix4x4Ptr;
			var __result = NoParamReturnMatrix4x4Func();
			return __result;
		}
		private static nint Param1Ptr = nint.Zero;
		internal static void Param1(int a)
		{
			if (Param1Ptr == nint.Zero) Param1Ptr = NativeMethods.GetMethodPtr("cpp_test.Param1");
			var Param1Func = (delegate* unmanaged[Cdecl]<int, void>)Param1Ptr;
			Param1Func(a);
		}
		private static nint Param2Ptr = nint.Zero;
		internal static void Param2(int a, float b)
		{
			if (Param2Ptr == nint.Zero) Param2Ptr = NativeMethods.GetMethodPtr("cpp_test.Param2");
			var Param2Func = (delegate* unmanaged[Cdecl]<int, float, void>)Param2Ptr;
			Param2Func(a, b);
		}
		private static nint Param3Ptr = nint.Zero;
		internal static void Param3(int a, float b, double c)
		{
			if (Param3Ptr == nint.Zero) Param3Ptr = NativeMethods.GetMethodPtr("cpp_test.Param3");
			var Param3Func = (delegate* unmanaged[Cdecl]<int, float, double, void>)Param3Ptr;
			Param3Func(a, b, c);
		}
		private static nint Param4Ptr = nint.Zero;
		internal static void Param4(int a, float b, double c, Vector4 d)
		{
			if (Param4Ptr == nint.Zero) Param4Ptr = NativeMethods.GetMethodPtr("cpp_test.Param4");
			var Param4Func = (delegate* unmanaged[Cdecl]<int, float, double, Vector4, void>)Param4Ptr;
			Param4Func(a, b, c, d);
		}
		private static nint Param5Ptr = nint.Zero;
		internal static void Param5(int a, float b, double c, Vector4 d, long[] e)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);

			if (Param5Ptr == nint.Zero) Param5Ptr = NativeMethods.GetMethodPtr("cpp_test.Param5");
			var Param5Func = (delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, void>)Param5Ptr;
			Param5Func(a, b, c, d, __e);

			NativeMethods.DeleteVectorInt64(__e);

		}
		private static nint Param6Ptr = nint.Zero;
		internal static void Param6(int a, float b, double c, Vector4 d, long[] e, char f)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);

			if (Param6Ptr == nint.Zero) Param6Ptr = NativeMethods.GetMethodPtr("cpp_test.Param6");
			var Param6Func = (delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, void>)Param6Ptr;
			Param6Func(a, b, c, d, __e, __f);

			NativeMethods.DeleteVectorInt64(__e);

		}
		private static nint Param7Ptr = nint.Zero;
		internal static void Param7(int a, float b, double c, Vector4 d, long[] e, char f, string g)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			if (Param7Ptr == nint.Zero) Param7Ptr = NativeMethods.GetMethodPtr("cpp_test.Param7");
			var Param7Func = (delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, nint, void>)Param7Ptr;
			Param7Func(a, b, c, d, __e, __f, __g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		private static nint Param8Ptr = nint.Zero;
		internal static void Param8(int a, float b, double c, Vector4 d, long[] e, char f, string g, float h)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			if (Param8Ptr == nint.Zero) Param8Ptr = NativeMethods.GetMethodPtr("cpp_test.Param8");
			var Param8Func = (delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, nint, float, void>)Param8Ptr;
			Param8Func(a, b, c, d, __e, __f, __g, h);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		private static nint Param9Ptr = nint.Zero;
		internal static void Param9(int a, float b, double c, Vector4 d, long[] e, char f, string g, float h, short k)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			if (Param9Ptr == nint.Zero) Param9Ptr = NativeMethods.GetMethodPtr("cpp_test.Param9");
			var Param9Func = (delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, nint, float, short, void>)Param9Ptr;
			Param9Func(a, b, c, d, __e, __f, __g, h, k);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		private static nint Param10Ptr = nint.Zero;
		internal static void Param10(int a, float b, double c, Vector4 d, long[] e, char f, string g, float h, short k, nint l)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			if (Param10Ptr == nint.Zero) Param10Ptr = NativeMethods.GetMethodPtr("cpp_test.Param10");
			var Param10Func = (delegate* unmanaged[Cdecl]<int, float, double, Vector4, nint, sbyte, nint, float, short, nint, void>)Param10Ptr;
			Param10Func(a, b, c, d, __e, __f, __g, h, k, l);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		private static nint ParamRef1Ptr = nint.Zero;
		internal static void ParamRef1(ref int a)
		{
			if (ParamRef1Ptr == nint.Zero) ParamRef1Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef1");
			var ParamRef1Func = (delegate* unmanaged[Cdecl]<ref int, void>)ParamRef1Ptr;
			ParamRef1Func(ref a);
		}
		private static nint ParamRef2Ptr = nint.Zero;
		internal static void ParamRef2(ref int a, ref float b)
		{
			if (ParamRef2Ptr == nint.Zero) ParamRef2Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef2");
			var ParamRef2Func = (delegate* unmanaged[Cdecl]<ref int, ref float, void>)ParamRef2Ptr;
			ParamRef2Func(ref a, ref b);
		}
		private static nint ParamRef3Ptr = nint.Zero;
		internal static void ParamRef3(ref int a, ref float b, ref double c)
		{
			if (ParamRef3Ptr == nint.Zero) ParamRef3Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef3");
			var ParamRef3Func = (delegate* unmanaged[Cdecl]<ref int, ref float, ref double, void>)ParamRef3Ptr;
			ParamRef3Func(ref a, ref b, ref c);
		}
		private static nint ParamRef4Ptr = nint.Zero;
		internal static void ParamRef4(ref int a, ref float b, ref double c, ref Vector4 d)
		{
			if (ParamRef4Ptr == nint.Zero) ParamRef4Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef4");
			var ParamRef4Func = (delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, void>)ParamRef4Ptr;
			ParamRef4Func(ref a, ref b, ref c, ref d);
		}
		private static nint ParamRef5Ptr = nint.Zero;
		internal static void ParamRef5(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);

			if (ParamRef5Ptr == nint.Zero) ParamRef5Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef5");
			var ParamRef5Func = (delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, void>)ParamRef5Ptr;
			ParamRef5Func(ref a, ref b, ref c, ref d, __e);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);

			NativeMethods.DeleteVectorInt64(__e);

		}
		private static nint ParamRef6Ptr = nint.Zero;
		internal static void ParamRef6(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);

			if (ParamRef6Ptr == nint.Zero) ParamRef6Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef6");
			var ParamRef6Func = (delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, void>)ParamRef6Ptr;
			ParamRef6Func(ref a, ref b, ref c, ref d, __e, ref __f);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);

			NativeMethods.DeleteVectorInt64(__e);

		}
		private static nint ParamRef7Ptr = nint.Zero;
		internal static void ParamRef7(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			if (ParamRef7Ptr == nint.Zero) ParamRef7Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef7");
			var ParamRef7Func = (delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, nint, void>)ParamRef7Ptr;
			ParamRef7Func(ref a, ref b, ref c, ref d, __e, ref __f, __g);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		private static nint ParamRef8Ptr = nint.Zero;
		internal static void ParamRef8(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref float h)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			if (ParamRef8Ptr == nint.Zero) ParamRef8Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef8");
			var ParamRef8Func = (delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, nint, ref float, void>)ParamRef8Ptr;
			ParamRef8Func(ref a, ref b, ref c, ref d, __e, ref __f, __g, ref h);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		private static nint ParamRef9Ptr = nint.Zero;
		internal static void ParamRef9(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref float h, ref short k)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			if (ParamRef9Ptr == nint.Zero) ParamRef9Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef9");
			var ParamRef9Func = (delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, nint, ref float, ref short, void>)ParamRef9Ptr;
			ParamRef9Func(ref a, ref b, ref c, ref d, __e, ref __f, __g, ref h, ref k);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		private static nint ParamRef10Ptr = nint.Zero;
		internal static void ParamRef10(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref float h, ref short k, ref nint l)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			if (ParamRef10Ptr == nint.Zero) ParamRef10Ptr = NativeMethods.GetMethodPtr("cpp_test.ParamRef10");
			var ParamRef10Func = (delegate* unmanaged[Cdecl]<ref int, ref float, ref double, ref Vector4, nint, ref sbyte, nint, ref float, ref short, ref nint, void>)ParamRef10Ptr;
			ParamRef10Func(ref a, ref b, ref c, ref d, __e, ref __f, __g, ref h, ref k, ref l);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		private static nint ParamRefVectorsPtr = nint.Zero;
		internal static void ParamRefVectors(ref bool[] p1, ref char[] p2, ref char[] p3, ref sbyte[] p4, ref short[] p5, ref int[] p6, ref long[] p7, ref byte[] p8, ref ushort[] p9, ref uint[] p10, ref ulong[] p11, ref nint[] p12, ref float[] p13, ref double[] p14, ref string[] p15)
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

			if (ParamRefVectorsPtr == nint.Zero) ParamRefVectorsPtr = NativeMethods.GetMethodPtr("cpp_test.ParamRefVectors");
			var ParamRefVectorsFunc = (delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void>)ParamRefVectorsPtr;
			ParamRefVectorsFunc(__p1, __p2, __p3, __p4, __p5, __p6, __p7, __p8, __p9, __p10, __p11, __p12, __p13, __p14, __p15);

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
		private static nint ParamAllPrimitivesPtr = nint.Zero;
		internal static long ParamAllPrimitives(bool p1, char p2, sbyte p3, short p4, int p5, long p6, byte p7, ushort p8, uint p9, ulong p10, nint p11, float p12, double p13)
		{
			var __p2 = Convert.ToUInt16(p2);

			if (ParamAllPrimitivesPtr == nint.Zero) ParamAllPrimitivesPtr = NativeMethods.GetMethodPtr("cpp_test.ParamAllPrimitives");
			var ParamAllPrimitivesFunc = (delegate* unmanaged[Cdecl]<bool, ushort, sbyte, short, int, long, byte, ushort, uint, ulong, nint, float, double, long>)ParamAllPrimitivesPtr;
			var __result = ParamAllPrimitivesFunc(p1, __p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
			return __result;
		}
	}
}
