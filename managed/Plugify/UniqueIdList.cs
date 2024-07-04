using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Plugify;

internal class UniqueIdList<T>
{
	private readonly Dictionary<int, WeakReference> _objects = new();

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
		_ = _objects.TryAdd(hashCode, new WeakReference(obj, false));
		return hashCode;
	}

	public bool TryGetValue(int id, [MaybeNullWhen(false)] out T obj)
	{
		if (!_objects.TryGetValue(id, out WeakReference? reference))
		{
			obj = default;
			return false;
		}

		object? target = reference.Target;
		
		if (target == null)
		{
			obj = default;
			_objects.Remove(id);
			return false;
		}
		
		obj = (T) target;
		return true;
	}

	public void Clear()
	{
		_objects.Clear();
	}
	
	public void RemoveUnusedObjects()
    {
        List<int> keysToRemove = [];

        foreach (KeyValuePair<int, WeakReference> kvp in _objects)
        {
            if (!kvp.Value.IsAlive)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (int key in keysToRemove)
        {
            _objects.Remove(key);
        }
    }
}
