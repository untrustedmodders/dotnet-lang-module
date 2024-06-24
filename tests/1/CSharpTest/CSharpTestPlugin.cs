using System;
using System.Linq;
using Plugify;
using static cpp_test.cpp_test;

namespace CSharpTest
{
    public class CSharpTestPlugin : Plugin
    {
        public void OnStart() 
        {
	        float epsilon = 0.0001f; // Define a suitable epsilon value
	        double epsilonD = 0.0001; // Define a suitable epsilon value for doubles

	        // No Params, Only Return
	        {
		        NoParamReturnVoid();

		        Assert(NoParamReturnBool() == true, $"Expected NoParamReturnBool() to return true, but got {NoParamReturnBool()}");
				Assert(NoParamReturnChar16() == char.MaxValue, $"Expected NoParamReturnChar16() to return {char.MaxValue}, but got {NoParamReturnChar16()}");
				Assert(NoParamReturnInt8() == sbyte.MaxValue, $"Expected NoParamReturnInt8() to return {sbyte.MaxValue}, but got {NoParamReturnInt8()}");
				Assert(NoParamReturnInt16() == short.MaxValue, $"Expected NoParamReturnInt16() to return {short.MaxValue}, but got {NoParamReturnInt16()}");
				Assert(NoParamReturnInt32() == int.MaxValue, $"Expected NoParamReturnInt32() to return {int.MaxValue}, but got {NoParamReturnInt32()}");
				Assert(NoParamReturnInt64() == long.MaxValue, $"Expected NoParamReturnInt64() to return {long.MaxValue}, but got {NoParamReturnInt64()}");
				Assert(NoParamReturnUInt8() == byte.MaxValue, $"Expected NoParamReturnUInt8() to return {byte.MaxValue}, but got {NoParamReturnUInt8()}");
				Assert(NoParamReturnUInt16() == ushort.MaxValue, $"Expected NoParamReturnUInt16() to return {ushort.MaxValue}, but got {NoParamReturnUInt16()}");
				Assert(NoParamReturnUInt32() == uint.MaxValue, $"Expected NoParamReturnUInt32() to return {uint.MaxValue}, but got {NoParamReturnUInt32()}");
				Assert(NoParamReturnUInt64() == ulong.MaxValue, $"Expected NoParamReturnUInt64() to return {ulong.MaxValue}, but got {NoParamReturnUInt64()}");
				Assert(NoParamReturnPtr64() == (IntPtr)0x1, $"Expected NoParamReturnPtr64() to return {(IntPtr)0x1}, but got {NoParamReturnPtr64()}");
				Assert(Math.Abs(NoParamReturnFloat() - float.MaxValue) < epsilon, $"Expected NoParamReturnFloat() to return {float.MaxValue}, but got {NoParamReturnFloat()}");
				Assert(Math.Abs(NoParamReturnDouble() - double.MaxValue) < epsilonD, $"Expected NoParamReturnDouble() to return {double.MaxValue}, but got {NoParamReturnDouble()}");
				Assert(NoParamReturnFunction() == IntPtr.Zero, $"Expected NoParamReturnFunction() to return IntPtr.Zero, but got {NoParamReturnFunction()}");
				Assert(NoParamReturnString() == "Hello World", $"Expected NoParamReturnString() to return 'Hello World', but got {NoParamReturnString()}");
				Assert(NoParamReturnArrayBool().SequenceEqual(new bool[] { true, false }), $"Expected NoParamReturnArrayBool() to return array [true, false], but got {string.Join(", ", NoParamReturnArrayBool())}");
				Assert(NoParamReturnArrayChar16().SequenceEqual(new char[] { 'a', 'b', 'c', 'd' }), $"Expected NoParamReturnArrayChar16() to return array ['a', 'b', 'c', 'd'], but got {string.Join(", ", NoParamReturnArrayChar16())}");
				Assert(NoParamReturnArrayInt8().SequenceEqual(new sbyte[] { -3, -2, -1, 0, 1 }), $"Expected NoParamReturnArrayInt8() to return array [-3, -2, -1, 0, 1], but got {string.Join(", ", NoParamReturnArrayInt8())}");
				Assert(NoParamReturnArrayInt16().SequenceEqual(new short[] { -4, -3, -2, -1, 0, 1 }), $"Expected NoParamReturnArrayInt16() to return array [-4, -3, -2, -1, 0, 1], but got {string.Join(", ", NoParamReturnArrayInt16())}");
				Assert(NoParamReturnArrayInt32().SequenceEqual(new int[] { -5, -4, -3, -2, -1, 0, 1 }), $"Expected NoParamReturnArrayInt32() to return array [-5, -4, -3, -2, -1, 0, 1], but got {string.Join(", ", NoParamReturnArrayInt32())}");
				Assert(NoParamReturnArrayInt64().SequenceEqual(new long[] { -6, -5, -4, -3, -2, -1, 0, 1 }), $"Expected NoParamReturnArrayInt64() to return array [-6, -5, -4, -3, -2, -1, 0, 1], but got {string.Join(", ", NoParamReturnArrayInt64())}");
				Assert(NoParamReturnArrayUInt8().SequenceEqual(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }), $"Expected NoParamReturnArrayUInt8() to return array [0, 1, 2, 3, 4, 5, 6, 7, 8], but got {string.Join(", ", NoParamReturnArrayUInt8())}");
				Assert(NoParamReturnArrayUInt16().SequenceEqual(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), $"Expected NoParamReturnArrayUInt16() to return array [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], but got {string.Join(", ", NoParamReturnArrayUInt16())}");
				Assert(NoParamReturnArrayUInt32().SequenceEqual(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }), $"Expected NoParamReturnArrayUInt32() to return array [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], but got {string.Join(", ", NoParamReturnArrayUInt32())}");
				Assert(NoParamReturnArrayUInt64().SequenceEqual(new ulong[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }), $"Expected NoParamReturnArrayUInt64() to return array [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], but got {string.Join(", ", NoParamReturnArrayUInt64())}");
				Assert(NoParamReturnArrayPtr64().SequenceEqual(new IntPtr[] { IntPtr.Zero, (IntPtr)1, (IntPtr)2, (IntPtr)3 }), $"Expected NoParamReturnArrayPtr64() to return array [IntPtr.Zero, (IntPtr)1, (IntPtr)2, (IntPtr)3], but got {string.Join(", ", NoParamReturnArrayPtr64())}");
				Assert(NoParamReturnArrayFloat().SequenceEqual(new float[] { -12.34f, 0.0f, 12.34f }), $"Expected NoParamReturnArrayFloat() to return values [-12.34, 0.0, 12.34] but got {string.Join(", ", NoParamReturnArrayFloat())}");
				Assert(NoParamReturnArrayDouble().SequenceEqual(new double[] { -12.345, 0.0, 12.345 }), $"Expected NoParamReturnArrayDouble() to return values [-12.345, 0.0, 12.345] but got {string.Join(", ", NoParamReturnArrayDouble())}");
				Assert(NoParamReturnArrayString().SequenceEqual(new string[] { "1st string", "2nd string", "3rd element string (Should be big enough to avoid small string optimization)" }), $"Expected NoParamReturnArrayString() to return values ['1st string', '2nd string', '3rd element string (Should be big enough to avoid small string optimization)'] but got {string.Join(", ", NoParamReturnArrayString())}");
				// Assertion for vector and matrix types could be added once the appropriate types are defined or imported.
				Assert(NoParamReturnVector2() == new Vector2(1, 2), $"Expected NoParamReturnVector2() to return values [1.0, 2.0] but got {NoParamReturnVector2().ToString()}");
				Assert(NoParamReturnVector3() == new Vector3(1, 2, 3), $"Expected NoParamReturnVector3() to return values [1.0, 2.0, 3.0] but got {NoParamReturnVector3().ToString()}");
				Assert(NoParamReturnVector4() == new Vector4(1, 2, 3, 4), $"Expected NoParamReturnVector4() to return values [1.0, 2.0, 3.0, 4.0] but got {NoParamReturnVector4().ToString()}");
				Assert(NoParamReturnMatrix4x4() == new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16), $"Expected NoParamReturnMatrix4x4() to return values [1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0] but got {NoParamReturnMatrix4x4().ToString()}");
            }
	        
	        int intValue = 42;
	        float floatValue = 3.14f;
	        double doubleValue = 6.28;
	        Vector4 vector4Value = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            long[] longListValue = new long[] { 100, 200, 300 };
	        char charValue = 'A';
	        string stringValue = "Hello";
	        float floatValue2 = 2.71f;
	        short shortValue = 10;
	        IntPtr ptrValue = IntPtr.Zero; // Provide appropriate value for pointer
	        
	        // Params (no refs)
	        {
		        Param1(intValue);
		        Param2(intValue, floatValue);
		        Param3(intValue, floatValue, doubleValue);
		        Param4(intValue, floatValue, doubleValue, vector4Value);
		        Param5(intValue, floatValue, doubleValue, vector4Value, longListValue);
		        Param6(intValue, floatValue, doubleValue, vector4Value, longListValue, charValue);
		        Param7(intValue, floatValue, doubleValue, vector4Value, longListValue, charValue, stringValue);
		        Param8(intValue, floatValue, doubleValue, vector4Value, longListValue, charValue, stringValue, floatValue2);
		        Param9(intValue, floatValue, doubleValue, vector4Value, longListValue, charValue, stringValue, floatValue2, shortValue);
		        Param10(intValue, floatValue, doubleValue, vector4Value, longListValue, charValue, stringValue, floatValue2, shortValue, ptrValue);
	        }
	        
	        // Params (with refs)
	        {
				// ParamRef1
			    ParamRef1(ref intValue);
			    Assert(intValue == 42, $"Expected intValue to be 42, but got {intValue}");

			    // ParamRef2
			    ParamRef2(ref intValue, ref floatValue);
			    Assert(intValue == 10, $"Expected intValue to be 10, but got {intValue}");
			    Assert(Math.Abs(floatValue - 3.14f) < epsilon, $"Expected floatValue to be approximately 3.14, but got {floatValue}");

			    // ParamRef3
			    ParamRef3(ref intValue, ref floatValue, ref doubleValue);
			    Assert(intValue == -20, $"Expected intValue to be -20, but got {intValue}");
			    Assert(Math.Abs(floatValue - 2.718f) < epsilon, $"Expected floatValue to be approximately 2.718, but got {floatValue}");
			    Assert(Math.Abs(doubleValue - 3.14159) < epsilonD, $"Expected doubleValue to be approximately 3.14159, but got {doubleValue}");

			    // ParamRef4
			    ParamRef4(ref intValue, ref floatValue, ref doubleValue, ref vector4Value);
			    Assert(intValue == 100, $"Expected intValue to be 100, but got {intValue}");
			    Assert(Math.Abs(floatValue + 5.55f) < epsilon, $"Expected floatValue to be approximately -5.55, but got {floatValue}");
			    Assert(Math.Abs(doubleValue - 1.618) < epsilonD, $"Expected doubleValue to be approximately 1.618, but got {doubleValue}");
			    Assert(vector4Value == new Vector4(1, 2, 3, 4), $"Expected vector4Value to be (1, 2, 3, 4), but got {vector4Value}");

			    // ParamRef5
			    ParamRef5(ref intValue, ref floatValue, ref doubleValue, ref vector4Value, ref longListValue);
			    Assert(intValue == 500, $"Expected intValue to be 500, but got {intValue}");
			    Assert(Math.Abs(floatValue + 10.5f) < epsilon, $"Expected floatValue to be approximately -10.5, but got {floatValue}");
			    Assert(Math.Abs(doubleValue - 2.71828) < epsilonD, $"Expected doubleValue to be approximately 2.71828, but got {doubleValue}");
			    Assert(vector4Value == new Vector4(-1, -2, -3, -4), $"Expected vector4Value to be (-1, -2, -3, -4), but got {vector4Value}");
			    Assert(longListValue.SequenceEqual(new long[] { -6, -5, -4, -3, -2, -1, 0, 1 }), $"Expected longListValue to be (-6, -5, -4, -3, -2, -1, 0, 1), but got {string.Join(", ", longListValue)}");

			    // ParamRef6
			    ParamRef6(ref intValue, ref floatValue, ref doubleValue, ref vector4Value, ref longListValue, ref charValue);
			    Assert(intValue == 750, $"Expected intValue to be 750, but got {intValue}");
			    Assert(Math.Abs(floatValue - 20.0f) < epsilon, $"Expected floatValue to be approximately 20.0, but got {floatValue}");
			    Assert(Math.Abs(doubleValue - 1.23456) < epsilonD, $"Expected doubleValue to be approximately 1.23456, but got {doubleValue}");
			    Assert(vector4Value == new Vector4(10, 20, 30, 40), $"Expected vector4Value to be (10, 20, 30, 40), but got {vector4Value}");
			    Assert(longListValue.SequenceEqual(new long[] { -6, -5, -4 }), $"Expected longListValue to be (-6, -5, -4), but got {string.Join(", ", longListValue)}");
			    Assert(charValue == 'Z', $"Expected charValue to be 'Z', but got {charValue}");

			    // ParamRef7
			    ParamRef7(ref intValue, ref floatValue, ref doubleValue, ref vector4Value, ref longListValue, ref charValue, ref stringValue);
			    Assert(intValue == -1000, $"Expected intValue to be -1000, but got {intValue}");
			    Assert(Math.Abs(floatValue - 3.0f) < epsilon, $"Expected floatValue to be approximately 3.0, but got {floatValue}");
			    Assert(doubleValue == -1, $"Expected doubleValue to be -1, but got {doubleValue}");
			    Assert(vector4Value == new Vector4(100, 200, 300, 400), $"Expected vector4Value to be (100, 200, 300, 400), but got {vector4Value}");
			    Assert(longListValue.SequenceEqual(new long[] { -6, -5, -4, -3 }), $"Expected longListValue to be (-6, -5, -4, -3), but got {string.Join(", ", longListValue)}");
			    Assert(charValue == 'X', $"Expected charValue to be 'X', but got {charValue}");
			    Assert(stringValue == "Hello, World!", $"Expected stringValue to be 'Hello, World!', but got {stringValue}");

			    // ParamRef8
			    ParamRef8(ref intValue, ref floatValue, ref doubleValue, ref vector4Value, ref longListValue, ref charValue, ref stringValue, ref floatValue2);
			    Assert(intValue == 999, $"Expected intValue to be 999, but got {intValue}");
			    Assert(Math.Abs(floatValue + 7.5f) < epsilon, $"Expected floatValue to be approximately -7.5, but got {floatValue}");
			    Assert(Math.Abs(doubleValue - 0.123456) < 0.000001, $"Expected doubleValue to be approximately 0.123456, but got {doubleValue}");
			    Assert(vector4Value == new Vector4(-100, -200, -300, -400), $"Expected vector4Value to be (-100, -200, -300, -400), but got {vector4Value}");
			    Assert(longListValue.SequenceEqual(new long[] { -6, -5, -4, -3, -2, -1 }), $"Expected longListValue to be (-6, -5, -4, -3, -2, -1), but got {string.Join(", ", longListValue)}");
			    Assert(charValue == 'Y', $"Expected charValue to be 'Y', but got {charValue}");
			    Assert(stringValue == "Goodbye, World!", $"Expected stringValue to be 'Goodbye, World!', but got {stringValue}");
			    Assert(Math.Abs(floatValue2 - 99.99f) < epsilon, $"Expected floatValue2 to be approximately 99.99, but got {floatValue2}");

			    // ParamRef9
			    ParamRef9(ref intValue, ref floatValue, ref doubleValue, ref vector4Value, ref longListValue, ref charValue, ref stringValue, ref floatValue2, ref shortValue);
			    Assert(intValue == -1234, $"Expected intValue to be -1234, but got {intValue}");
			    Assert(Math.Abs(floatValue - 123.45f) < epsilon, $"Expected floatValue to be approximately 123.45, but got {floatValue}");
			    Assert(Math.Abs(doubleValue + 678.9) < epsilonD, $"Expected doubleValue to be approximately -678.9, but got {doubleValue}");
			    Assert(vector4Value == new Vector4(987.65f , 432.1f, 123.456f, 789.123f), $"Expected vector4Value to be (987.65 , 432.1, 123.456, 789.123), but got {vector4Value}");
			    Assert(longListValue.SequenceEqual(new long[]{ -6, -5, -4, -3, -2, -1, 0, 1, 5, 9 }), $"Expected longListValue to be (-6, -5, -4, -3, -2, -1, 0, 1, 5, 9), but got {string.Join(", ", longListValue)}");
			    Assert(charValue == 'A', $"Expected charValue to be 'A', but got {charValue}");
			    Assert(stringValue == "Testing, 1 2 3", $"Expected stringValue to be 'Testing, 1 2 3', but got {stringValue}");
			    Assert(Math.Abs(floatValue2 + 987.654f) < epsilon, $"Expected floatValue2 to be approximately -987.654, but got {floatValue2}");
			    Assert(shortValue == 42, $"Expected shortValue to be 42, but got {shortValue}");

			    // ParamRef10
			    ParamRef10(ref intValue, ref floatValue, ref doubleValue, ref vector4Value, ref longListValue, ref charValue, ref stringValue, ref floatValue2, ref shortValue, ref ptrValue);
			    Assert(intValue == 987, $"Expected intValue to be 987, but got {intValue}");
			    Assert(Math.Abs(floatValue + 0.123f) < epsilon, $"Expected floatValue to be approximately -0.123, but got {floatValue}");
			    Assert(Math.Abs(doubleValue - 456.789) < 0.000001, $"Expected doubleValue to be approximately 456.789, but got {doubleValue}");
			    Assert(vector4Value == new Vector4(-123.456f, 0.987f, 654.321f, -789.123f), $"Expected vector4Value to be (-123.456, 0.987, 654.321, -789.123), but got {vector4Value}");
			    Assert(longListValue.SequenceEqual(new long[] { -6, -5, -4, -3, -2, -1, 0, 1, 5, 9, 4, -7 }), $"Expected longListValue to be (-6, -5, -4, -3, -2, -1, 0, 1, 5, 9, 4, -7), but got {string.Join(", ", longListValue)}");
			    Assert(charValue == 'B', $"Expected charValue to be 'B', but got {charValue}");
			    Assert(stringValue == "Another string", $"Expected stringValue to be 'Another string', but got {stringValue}");
			    Assert(Math.Abs(floatValue2 - 3.141592f) < 0.000001, $"Expected floatValue2 to be approximately 3.141592, but got {floatValue2}");
			    Assert(shortValue == -32768, $"Expected shortValue to be -32768, but got {shortValue}");
			    Assert(ptrValue == IntPtr.Zero, $"Expected ptrValue to be IntPtr.Zero, but got {ptrValue}");
	        }
	        
	        // Initialize arrays
	        bool[] boolArray = { true, false, true };
	        char[] charArray = { 'a', 'b', 'c' };
	        char[] char16Array = { 'A', 'B', 'C' };
	        sbyte[] sbyteArray = { -1, -2, -3 };
	        short[] shortArray = { 10, 20, 30 };
	        int[] intArray = { 100, 200, 300 };
	        long[] longArray = { 1000, 2000, 3000 };
	        byte[] byteArray = { 1, 2, 3 };
	        ushort[] ushortArray = { 1000, 2000, 3000 };
	        uint[] uintArray = { 10000, 20000, 30000 };
	        ulong[] ulongArray = { 100000, 200000, 300000 };
	        IntPtr[] intPtrArray = { IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };
	        float[] floatArray = { 1.1f, 2.2f, 3.3f };
	        double[] doubleArray = { 1.1, 2.2, 3.3 };
	        string[] stringArray = { "Hello", "World", "!" };

	        {
		        // Call function ParamRefVectors and print how values change
		        ParamRefVectors(ref boolArray, ref charArray, ref char16Array, ref sbyteArray, ref shortArray, ref intArray, ref longArray,
			        ref byteArray, ref ushortArray, ref uintArray, ref ulongArray, ref intPtrArray, ref floatArray, ref doubleArray, ref stringArray);
		        
				Assert(boolArray.SequenceEqual(new bool[] { true }), $"Expected boolArray to be (true), but got {string.Join(", ", boolArray)}");
				Assert(charArray.SequenceEqual(new char[] { 'a', 'b', 'c' }), $"Expected charArray to be ('a', 'b', 'c'), but got {string.Join(", ", charArray)}");
				Assert(char16Array.SequenceEqual(new char[] { 'a', 'b', 'c' }), $"Expected char16Array to be ('a', 'b', 'c'), but got {string.Join(", ", char16Array)}");
				Assert(sbyteArray.SequenceEqual(new sbyte[] { -3, -2, -1, 0, 1, 2, 3 }), $"Expected sbyteArray to be (-3, -2, -1, 0, 1, 2, 3), but got {string.Join(", ", sbyteArray)}");
				Assert(shortArray.SequenceEqual(new short[] { -4, -3, -2, -1, 0, 1, 2, 3, 4 }), $"Expected shortArray to be (-4, -3, -2, -1, 0, 1, 2, 3, 4), but got {string.Join(", ", shortArray)}");
				Assert(intArray.SequenceEqual(new int[] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5 }), $"Expected intArray to be (-5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5), but got {string.Join(", ", intArray)}");
				Assert(longArray.SequenceEqual(new long[] { -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6 }), $"Expected longArray to be (-6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6), but got {string.Join(", ", longArray)}");
				Assert(byteArray.SequenceEqual(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }), $"Expected byteArray to be (0, 1, 2, 3, 4, 5, 6, 7), but got {string.Join(", ", byteArray)}");
				Assert(ushortArray.SequenceEqual(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }), $"Expected ushortArray to be (0, 1, 2, 3, 4, 5, 6, 7, 8), but got {string.Join(", ", ushortArray)}");
				Assert(uintArray.SequenceEqual(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), $"Expected uintArray to be (0, 1, 2, 3, 4, 5, 6, 7, 8, 9), but got {string.Join(", ", uintArray)}");
				Assert(ulongArray.SequenceEqual(new ulong[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }), $"Expected ulongArray to be (0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10), but got {string.Join(", ", ulongArray)}");
				Assert(intPtrArray.SequenceEqual(new IntPtr[] { IntPtr.Zero, IntPtr.Zero + 1 }), $"Expected intPtrArray to be (IntPtr.Zero, IntPtr.Zero + 1), but got {string.Join(", ", intPtrArray)}");
				Assert(floatArray.SequenceEqual(new float[] { -12.34f, 0.0f, 12.34f }), $"Expected floatArray to be (-12.34f, 0.0f, 12.34f), but got {string.Join(", ", floatArray)}");
				Assert(doubleArray.SequenceEqual(new double[] { -12.345, 0.0, 12.345 }), $"Expected doubleArray to be (-12.345, 0.0, 12.345), but got {string.Join(", ", doubleArray)}");
				Assert(stringArray.SequenceEqual(new string[] { "Hello", "World", "OpenAI" }), $"Expected stringArray to be ('Hello', 'World', 'OpenAI'), but got {string.Join(", ", stringArray)}");

		        // Call function ParamAllPrimitives and print the returned value
		        long returnValue = ParamAllPrimitives(boolArray[0], charArray[0], sbyteArray[0], shortArray[0], intArray[0], longArray[0],
			        byteArray[0], ushortArray[0], uintArray[0], ulongArray[0], intPtrArray[0], floatArray[0], doubleArray[0]);

		        Assert(returnValue == 56, $"Expected return value to be 56, but got {returnValue}");
	        }
	        
	        Console.WriteLine("All tests passed!");
        }
        
        private void Assert(bool condition, string message)
        {
	        if (!condition)
	        {
		        throw new Exception(message);
	        }
        }
    }
}