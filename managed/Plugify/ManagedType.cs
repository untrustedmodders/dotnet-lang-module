using System.Runtime.InteropServices;

namespace Plugify;

[StructLayout(LayoutKind.Sequential, Size = 2)]
public struct ManagedType
{
	private byte valueType;
	private byte reference;

	public ValueType ValueType => (ValueType) valueType;
	public bool IsByRef => reference == 1;

	public static ManagedType Invalid => new();

	public ManagedType(Type type, object[] attributes)
	{
		reference = (byte)(type.IsByRef ? 1 : 0);

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
