using System.Runtime.InteropServices;

namespace Plugify;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.ReturnValue)]
public sealed class CharSetAttribute : Attribute
{
	public CharSetAttribute(CharSet charSet)
	{
		Value = charSet;
	}
	
	public CharSet Value;
}