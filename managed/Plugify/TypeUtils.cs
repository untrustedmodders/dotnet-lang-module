using System.Numerics;

namespace Plugify;

public enum ValueType : byte {
	Invalid,

	// C types
	Void,
	Bool,
	Char8,
	Char16,
	Int8,
	Int16,
	Int32,
	Int64,
	UInt8,
	UInt16,
	UInt32,
	UInt64,
	Pointer,
	Float,
	Double,
	Function,

	// std::string
	String,

	// std::vector
	ArrayBool,
	ArrayChar8,
	ArrayChar16,
	ArrayInt8,
	ArrayInt16,
	ArrayInt32,
	ArrayInt64,
	ArrayUInt8,
	ArrayUInt16,
	ArrayUInt32,
	ArrayUInt64,
	ArrayPointer,
	ArrayFloat,
	ArrayDouble,
	ArrayString,

	// glm:vec
	Vector2,
	Vector3,
	Vector4,

	// glm:mat
	Matrix4x4,
	//Matrix2x2,
	//Matrix2x3,
	//Matrix2x4,
	//Matrix3x2,
	//Matrix3x3,
	//Matrix3x4,
	//Matrix4x2,
	//Matrix4x3,
};

internal static class TypeUtils
{
    private static readonly Dictionary<Type, ValueType> TypeSwitcher = new()
    {
	    [typeof(void)] = ValueType.Void,
	    [typeof(bool)] = ValueType.Bool,
	    //[typeof(char)] = ValueType.Char8,
	    [typeof(char)] = ValueType.Char16,
	    [typeof(sbyte)] = ValueType.Int8,
	    [typeof(short)] = ValueType.Int16,
	    [typeof(int)] = ValueType.Int32,
	    [typeof(long)] = ValueType.Int64,
	    [typeof(byte)] = ValueType.UInt8,
	    [typeof(ushort)] = ValueType.UInt16,
	    [typeof(uint)] = ValueType.UInt32,
	    [typeof(ulong)] = ValueType.UInt64,
	    [typeof(nuint)] = ValueType.Pointer,
	    [typeof(nint)] = ValueType.Pointer,
	    [typeof(float)] = ValueType.Float,
	    [typeof(double)] = ValueType.Double,
	    [typeof(Delegate)] = ValueType.Function,
	    [typeof(MulticastDelegate)] = ValueType.Function,
	    // std::string
	    [typeof(string)] = ValueType.String,
	    // std::vector
	    [typeof(bool[])] = ValueType.ArrayBool,
	    //[typeof(char[])] = ValueType.ArrayChar8,
	    [typeof(char[])] = ValueType.ArrayChar16,
	    [typeof(sbyte[])] = ValueType.ArrayInt8,
	    [typeof(short[])] = ValueType.ArrayInt16,
	    [typeof(int[])] = ValueType.ArrayInt32,
	    [typeof(long[])] = ValueType.ArrayInt64,
	    [typeof(byte[])] = ValueType.ArrayUInt8,
	    [typeof(ushort[])] = ValueType.ArrayUInt16,
	    [typeof(uint[])] = ValueType.ArrayUInt32,
	    [typeof(ulong[])] = ValueType.ArrayUInt64,
	    [typeof(nuint[])] = ValueType.ArrayPointer,
	    [typeof(nint[])] = ValueType.ArrayPointer,
	    [typeof(float[])] = ValueType.ArrayFloat,
	    [typeof(double[])] = ValueType.ArrayDouble,
	    [typeof(string[])] = ValueType.ArrayString,
	    // glm:vec
	    [typeof(Vector2)] = ValueType.Vector2,
	    [typeof(Vector3)] = ValueType.Vector3,
	    [typeof(Vector4)] = ValueType.Vector4,
	    // glm:mat
	    [typeof(Matrix4x4)] = ValueType.Matrix4x4
    };

    internal static ValueType ConvertToValueType(Type type)
    {
	    if (TypeSwitcher.TryGetValue(type.IsByRef ? type.GetElementType() : type, out var valueType))
	    {
		    return valueType;
	    }

	    return type.IsDelegate() ? ValueType.Function : ValueType.Invalid;
    }

    internal static bool IsUseAnsi(object[] customAttributes)
    {
	    foreach (var attribute in customAttributes)
	    {
		    if (attribute is MarshalAttribute a)
		    {
			    return a.Value is ValueType.Char8 or ValueType.ArrayChar8;
		    }
	    }

	    return false;
    }
}
