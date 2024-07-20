using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Plugify;

//generated with https://github.com/untrustedmodders/dotnet-lang-module/blob/main/generator/generator.py from cross_call_master 

namespace cross_call_master
{
	delegate int NoParamReturnFunctionCallbackFunc();

	internal static unsafe class cross_call_master
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

		internal static delegate* <string, void> ReverseReturn = &___ReverseReturn;
		internal static delegate* unmanaged[Cdecl]<nint, void> __ReverseReturn;
		private static void ___ReverseReturn(string returnString)
		{
			var __returnString = NativeMethods.CreateString(returnString);

			__ReverseReturn(__returnString);

			NativeMethods.DeleteString(__returnString);

		}
		internal static delegate* <void> NoParamReturnVoidCallback = &___NoParamReturnVoidCallback;
		internal static delegate* unmanaged[Cdecl]<void> __NoParamReturnVoidCallback;
		private static void ___NoParamReturnVoidCallback()
		{
			__NoParamReturnVoidCallback();
		}
		internal static delegate* <bool> NoParamReturnBoolCallback = &___NoParamReturnBoolCallback;
		internal static delegate* unmanaged[Cdecl]<bool> __NoParamReturnBoolCallback;
		private static bool ___NoParamReturnBoolCallback()
		{
			var __result = __NoParamReturnBoolCallback();
			return __result;
		}
		internal static delegate* <char> NoParamReturnChar8Callback = &___NoParamReturnChar8Callback;
		internal static delegate* unmanaged[Cdecl]<sbyte> __NoParamReturnChar8Callback;
		private static char ___NoParamReturnChar8Callback()
		{
			var __result = __NoParamReturnChar8Callback();
			return (char)__result;
		}
		internal static delegate* <char> NoParamReturnChar16Callback = &___NoParamReturnChar16Callback;
		internal static delegate* unmanaged[Cdecl]<ushort> __NoParamReturnChar16Callback;
		private static char ___NoParamReturnChar16Callback()
		{
			var __result = __NoParamReturnChar16Callback();
			return (char)__result;
		}
		internal static delegate* <sbyte> NoParamReturnInt8Callback = &___NoParamReturnInt8Callback;
		internal static delegate* unmanaged[Cdecl]<sbyte> __NoParamReturnInt8Callback;
		private static sbyte ___NoParamReturnInt8Callback()
		{
			var __result = __NoParamReturnInt8Callback();
			return __result;
		}
		internal static delegate* <short> NoParamReturnInt16Callback = &___NoParamReturnInt16Callback;
		internal static delegate* unmanaged[Cdecl]<short> __NoParamReturnInt16Callback;
		private static short ___NoParamReturnInt16Callback()
		{
			var __result = __NoParamReturnInt16Callback();
			return __result;
		}
		internal static delegate* <int> NoParamReturnInt32Callback = &___NoParamReturnInt32Callback;
		internal static delegate* unmanaged[Cdecl]<int> __NoParamReturnInt32Callback;
		private static int ___NoParamReturnInt32Callback()
		{
			var __result = __NoParamReturnInt32Callback();
			return __result;
		}
		internal static delegate* <long> NoParamReturnInt64Callback = &___NoParamReturnInt64Callback;
		internal static delegate* unmanaged[Cdecl]<long> __NoParamReturnInt64Callback;
		private static long ___NoParamReturnInt64Callback()
		{
			var __result = __NoParamReturnInt64Callback();
			return __result;
		}
		internal static delegate* <byte> NoParamReturnUInt8Callback = &___NoParamReturnUInt8Callback;
		internal static delegate* unmanaged[Cdecl]<byte> __NoParamReturnUInt8Callback;
		private static byte ___NoParamReturnUInt8Callback()
		{
			var __result = __NoParamReturnUInt8Callback();
			return __result;
		}
		internal static delegate* <ushort> NoParamReturnUInt16Callback = &___NoParamReturnUInt16Callback;
		internal static delegate* unmanaged[Cdecl]<ushort> __NoParamReturnUInt16Callback;
		private static ushort ___NoParamReturnUInt16Callback()
		{
			var __result = __NoParamReturnUInt16Callback();
			return __result;
		}
		internal static delegate* <uint> NoParamReturnUInt32Callback = &___NoParamReturnUInt32Callback;
		internal static delegate* unmanaged[Cdecl]<uint> __NoParamReturnUInt32Callback;
		private static uint ___NoParamReturnUInt32Callback()
		{
			var __result = __NoParamReturnUInt32Callback();
			return __result;
		}
		internal static delegate* <ulong> NoParamReturnUInt64Callback = &___NoParamReturnUInt64Callback;
		internal static delegate* unmanaged[Cdecl]<ulong> __NoParamReturnUInt64Callback;
		private static ulong ___NoParamReturnUInt64Callback()
		{
			var __result = __NoParamReturnUInt64Callback();
			return __result;
		}
		internal static delegate* <nint> NoParamReturnPointerCallback = &___NoParamReturnPointerCallback;
		internal static delegate* unmanaged[Cdecl]<nint> __NoParamReturnPointerCallback;
		private static nint ___NoParamReturnPointerCallback()
		{
			var __result = __NoParamReturnPointerCallback();
			return __result;
		}
		internal static delegate* <float> NoParamReturnFloatCallback = &___NoParamReturnFloatCallback;
		internal static delegate* unmanaged[Cdecl]<float> __NoParamReturnFloatCallback;
		private static float ___NoParamReturnFloatCallback()
		{
			var __result = __NoParamReturnFloatCallback();
			return __result;
		}
		internal static delegate* <double> NoParamReturnDoubleCallback = &___NoParamReturnDoubleCallback;
		internal static delegate* unmanaged[Cdecl]<double> __NoParamReturnDoubleCallback;
		private static double ___NoParamReturnDoubleCallback()
		{
			var __result = __NoParamReturnDoubleCallback();
			return __result;
		}
		internal static delegate* <NoParamReturnFunctionCallbackFunc> NoParamReturnFunctionCallback = &___NoParamReturnFunctionCallback;
		internal static delegate* unmanaged[Cdecl]<nint> __NoParamReturnFunctionCallback;
		private static NoParamReturnFunctionCallbackFunc ___NoParamReturnFunctionCallback()
		{
			var __result = __NoParamReturnFunctionCallback();
			return Marshal.GetDelegateForFunctionPointer<NoParamReturnFunctionCallbackFunc>(__result);
		}
		internal static delegate* <string> NoParamReturnStringCallback = &___NoParamReturnStringCallback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnStringCallback;
		private static string ___NoParamReturnStringCallback()
		{
			var __output = NativeMethods.AllocateString();

			__NoParamReturnStringCallback(__output);

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <bool[]> NoParamReturnArrayBoolCallback = &___NoParamReturnArrayBoolCallback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayBoolCallback;
		private static bool[] ___NoParamReturnArrayBoolCallback()
		{
			var __output = NativeMethods.AllocateVectorBool();

			__NoParamReturnArrayBoolCallback(__output);

			var output = new bool[NativeMethods.GetVectorSizeBool(__output)];
			NativeMethods.GetVectorDataBool(__output, output);

			NativeMethods.FreeVectorBool(__output);

			return output;
		}
		internal static delegate* <char[]> NoParamReturnArrayChar8Callback = &___NoParamReturnArrayChar8Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayChar8Callback;
		private static char[] ___NoParamReturnArrayChar8Callback()
		{
			var __output = NativeMethods.AllocateVectorChar8();

			__NoParamReturnArrayChar8Callback(__output);

			var output = new char[NativeMethods.GetVectorSizeChar8(__output)];
			NativeMethods.GetVectorDataChar8(__output, output);

			NativeMethods.FreeVectorChar8(__output);

			return output;
		}
		internal static delegate* <char[]> NoParamReturnArrayChar16Callback = &___NoParamReturnArrayChar16Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayChar16Callback;
		private static char[] ___NoParamReturnArrayChar16Callback()
		{
			var __output = NativeMethods.AllocateVectorChar16();

			__NoParamReturnArrayChar16Callback(__output);

			var output = new char[NativeMethods.GetVectorSizeChar16(__output)];
			NativeMethods.GetVectorDataChar16(__output, output);

			NativeMethods.FreeVectorChar16(__output);

			return output;
		}
		internal static delegate* <sbyte[]> NoParamReturnArrayInt8Callback = &___NoParamReturnArrayInt8Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayInt8Callback;
		private static sbyte[] ___NoParamReturnArrayInt8Callback()
		{
			var __output = NativeMethods.AllocateVectorInt8();

			__NoParamReturnArrayInt8Callback(__output);

			var output = new sbyte[NativeMethods.GetVectorSizeInt8(__output)];
			NativeMethods.GetVectorDataInt8(__output, output);

			NativeMethods.FreeVectorInt8(__output);

			return output;
		}
		internal static delegate* <short[]> NoParamReturnArrayInt16Callback = &___NoParamReturnArrayInt16Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayInt16Callback;
		private static short[] ___NoParamReturnArrayInt16Callback()
		{
			var __output = NativeMethods.AllocateVectorInt16();

			__NoParamReturnArrayInt16Callback(__output);

			var output = new short[NativeMethods.GetVectorSizeInt16(__output)];
			NativeMethods.GetVectorDataInt16(__output, output);

			NativeMethods.FreeVectorInt16(__output);

			return output;
		}
		internal static delegate* <int[]> NoParamReturnArrayInt32Callback = &___NoParamReturnArrayInt32Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayInt32Callback;
		private static int[] ___NoParamReturnArrayInt32Callback()
		{
			var __output = NativeMethods.AllocateVectorInt32();

			__NoParamReturnArrayInt32Callback(__output);

			var output = new int[NativeMethods.GetVectorSizeInt32(__output)];
			NativeMethods.GetVectorDataInt32(__output, output);

			NativeMethods.FreeVectorInt32(__output);

			return output;
		}
		internal static delegate* <long[]> NoParamReturnArrayInt64Callback = &___NoParamReturnArrayInt64Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayInt64Callback;
		private static long[] ___NoParamReturnArrayInt64Callback()
		{
			var __output = NativeMethods.AllocateVectorInt64();

			__NoParamReturnArrayInt64Callback(__output);

			var output = new long[NativeMethods.GetVectorSizeInt64(__output)];
			NativeMethods.GetVectorDataInt64(__output, output);

			NativeMethods.FreeVectorInt64(__output);

			return output;
		}
		internal static delegate* <byte[]> NoParamReturnArrayUInt8Callback = &___NoParamReturnArrayUInt8Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayUInt8Callback;
		private static byte[] ___NoParamReturnArrayUInt8Callback()
		{
			var __output = NativeMethods.AllocateVectorUInt8();

			__NoParamReturnArrayUInt8Callback(__output);

			var output = new byte[NativeMethods.GetVectorSizeUInt8(__output)];
			NativeMethods.GetVectorDataUInt8(__output, output);

			NativeMethods.FreeVectorUInt8(__output);

			return output;
		}
		internal static delegate* <ushort[]> NoParamReturnArrayUInt16Callback = &___NoParamReturnArrayUInt16Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayUInt16Callback;
		private static ushort[] ___NoParamReturnArrayUInt16Callback()
		{
			var __output = NativeMethods.AllocateVectorUInt16();

			__NoParamReturnArrayUInt16Callback(__output);

			var output = new ushort[NativeMethods.GetVectorSizeUInt16(__output)];
			NativeMethods.GetVectorDataUInt16(__output, output);

			NativeMethods.FreeVectorUInt16(__output);

			return output;
		}
		internal static delegate* <uint[]> NoParamReturnArrayUInt32Callback = &___NoParamReturnArrayUInt32Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayUInt32Callback;
		private static uint[] ___NoParamReturnArrayUInt32Callback()
		{
			var __output = NativeMethods.AllocateVectorUInt32();

			__NoParamReturnArrayUInt32Callback(__output);

			var output = new uint[NativeMethods.GetVectorSizeUInt32(__output)];
			NativeMethods.GetVectorDataUInt32(__output, output);

			NativeMethods.FreeVectorUInt32(__output);

			return output;
		}
		internal static delegate* <ulong[]> NoParamReturnArrayUInt64Callback = &___NoParamReturnArrayUInt64Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayUInt64Callback;
		private static ulong[] ___NoParamReturnArrayUInt64Callback()
		{
			var __output = NativeMethods.AllocateVectorUInt64();

			__NoParamReturnArrayUInt64Callback(__output);

			var output = new ulong[NativeMethods.GetVectorSizeUInt64(__output)];
			NativeMethods.GetVectorDataUInt64(__output, output);

			NativeMethods.FreeVectorUInt64(__output);

			return output;
		}
		internal static delegate* <nint[]> NoParamReturnArrayPointerCallback = &___NoParamReturnArrayPointerCallback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayPointerCallback;
		private static nint[] ___NoParamReturnArrayPointerCallback()
		{
			var __output = NativeMethods.AllocateVectorIntPtr();

			__NoParamReturnArrayPointerCallback(__output);

			var output = new nint[NativeMethods.GetVectorSizeIntPtr(__output)];
			NativeMethods.GetVectorDataIntPtr(__output, output);

			NativeMethods.FreeVectorIntPtr(__output);

			return output;
		}
		internal static delegate* <float[]> NoParamReturnArrayFloatCallback = &___NoParamReturnArrayFloatCallback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayFloatCallback;
		private static float[] ___NoParamReturnArrayFloatCallback()
		{
			var __output = NativeMethods.AllocateVectorFloat();

			__NoParamReturnArrayFloatCallback(__output);

			var output = new float[NativeMethods.GetVectorSizeFloat(__output)];
			NativeMethods.GetVectorDataFloat(__output, output);

			NativeMethods.FreeVectorFloat(__output);

			return output;
		}
		internal static delegate* <double[]> NoParamReturnArrayDoubleCallback = &___NoParamReturnArrayDoubleCallback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayDoubleCallback;
		private static double[] ___NoParamReturnArrayDoubleCallback()
		{
			var __output = NativeMethods.AllocateVectorDouble();

			__NoParamReturnArrayDoubleCallback(__output);

			var output = new double[NativeMethods.GetVectorSizeDouble(__output)];
			NativeMethods.GetVectorDataDouble(__output, output);

			NativeMethods.FreeVectorDouble(__output);

			return output;
		}
		internal static delegate* <string[]> NoParamReturnArrayStringCallback = &___NoParamReturnArrayStringCallback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __NoParamReturnArrayStringCallback;
		private static string[] ___NoParamReturnArrayStringCallback()
		{
			var __output = NativeMethods.AllocateVectorString();

			__NoParamReturnArrayStringCallback(__output);

			var output = new string[NativeMethods.GetVectorSizeString(__output)];
			NativeMethods.GetVectorDataString(__output, output);

			NativeMethods.FreeVectorString(__output);

			return output;
		}
		internal static delegate* <Vector2> NoParamReturnVector2Callback = &___NoParamReturnVector2Callback;
		internal static delegate* unmanaged[Cdecl]<Vector2> __NoParamReturnVector2Callback;
		private static Vector2 ___NoParamReturnVector2Callback()
		{
			var __result = __NoParamReturnVector2Callback();
			return __result;
		}
		internal static delegate* <Vector3> NoParamReturnVector3Callback = &___NoParamReturnVector3Callback;
		internal static delegate* unmanaged[Cdecl]<Vector3> __NoParamReturnVector3Callback;
		private static Vector3 ___NoParamReturnVector3Callback()
		{
			var __result = __NoParamReturnVector3Callback();
			return __result;
		}
		internal static delegate* <Vector4> NoParamReturnVector4Callback = &___NoParamReturnVector4Callback;
		internal static delegate* unmanaged[Cdecl]<Vector4> __NoParamReturnVector4Callback;
		private static Vector4 ___NoParamReturnVector4Callback()
		{
			var __result = __NoParamReturnVector4Callback();
			return __result;
		}
		internal static delegate* <Matrix4x4> NoParamReturnMatrix4x4Callback = &___NoParamReturnMatrix4x4Callback;
		internal static delegate* unmanaged[Cdecl]<Matrix4x4> __NoParamReturnMatrix4x4Callback;
		private static Matrix4x4 ___NoParamReturnMatrix4x4Callback()
		{
			var __result = __NoParamReturnMatrix4x4Callback();
			return __result;
		}
		internal static delegate* <int, void> Param1Callback = &___Param1Callback;
		internal static delegate* unmanaged[Cdecl]<int, void> __Param1Callback;
		private static void ___Param1Callback(int a)
		{
			__Param1Callback(a);
		}
		internal static delegate* <int, float, void> Param2Callback = &___Param2Callback;
		internal static delegate* unmanaged[Cdecl]<int, float, void> __Param2Callback;
		private static void ___Param2Callback(int a, float b)
		{
			__Param2Callback(a, b);
		}
		internal static delegate* <int, float, double, void> Param3Callback = &___Param3Callback;
		internal static delegate* unmanaged[Cdecl]<int, float, double, void> __Param3Callback;
		private static void ___Param3Callback(int a, float b, double c)
		{
			__Param3Callback(a, b, c);
		}
		internal static delegate* <int, float, double, Vector4, void> Param4Callback = &___Param4Callback;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4*, void> __Param4Callback;
		private static void ___Param4Callback(int a, float b, double c, Vector4 d)
		{
			__Param4Callback(a, b, c, &d);
		}
		internal static delegate* <int, float, double, Vector4, long[], void> Param5Callback = &___Param5Callback;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4*, nint, void> __Param5Callback;
		private static void ___Param5Callback(int a, float b, double c, Vector4 d, long[] e)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);

			__Param5Callback(a, b, c, &d, __e);

			NativeMethods.DeleteVectorInt64(__e);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, void> Param6Callback = &___Param6Callback;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4*, nint, sbyte, void> __Param6Callback;
		private static void ___Param6Callback(int a, float b, double c, Vector4 d, long[] e, char f)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);

			__Param6Callback(a, b, c, &d, __e, __f);

			NativeMethods.DeleteVectorInt64(__e);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, string, void> Param7Callback = &___Param7Callback;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4*, nint, sbyte, nint, void> __Param7Callback;
		private static void ___Param7Callback(int a, float b, double c, Vector4 d, long[] e, char f, string g)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__Param7Callback(a, b, c, &d, __e, __f, __g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, string, char, void> Param8Callback = &___Param8Callback;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4*, nint, sbyte, nint, ushort, void> __Param8Callback;
		private static void ___Param8Callback(int a, float b, double c, Vector4 d, long[] e, char f, string g, char h)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);
			var __h = Convert.ToUInt16(h);

			__Param8Callback(a, b, c, &d, __e, __f, __g, __h);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, string, char, short, void> Param9Callback = &___Param9Callback;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4*, nint, sbyte, nint, ushort, short, void> __Param9Callback;
		private static void ___Param9Callback(int a, float b, double c, Vector4 d, long[] e, char f, string g, char h, short k)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);
			var __h = Convert.ToUInt16(h);

			__Param9Callback(a, b, c, &d, __e, __f, __g, __h, k);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <int, float, double, Vector4, long[], char, string, char, short, nint, void> Param10Callback = &___Param10Callback;
		internal static delegate* unmanaged[Cdecl]<int, float, double, Vector4*, nint, sbyte, nint, ushort, short, nint, void> __Param10Callback;
		private static void ___Param10Callback(int a, float b, double c, Vector4 d, long[] e, char f, string g, char h, short k, nint l)
		{
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);
			var __h = Convert.ToUInt16(h);

			__Param10Callback(a, b, c, &d, __e, __f, __g, __h, k, l);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);

		}
		internal static delegate* <ref int, void> ParamRef1Callback = &___ParamRef1Callback;
		internal static delegate* unmanaged[Cdecl]<int*, void> __ParamRef1Callback;
		private static void ___ParamRef1Callback(ref int a)
		{
			fixed(int* __a = &a) {

			__ParamRef1Callback(__a);

			}
		}
		internal static delegate* <ref int, ref float, void> ParamRef2Callback = &___ParamRef2Callback;
		internal static delegate* unmanaged[Cdecl]<int*, float*, void> __ParamRef2Callback;
		private static void ___ParamRef2Callback(ref int a, ref float b)
		{
			fixed(int* __a = &a) {
			fixed(float* __b = &b) {

			__ParamRef2Callback(__a, __b);

			}
			}
		}
		internal static delegate* <ref int, ref float, ref double, void> ParamRef3Callback = &___ParamRef3Callback;
		internal static delegate* unmanaged[Cdecl]<int*, float*, double*, void> __ParamRef3Callback;
		private static void ___ParamRef3Callback(ref int a, ref float b, ref double c)
		{
			fixed(int* __a = &a) {
			fixed(float* __b = &b) {
			fixed(double* __c = &c) {

			__ParamRef3Callback(__a, __b, __c);

			}
			}
			}
		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, void> ParamRef4Callback = &___ParamRef4Callback;
		internal static delegate* unmanaged[Cdecl]<int*, float*, double*, Vector4*, void> __ParamRef4Callback;
		private static void ___ParamRef4Callback(ref int a, ref float b, ref double c, ref Vector4 d)
		{
			fixed(int* __a = &a) {
			fixed(float* __b = &b) {
			fixed(double* __c = &c) {
			fixed(Vector4* __d = &d) {

			__ParamRef4Callback(__a, __b, __c, __d);

			}
			}
			}
			}
		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], void> ParamRef5Callback = &___ParamRef5Callback;
		internal static delegate* unmanaged[Cdecl]<int*, float*, double*, Vector4*, nint, void> __ParamRef5Callback;
		private static void ___ParamRef5Callback(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e)
		{
			fixed(int* __a = &a) {
			fixed(float* __b = &b) {
			fixed(double* __c = &c) {
			fixed(Vector4* __d = &d) {
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);

			__ParamRef5Callback(__a, __b, __c, __d, __e);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);

			NativeMethods.DeleteVectorInt64(__e);


			}
			}
			}
			}
		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, void> ParamRef6Callback = &___ParamRef6Callback;
		internal static delegate* unmanaged[Cdecl]<int*, float*, double*, Vector4*, nint, sbyte*, void> __ParamRef6Callback;
		private static void ___ParamRef6Callback(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f)
		{
			fixed(int* __a = &a) {
			fixed(float* __b = &b) {
			fixed(double* __c = &c) {
			fixed(Vector4* __d = &d) {
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);

			__ParamRef6Callback(__a, __b, __c, __d, __e, &__f);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);

			NativeMethods.DeleteVectorInt64(__e);


			}
			}
			}
			}
		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, ref string, void> ParamRef7Callback = &___ParamRef7Callback;
		internal static delegate* unmanaged[Cdecl]<int*, float*, double*, Vector4*, nint, sbyte*, nint, void> __ParamRef7Callback;
		private static void ___ParamRef7Callback(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g)
		{
			fixed(int* __a = &a) {
			fixed(float* __b = &b) {
			fixed(double* __c = &c) {
			fixed(Vector4* __d = &d) {
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);

			__ParamRef7Callback(__a, __b, __c, __d, __e, &__f, __g);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);


			}
			}
			}
			}
		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, ref string, ref char, void> ParamRef8Callback = &___ParamRef8Callback;
		internal static delegate* unmanaged[Cdecl]<int*, float*, double*, Vector4*, nint, sbyte*, nint, ushort*, void> __ParamRef8Callback;
		private static void ___ParamRef8Callback(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref char h)
		{
			fixed(int* __a = &a) {
			fixed(float* __b = &b) {
			fixed(double* __c = &c) {
			fixed(Vector4* __d = &d) {
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);
			var __h = Convert.ToUInt16(h);

			__ParamRef8Callback(__a, __b, __c, __d, __e, &__f, __g, &__h);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);
			h = Convert.ToChar(__h);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);


			}
			}
			}
			}
		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, ref string, ref char, ref short, void> ParamRef9Callback = &___ParamRef9Callback;
		internal static delegate* unmanaged[Cdecl]<int*, float*, double*, Vector4*, nint, sbyte*, nint, ushort*, short*, void> __ParamRef9Callback;
		private static void ___ParamRef9Callback(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref char h, ref short k)
		{
			fixed(int* __a = &a) {
			fixed(float* __b = &b) {
			fixed(double* __c = &c) {
			fixed(Vector4* __d = &d) {
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);
			var __h = Convert.ToUInt16(h);
			fixed(short* __k = &k) {

			__ParamRef9Callback(__a, __b, __c, __d, __e, &__f, __g, &__h, __k);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);
			h = Convert.ToChar(__h);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);


			}
			}
			}
			}
			}
		}
		internal static delegate* <ref int, ref float, ref double, ref Vector4, ref long[], ref char, ref string, ref char, ref short, ref nint, void> ParamRef10Callback = &___ParamRef10Callback;
		internal static delegate* unmanaged[Cdecl]<int*, float*, double*, Vector4*, nint, sbyte*, nint, ushort*, short*, nint*, void> __ParamRef10Callback;
		private static void ___ParamRef10Callback(ref int a, ref float b, ref double c, ref Vector4 d, ref long[] e, ref char f, ref string g, ref char h, ref short k, ref nint l)
		{
			fixed(int* __a = &a) {
			fixed(float* __b = &b) {
			fixed(double* __c = &c) {
			fixed(Vector4* __d = &d) {
			var __e = NativeMethods.CreateVectorInt64(e, e.Length);
			var __f = Convert.ToSByte(f);
			var __g = NativeMethods.CreateString(g);
			var __h = Convert.ToUInt16(h);
			fixed(short* __k = &k) {
			fixed(nint* __l = &l) {

			__ParamRef10Callback(__a, __b, __c, __d, __e, &__f, __g, &__h, __k, __l);

			Array.Resize(ref e, NativeMethods.GetVectorSizeInt64(__e));
			NativeMethods.GetVectorDataInt64(__e, e);
			f = Convert.ToChar(__f);
			g = NativeMethods.GetStringData(__g);
			h = Convert.ToChar(__h);

			NativeMethods.DeleteVectorInt64(__e);
			NativeMethods.DeleteString(__g);


			}
			}
			}
			}
			}
			}
		}
		internal static delegate* <ref bool[], ref char[], ref char[], ref sbyte[], ref short[], ref int[], ref long[], ref byte[], ref ushort[], ref uint[], ref ulong[], ref nint[], ref float[], ref double[], ref string[], void> ParamRefVectorsCallback = &___ParamRefVectorsCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void> __ParamRefVectorsCallback;
		private static void ___ParamRefVectorsCallback(ref bool[] p1, ref char[] p2, ref char[] p3, ref sbyte[] p4, ref short[] p5, ref int[] p6, ref long[] p7, ref byte[] p8, ref ushort[] p9, ref uint[] p10, ref ulong[] p11, ref nint[] p12, ref float[] p13, ref double[] p14, ref string[] p15)
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

			__ParamRefVectorsCallback(__p1, __p2, __p3, __p4, __p5, __p6, __p7, __p8, __p9, __p10, __p11, __p12, __p13, __p14, __p15);

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
		internal static delegate* <bool, char, char, sbyte, short, int, long, byte, ushort, uint, ulong, nint, float, double, long> ParamAllPrimitivesCallback = &___ParamAllPrimitivesCallback;
		internal static delegate* unmanaged[Cdecl]<bool, sbyte, ushort, sbyte, short, int, long, byte, ushort, uint, ulong, nint, float, double, long> __ParamAllPrimitivesCallback;
		private static long ___ParamAllPrimitivesCallback(bool p1, char p2, char p3, sbyte p4, short p5, int p6, long p7, byte p8, ushort p9, uint p10, ulong p11, nint p12, float p13, double p14)
		{
			var __p2 = Convert.ToSByte(p2);
			var __p3 = Convert.ToUInt16(p3);

			var __result = __ParamAllPrimitivesCallback(p1, __p2, __p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
			return __result;
		}
	}
}
