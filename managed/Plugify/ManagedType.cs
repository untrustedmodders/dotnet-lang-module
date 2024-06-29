using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Plugify;

[StructLayout(LayoutKind.Sequential, Size = 2)]
public struct ManagedType
{
    private byte valueType;
    private byte reference;

	public ValueType ValueType => (ValueType) valueType;
	public bool IsByRef => reference == 1;

	public ManagedType(Type type, object[] attributes)
	{
		if (type.IsByRef)
		{
			type = TypeUtils.ConvertToUnrefType(type);
			reference = 1;
		}
		else
		{
			reference = 0;
		}

		var vt = TypeUtils.ConvertToValueType(type);
		switch (vt)
		{
			case ValueType.Char16 when TypeUtils.IsUseAnsi(attributes):
				vt = ValueType.Char8;
				break;
			case ValueType.ArrayChar16 when TypeUtils.IsUseAnsi(attributes):
				vt = ValueType.ArrayChar8;
				break;
		}
		valueType = (byte) vt;
	}
}

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
	    if (TypeSwitcher.TryGetValue(type, out var valueType))
	    {
		    return valueType;
	    }

	    return type.IsDelegate() ? ValueType.Function : ValueType.Invalid;
    }
    
    internal static bool IsUseAnsi(object[] customAttributes)
    {
	    foreach (var a in customAttributes)
	    {
		    if (a is MarshalAsAttribute attribute)
		    {
			    return attribute.Value is UnmanagedType.I1 or UnmanagedType.U1;
		    }
	    }

	    return false;
    }

    internal static Type ConvertToUnrefType(Type paramType)
    {
	    string? paramName = paramType.FullName;
	    var type = paramName is { Length: > 0 } ? Type.GetType(paramName[..^1]) : null;
	    if (type == null)
	    {
		    throw new NullReferenceException("Reference type not exist");
	    }
	    return type;
    }
    
    internal static bool IsDelegate(this Type paramType)
    {
	    return typeof(Delegate).IsAssignableFrom(paramType);
    }
}