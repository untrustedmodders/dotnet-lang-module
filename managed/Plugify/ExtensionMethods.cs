using System;

namespace Plugify;

public static class ExtensionMethods
{
	public static bool IsDelegate(this Type type)
	{
		return typeof(MulticastDelegate).IsAssignableFrom(type.BaseType);
	}
	
	public static bool IsChar(this Type type)
	{
		return type == typeof(char) || type == typeof(char[]);
	}
}
