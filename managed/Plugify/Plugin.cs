namespace Plugify
{
	public abstract class Plugin : IEquatable<Plugin>, IComparable<Plugin>
	{
		private readonly IntPtr _handle;
		public long Id { get; }
		public string Name { get; }
		public string FullName { get; }
		public string Description { get; }
		public string Version { get; }
		public string Author { get; }
		public string Website { get; }
		public string BaseDir { get; }
		public string[] Dependencies { get; }
		
		protected Plugin(IntPtr handle)
		{
			_handle = handle;
			Id = NativeMethods.GetPluginId(_handle);
			Name = NativeMethods.GetPluginName(_handle);
			FullName = NativeMethods.GetPluginFullName(_handle);
			Description = NativeMethods.GetPluginDescription(_handle);
			Version = NativeMethods.GetPluginVersion(_handle);
			Author = NativeMethods.GetPluginAuthor(_handle);
			Website = NativeMethods.GetPluginWebsite(_handle);
			BaseDir = NativeMethods.GetPluginBaseDir(_handle);
			Dependencies = new string[NativeMethods.GetPluginDependenciesSize(_handle)];
			NativeMethods.GetPluginDependencies(_handle, Dependencies);
		}
		
		public string FindResource(string path)
		{
			return NativeMethods.FindPluginResource(_handle, path);
		}

		public abstract void OnStart();
		public abstract void OnEnd();

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
}