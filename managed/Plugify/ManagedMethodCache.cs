using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Plugify;

internal struct StoredManagedMethod
{
    public MethodInfo methodInfo;
    public Guid assemblyGuid;
}

internal class ManagedMethodCache
{
    private static ManagedMethodCache? _instance = null;

    public static ManagedMethodCache Instance
    {
        get { return _instance ??= new ManagedMethodCache(); }
    }

    private readonly Dictionary<Guid, StoredManagedMethod> _methodCache = new();

    public MethodInfo? GetMethod(Guid guid)
    {
        return _methodCache.TryGetValue(guid, out var value) ? value.methodInfo : null;
    }

    public void AddMethod(Guid assemblyGuid, Guid methodGuid, MethodInfo methodInfo)
    {
        if (_methodCache.TryGetValue(methodGuid, out var value))
        {
            if (value.assemblyGuid == assemblyGuid)
            {
                return;
            }

            throw new Exception("Method already exists in cache for a different assembly!");
        }

        _methodCache.Add(methodGuid, new StoredManagedMethod
        {
            methodInfo = methodInfo,
            assemblyGuid = assemblyGuid
        });
    }

    public int RemoveMethodsForAssembly(Guid assemblyGuid)
    {
        List<Guid> keysToRemove = [];

        foreach (KeyValuePair<Guid, StoredManagedMethod> kvp in _methodCache)
        {
            if (kvp.Value.assemblyGuid == assemblyGuid)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        int numKeysToRemove = keysToRemove.Count;

        foreach (Guid key in keysToRemove)
        {
            _methodCache.Remove(key);
        }

        return numKeysToRemove;
    }
}