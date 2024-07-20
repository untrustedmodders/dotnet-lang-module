using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Plugify;

internal class UniqueIdList<T>
{
	private readonly Dictionary<int, T> _objects = new();

	public bool Contains(int id)
	{
		return _objects.ContainsKey(id);
	}

	public int Add(T? obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException(nameof(obj));
		}

		int hashCode = RuntimeHelpers.GetHashCode(obj);
		_ = _objects.TryAdd(hashCode, obj);
		return hashCode;
	}

	public bool TryGetValue(int id, [MaybeNullWhen(false)] out T obj)
	{
		return _objects.TryGetValue(id, out obj);
	}

	public void Clear()
	{
		_objects.Clear();
	}
}
