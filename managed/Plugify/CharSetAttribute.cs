using System.Runtime.InteropServices;

namespace Plugify;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public sealed class CharSetAttribute(CharSet charSet) : Attribute
{
    public CharSet CharSet = charSet;
}