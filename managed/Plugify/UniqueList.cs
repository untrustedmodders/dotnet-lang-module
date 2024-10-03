using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Plugify;

internal class UniqueIdList<T>
{
	private readonly Dictionary<nint, T> _objects = new();

	public nint Add(T? obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException(nameof(obj));
		}

		nint handle = GetRuntimeHandle(obj);
		_ = _objects.TryAdd(handle, obj);
		return handle;
	}

	public bool TryGetValue(nint handle, [MaybeNullWhen(false)] out T obj)
	{
		return _objects.TryGetValue(handle, out obj);
	}

	public void Clear()
	{
		_objects.Clear();
	}

	private static nint GetRuntimeHandle(object obj)
	{
		return obj switch
		{
			Type type => type.TypeHandle.Value,
			MethodInfo method => method.MethodHandle.Value,
			FieldInfo field => field.FieldHandle.Value,
			PropertyInfo property => property.GetAccessors(true).First().MethodHandle.Value,
			Attribute attribute => attribute.GetType().TypeHandle.Value,
			_ => throw new NotSupportedException($"Type {nameof(obj.GetType)} not supported for runtime handle")
		};
	}
}