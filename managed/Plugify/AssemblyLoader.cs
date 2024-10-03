using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace Plugify;

using static ManagedHost;

internal enum AssemblyLoadStatus
{
	Success, FileNotFound, FileLoadFailure, InvalidFilePath, InvalidAssembly, UnknownError
}

internal class AssemblyInstance(Assembly assembly)
{
	public Assembly Assembly { get; } = assembly;
	public Guid Id { get; } = Guid.NewGuid();
}

internal static class AssemblyLoader
{
	private static readonly Dictionary<Type, AssemblyLoadStatus> AssemblyLoadErrorLookup = new();
	private static readonly Dictionary<Guid, AssemblyLoadContext?> AssemblyContexts = new();
	private static readonly Dictionary<string, AssemblyInstance> AssemblyCache = new();
	private static readonly Dictionary<string, List<GCHandle>> AllocatedHandles = new();
	private static AssemblyLoadStatus LastLoadStatus = AssemblyLoadStatus.Success;
	
	//private static int _contextCounter = 0;
	private static readonly AssemblyLoadContext? PlugifyAssemblyLoadContext;

	static AssemblyLoader()
	{
		AssemblyLoadErrorLookup.Add(typeof(BadImageFormatException), AssemblyLoadStatus.InvalidAssembly);
		AssemblyLoadErrorLookup.Add(typeof(FileNotFoundException), AssemblyLoadStatus.FileNotFound);
		AssemblyLoadErrorLookup.Add(typeof(FileLoadException), AssemblyLoadStatus.FileLoadFailure);
		AssemblyLoadErrorLookup.Add(typeof(ArgumentNullException), AssemblyLoadStatus.InvalidFilePath);
		AssemblyLoadErrorLookup.Add(typeof(ArgumentException), AssemblyLoadStatus.InvalidFilePath);

		PlugifyAssemblyLoadContext = AssemblyLoadContext.GetLoadContext(typeof(AssemblyLoader).Assembly);
		PlugifyAssemblyLoadContext!.Resolving += ResolveAssembly;

		CachePlugifyAssemblies();
	}

	private static void CachePlugifyAssemblies()
	{
		foreach (var assembly in PlugifyAssemblyLoadContext!.Assemblies)
		{
			var assemblyName = assembly.GetName();
			AssemblyCache.Add(assemblyName.Name!, new AssemblyInstance(assembly));
		}
	}

	internal static bool TryGetAssembly(Guid guid, [MaybeNullWhen(false)] out Assembly assembly)
	{
		foreach (var assemblyInstance in AssemblyCache.Values)
		{
			if (assemblyInstance.Id == guid)
			{
				assembly = assemblyInstance.Assembly;
				return true;
			}
		}

		assembly = null;
		return false;
	}
	
	internal static Assembly? ResolveAssembly(AssemblyLoadContext? assemblyLoadContext, AssemblyName assemblyName)
	{
		try
		{
			if (AssemblyCache.TryGetValue(assemblyName.Name!, out var cachedAssembly))
			{
				return cachedAssembly.Assembly;
			}

			foreach (var loadContext in AssemblyLoadContext.All)
			{
				foreach (var assembly in loadContext.Assemblies)
				{
					if (assembly.GetName().Name != assemblyName.Name)
						continue;

					AssemblyCache.Add(assemblyName.Name!, new AssemblyInstance(assembly));
					return assembly;
				}
			}
		}
		catch (Exception e)
		{
			HandleException(e);
		}

		return null;
	}

	[UnmanagedCallersOnly]
	private static Guid CreateAssemblyLoadContext(NativeString name)
	{
		string? contextName = name;

		if (contextName == null)
			return Guid.Empty;

		var alc = new AssemblyLoadContext(contextName, true);
		alc.Resolving += ResolveAssembly;
		alc.Unloading += ctx =>
		{
			foreach (var assembly in ctx.Assemblies)
			{
				string assemblyName = assembly.GetName().Name!;
				AssemblyCache.Remove(assemblyName);
			}
		};

		Guid contextId = Guid.NewGuid();
		AssemblyContexts.Add(contextId, alc);
		return contextId;
	}

	[UnmanagedCallersOnly]
	private static void UnloadAssemblyLoadContext(Guid contextId)
	{
		if (!AssemblyContexts.TryGetValue(contextId, out var alc))
		{
			LogMessage($"Cannot unload AssemblyLoadContext '{contextId}', it was either never loaded or already unloaded.", MessageLevel.Warning);
			return;
		}

		if (alc == null)
		{
			LogMessage($"AssemblyLoadContext '{contextId}' was found in dictionary but was null. This is most likely a bug.", MessageLevel.Error);
			return;
		}

		foreach (var assembly in alc.Assemblies)
		{
			var assemblyName = assembly.GetName();

			if (!AllocatedHandles.TryGetValue(assemblyName.Name!, out var handles))
			{
				continue;
			}

			foreach (var handle in handles)
			{
				if (!handle.IsAllocated || handle.Target == null)
				{
					continue;
				}

				LogMessage($"Found unfreed object '{handle.Target}' from assembly '{assemblyName}'. Deallocating.", MessageLevel.Warning);
				handle.Free();
			}
		}

		//ManagedObject.CachedMethods.Clear();

		TypeInterface.CachedTypes.Clear();
		TypeInterface.CachedMethods.Clear();
		TypeInterface.CachedFields.Clear();
		TypeInterface.CachedProperties.Clear();
		TypeInterface.CachedAttributes.Clear();

		AssemblyContexts.Remove(contextId);
		alc.Unload();
	}

	[UnmanagedCallersOnly]
	private static Guid LoadAssembly(Guid contextId, NativeString assemblyFilePath)
	{
		try
		{
			string? assemblyPath = assemblyFilePath!;
			
			if (string.IsNullOrEmpty(assemblyPath))
			{
				LastLoadStatus = AssemblyLoadStatus.InvalidFilePath;
				return Guid.Empty;
			}

			if (!File.Exists(assemblyPath))
			{
				LogMessage($"Failed to load assembly '{assemblyPath}', file not found.", MessageLevel.Error);
				LastLoadStatus = AssemblyLoadStatus.FileNotFound;
				return Guid.Empty;
			}

			if (!AssemblyContexts.TryGetValue(contextId, out var alc))
			{
				LogMessage($"Failed to load assembly '{assemblyPath}', couldn't find AssemblyLoadContext with id '{contextId}'.", MessageLevel.Error);
				LastLoadStatus = AssemblyLoadStatus.UnknownError;
				return Guid.Empty;
			}

			if (alc == null)
			{
				LogMessage($"Failed to load assembly '{assemblyPath}', AssemblyLoadContext with id '{contextId}' was null.", MessageLevel.Error);
				LastLoadStatus = AssemblyLoadStatus.UnknownError;
				return Guid.Empty;
			}

			LogMessage($"Loading assembly '{assemblyPath}'.", MessageLevel.Info);
			Assembly assembly = alc.LoadFromAssemblyPath(assemblyPath);
			
			/*Assembly? assembly = null;

			using (var file = MemoryMappedFile.CreateFromFile(assemblyPath!))
			{
				using var stream = file.CreateViewStream();
				assembly = alc.LoadFromStream(stream);
			}*/

			var assemblyName = assembly.GetName();
			var assemblyInstance = new AssemblyInstance(assembly);
			AssemblyCache.Add(assemblyName.Name!, assemblyInstance);
			LastLoadStatus = AssemblyLoadStatus.Success;
			return assemblyInstance.Id;
		}
		catch (Exception e)
		{
			AssemblyLoadErrorLookup.TryGetValue(e.GetType(), out LastLoadStatus);
			HandleException(e);
			return Guid.Empty;
		}
	}

	[UnmanagedCallersOnly]
	private static AssemblyLoadStatus GetLastLoadStatus() => LastLoadStatus;

	[UnmanagedCallersOnly]
	private static NativeString GetAssemblyName(Guid assemblyId)
	{
		if (!TryGetAssembly(assemblyId, out var assembly))
		{
			LogMessage($"Couldn't get assembly name for assembly '{assemblyId}', assembly not in dictionary.", MessageLevel.Error);
			return "";
		}

		var assemblyName = assembly.GetName();
		return assemblyName.Name;
	}

	internal static void RegisterHandle(Assembly assembly, GCHandle handle)
	{
		var assemblyName = assembly.GetName();

		if (!AllocatedHandles.TryGetValue(assemblyName.Name!, out var handles))
		{
			handles = [];
			AllocatedHandles.Add(assemblyName.Name!, handles);
		}

		handles.Add(handle);
	}

}
