using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Plugify;

internal class StoredManagedObject : IDisposable
{
    public Guid guid;
    public Guid assemblyGuid;
    public object obj;
    public GCHandle gcHandle;

    public StoredManagedObject(Guid objectGuid, Guid assemblyGuid, object obj)
    {
        this.guid = objectGuid;
        this.assemblyGuid = assemblyGuid;
        this.obj = obj;
        this.gcHandle = GCHandle.Alloc(this.obj);
    }

    public void Dispose()
    {
        gcHandle.Free();
    }

    public ManagedObject ToManagedObject()
    {
        return new ManagedObject
        {
            guid = guid,
            ptr = GCHandle.ToIntPtr(gcHandle)
        };
    }
}

internal class ManagedObjectCache
{
    private static ManagedObjectCache? _instance;

    public static ManagedObjectCache Instance
    {
        get { return _instance ??= new ManagedObjectCache(); }
    }

    private readonly Dictionary<Guid, StoredManagedObject> _objects = new();
    private readonly object _lockObject = new();

    public ManagedObject AddObject(Guid assemblyGuid, Guid objectGuid, object obj)
    {
        Logger.Log(Severity.Debug, $"Adding object {obj} to cache for assembly {assemblyGuid}");

        lock (_lockObject)
        {
            StoredManagedObject storedObject = new StoredManagedObject(objectGuid, assemblyGuid, obj);

            _objects.Add(objectGuid, storedObject);

            return storedObject.ToManagedObject();
        }
    }

    public StoredManagedObject? GetObject(Guid guid)
    {
        lock (_lockObject)
        {
            return _objects.GetValueOrDefault(guid);
        }
    }

    public bool RemoveObject(Guid guid)
    {
        lock (_lockObject)
        {
            if (_objects.TryGetValue(guid, out StoredManagedObject? value))
            {
                value.Dispose();
                _objects.Remove(guid);
                    
                return true;
            }
        }
            
        return false;
    }

    public int RemoveObjectsForAssembly(Guid assemblyGuid)
    {
        lock (_lockObject)
        {
            List<Guid> keysToRemove = [];

            foreach (KeyValuePair<Guid, StoredManagedObject> entry in _objects)
            {
                Logger.Log(Severity.Debug, $"Checking object {entry.Key} for assembly {assemblyGuid}. Object assembly: {entry.Value.assemblyGuid}");

                if (entry.Value.assemblyGuid == assemblyGuid)
                {
                    keysToRemove.Add(entry.Key);
                }
            }

            foreach (Guid key in keysToRemove)
            {
                _objects[key].Dispose();
                _objects.Remove(key);
            }

            return keysToRemove.Count;
        }
    }
}
