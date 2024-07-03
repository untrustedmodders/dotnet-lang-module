namespace Plugify;

internal struct StoredManagedType
{
    public Type type;
    public Guid assemblyGuid;
}

internal class ManagedTypeCache
{
    private static ManagedTypeCache? _instance = null;

    public static ManagedTypeCache Instance
    {
        get { return _instance ??= new ManagedTypeCache(); }
    }

    private readonly Dictionary<Guid, StoredManagedType> _typeCache = new();

    public Type? GetType(Guid guid)
    {
        return _typeCache.TryGetValue(guid, out var value) ? value.type : null;
    }

    public void AddType(Guid assemblyGuid, Guid typeGuid, Type type)
    {
        if (_typeCache.TryGetValue(typeGuid, out var value))
        {
            if (value.assemblyGuid == assemblyGuid)
            {
                return;
            }

            throw new Exception("Method already exists in cache for a different assembly!");
        }

        _typeCache.Add(typeGuid, new StoredManagedType
        {
            type = type,
            assemblyGuid = assemblyGuid
        });
    }

    public int RemoveTypesForAssembly(Guid assemblyGuid)
    {
        List<Guid> keysToRemove = [];

        foreach (KeyValuePair<Guid, StoredManagedType> kvp in _typeCache)
        {
            if (kvp.Value.assemblyGuid == assemblyGuid)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        int numKeysToRemove = keysToRemove.Count;

        foreach (Guid key in keysToRemove)
        {
            _typeCache.Remove(key);
        }

        return numKeysToRemove;
    }
}
