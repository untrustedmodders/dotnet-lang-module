using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Plugify;

//generated with https://github.com/untrustedmodders/plugify-module-dotnet/blob/main/generator/generator.py from cross_call_master 

namespace cross_call_master
{
#pragma warning disable CS0649
	public delegate int NoParamReturnFunctionCallbackFunc();
	public delegate void FuncVoid();
	public delegate bool FuncBool();
	public delegate byte FuncBoolWrapper();
	[return: CharSet(CharSet.Ansi)]
	public delegate char FuncChar8();
	public delegate sbyte FuncChar8Wrapper();
	public delegate char FuncChar16();
	public delegate ushort FuncChar16Wrapper();
	public delegate sbyte FuncInt8();
	public delegate short FuncInt16();
	public delegate int FuncInt32();
	public delegate long FuncInt64();
	public delegate byte FuncUInt8();
	public delegate ushort FuncUInt16();
	public delegate uint FuncUInt32();
	public delegate ulong FuncUInt64();
	public delegate nint FuncPtr();
	public delegate float FuncFloat();
	public delegate double FuncDouble();
	public delegate string FuncString();
	public delegate nint FuncStringWrapper(nint __output);
	public delegate nint FuncFunction();
	public delegate bool[] FuncBoolVector();
	public delegate nint FuncBoolVectorWrapper(nint __output);
	[return: CharSet(CharSet.Ansi)]
	public delegate char[] FuncChar8Vector();
	public delegate nint FuncChar8VectorWrapper(nint __output);
	public delegate char[] FuncChar16Vector();
	public delegate nint FuncChar16VectorWrapper(nint __output);
	public delegate sbyte[] FuncInt8Vector();
	public delegate nint FuncInt8VectorWrapper(nint __output);
	public delegate short[] FuncInt16Vector();
	public delegate nint FuncInt16VectorWrapper(nint __output);
	public delegate int[] FuncInt32Vector();
	public delegate nint FuncInt32VectorWrapper(nint __output);
	public delegate long[] FuncInt64Vector();
	public delegate nint FuncInt64VectorWrapper(nint __output);
	public delegate byte[] FuncUInt8Vector();
	public delegate nint FuncUInt8VectorWrapper(nint __output);
	public delegate ushort[] FuncUInt16Vector();
	public delegate nint FuncUInt16VectorWrapper(nint __output);
	public delegate uint[] FuncUInt32Vector();
	public delegate nint FuncUInt32VectorWrapper(nint __output);
	public delegate ulong[] FuncUInt64Vector();
	public delegate nint FuncUInt64VectorWrapper(nint __output);
	public delegate nint[] FuncPtrVector();
	public delegate nint FuncPtrVectorWrapper(nint __output);
	public delegate float[] FuncFloatVector();
	public delegate nint FuncFloatVectorWrapper(nint __output);
	public delegate string[] FuncStringVector();
	public delegate nint FuncStringVectorWrapper(nint __output);
	public delegate double[] FuncDoubleVector();
	public delegate nint FuncDoubleVectorWrapper(nint __output);
	public delegate Vector2 FuncVec2();
	public delegate Vector3 FuncVec3();
	public delegate Vector4 FuncVec4();
	public delegate Matrix4x4 FuncMat4x4();
	public delegate int Func1(ref Vector3 a);
	[return: CharSet(CharSet.Ansi)]
	public delegate char Func2(float a, long b);
	public delegate sbyte Func2Wrapper(float a, long b);
	public delegate void Func3(nint a, ref Vector4 b, string c);
	public delegate void Func3Wrapper(nint a, ref Vector4 b, nint c);
	public delegate Vector4 Func4(bool a, int b, char c, ref Matrix4x4 d);
	public delegate bool Func5(sbyte a, ref Vector2 b, nint c, double d, ulong[] e);
	public delegate byte Func5Wrapper(sbyte a, ref Vector2 b, nint c, double d, nint e);
	public delegate long Func6(string a, float b, float[] c, short d, byte[] e, nint f);
	public delegate long Func6Wrapper(nint a, float b, nint c, short d, nint e, nint f);
	public delegate double Func7([CharSet(CharSet.Ansi)] char[] vecC, ushort u16, char ch16, uint[] vecU32, ref Vector4 vec4, bool b, ulong u64);
	public delegate double Func7Wrapper(nint vecC, ushort u16, ushort ch16, nint vecU32, ref Vector4 vec4, byte b, ulong u64);
	public delegate Matrix4x4 Func8(ref Vector3 vec3, uint[] vecU32, short i16, bool b, ref Vector4 vec4, char[] vecC16, char ch16, int i32);
	public delegate Matrix4x4 Func8Wrapper(ref Vector3 vec3, nint vecU32, short i16, byte b, ref Vector4 vec4, nint vecC16, ushort ch16, int i32);
	public delegate void Func9(float f, ref Vector2 vec2, sbyte[] vecI8, ulong u64, bool b, string str, ref Vector4 vec4, short i16, nint ptr);
	public delegate void Func9Wrapper(float f, ref Vector2 vec2, nint vecI8, ulong u64, byte b, nint str, ref Vector4 vec4, short i16, nint ptr);
	public delegate uint Func10(ref Vector4 vec4, ref Matrix4x4 mat, uint[] vecU32, ulong u64, [CharSet(CharSet.Ansi)] char[] vecC, int i32, bool b, ref Vector2 vec2, long i64, double d);
	public delegate uint Func10Wrapper(ref Vector4 vec4, ref Matrix4x4 mat, nint vecU32, ulong u64, nint vecC, int i32, byte b, ref Vector2 vec2, long i64, double d);
	public delegate nint Func11(bool[] vecB, char ch16, byte u8, double d, ref Vector3 vec3, sbyte[] vecI8, long i64, ushort u16, float f, ref Vector2 vec2, uint u32);
	public delegate nint Func11Wrapper(nint vecB, ushort ch16, byte u8, double d, ref Vector3 vec3, nint vecI8, long i64, ushort u16, float f, ref Vector2 vec2, uint u32);
	public delegate bool Func12(nint ptr, double[] vecD, uint u32, double d, bool b, int i32, sbyte i8, ulong u64, float f, nint[] vecPtr, long i64, [CharSet(CharSet.Ansi)] char ch);
	public delegate byte Func12Wrapper(nint ptr, nint vecD, uint u32, double d, byte b, int i32, sbyte i8, ulong u64, float f, nint vecPtr, long i64, sbyte ch);
	public delegate string Func13(long i64, [CharSet(CharSet.Ansi)] char[] vecC, ushort d, float f, bool[] b, ref Vector4 vec4, string str, int int32, ref Vector3 vec3, nint ptr, ref Vector2 vec2, byte[] arr, short i16);
	public delegate nint Func13Wrapper(nint __output, long i64, nint vecC, ushort d, float f, nint b, ref Vector4 vec4, nint str, int int32, ref Vector3 vec3, nint ptr, ref Vector2 vec2, nint arr, short i16);
	public delegate string[] Func14([CharSet(CharSet.Ansi)] char[] vecC, uint[] vecU32, ref Matrix4x4 mat, bool b, char ch16, int i32, float[] vecF, ushort u16, byte[] vecU8, sbyte i8, ref Vector3 vec3, ref Vector4 vec4, double d, nint ptr);
	public delegate nint Func14Wrapper(nint __output, nint vecC, nint vecU32, ref Matrix4x4 mat, byte b, ushort ch16, int i32, nint vecF, ushort u16, nint vecU8, sbyte i8, ref Vector3 vec3, ref Vector4 vec4, double d, nint ptr);
	public delegate short Func15(short[] vecI16, ref Matrix4x4 mat, ref Vector4 vec4, nint ptr, ulong u64, uint[] vecU32, bool b, float f, char[] vecC16, byte u8, int i32, ref Vector2 vec2, ushort u16, double d, byte[] vecU8);
	public delegate short Func15Wrapper(nint vecI16, ref Matrix4x4 mat, ref Vector4 vec4, nint ptr, ulong u64, nint vecU32, byte b, float f, nint vecC16, byte u8, int i32, ref Vector2 vec2, ushort u16, double d, nint vecU8);
	public delegate nint Func16(bool[] vecB, short i16, sbyte[] vecI8, ref Vector4 vec4, ref Matrix4x4 mat, ref Vector2 vec2, ulong[] vecU64, [CharSet(CharSet.Ansi)] char[] vecC, string str, long i64, uint[] vecU32, ref Vector3 vec3, float f, double d, sbyte i8, ushort u16);
	public delegate nint Func16Wrapper(nint vecB, short i16, nint vecI8, ref Vector4 vec4, ref Matrix4x4 mat, ref Vector2 vec2, nint vecU64, nint vecC, nint str, long i64, nint vecU32, ref Vector3 vec3, float f, double d, sbyte i8, ushort u16);
	public delegate void Func17(ref int i32);
	public delegate Vector2 Func18(ref sbyte i8, ref short i16);
	public delegate void Func19(ref uint u32, ref Vector3 vec3, ref uint[] vecU32);
	public delegate void Func19Wrapper(ref uint u32, ref Vector3 vec3, nint vecU32);
	public delegate int Func20(ref char ch16, ref Vector4 vec4, ref ulong[] vecU64, [CharSet(CharSet.Ansi)] ref char ch);
	public delegate int Func20Wrapper(ref ushort ch16, ref Vector4 vec4, nint vecU64, ref sbyte ch);
	public delegate float Func21(ref Matrix4x4 mat, ref int[] vecI32, ref Vector2 vec2, ref bool b, ref double extraParam);
	public delegate float Func21Wrapper(ref Matrix4x4 mat, nint vecI32, ref Vector2 vec2, ref byte b, ref double extraParam);
	public delegate ulong Func22(ref nint ptr64Ref, ref uint uint32Ref, ref double[] vectorDoubleRef, ref short int16Ref, ref string plgStringRef, ref Vector4 plgVector4Ref);
	public delegate ulong Func22Wrapper(ref nint ptr64Ref, ref uint uint32Ref, nint vectorDoubleRef, ref short int16Ref, nint plgStringRef, ref Vector4 plgVector4Ref);
	public delegate void Func23(ref ulong uint64Ref, ref Vector2 plgVector2Ref, ref short[] vectorInt16Ref, ref char char16Ref, ref float floatRef, ref sbyte int8Ref, ref byte[] vectorUInt8Ref);
	public delegate void Func23Wrapper(ref ulong uint64Ref, ref Vector2 plgVector2Ref, nint vectorInt16Ref, ref ushort char16Ref, ref float floatRef, ref sbyte int8Ref, nint vectorUInt8Ref);
	public delegate Matrix4x4 Func24([CharSet(CharSet.Ansi)] ref char[] vectorCharRef, ref long int64Ref, ref byte[] vectorUInt8Ref, ref Vector4 plgVector4Ref, ref ulong uint64Ref, ref nint[] vectorptr64Ref, ref double doubleRef, ref nint[] vectorptr64Ref2);
	public delegate Matrix4x4 Func24Wrapper(nint vectorCharRef, ref long int64Ref, nint vectorUInt8Ref, ref Vector4 plgVector4Ref, ref ulong uint64Ref, nint vectorptr64Ref, ref double doubleRef, nint vectorptr64Ref2);
	public delegate double Func25(ref int int32Ref, ref nint[] vectorptr64Ref, ref bool boolRef, ref byte uint8Ref, ref string plgStringRef, ref Vector3 plgVector3Ref, ref long int64Ref, ref Vector4 plgVector4Ref, ref ushort uint16Ref);
	public delegate double Func25Wrapper(ref int int32Ref, nint vectorptr64Ref, ref byte boolRef, ref byte uint8Ref, nint plgStringRef, ref Vector3 plgVector3Ref, ref long int64Ref, ref Vector4 plgVector4Ref, ref ushort uint16Ref);
	[return: CharSet(CharSet.Ansi)]
	public delegate char Func26(ref char char16Ref, ref Vector2 plgVector2Ref, ref Matrix4x4 plgMatrix4x4Ref, ref float[] vectorFloatRef, ref short int16Ref, ref ulong uint64Ref, ref uint uint32Ref, ref ushort[] vectorUInt16Ref, ref nint ptr64Ref, ref bool boolRef);
	public delegate sbyte Func26Wrapper(ref ushort char16Ref, ref Vector2 plgVector2Ref, ref Matrix4x4 plgMatrix4x4Ref, nint vectorFloatRef, ref short int16Ref, ref ulong uint64Ref, ref uint uint32Ref, nint vectorUInt16Ref, ref nint ptr64Ref, ref byte boolRef);
	public delegate byte Func27(ref float floatRef, ref Vector3 plgVector3Ref, ref nint ptr64Ref, ref Vector2 plgVector2Ref, ref short[] vectorInt16Ref, ref Matrix4x4 plgMatrix4x4Ref, ref bool boolRef, ref Vector4 plgVector4Ref, ref sbyte int8Ref, ref int int32Ref, ref byte[] vectorUInt8Ref);
	public delegate byte Func27Wrapper(ref float floatRef, ref Vector3 plgVector3Ref, ref nint ptr64Ref, ref Vector2 plgVector2Ref, nint vectorInt16Ref, ref Matrix4x4 plgMatrix4x4Ref, ref byte boolRef, ref Vector4 plgVector4Ref, ref sbyte int8Ref, ref int int32Ref, nint vectorUInt8Ref);
	public delegate string Func28(ref nint ptr64Ref, ref ushort uint16Ref, ref uint[] vectorUInt32Ref, ref Matrix4x4 plgMatrix4x4Ref, ref float floatRef, ref Vector4 plgVector4Ref, ref string plgStringRef, ref ulong[] vectorUInt64Ref, ref long int64Ref, ref bool boolRef, ref Vector3 plgVector3Ref, ref float[] vectorFloatRef);
	public delegate nint Func28Wrapper(nint __output, ref nint ptr64Ref, ref ushort uint16Ref, nint vectorUInt32Ref, ref Matrix4x4 plgMatrix4x4Ref, ref float floatRef, ref Vector4 plgVector4Ref, nint plgStringRef, nint vectorUInt64Ref, ref long int64Ref, ref byte boolRef, ref Vector3 plgVector3Ref, nint vectorFloatRef);
	public delegate string[] Func29(ref Vector4 plgVector4Ref, ref int int32Ref, ref sbyte[] vectorInt8Ref, ref double doubleRef, ref bool boolRef, ref sbyte int8Ref, ref ushort[] vectorUInt16Ref, ref float floatRef, ref string plgStringRef, ref Matrix4x4 plgMatrix4x4Ref, ref ulong uint64Ref, ref Vector3 plgVector3Ref, ref long[] vectorInt64Ref);
	public delegate nint Func29Wrapper(nint __output, ref Vector4 plgVector4Ref, ref int int32Ref, nint vectorInt8Ref, ref double doubleRef, ref byte boolRef, ref sbyte int8Ref, nint vectorUInt16Ref, ref float floatRef, nint plgStringRef, ref Matrix4x4 plgMatrix4x4Ref, ref ulong uint64Ref, ref Vector3 plgVector3Ref, nint vectorInt64Ref);
	public delegate int Func30(ref nint ptr64Ref, ref Vector4 plgVector4Ref, ref long int64Ref, ref uint[] vectorUInt32Ref, ref bool boolRef, ref string plgStringRef, ref Vector3 plgVector3Ref, ref byte[] vectorUInt8Ref, ref float floatRef, ref Vector2 plgVector2Ref, ref Matrix4x4 plgMatrix4x4Ref, ref sbyte int8Ref, ref float[] vectorFloatRef, ref double doubleRef);
	public delegate int Func30Wrapper(ref nint ptr64Ref, ref Vector4 plgVector4Ref, ref long int64Ref, nint vectorUInt32Ref, ref byte boolRef, nint plgStringRef, ref Vector3 plgVector3Ref, nint vectorUInt8Ref, ref float floatRef, ref Vector2 plgVector2Ref, ref Matrix4x4 plgMatrix4x4Ref, ref sbyte int8Ref, nint vectorFloatRef, ref double doubleRef);
	public delegate Vector3 Func31([CharSet(CharSet.Ansi)] ref char charRef, ref uint uint32Ref, ref ulong[] vectorUInt64Ref, ref Vector4 plgVector4Ref, ref string plgStringRef, ref bool boolRef, ref long int64Ref, ref Vector2 vec2Ref, ref sbyte int8Ref, ref ushort uint16Ref, ref short[] vectorInt16Ref, ref Matrix4x4 mat4x4Ref, ref Vector3 vec3Ref, ref float floatRef, ref double[] vectorDoubleRef);
	public delegate Vector3 Func31Wrapper(ref sbyte charRef, ref uint uint32Ref, nint vectorUInt64Ref, ref Vector4 plgVector4Ref, nint plgStringRef, ref byte boolRef, ref long int64Ref, ref Vector2 vec2Ref, ref sbyte int8Ref, ref ushort uint16Ref, nint vectorInt16Ref, ref Matrix4x4 mat4x4Ref, ref Vector3 vec3Ref, ref float floatRef, nint vectorDoubleRef);
	public delegate double Func32(ref int p1, ref ushort p2, ref sbyte[] p3, ref Vector4 p4, ref nint p5, ref uint[] p6, ref Matrix4x4 p7, ref ulong p8, ref string p9, ref long p10, ref Vector2 p11, ref sbyte[] p12, ref bool p13, ref Vector3 p14, ref byte p15, ref char[] p16);
	public delegate double Func32Wrapper(ref int p1, ref ushort p2, nint p3, ref Vector4 p4, ref nint p5, nint p6, ref Matrix4x4 p7, ref ulong p8, nint p9, ref long p10, ref Vector2 p11, nint p12, ref byte p13, ref Vector3 p14, ref byte p15, nint p16);

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
		internal static delegate* unmanaged[Cdecl]<byte> __NoParamReturnBoolCallback;
		private static bool ___NoParamReturnBoolCallback()
		{
			var __result = __NoParamReturnBoolCallback();
			return __result == 1;
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
		internal static delegate* unmanaged[Cdecl]<byte, sbyte, ushort, sbyte, short, int, long, byte, ushort, uint, ulong, nint, float, double, long> __ParamAllPrimitivesCallback;
		private static long ___ParamAllPrimitivesCallback(bool p1, char p2, char p3, sbyte p4, short p5, int p6, long p7, byte p8, ushort p9, uint p10, ulong p11, nint p12, float p13, double p14)
		{
			var __p1 = Convert.ToByte(p1);
			var __p2 = Convert.ToSByte(p2);
			var __p3 = Convert.ToUInt16(p3);

			var __result = __ParamAllPrimitivesCallback(__p1, __p2, __p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
			return __result;
		}
		internal static delegate* <FuncVoid, void> CallFuncVoidCallback = &___CallFuncVoidCallback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __CallFuncVoidCallback;
		private static void ___CallFuncVoidCallback(FuncVoid func)
		{
			__CallFuncVoidCallback(Marshal.GetFunctionPointerForDelegate(func));
		}
		internal static delegate* <FuncBool, bool> CallFuncBoolCallback = &___CallFuncBoolCallback;
		internal static delegate* unmanaged[Cdecl]<nint, byte> __CallFuncBoolCallback;
		private static bool ___CallFuncBoolCallback(FuncBool func)
		{
			var CallFuncBoolCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncBoolWrapper(() => {
				var __result__ = func();
				return Convert.ToByte(__result__);
			}));
			var __result = __CallFuncBoolCallback(Marshal.GetFunctionPointerForDelegate(CallFuncBoolCallback_func));
			return __result == 1;
		}
		internal static delegate* <FuncChar8, char> CallFuncChar8Callback = &___CallFuncChar8Callback;
		internal static delegate* unmanaged[Cdecl]<nint, sbyte> __CallFuncChar8Callback;
		private static char ___CallFuncChar8Callback(FuncChar8 func)
		{
			var CallFuncChar8Callback_func = s_DelegateHolder.GetOrAdd(func, new FuncChar8Wrapper(() => {
				var __result__ = func();
				return Convert.ToSByte(__result__);
			}));
			var __result = __CallFuncChar8Callback(Marshal.GetFunctionPointerForDelegate(CallFuncChar8Callback_func));
			return (char)__result;
		}
		internal static delegate* <FuncChar16, char> CallFuncChar16Callback = &___CallFuncChar16Callback;
		internal static delegate* unmanaged[Cdecl]<nint, ushort> __CallFuncChar16Callback;
		private static char ___CallFuncChar16Callback(FuncChar16 func)
		{
			var CallFuncChar16Callback_func = s_DelegateHolder.GetOrAdd(func, new FuncChar16Wrapper(() => {
				var __result__ = func();
				return __result__;
			}));
			var __result = __CallFuncChar16Callback(Marshal.GetFunctionPointerForDelegate(CallFuncChar16Callback_func));
			return (char)__result;
		}
		internal static delegate* <FuncInt8, sbyte> CallFuncInt8Callback = &___CallFuncInt8Callback;
		internal static delegate* unmanaged[Cdecl]<nint, sbyte> __CallFuncInt8Callback;
		private static sbyte ___CallFuncInt8Callback(FuncInt8 func)
		{
			var __result = __CallFuncInt8Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncInt16, short> CallFuncInt16Callback = &___CallFuncInt16Callback;
		internal static delegate* unmanaged[Cdecl]<nint, short> __CallFuncInt16Callback;
		private static short ___CallFuncInt16Callback(FuncInt16 func)
		{
			var __result = __CallFuncInt16Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncInt32, int> CallFuncInt32Callback = &___CallFuncInt32Callback;
		internal static delegate* unmanaged[Cdecl]<nint, int> __CallFuncInt32Callback;
		private static int ___CallFuncInt32Callback(FuncInt32 func)
		{
			var __result = __CallFuncInt32Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncInt64, long> CallFuncInt64Callback = &___CallFuncInt64Callback;
		internal static delegate* unmanaged[Cdecl]<nint, long> __CallFuncInt64Callback;
		private static long ___CallFuncInt64Callback(FuncInt64 func)
		{
			var __result = __CallFuncInt64Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncUInt8, byte> CallFuncUInt8Callback = &___CallFuncUInt8Callback;
		internal static delegate* unmanaged[Cdecl]<nint, byte> __CallFuncUInt8Callback;
		private static byte ___CallFuncUInt8Callback(FuncUInt8 func)
		{
			var __result = __CallFuncUInt8Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncUInt16, ushort> CallFuncUInt16Callback = &___CallFuncUInt16Callback;
		internal static delegate* unmanaged[Cdecl]<nint, ushort> __CallFuncUInt16Callback;
		private static ushort ___CallFuncUInt16Callback(FuncUInt16 func)
		{
			var __result = __CallFuncUInt16Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncUInt32, uint> CallFuncUInt32Callback = &___CallFuncUInt32Callback;
		internal static delegate* unmanaged[Cdecl]<nint, uint> __CallFuncUInt32Callback;
		private static uint ___CallFuncUInt32Callback(FuncUInt32 func)
		{
			var __result = __CallFuncUInt32Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncUInt64, ulong> CallFuncUInt64Callback = &___CallFuncUInt64Callback;
		internal static delegate* unmanaged[Cdecl]<nint, ulong> __CallFuncUInt64Callback;
		private static ulong ___CallFuncUInt64Callback(FuncUInt64 func)
		{
			var __result = __CallFuncUInt64Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncPtr, nint> CallFuncPtrCallback = &___CallFuncPtrCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint> __CallFuncPtrCallback;
		private static nint ___CallFuncPtrCallback(FuncPtr func)
		{
			var __result = __CallFuncPtrCallback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncFloat, float> CallFuncFloatCallback = &___CallFuncFloatCallback;
		internal static delegate* unmanaged[Cdecl]<nint, float> __CallFuncFloatCallback;
		private static float ___CallFuncFloatCallback(FuncFloat func)
		{
			var __result = __CallFuncFloatCallback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncDouble, double> CallFuncDoubleCallback = &___CallFuncDoubleCallback;
		internal static delegate* unmanaged[Cdecl]<nint, double> __CallFuncDoubleCallback;
		private static double ___CallFuncDoubleCallback(FuncDouble func)
		{
			var __result = __CallFuncDoubleCallback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncString, string> CallFuncStringCallback = &___CallFuncStringCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncStringCallback;
		private static string ___CallFuncStringCallback(FuncString func)
		{
			var CallFuncStringCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncStringWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructString(@__output, __result__);
				return @__output;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFuncStringCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncStringCallback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <FuncFunction, nint> CallFuncFunctionCallback = &___CallFuncFunctionCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint> __CallFuncFunctionCallback;
		private static nint ___CallFuncFunctionCallback(FuncFunction func)
		{
			var __result = __CallFuncFunctionCallback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncBoolVector, bool[]> CallFuncBoolVectorCallback = &___CallFuncBoolVectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncBoolVectorCallback;
		private static bool[] ___CallFuncBoolVectorCallback(FuncBoolVector func)
		{
			var CallFuncBoolVectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncBoolVectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorBool(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorBool();

			__CallFuncBoolVectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncBoolVectorCallback_func));

			var output = new bool[NativeMethods.GetVectorSizeBool(__output)];
			NativeMethods.GetVectorDataBool(__output, output);

			NativeMethods.FreeVectorBool(__output);

			return output;
		}
		internal static delegate* <FuncChar8Vector, char[]> CallFuncChar8VectorCallback = &___CallFuncChar8VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncChar8VectorCallback;
		private static char[] ___CallFuncChar8VectorCallback(FuncChar8Vector func)
		{
			var CallFuncChar8VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncChar8VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorChar8(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorChar8();

			__CallFuncChar8VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncChar8VectorCallback_func));

			var output = new char[NativeMethods.GetVectorSizeChar8(__output)];
			NativeMethods.GetVectorDataChar8(__output, output);

			NativeMethods.FreeVectorChar8(__output);

			return output;
		}
		internal static delegate* <FuncChar16Vector, char[]> CallFuncChar16VectorCallback = &___CallFuncChar16VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncChar16VectorCallback;
		private static char[] ___CallFuncChar16VectorCallback(FuncChar16Vector func)
		{
			var CallFuncChar16VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncChar16VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorChar16(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorChar16();

			__CallFuncChar16VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncChar16VectorCallback_func));

			var output = new char[NativeMethods.GetVectorSizeChar16(__output)];
			NativeMethods.GetVectorDataChar16(__output, output);

			NativeMethods.FreeVectorChar16(__output);

			return output;
		}
		internal static delegate* <FuncInt8Vector, sbyte[]> CallFuncInt8VectorCallback = &___CallFuncInt8VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncInt8VectorCallback;
		private static sbyte[] ___CallFuncInt8VectorCallback(FuncInt8Vector func)
		{
			var CallFuncInt8VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncInt8VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorInt8(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorInt8();

			__CallFuncInt8VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncInt8VectorCallback_func));

			var output = new sbyte[NativeMethods.GetVectorSizeInt8(__output)];
			NativeMethods.GetVectorDataInt8(__output, output);

			NativeMethods.FreeVectorInt8(__output);

			return output;
		}
		internal static delegate* <FuncInt16Vector, short[]> CallFuncInt16VectorCallback = &___CallFuncInt16VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncInt16VectorCallback;
		private static short[] ___CallFuncInt16VectorCallback(FuncInt16Vector func)
		{
			var CallFuncInt16VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncInt16VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorInt16(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorInt16();

			__CallFuncInt16VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncInt16VectorCallback_func));

			var output = new short[NativeMethods.GetVectorSizeInt16(__output)];
			NativeMethods.GetVectorDataInt16(__output, output);

			NativeMethods.FreeVectorInt16(__output);

			return output;
		}
		internal static delegate* <FuncInt32Vector, int[]> CallFuncInt32VectorCallback = &___CallFuncInt32VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncInt32VectorCallback;
		private static int[] ___CallFuncInt32VectorCallback(FuncInt32Vector func)
		{
			var CallFuncInt32VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncInt32VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorInt32(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorInt32();

			__CallFuncInt32VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncInt32VectorCallback_func));

			var output = new int[NativeMethods.GetVectorSizeInt32(__output)];
			NativeMethods.GetVectorDataInt32(__output, output);

			NativeMethods.FreeVectorInt32(__output);

			return output;
		}
		internal static delegate* <FuncInt64Vector, long[]> CallFuncInt64VectorCallback = &___CallFuncInt64VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncInt64VectorCallback;
		private static long[] ___CallFuncInt64VectorCallback(FuncInt64Vector func)
		{
			var CallFuncInt64VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncInt64VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorInt64(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorInt64();

			__CallFuncInt64VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncInt64VectorCallback_func));

			var output = new long[NativeMethods.GetVectorSizeInt64(__output)];
			NativeMethods.GetVectorDataInt64(__output, output);

			NativeMethods.FreeVectorInt64(__output);

			return output;
		}
		internal static delegate* <FuncUInt8Vector, byte[]> CallFuncUInt8VectorCallback = &___CallFuncUInt8VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncUInt8VectorCallback;
		private static byte[] ___CallFuncUInt8VectorCallback(FuncUInt8Vector func)
		{
			var CallFuncUInt8VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncUInt8VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorUInt8(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorUInt8();

			__CallFuncUInt8VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncUInt8VectorCallback_func));

			var output = new byte[NativeMethods.GetVectorSizeUInt8(__output)];
			NativeMethods.GetVectorDataUInt8(__output, output);

			NativeMethods.FreeVectorUInt8(__output);

			return output;
		}
		internal static delegate* <FuncUInt16Vector, ushort[]> CallFuncUInt16VectorCallback = &___CallFuncUInt16VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncUInt16VectorCallback;
		private static ushort[] ___CallFuncUInt16VectorCallback(FuncUInt16Vector func)
		{
			var CallFuncUInt16VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncUInt16VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorUInt16(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorUInt16();

			__CallFuncUInt16VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncUInt16VectorCallback_func));

			var output = new ushort[NativeMethods.GetVectorSizeUInt16(__output)];
			NativeMethods.GetVectorDataUInt16(__output, output);

			NativeMethods.FreeVectorUInt16(__output);

			return output;
		}
		internal static delegate* <FuncUInt32Vector, uint[]> CallFuncUInt32VectorCallback = &___CallFuncUInt32VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncUInt32VectorCallback;
		private static uint[] ___CallFuncUInt32VectorCallback(FuncUInt32Vector func)
		{
			var CallFuncUInt32VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncUInt32VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorUInt32(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorUInt32();

			__CallFuncUInt32VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncUInt32VectorCallback_func));

			var output = new uint[NativeMethods.GetVectorSizeUInt32(__output)];
			NativeMethods.GetVectorDataUInt32(__output, output);

			NativeMethods.FreeVectorUInt32(__output);

			return output;
		}
		internal static delegate* <FuncUInt64Vector, ulong[]> CallFuncUInt64VectorCallback = &___CallFuncUInt64VectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncUInt64VectorCallback;
		private static ulong[] ___CallFuncUInt64VectorCallback(FuncUInt64Vector func)
		{
			var CallFuncUInt64VectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncUInt64VectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorUInt64(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorUInt64();

			__CallFuncUInt64VectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncUInt64VectorCallback_func));

			var output = new ulong[NativeMethods.GetVectorSizeUInt64(__output)];
			NativeMethods.GetVectorDataUInt64(__output, output);

			NativeMethods.FreeVectorUInt64(__output);

			return output;
		}
		internal static delegate* <FuncPtrVector, nint[]> CallFuncPtrVectorCallback = &___CallFuncPtrVectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncPtrVectorCallback;
		private static nint[] ___CallFuncPtrVectorCallback(FuncPtrVector func)
		{
			var CallFuncPtrVectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncPtrVectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorIntPtr(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorIntPtr();

			__CallFuncPtrVectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncPtrVectorCallback_func));

			var output = new nint[NativeMethods.GetVectorSizeIntPtr(__output)];
			NativeMethods.GetVectorDataIntPtr(__output, output);

			NativeMethods.FreeVectorIntPtr(__output);

			return output;
		}
		internal static delegate* <FuncFloatVector, float[]> CallFuncFloatVectorCallback = &___CallFuncFloatVectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncFloatVectorCallback;
		private static float[] ___CallFuncFloatVectorCallback(FuncFloatVector func)
		{
			var CallFuncFloatVectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncFloatVectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorFloat(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorFloat();

			__CallFuncFloatVectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncFloatVectorCallback_func));

			var output = new float[NativeMethods.GetVectorSizeFloat(__output)];
			NativeMethods.GetVectorDataFloat(__output, output);

			NativeMethods.FreeVectorFloat(__output);

			return output;
		}
		internal static delegate* <FuncStringVector, string[]> CallFuncStringVectorCallback = &___CallFuncStringVectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncStringVectorCallback;
		private static string[] ___CallFuncStringVectorCallback(FuncStringVector func)
		{
			var CallFuncStringVectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncStringVectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorString(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorString();

			__CallFuncStringVectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncStringVectorCallback_func));

			var output = new string[NativeMethods.GetVectorSizeString(__output)];
			NativeMethods.GetVectorDataString(__output, output);

			NativeMethods.FreeVectorString(__output);

			return output;
		}
		internal static delegate* <FuncDoubleVector, double[]> CallFuncDoubleVectorCallback = &___CallFuncDoubleVectorCallback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFuncDoubleVectorCallback;
		private static double[] ___CallFuncDoubleVectorCallback(FuncDoubleVector func)
		{
			var CallFuncDoubleVectorCallback_func = s_DelegateHolder.GetOrAdd(func, new FuncDoubleVectorWrapper((nint @__output) => {
				var __result__ = func();

				NativeMethods.ConstructVectorDouble(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorDouble();

			__CallFuncDoubleVectorCallback(__output, Marshal.GetFunctionPointerForDelegate(CallFuncDoubleVectorCallback_func));

			var output = new double[NativeMethods.GetVectorSizeDouble(__output)];
			NativeMethods.GetVectorDataDouble(__output, output);

			NativeMethods.FreeVectorDouble(__output);

			return output;
		}
		internal static delegate* <FuncVec2, Vector2> CallFuncVec2Callback = &___CallFuncVec2Callback;
		internal static delegate* unmanaged[Cdecl]<nint, Vector2> __CallFuncVec2Callback;
		private static Vector2 ___CallFuncVec2Callback(FuncVec2 func)
		{
			var __result = __CallFuncVec2Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncVec3, Vector3> CallFuncVec3Callback = &___CallFuncVec3Callback;
		internal static delegate* unmanaged[Cdecl]<nint, Vector3> __CallFuncVec3Callback;
		private static Vector3 ___CallFuncVec3Callback(FuncVec3 func)
		{
			var __result = __CallFuncVec3Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncVec4, Vector4> CallFuncVec4Callback = &___CallFuncVec4Callback;
		internal static delegate* unmanaged[Cdecl]<nint, Vector4> __CallFuncVec4Callback;
		private static Vector4 ___CallFuncVec4Callback(FuncVec4 func)
		{
			var __result = __CallFuncVec4Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <FuncMat4x4, Matrix4x4> CallFuncMat4x4Callback = &___CallFuncMat4x4Callback;
		internal static delegate* unmanaged[Cdecl]<nint, Matrix4x4> __CallFuncMat4x4Callback;
		private static Matrix4x4 ___CallFuncMat4x4Callback(FuncMat4x4 func)
		{
			var __result = __CallFuncMat4x4Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <Func1, int> CallFunc1Callback = &___CallFunc1Callback;
		internal static delegate* unmanaged[Cdecl]<nint, int> __CallFunc1Callback;
		private static int ___CallFunc1Callback(Func1 func)
		{
			var __result = __CallFunc1Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <Func2, char> CallFunc2Callback = &___CallFunc2Callback;
		internal static delegate* unmanaged[Cdecl]<nint, sbyte> __CallFunc2Callback;
		private static char ___CallFunc2Callback(Func2 func)
		{
			var CallFunc2Callback_func = s_DelegateHolder.GetOrAdd(func, new Func2Wrapper((float @__a, long @__b) => {
				var __result__ = func(@__a, @__b);
				return Convert.ToSByte(__result__);
			}));
			var __result = __CallFunc2Callback(Marshal.GetFunctionPointerForDelegate(CallFunc2Callback_func));
			return (char)__result;
		}
		internal static delegate* <Func3, void> CallFunc3Callback = &___CallFunc3Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __CallFunc3Callback;
		private static void ___CallFunc3Callback(Func3 func)
		{
			var CallFunc3Callback_func = s_DelegateHolder.GetOrAdd(func, new Func3Wrapper((nint @__a, ref Vector4 @__b, nint @__c) => {
				var __c__ = NativeMethods.GetStringData(@__c);

				func(@__a, ref @__b, __c__);
			}));
			__CallFunc3Callback(Marshal.GetFunctionPointerForDelegate(CallFunc3Callback_func));
		}
		internal static delegate* <Func4, Vector4> CallFunc4Callback = &___CallFunc4Callback;
		internal static delegate* unmanaged[Cdecl]<nint, Vector4> __CallFunc4Callback;
		private static Vector4 ___CallFunc4Callback(Func4 func)
		{
			var __result = __CallFunc4Callback(Marshal.GetFunctionPointerForDelegate(func));
			return __result;
		}
		internal static delegate* <Func5, bool> CallFunc5Callback = &___CallFunc5Callback;
		internal static delegate* unmanaged[Cdecl]<nint, byte> __CallFunc5Callback;
		private static bool ___CallFunc5Callback(Func5 func)
		{
			var CallFunc5Callback_func = s_DelegateHolder.GetOrAdd(func, new Func5Wrapper((sbyte @__a, ref Vector2 @__b, nint @__c, double @__d, nint @__e) => {
				var __e__ = new ulong[NativeMethods.GetVectorSizeUInt64(@__e)];
				NativeMethods.GetVectorDataUInt64(@__e, __e__);

				var __result__ = func(@__a, ref @__b, @__c, @__d, __e__);
				return Convert.ToByte(__result__);
			}));
			var __result = __CallFunc5Callback(Marshal.GetFunctionPointerForDelegate(CallFunc5Callback_func));
			return __result == 1;
		}
		internal static delegate* <Func6, long> CallFunc6Callback = &___CallFunc6Callback;
		internal static delegate* unmanaged[Cdecl]<nint, long> __CallFunc6Callback;
		private static long ___CallFunc6Callback(Func6 func)
		{
			var CallFunc6Callback_func = s_DelegateHolder.GetOrAdd(func, new Func6Wrapper((nint @__a, float @__b, nint @__c, short @__d, nint @__e, nint @__f) => {
				var __a__ = NativeMethods.GetStringData(@__a);
				var __c__ = new float[NativeMethods.GetVectorSizeFloat(@__c)];
				NativeMethods.GetVectorDataFloat(@__c, __c__);
				var __e__ = new byte[NativeMethods.GetVectorSizeUInt8(@__e)];
				NativeMethods.GetVectorDataUInt8(@__e, __e__);

				var __result__ = func(__a__, @__b, __c__, @__d, __e__, @__f);
				return __result__;
			}));
			var __result = __CallFunc6Callback(Marshal.GetFunctionPointerForDelegate(CallFunc6Callback_func));
			return __result;
		}
		internal static delegate* <Func7, double> CallFunc7Callback = &___CallFunc7Callback;
		internal static delegate* unmanaged[Cdecl]<nint, double> __CallFunc7Callback;
		private static double ___CallFunc7Callback(Func7 func)
		{
			var CallFunc7Callback_func = s_DelegateHolder.GetOrAdd(func, new Func7Wrapper((nint @__vecC, ushort @__u16, ushort @__ch16, nint @__vecU32, ref Vector4 @__vec4, byte @__b, ulong @__u64) => {
				var __vecC__ = new char[NativeMethods.GetVectorSizeChar8(@__vecC)];
				NativeMethods.GetVectorDataChar8(@__vecC, __vecC__);
				var __ch16__ = Convert.ToChar(@__ch16);
				var __vecU32__ = new uint[NativeMethods.GetVectorSizeUInt32(@__vecU32)];
				NativeMethods.GetVectorDataUInt32(@__vecU32, __vecU32__);
				var __b__ = Convert.ToBoolean(@__b);

				var __result__ = func(__vecC__, @__u16, __ch16__, __vecU32__, ref @__vec4, __b__, @__u64);
				return __result__;
			}));
			var __result = __CallFunc7Callback(Marshal.GetFunctionPointerForDelegate(CallFunc7Callback_func));
			return __result;
		}
		internal static delegate* <Func8, Matrix4x4> CallFunc8Callback = &___CallFunc8Callback;
		internal static delegate* unmanaged[Cdecl]<nint, Matrix4x4> __CallFunc8Callback;
		private static Matrix4x4 ___CallFunc8Callback(Func8 func)
		{
			var CallFunc8Callback_func = s_DelegateHolder.GetOrAdd(func, new Func8Wrapper((ref Vector3 @__vec3, nint @__vecU32, short @__i16, byte @__b, ref Vector4 @__vec4, nint @__vecC16, ushort @__ch16, int @__i32) => {
				var __vecU32__ = new uint[NativeMethods.GetVectorSizeUInt32(@__vecU32)];
				NativeMethods.GetVectorDataUInt32(@__vecU32, __vecU32__);
				var __b__ = Convert.ToBoolean(@__b);
				var __vecC16__ = new char[NativeMethods.GetVectorSizeChar16(@__vecC16)];
				NativeMethods.GetVectorDataChar16(@__vecC16, __vecC16__);
				var __ch16__ = Convert.ToChar(@__ch16);

				var __result__ = func(ref @__vec3, __vecU32__, @__i16, __b__, ref @__vec4, __vecC16__, __ch16__, @__i32);
				return __result__;
			}));
			var __result = __CallFunc8Callback(Marshal.GetFunctionPointerForDelegate(CallFunc8Callback_func));
			return __result;
		}
		internal static delegate* <Func9, void> CallFunc9Callback = &___CallFunc9Callback;
		internal static delegate* unmanaged[Cdecl]<nint, void> __CallFunc9Callback;
		private static void ___CallFunc9Callback(Func9 func)
		{
			var CallFunc9Callback_func = s_DelegateHolder.GetOrAdd(func, new Func9Wrapper((float @__f, ref Vector2 @__vec2, nint @__vecI8, ulong @__u64, byte @__b, nint @__str, ref Vector4 @__vec4, short @__i16, nint @__ptr) => {
				var __vecI8__ = new sbyte[NativeMethods.GetVectorSizeInt8(@__vecI8)];
				NativeMethods.GetVectorDataInt8(@__vecI8, __vecI8__);
				var __b__ = Convert.ToBoolean(@__b);
				var __str__ = NativeMethods.GetStringData(@__str);

				func(@__f, ref @__vec2, __vecI8__, @__u64, __b__, __str__, ref @__vec4, @__i16, @__ptr);
			}));
			__CallFunc9Callback(Marshal.GetFunctionPointerForDelegate(CallFunc9Callback_func));
		}
		internal static delegate* <Func10, uint> CallFunc10Callback = &___CallFunc10Callback;
		internal static delegate* unmanaged[Cdecl]<nint, uint> __CallFunc10Callback;
		private static uint ___CallFunc10Callback(Func10 func)
		{
			var CallFunc10Callback_func = s_DelegateHolder.GetOrAdd(func, new Func10Wrapper((ref Vector4 @__vec4, ref Matrix4x4 @__mat, nint @__vecU32, ulong @__u64, nint @__vecC, int @__i32, byte @__b, ref Vector2 @__vec2, long @__i64, double @__d) => {
				var __vecU32__ = new uint[NativeMethods.GetVectorSizeUInt32(@__vecU32)];
				NativeMethods.GetVectorDataUInt32(@__vecU32, __vecU32__);
				var __vecC__ = new char[NativeMethods.GetVectorSizeChar8(@__vecC)];
				NativeMethods.GetVectorDataChar8(@__vecC, __vecC__);
				var __b__ = Convert.ToBoolean(@__b);

				var __result__ = func(ref @__vec4, ref @__mat, __vecU32__, @__u64, __vecC__, @__i32, __b__, ref @__vec2, @__i64, @__d);
				return __result__;
			}));
			var __result = __CallFunc10Callback(Marshal.GetFunctionPointerForDelegate(CallFunc10Callback_func));
			return __result;
		}
		internal static delegate* <Func11, nint> CallFunc11Callback = &___CallFunc11Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint> __CallFunc11Callback;
		private static nint ___CallFunc11Callback(Func11 func)
		{
			var CallFunc11Callback_func = s_DelegateHolder.GetOrAdd(func, new Func11Wrapper((nint @__vecB, ushort @__ch16, byte @__u8, double @__d, ref Vector3 @__vec3, nint @__vecI8, long @__i64, ushort @__u16, float @__f, ref Vector2 @__vec2, uint @__u32) => {
				var __vecB__ = new bool[NativeMethods.GetVectorSizeBool(@__vecB)];
				NativeMethods.GetVectorDataBool(@__vecB, __vecB__);
				var __ch16__ = Convert.ToChar(@__ch16);
				var __vecI8__ = new sbyte[NativeMethods.GetVectorSizeInt8(@__vecI8)];
				NativeMethods.GetVectorDataInt8(@__vecI8, __vecI8__);

				var __result__ = func(__vecB__, __ch16__, @__u8, @__d, ref @__vec3, __vecI8__, @__i64, @__u16, @__f, ref @__vec2, @__u32);
				return __result__;
			}));
			var __result = __CallFunc11Callback(Marshal.GetFunctionPointerForDelegate(CallFunc11Callback_func));
			return __result;
		}
		internal static delegate* <Func12, bool> CallFunc12Callback = &___CallFunc12Callback;
		internal static delegate* unmanaged[Cdecl]<nint, byte> __CallFunc12Callback;
		private static bool ___CallFunc12Callback(Func12 func)
		{
			var CallFunc12Callback_func = s_DelegateHolder.GetOrAdd(func, new Func12Wrapper((nint @__ptr, nint @__vecD, uint @__u32, double @__d, byte @__b, int @__i32, sbyte @__i8, ulong @__u64, float @__f, nint @__vecPtr, long @__i64, sbyte @__ch) => {
				var __vecD__ = new double[NativeMethods.GetVectorSizeDouble(@__vecD)];
				NativeMethods.GetVectorDataDouble(@__vecD, __vecD__);
				var __b__ = Convert.ToBoolean(@__b);
				var __vecPtr__ = new nint[NativeMethods.GetVectorSizeIntPtr(@__vecPtr)];
				NativeMethods.GetVectorDataIntPtr(@__vecPtr, __vecPtr__);
				var __ch__ = Convert.ToChar(@__ch);

				var __result__ = func(@__ptr, __vecD__, @__u32, @__d, __b__, @__i32, @__i8, @__u64, @__f, __vecPtr__, @__i64, __ch__);
				return Convert.ToByte(__result__);
			}));
			var __result = __CallFunc12Callback(Marshal.GetFunctionPointerForDelegate(CallFunc12Callback_func));
			return __result == 1;
		}
		internal static delegate* <Func13, string> CallFunc13Callback = &___CallFunc13Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc13Callback;
		private static string ___CallFunc13Callback(Func13 func)
		{
			var CallFunc13Callback_func = s_DelegateHolder.GetOrAdd(func, new Func13Wrapper((nint @__output, long @__i64, nint @__vecC, ushort @__d, float @__f, nint @__b, ref Vector4 @__vec4, nint @__str, int @__int32, ref Vector3 @__vec3, nint @__ptr, ref Vector2 @__vec2, nint @__arr, short @__i16) => {
				var __vecC__ = new char[NativeMethods.GetVectorSizeChar8(@__vecC)];
				NativeMethods.GetVectorDataChar8(@__vecC, __vecC__);
				var __b__ = new bool[NativeMethods.GetVectorSizeBool(@__b)];
				NativeMethods.GetVectorDataBool(@__b, __b__);
				var __str__ = NativeMethods.GetStringData(@__str);
				var __arr__ = new byte[NativeMethods.GetVectorSizeUInt8(@__arr)];
				NativeMethods.GetVectorDataUInt8(@__arr, __arr__);

				var __result__ = func(@__i64, __vecC__, @__d, @__f, __b__, ref @__vec4, __str__, @__int32, ref @__vec3, @__ptr, ref @__vec2, __arr__, @__i16);

				NativeMethods.ConstructString(@__output, __result__);
				return @__output;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc13Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc13Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func14, string[]> CallFunc14Callback = &___CallFunc14Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc14Callback;
		private static string[] ___CallFunc14Callback(Func14 func)
		{
			var CallFunc14Callback_func = s_DelegateHolder.GetOrAdd(func, new Func14Wrapper((nint @__output, nint @__vecC, nint @__vecU32, ref Matrix4x4 @__mat, byte @__b, ushort @__ch16, int @__i32, nint @__vecF, ushort @__u16, nint @__vecU8, sbyte @__i8, ref Vector3 @__vec3, ref Vector4 @__vec4, double @__d, nint @__ptr) => {
				var __vecC__ = new char[NativeMethods.GetVectorSizeChar8(@__vecC)];
				NativeMethods.GetVectorDataChar8(@__vecC, __vecC__);
				var __vecU32__ = new uint[NativeMethods.GetVectorSizeUInt32(@__vecU32)];
				NativeMethods.GetVectorDataUInt32(@__vecU32, __vecU32__);
				var __b__ = Convert.ToBoolean(@__b);
				var __ch16__ = Convert.ToChar(@__ch16);
				var __vecF__ = new float[NativeMethods.GetVectorSizeFloat(@__vecF)];
				NativeMethods.GetVectorDataFloat(@__vecF, __vecF__);
				var __vecU8__ = new byte[NativeMethods.GetVectorSizeUInt8(@__vecU8)];
				NativeMethods.GetVectorDataUInt8(@__vecU8, __vecU8__);

				var __result__ = func(__vecC__, __vecU32__, ref @__mat, __b__, __ch16__, @__i32, __vecF__, @__u16, __vecU8__, @__i8, ref @__vec3, ref @__vec4, @__d, @__ptr);

				NativeMethods.ConstructVectorString(@__output, __result__, __result__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateVectorString();

			__CallFunc14Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc14Callback_func));

			var output = new string[NativeMethods.GetVectorSizeString(__output)];
			NativeMethods.GetVectorDataString(__output, output);

			NativeMethods.FreeVectorString(__output);

			return output;
		}
		internal static delegate* <Func15, short> CallFunc15Callback = &___CallFunc15Callback;
		internal static delegate* unmanaged[Cdecl]<nint, short> __CallFunc15Callback;
		private static short ___CallFunc15Callback(Func15 func)
		{
			var CallFunc15Callback_func = s_DelegateHolder.GetOrAdd(func, new Func15Wrapper((nint @__vecI16, ref Matrix4x4 @__mat, ref Vector4 @__vec4, nint @__ptr, ulong @__u64, nint @__vecU32, byte @__b, float @__f, nint @__vecC16, byte @__u8, int @__i32, ref Vector2 @__vec2, ushort @__u16, double @__d, nint @__vecU8) => {
				var __vecI16__ = new short[NativeMethods.GetVectorSizeInt16(@__vecI16)];
				NativeMethods.GetVectorDataInt16(@__vecI16, __vecI16__);
				var __vecU32__ = new uint[NativeMethods.GetVectorSizeUInt32(@__vecU32)];
				NativeMethods.GetVectorDataUInt32(@__vecU32, __vecU32__);
				var __b__ = Convert.ToBoolean(@__b);
				var __vecC16__ = new char[NativeMethods.GetVectorSizeChar16(@__vecC16)];
				NativeMethods.GetVectorDataChar16(@__vecC16, __vecC16__);
				var __vecU8__ = new byte[NativeMethods.GetVectorSizeUInt8(@__vecU8)];
				NativeMethods.GetVectorDataUInt8(@__vecU8, __vecU8__);

				var __result__ = func(__vecI16__, ref @__mat, ref @__vec4, @__ptr, @__u64, __vecU32__, __b__, @__f, __vecC16__, @__u8, @__i32, ref @__vec2, @__u16, @__d, __vecU8__);
				return __result__;
			}));
			var __result = __CallFunc15Callback(Marshal.GetFunctionPointerForDelegate(CallFunc15Callback_func));
			return __result;
		}
		internal static delegate* <Func16, nint> CallFunc16Callback = &___CallFunc16Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint> __CallFunc16Callback;
		private static nint ___CallFunc16Callback(Func16 func)
		{
			var CallFunc16Callback_func = s_DelegateHolder.GetOrAdd(func, new Func16Wrapper((nint @__vecB, short @__i16, nint @__vecI8, ref Vector4 @__vec4, ref Matrix4x4 @__mat, ref Vector2 @__vec2, nint @__vecU64, nint @__vecC, nint @__str, long @__i64, nint @__vecU32, ref Vector3 @__vec3, float @__f, double @__d, sbyte @__i8, ushort @__u16) => {
				var __vecB__ = new bool[NativeMethods.GetVectorSizeBool(@__vecB)];
				NativeMethods.GetVectorDataBool(@__vecB, __vecB__);
				var __vecI8__ = new sbyte[NativeMethods.GetVectorSizeInt8(@__vecI8)];
				NativeMethods.GetVectorDataInt8(@__vecI8, __vecI8__);
				var __vecU64__ = new ulong[NativeMethods.GetVectorSizeUInt64(@__vecU64)];
				NativeMethods.GetVectorDataUInt64(@__vecU64, __vecU64__);
				var __vecC__ = new char[NativeMethods.GetVectorSizeChar8(@__vecC)];
				NativeMethods.GetVectorDataChar8(@__vecC, __vecC__);
				var __str__ = NativeMethods.GetStringData(@__str);
				var __vecU32__ = new uint[NativeMethods.GetVectorSizeUInt32(@__vecU32)];
				NativeMethods.GetVectorDataUInt32(@__vecU32, __vecU32__);

				var __result__ = func(__vecB__, @__i16, __vecI8__, ref @__vec4, ref @__mat, ref @__vec2, __vecU64__, __vecC__, __str__, @__i64, __vecU32__, ref @__vec3, @__f, @__d, @__i8, @__u16);
				return __result__;
			}));
			var __result = __CallFunc16Callback(Marshal.GetFunctionPointerForDelegate(CallFunc16Callback_func));
			return __result;
		}
		internal static delegate* <Func17, string> CallFunc17Callback = &___CallFunc17Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc17Callback;
		private static string ___CallFunc17Callback(Func17 func)
		{
			var __output = NativeMethods.AllocateString();

			__CallFunc17Callback(__output, Marshal.GetFunctionPointerForDelegate(func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func18, string> CallFunc18Callback = &___CallFunc18Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc18Callback;
		private static string ___CallFunc18Callback(Func18 func)
		{
			var __output = NativeMethods.AllocateString();

			__CallFunc18Callback(__output, Marshal.GetFunctionPointerForDelegate(func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func19, string> CallFunc19Callback = &___CallFunc19Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc19Callback;
		private static string ___CallFunc19Callback(Func19 func)
		{
			var CallFunc19Callback_func = s_DelegateHolder.GetOrAdd(func, new Func19Wrapper((ref uint @__u32, ref Vector3 @__vec3, nint @__vecU32) => {
				var __vecU32__ = new uint[NativeMethods.GetVectorSizeUInt32(@__vecU32)];
				NativeMethods.GetVectorDataUInt32(@__vecU32, __vecU32__);

				func(ref @__u32, ref @__vec3, ref __vecU32__);

				NativeMethods.AssignVectorUInt32(@__vecU32, __vecU32__, __vecU32__.Length);
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc19Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc19Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func20, string> CallFunc20Callback = &___CallFunc20Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc20Callback;
		private static string ___CallFunc20Callback(Func20 func)
		{
			var CallFunc20Callback_func = s_DelegateHolder.GetOrAdd(func, new Func20Wrapper((ref ushort @__ch16, ref Vector4 @__vec4, nint @__vecU64, ref sbyte @__ch) => {
				var __ch16__ = Convert.ToChar(@__ch16);
				var __vecU64__ = new ulong[NativeMethods.GetVectorSizeUInt64(@__vecU64)];
				NativeMethods.GetVectorDataUInt64(@__vecU64, __vecU64__);
				var __ch__ = Convert.ToChar(@__ch);

				var __result__ = func(ref __ch16__, ref @__vec4, ref __vecU64__, ref __ch__);

				@__ch16 = Convert.ToUInt16(__ch16__);
				NativeMethods.AssignVectorUInt64(@__vecU64, __vecU64__, __vecU64__.Length);
				@__ch = Convert.ToSByte(__ch__);
				return __result__;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc20Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc20Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func21, string> CallFunc21Callback = &___CallFunc21Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc21Callback;
		private static string ___CallFunc21Callback(Func21 func)
		{
			var CallFunc21Callback_func = s_DelegateHolder.GetOrAdd(func, new Func21Wrapper((ref Matrix4x4 @__mat, nint @__vecI32, ref Vector2 @__vec2, ref byte @__b, ref double @__extraParam) => {
				var __vecI32__ = new int[NativeMethods.GetVectorSizeInt32(@__vecI32)];
				NativeMethods.GetVectorDataInt32(@__vecI32, __vecI32__);
				var __b__ = Convert.ToBoolean(@__b);

				var __result__ = func(ref @__mat, ref __vecI32__, ref @__vec2, ref __b__, ref @__extraParam);

				NativeMethods.AssignVectorInt32(@__vecI32, __vecI32__, __vecI32__.Length);
				@__b = Convert.ToByte(__b__);
				return __result__;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc21Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc21Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func22, string> CallFunc22Callback = &___CallFunc22Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc22Callback;
		private static string ___CallFunc22Callback(Func22 func)
		{
			var CallFunc22Callback_func = s_DelegateHolder.GetOrAdd(func, new Func22Wrapper((ref nint @__ptr64Ref, ref uint @__uint32Ref, nint @__vectorDoubleRef, ref short @__int16Ref, nint @__plgStringRef, ref Vector4 @__plgVector4Ref) => {
				var __vectorDoubleRef__ = new double[NativeMethods.GetVectorSizeDouble(@__vectorDoubleRef)];
				NativeMethods.GetVectorDataDouble(@__vectorDoubleRef, __vectorDoubleRef__);
				var __plgStringRef__ = NativeMethods.GetStringData(@__plgStringRef);

				var __result__ = func(ref @__ptr64Ref, ref @__uint32Ref, ref __vectorDoubleRef__, ref @__int16Ref, ref __plgStringRef__, ref @__plgVector4Ref);

				NativeMethods.AssignVectorDouble(@__vectorDoubleRef, __vectorDoubleRef__, __vectorDoubleRef__.Length);
				NativeMethods.AssignString(@__plgStringRef, __plgStringRef__);
				return __result__;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc22Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc22Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func23, string> CallFunc23Callback = &___CallFunc23Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc23Callback;
		private static string ___CallFunc23Callback(Func23 func)
		{
			var CallFunc23Callback_func = s_DelegateHolder.GetOrAdd(func, new Func23Wrapper((ref ulong @__uint64Ref, ref Vector2 @__plgVector2Ref, nint @__vectorInt16Ref, ref ushort @__char16Ref, ref float @__floatRef, ref sbyte @__int8Ref, nint @__vectorUInt8Ref) => {
				var __vectorInt16Ref__ = new short[NativeMethods.GetVectorSizeInt16(@__vectorInt16Ref)];
				NativeMethods.GetVectorDataInt16(@__vectorInt16Ref, __vectorInt16Ref__);
				var __char16Ref__ = Convert.ToChar(@__char16Ref);
				var __vectorUInt8Ref__ = new byte[NativeMethods.GetVectorSizeUInt8(@__vectorUInt8Ref)];
				NativeMethods.GetVectorDataUInt8(@__vectorUInt8Ref, __vectorUInt8Ref__);

				func(ref @__uint64Ref, ref @__plgVector2Ref, ref __vectorInt16Ref__, ref __char16Ref__, ref @__floatRef, ref @__int8Ref, ref __vectorUInt8Ref__);

				NativeMethods.AssignVectorInt16(@__vectorInt16Ref, __vectorInt16Ref__, __vectorInt16Ref__.Length);
				@__char16Ref = Convert.ToUInt16(__char16Ref__);
				NativeMethods.AssignVectorUInt8(@__vectorUInt8Ref, __vectorUInt8Ref__, __vectorUInt8Ref__.Length);
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc23Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc23Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func24, string> CallFunc24Callback = &___CallFunc24Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc24Callback;
		private static string ___CallFunc24Callback(Func24 func)
		{
			var CallFunc24Callback_func = s_DelegateHolder.GetOrAdd(func, new Func24Wrapper((nint @__vectorCharRef, ref long @__int64Ref, nint @__vectorUInt8Ref, ref Vector4 @__plgVector4Ref, ref ulong @__uint64Ref, nint @__vectorptr64Ref, ref double @__doubleRef, nint @__vectorptr64Ref2) => {
				var __vectorCharRef__ = new char[NativeMethods.GetVectorSizeChar8(@__vectorCharRef)];
				NativeMethods.GetVectorDataChar8(@__vectorCharRef, __vectorCharRef__);
				var __vectorUInt8Ref__ = new byte[NativeMethods.GetVectorSizeUInt8(@__vectorUInt8Ref)];
				NativeMethods.GetVectorDataUInt8(@__vectorUInt8Ref, __vectorUInt8Ref__);
				var __vectorptr64Ref__ = new nint[NativeMethods.GetVectorSizeIntPtr(@__vectorptr64Ref)];
				NativeMethods.GetVectorDataIntPtr(@__vectorptr64Ref, __vectorptr64Ref__);
				var __vectorptr64Ref2__ = new nint[NativeMethods.GetVectorSizeIntPtr(@__vectorptr64Ref2)];
				NativeMethods.GetVectorDataIntPtr(@__vectorptr64Ref2, __vectorptr64Ref2__);

				var __result__ = func(ref __vectorCharRef__, ref @__int64Ref, ref __vectorUInt8Ref__, ref @__plgVector4Ref, ref @__uint64Ref, ref __vectorptr64Ref__, ref @__doubleRef, ref __vectorptr64Ref2__);

				NativeMethods.AssignVectorChar8(@__vectorCharRef, __vectorCharRef__, __vectorCharRef__.Length);
				NativeMethods.AssignVectorUInt8(@__vectorUInt8Ref, __vectorUInt8Ref__, __vectorUInt8Ref__.Length);
				NativeMethods.AssignVectorIntPtr(@__vectorptr64Ref, __vectorptr64Ref__, __vectorptr64Ref__.Length);
				NativeMethods.AssignVectorIntPtr(@__vectorptr64Ref2, __vectorptr64Ref2__, __vectorptr64Ref2__.Length);
				return __result__;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc24Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc24Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func25, string> CallFunc25Callback = &___CallFunc25Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc25Callback;
		private static string ___CallFunc25Callback(Func25 func)
		{
			var CallFunc25Callback_func = s_DelegateHolder.GetOrAdd(func, new Func25Wrapper((ref int @__int32Ref, nint @__vectorptr64Ref, ref byte @__boolRef, ref byte @__uint8Ref, nint @__plgStringRef, ref Vector3 @__plgVector3Ref, ref long @__int64Ref, ref Vector4 @__plgVector4Ref, ref ushort @__uint16Ref) => {
				var __vectorptr64Ref__ = new nint[NativeMethods.GetVectorSizeIntPtr(@__vectorptr64Ref)];
				NativeMethods.GetVectorDataIntPtr(@__vectorptr64Ref, __vectorptr64Ref__);
				var __boolRef__ = Convert.ToBoolean(@__boolRef);
				var __plgStringRef__ = NativeMethods.GetStringData(@__plgStringRef);

				var __result__ = func(ref @__int32Ref, ref __vectorptr64Ref__, ref __boolRef__, ref @__uint8Ref, ref __plgStringRef__, ref @__plgVector3Ref, ref @__int64Ref, ref @__plgVector4Ref, ref @__uint16Ref);

				NativeMethods.AssignVectorIntPtr(@__vectorptr64Ref, __vectorptr64Ref__, __vectorptr64Ref__.Length);
				@__boolRef = Convert.ToByte(__boolRef__);
				NativeMethods.AssignString(@__plgStringRef, __plgStringRef__);
				return __result__;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc25Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc25Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func26, string> CallFunc26Callback = &___CallFunc26Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc26Callback;
		private static string ___CallFunc26Callback(Func26 func)
		{
			var CallFunc26Callback_func = s_DelegateHolder.GetOrAdd(func, new Func26Wrapper((ref ushort @__char16Ref, ref Vector2 @__plgVector2Ref, ref Matrix4x4 @__plgMatrix4x4Ref, nint @__vectorFloatRef, ref short @__int16Ref, ref ulong @__uint64Ref, ref uint @__uint32Ref, nint @__vectorUInt16Ref, ref nint @__ptr64Ref, ref byte @__boolRef) => {
				var __char16Ref__ = Convert.ToChar(@__char16Ref);
				var __vectorFloatRef__ = new float[NativeMethods.GetVectorSizeFloat(@__vectorFloatRef)];
				NativeMethods.GetVectorDataFloat(@__vectorFloatRef, __vectorFloatRef__);
				var __vectorUInt16Ref__ = new ushort[NativeMethods.GetVectorSizeUInt16(@__vectorUInt16Ref)];
				NativeMethods.GetVectorDataUInt16(@__vectorUInt16Ref, __vectorUInt16Ref__);
				var __boolRef__ = Convert.ToBoolean(@__boolRef);

				var __result__ = func(ref __char16Ref__, ref @__plgVector2Ref, ref @__plgMatrix4x4Ref, ref __vectorFloatRef__, ref @__int16Ref, ref @__uint64Ref, ref @__uint32Ref, ref __vectorUInt16Ref__, ref @__ptr64Ref, ref __boolRef__);

				@__char16Ref = Convert.ToUInt16(__char16Ref__);
				NativeMethods.AssignVectorFloat(@__vectorFloatRef, __vectorFloatRef__, __vectorFloatRef__.Length);
				NativeMethods.AssignVectorUInt16(@__vectorUInt16Ref, __vectorUInt16Ref__, __vectorUInt16Ref__.Length);
				@__boolRef = Convert.ToByte(__boolRef__);
				return Convert.ToSByte(__result__);
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc26Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc26Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func27, string> CallFunc27Callback = &___CallFunc27Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc27Callback;
		private static string ___CallFunc27Callback(Func27 func)
		{
			var CallFunc27Callback_func = s_DelegateHolder.GetOrAdd(func, new Func27Wrapper((ref float @__floatRef, ref Vector3 @__plgVector3Ref, ref nint @__ptr64Ref, ref Vector2 @__plgVector2Ref, nint @__vectorInt16Ref, ref Matrix4x4 @__plgMatrix4x4Ref, ref byte @__boolRef, ref Vector4 @__plgVector4Ref, ref sbyte @__int8Ref, ref int @__int32Ref, nint @__vectorUInt8Ref) => {
				var __vectorInt16Ref__ = new short[NativeMethods.GetVectorSizeInt16(@__vectorInt16Ref)];
				NativeMethods.GetVectorDataInt16(@__vectorInt16Ref, __vectorInt16Ref__);
				var __boolRef__ = Convert.ToBoolean(@__boolRef);
				var __vectorUInt8Ref__ = new byte[NativeMethods.GetVectorSizeUInt8(@__vectorUInt8Ref)];
				NativeMethods.GetVectorDataUInt8(@__vectorUInt8Ref, __vectorUInt8Ref__);

				var __result__ = func(ref @__floatRef, ref @__plgVector3Ref, ref @__ptr64Ref, ref @__plgVector2Ref, ref __vectorInt16Ref__, ref @__plgMatrix4x4Ref, ref __boolRef__, ref @__plgVector4Ref, ref @__int8Ref, ref @__int32Ref, ref __vectorUInt8Ref__);

				NativeMethods.AssignVectorInt16(@__vectorInt16Ref, __vectorInt16Ref__, __vectorInt16Ref__.Length);
				@__boolRef = Convert.ToByte(__boolRef__);
				NativeMethods.AssignVectorUInt8(@__vectorUInt8Ref, __vectorUInt8Ref__, __vectorUInt8Ref__.Length);
				return __result__;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc27Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc27Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func28, string> CallFunc28Callback = &___CallFunc28Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc28Callback;
		private static string ___CallFunc28Callback(Func28 func)
		{
			var CallFunc28Callback_func = s_DelegateHolder.GetOrAdd(func, new Func28Wrapper((nint @__output, ref nint @__ptr64Ref, ref ushort @__uint16Ref, nint @__vectorUInt32Ref, ref Matrix4x4 @__plgMatrix4x4Ref, ref float @__floatRef, ref Vector4 @__plgVector4Ref, nint @__plgStringRef, nint @__vectorUInt64Ref, ref long @__int64Ref, ref byte @__boolRef, ref Vector3 @__plgVector3Ref, nint @__vectorFloatRef) => {
				var __vectorUInt32Ref__ = new uint[NativeMethods.GetVectorSizeUInt32(@__vectorUInt32Ref)];
				NativeMethods.GetVectorDataUInt32(@__vectorUInt32Ref, __vectorUInt32Ref__);
				var __plgStringRef__ = NativeMethods.GetStringData(@__plgStringRef);
				var __vectorUInt64Ref__ = new ulong[NativeMethods.GetVectorSizeUInt64(@__vectorUInt64Ref)];
				NativeMethods.GetVectorDataUInt64(@__vectorUInt64Ref, __vectorUInt64Ref__);
				var __boolRef__ = Convert.ToBoolean(@__boolRef);
				var __vectorFloatRef__ = new float[NativeMethods.GetVectorSizeFloat(@__vectorFloatRef)];
				NativeMethods.GetVectorDataFloat(@__vectorFloatRef, __vectorFloatRef__);

				var __result__ = func(ref @__ptr64Ref, ref @__uint16Ref, ref __vectorUInt32Ref__, ref @__plgMatrix4x4Ref, ref @__floatRef, ref @__plgVector4Ref, ref __plgStringRef__, ref __vectorUInt64Ref__, ref @__int64Ref, ref __boolRef__, ref @__plgVector3Ref, ref __vectorFloatRef__);

				NativeMethods.ConstructString(@__output, __result__);
				NativeMethods.AssignVectorUInt32(@__vectorUInt32Ref, __vectorUInt32Ref__, __vectorUInt32Ref__.Length);
				NativeMethods.AssignString(@__plgStringRef, __plgStringRef__);
				NativeMethods.AssignVectorUInt64(@__vectorUInt64Ref, __vectorUInt64Ref__, __vectorUInt64Ref__.Length);
				@__boolRef = Convert.ToByte(__boolRef__);
				NativeMethods.AssignVectorFloat(@__vectorFloatRef, __vectorFloatRef__, __vectorFloatRef__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc28Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc28Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func29, string> CallFunc29Callback = &___CallFunc29Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc29Callback;
		private static string ___CallFunc29Callback(Func29 func)
		{
			var CallFunc29Callback_func = s_DelegateHolder.GetOrAdd(func, new Func29Wrapper((nint @__output, ref Vector4 @__plgVector4Ref, ref int @__int32Ref, nint @__vectorInt8Ref, ref double @__doubleRef, ref byte @__boolRef, ref sbyte @__int8Ref, nint @__vectorUInt16Ref, ref float @__floatRef, nint @__plgStringRef, ref Matrix4x4 @__plgMatrix4x4Ref, ref ulong @__uint64Ref, ref Vector3 @__plgVector3Ref, nint @__vectorInt64Ref) => {
				var __vectorInt8Ref__ = new sbyte[NativeMethods.GetVectorSizeInt8(@__vectorInt8Ref)];
				NativeMethods.GetVectorDataInt8(@__vectorInt8Ref, __vectorInt8Ref__);
				var __boolRef__ = Convert.ToBoolean(@__boolRef);
				var __vectorUInt16Ref__ = new ushort[NativeMethods.GetVectorSizeUInt16(@__vectorUInt16Ref)];
				NativeMethods.GetVectorDataUInt16(@__vectorUInt16Ref, __vectorUInt16Ref__);
				var __plgStringRef__ = NativeMethods.GetStringData(@__plgStringRef);
				var __vectorInt64Ref__ = new long[NativeMethods.GetVectorSizeInt64(@__vectorInt64Ref)];
				NativeMethods.GetVectorDataInt64(@__vectorInt64Ref, __vectorInt64Ref__);

				var __result__ = func(ref @__plgVector4Ref, ref @__int32Ref, ref __vectorInt8Ref__, ref @__doubleRef, ref __boolRef__, ref @__int8Ref, ref __vectorUInt16Ref__, ref @__floatRef, ref __plgStringRef__, ref @__plgMatrix4x4Ref, ref @__uint64Ref, ref @__plgVector3Ref, ref __vectorInt64Ref__);

				NativeMethods.ConstructVectorString(@__output, __result__, __result__.Length);
				NativeMethods.AssignVectorInt8(@__vectorInt8Ref, __vectorInt8Ref__, __vectorInt8Ref__.Length);
				@__boolRef = Convert.ToByte(__boolRef__);
				NativeMethods.AssignVectorUInt16(@__vectorUInt16Ref, __vectorUInt16Ref__, __vectorUInt16Ref__.Length);
				NativeMethods.AssignString(@__plgStringRef, __plgStringRef__);
				NativeMethods.AssignVectorInt64(@__vectorInt64Ref, __vectorInt64Ref__, __vectorInt64Ref__.Length);
				return @__output;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc29Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc29Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func30, string> CallFunc30Callback = &___CallFunc30Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc30Callback;
		private static string ___CallFunc30Callback(Func30 func)
		{
			var CallFunc30Callback_func = s_DelegateHolder.GetOrAdd(func, new Func30Wrapper((ref nint @__ptr64Ref, ref Vector4 @__plgVector4Ref, ref long @__int64Ref, nint @__vectorUInt32Ref, ref byte @__boolRef, nint @__plgStringRef, ref Vector3 @__plgVector3Ref, nint @__vectorUInt8Ref, ref float @__floatRef, ref Vector2 @__plgVector2Ref, ref Matrix4x4 @__plgMatrix4x4Ref, ref sbyte @__int8Ref, nint @__vectorFloatRef, ref double @__doubleRef) => {
				var __vectorUInt32Ref__ = new uint[NativeMethods.GetVectorSizeUInt32(@__vectorUInt32Ref)];
				NativeMethods.GetVectorDataUInt32(@__vectorUInt32Ref, __vectorUInt32Ref__);
				var __boolRef__ = Convert.ToBoolean(@__boolRef);
				var __plgStringRef__ = NativeMethods.GetStringData(@__plgStringRef);
				var __vectorUInt8Ref__ = new byte[NativeMethods.GetVectorSizeUInt8(@__vectorUInt8Ref)];
				NativeMethods.GetVectorDataUInt8(@__vectorUInt8Ref, __vectorUInt8Ref__);
				var __vectorFloatRef__ = new float[NativeMethods.GetVectorSizeFloat(@__vectorFloatRef)];
				NativeMethods.GetVectorDataFloat(@__vectorFloatRef, __vectorFloatRef__);

				var __result__ = func(ref @__ptr64Ref, ref @__plgVector4Ref, ref @__int64Ref, ref __vectorUInt32Ref__, ref __boolRef__, ref __plgStringRef__, ref @__plgVector3Ref, ref __vectorUInt8Ref__, ref @__floatRef, ref @__plgVector2Ref, ref @__plgMatrix4x4Ref, ref @__int8Ref, ref __vectorFloatRef__, ref @__doubleRef);

				NativeMethods.AssignVectorUInt32(@__vectorUInt32Ref, __vectorUInt32Ref__, __vectorUInt32Ref__.Length);
				@__boolRef = Convert.ToByte(__boolRef__);
				NativeMethods.AssignString(@__plgStringRef, __plgStringRef__);
				NativeMethods.AssignVectorUInt8(@__vectorUInt8Ref, __vectorUInt8Ref__, __vectorUInt8Ref__.Length);
				NativeMethods.AssignVectorFloat(@__vectorFloatRef, __vectorFloatRef__, __vectorFloatRef__.Length);
				return __result__;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc30Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc30Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func31, string> CallFunc31Callback = &___CallFunc31Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc31Callback;
		private static string ___CallFunc31Callback(Func31 func)
		{
			var CallFunc31Callback_func = s_DelegateHolder.GetOrAdd(func, new Func31Wrapper((ref sbyte @__charRef, ref uint @__uint32Ref, nint @__vectorUInt64Ref, ref Vector4 @__plgVector4Ref, nint @__plgStringRef, ref byte @__boolRef, ref long @__int64Ref, ref Vector2 @__vec2Ref, ref sbyte @__int8Ref, ref ushort @__uint16Ref, nint @__vectorInt16Ref, ref Matrix4x4 @__mat4x4Ref, ref Vector3 @__vec3Ref, ref float @__floatRef, nint @__vectorDoubleRef) => {
				var __charRef__ = Convert.ToChar(@__charRef);
				var __vectorUInt64Ref__ = new ulong[NativeMethods.GetVectorSizeUInt64(@__vectorUInt64Ref)];
				NativeMethods.GetVectorDataUInt64(@__vectorUInt64Ref, __vectorUInt64Ref__);
				var __plgStringRef__ = NativeMethods.GetStringData(@__plgStringRef);
				var __boolRef__ = Convert.ToBoolean(@__boolRef);
				var __vectorInt16Ref__ = new short[NativeMethods.GetVectorSizeInt16(@__vectorInt16Ref)];
				NativeMethods.GetVectorDataInt16(@__vectorInt16Ref, __vectorInt16Ref__);
				var __vectorDoubleRef__ = new double[NativeMethods.GetVectorSizeDouble(@__vectorDoubleRef)];
				NativeMethods.GetVectorDataDouble(@__vectorDoubleRef, __vectorDoubleRef__);

				var __result__ = func(ref __charRef__, ref @__uint32Ref, ref __vectorUInt64Ref__, ref @__plgVector4Ref, ref __plgStringRef__, ref __boolRef__, ref @__int64Ref, ref @__vec2Ref, ref @__int8Ref, ref @__uint16Ref, ref __vectorInt16Ref__, ref @__mat4x4Ref, ref @__vec3Ref, ref @__floatRef, ref __vectorDoubleRef__);

				@__charRef = Convert.ToSByte(__charRef__);
				NativeMethods.AssignVectorUInt64(@__vectorUInt64Ref, __vectorUInt64Ref__, __vectorUInt64Ref__.Length);
				NativeMethods.AssignString(@__plgStringRef, __plgStringRef__);
				@__boolRef = Convert.ToByte(__boolRef__);
				NativeMethods.AssignVectorInt16(@__vectorInt16Ref, __vectorInt16Ref__, __vectorInt16Ref__.Length);
				NativeMethods.AssignVectorDouble(@__vectorDoubleRef, __vectorDoubleRef__, __vectorDoubleRef__.Length);
				return __result__;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc31Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc31Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
		internal static delegate* <Func32, string> CallFunc32Callback = &___CallFunc32Callback;
		internal static delegate* unmanaged[Cdecl]<nint, nint, void> __CallFunc32Callback;
		private static string ___CallFunc32Callback(Func32 func)
		{
			var CallFunc32Callback_func = s_DelegateHolder.GetOrAdd(func, new Func32Wrapper((ref int @__p1, ref ushort @__p2, nint @__p3, ref Vector4 @__p4, ref nint @__p5, nint @__p6, ref Matrix4x4 @__p7, ref ulong @__p8, nint @__p9, ref long @__p10, ref Vector2 @__p11, nint @__p12, ref byte @__p13, ref Vector3 @__p14, ref byte @__p15, nint @__p16) => {
				var __p3__ = new sbyte[NativeMethods.GetVectorSizeInt8(@__p3)];
				NativeMethods.GetVectorDataInt8(@__p3, __p3__);
				var __p6__ = new uint[NativeMethods.GetVectorSizeUInt32(@__p6)];
				NativeMethods.GetVectorDataUInt32(@__p6, __p6__);
				var __p9__ = NativeMethods.GetStringData(@__p9);
				var __p12__ = new sbyte[NativeMethods.GetVectorSizeInt8(@__p12)];
				NativeMethods.GetVectorDataInt8(@__p12, __p12__);
				var __p13__ = Convert.ToBoolean(@__p13);
				var __p16__ = new char[NativeMethods.GetVectorSizeChar16(@__p16)];
				NativeMethods.GetVectorDataChar16(@__p16, __p16__);

				var __result__ = func(ref @__p1, ref @__p2, ref __p3__, ref @__p4, ref @__p5, ref __p6__, ref @__p7, ref @__p8, ref __p9__, ref @__p10, ref @__p11, ref __p12__, ref __p13__, ref @__p14, ref @__p15, ref __p16__);

				NativeMethods.AssignVectorInt8(@__p3, __p3__, __p3__.Length);
				NativeMethods.AssignVectorUInt32(@__p6, __p6__, __p6__.Length);
				NativeMethods.AssignString(@__p9, __p9__);
				NativeMethods.AssignVectorInt8(@__p12, __p12__, __p12__.Length);
				@__p13 = Convert.ToByte(__p13__);
				NativeMethods.AssignVectorChar16(@__p16, __p16__, __p16__.Length);
				return __result__;
			}));
			var __output = NativeMethods.AllocateString();

			__CallFunc32Callback(__output, Marshal.GetFunctionPointerForDelegate(CallFunc32Callback_func));

			var output = NativeMethods.GetStringData(__output);

			NativeMethods.FreeString(__output);

			return output;
		}
	}
#pragma warning restore CS0649
}
