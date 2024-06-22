using System;

namespace Plugify;

[AttributeUsage(AttributeTargets.Class)]
public class MinimumApiVersion : System.Attribute
{
	public int Version { get; }

	/// <summary>
	/// API version that this plugin requires to work correctly.
	/// </summary>
	/// <param name="version"></param>
	public MinimumApiVersion(int version)
	{
		Version = version;
	}
}