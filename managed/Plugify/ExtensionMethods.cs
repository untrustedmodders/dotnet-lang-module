namespace Plugify;

internal static class ExtensionMethods
{
	public static bool IsDelegate(this Type type)
	{
		return typeof(MulticastDelegate).IsAssignableFrom(type.BaseType);
	}
	
	public static bool IsChar(this Type type)
	{
		var elementType = type.IsByRef ? type.GetElementType() : type;
		return elementType == typeof(char) || elementType == typeof(char[]);
	}
}
