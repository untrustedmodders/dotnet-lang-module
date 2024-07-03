using System.Reflection;
using System.Runtime.Loader;

namespace Plugify;

internal class PluginLoadContext(string pluginPath) : AssemblyLoadContext(isCollectible: true)
{
    private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name == "Plugify") 
        {
            return Assembly.Load(assemblyName);
        }
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : nint.Zero;
    }
}
	
internal class AssemblyInstance(Guid guid, string path)
{
    private AssemblyLoadContext? _ctx;
    private Assembly? _assembly;

    public Assembly? Assembly => _assembly;

    public string AssemblyPath => path;

    public Guid Guid => guid;

    public void Load()
    {
        if (_assembly != null)
        {
            return;
        }

        _ctx = new PluginLoadContext(path);
        _assembly = _ctx.LoadFromAssemblyPath(path);

#if DEBUG // Load all referenced assemblies to ensure nothing will crash later
        Logger.Log(Severity.Info, "Loaded assembly: {0}, with {1} referenced assemblies.", _assembly.FullName ??  "<Unknown>", _assembly.GetReferencedAssemblies().Length);

        for (int i = 0; i < _assembly.GetReferencedAssemblies().Length; i++)
        {
            var assembly = _assembly.GetReferencedAssemblies()[i];
	            
            Logger.Log(Severity.Info, "Found referenced assembly: {0}", assembly.Name ?? "<Unknown>");

            _ctx.LoadFromAssemblyName(assembly);
        }
#endif
    }

    public void Unload()
    {
        if (_assembly == null)
        {
            return;
        }

        _ctx?.Unload();

        Logger.Log(Severity.Info, $"Unloaded assembly {guid} from {path}.");

        _assembly = null;
        _ctx = null;
    }
}
	
internal class AssemblyCache
{
    private static AssemblyCache? _instance = null;

    public static AssemblyCache Instance
    {
        get { return _instance ??= new AssemblyCache(); }
    }

    private readonly Dictionary<Guid, AssemblyInstance> _assemblies = new();

    public Dictionary<Guid, AssemblyInstance> Assemblies => _assemblies;

    public AssemblyInstance? Get(Guid guid)
    {
        return _assemblies.GetValueOrDefault(guid);
    }

    public AssemblyInstance? Get(string path)
    {
        foreach (AssemblyInstance assembly in _assemblies.Values)
        {
            if (assembly.AssemblyPath == path)
            {
                return assembly;
            }
        }

        return null;
    }

    public AssemblyInstance Add(Guid guid, string path)
    {
        if (_assemblies.ContainsKey(guid))
        {
            throw new Exception("Assembly already exists in cache");
        }
        
        Logger.Log(Severity.Info, $"Starting assembly loading {guid} from {path}");

        AssemblyInstance assemblyInstance = new AssemblyInstance(guid, path);
        assemblyInstance.Load();

        _assemblies.Add(guid, assemblyInstance);

        Logger.Log(Severity.Info, $"Added assembly {guid} from {path}");

        return assemblyInstance;
    }

    public void Remove(Guid guid)
    {
        if (!_assemblies.TryGetValue(guid, out AssemblyInstance? value))
        {
            throw new Exception("Assembly does not exist in cache");
        }

        value.Unload();
        _assemblies.Remove(guid);

        Logger.Log(Severity.Info, $"Removed assembly {guid}");
    }
}
