namespace Plugify;

public class Plugin : IEquatable<Plugin>, IComparable<Plugin>
{
	public long Id { get; private set; } = -1;
	public string Name { get; private set; } = "";
	public string FullName { get; private set; } = "";
	public string Description { get; private set; } = "";
	public string Version { get; private set; } = "";
	public string Author { get; private set; } = "";
	public string Website { get; private set; } = "";
	public string BaseDir { get; private set; } = "";
	public string[] Dependencies { get; private set; } = [];

	public string FindResource(string path)
	{
		return NativeMethods.FindPluginResource(Name, path);
	}

	public static bool operator ==(Plugin lhs, Plugin rhs)
	{
		return lhs.Id == rhs.Id;
	}

	public static bool operator !=(Plugin lhs, Plugin rhs)
	{
		return lhs.Id != rhs.Id;
	}
		
	public int CompareTo(Plugin? other)
	{
		if (ReferenceEquals(this, other)) return 0;
		if (ReferenceEquals(null, other)) return 1;
		return Id.CompareTo(other.Id);
	}
		
	public bool Equals(Plugin? other)
	{
		return !ReferenceEquals(other, null) && Id == other.Id;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == GetType() && Id == ((Plugin)obj).Id;
	}
		
	public bool IsNull()
	{
		return Id == -1;
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	public override string ToString()
	{
		return IsNull() ? "Plugin.Null" : $"Plugin({Id})";
	}
}
