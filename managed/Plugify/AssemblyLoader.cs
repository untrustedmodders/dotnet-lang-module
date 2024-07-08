using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace Plugify;

using static ManagedHost;

public enum AssemblyLoadStatus
{
	Success, FileNotFound, FileLoadFailure, InvalidFilePath, InvalidAssembly, UnknownError
}

public static class AssemblyLoader
{
	private static readonly Dictionary<Type, AssemblyLoadStatus> AssemblyLoadErrorLookup = new();
	private static readonly Dictionary<int, AssemblyLoadContext?> AssemblyContexts = new();
	private static readonly Dictionary<int, Assembly> AssemblyCache = new();
	private static readonly Dictionary<int, List<GCHandle>> AllocatedHandles = new();
	private static AssemblyLoadStatus LastLoadStatus = AssemblyLoadStatus.Success;

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
			int assemblyId = assembly.GetName().Name!.GetHashCode();
			AssemblyCache.Add(assemblyId, assembly);
		}
	}

	internal static bool TryGetAssembly(int assemblyId, out Assembly? assembly)
	{
		return AssemblyCache.TryGetValue(assemblyId, out assembly);
	}

	internal static Assembly? ResolveAssembly(AssemblyLoadContext? assemblyLoadContext, AssemblyName assemblyName)
	{
		try
		{
			int assemblyId = assemblyName.Name!.GetHashCode();
			
			if (AssemblyCache.TryGetValue(assemblyId, out var cachedAssembly))
			{
				return cachedAssembly;
			}

			foreach (var loadContext in AssemblyLoadContext.All)
			{
				foreach (var assembly in loadContext.Assemblies)
				{
					if (assembly.GetName().Name != assemblyName.Name)
						continue;

					AssemblyCache.Add(assemblyId, assembly);
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
	private static int CreateAssemblyLoadContext(NativeString name)
	{
		string? contextName = name;

		if (contextName == null)
			return -1;

		var alc = new AssemblyLoadContext(contextName, true);
		alc.Resolving += ResolveAssembly;
		alc.Unloading += ctx =>
		{
			foreach (var assembly in ctx.Assemblies)
			{
				var assemblyName = assembly.GetName();
				int assemblyId = assemblyName.Name!.GetHashCode();
				AssemblyCache.Remove(assemblyId);
			}
		};

		int contextId = contextName.GetHashCode();
		AssemblyContexts.Add(contextId, alc);
		return contextId;
	}

	[UnmanagedCallersOnly]
	private static void UnloadAssemblyLoadContext(int contextId)
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
			int assemblyId = assemblyName.Name!.GetHashCode();

			if (!AllocatedHandles.TryGetValue(assemblyId, out var handles))
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
	private static int LoadAssembly(int contextId, NativeString assemblyFilePath)
	{
		try
		{
			string? assemblyPath = assemblyFilePath!;
			
			if (string.IsNullOrEmpty(assemblyPath))
			{
				LastLoadStatus = AssemblyLoadStatus.InvalidFilePath;
				return -1;
			}

			if (!File.Exists(assemblyPath))
			{
				LogMessage($"Failed to load assembly '{assemblyPath}', file not found.", MessageLevel.Error);
				LastLoadStatus = AssemblyLoadStatus.FileNotFound;
				return -1;
			}

			if (!AssemblyContexts.TryGetValue(contextId, out var alc))
			{
				LogMessage($"Failed to load assembly '{assemblyPath}', couldn't find AssemblyLoadContext with id {contextId}.", MessageLevel.Error);
				LastLoadStatus = AssemblyLoadStatus.UnknownError;
				return -1;
			}

			if (alc == null)
			{
				LogMessage($"Failed to load assembly '{assemblyPath}', AssemblyLoadContext with id {contextId} was null.", MessageLevel.Error);
				LastLoadStatus = AssemblyLoadStatus.UnknownError;
				return -1;
			}

			Assembly assembly = alc.LoadFromAssemblyPath(assemblyPath!);
			
			/*Assembly? assembly = null;

			using (var file = MemoryMappedFile.CreateFromFile(assemblyPath!))
			{
				using var stream = file.CreateViewStream();
				assembly = alc.LoadFromStream(stream);
			}*/

			LogMessage($"Loading assembly '{assemblyPath}'", MessageLevel.Info);
			var assemblyName = assembly.GetName();
			int assemblyId = assemblyName.Name!.GetHashCode();
			AssemblyCache.Add(assemblyId, assembly);
			LastLoadStatus = AssemblyLoadStatus.Success;
			return assemblyId;
		}
		catch (Exception e)
		{
			AssemblyLoadErrorLookup.TryGetValue(e.GetType(), out LastLoadStatus);
			HandleException(e);
			return -1;
		}
	}

	[UnmanagedCallersOnly]
	private static AssemblyLoadStatus GetLastLoadStatus() => LastLoadStatus;

	[UnmanagedCallersOnly]
	private static NativeString GetAssemblyName(int assemblyId)
	{
		if (!AssemblyCache.TryGetValue(assemblyId, out var assembly))
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
		int assemblyId = assemblyName.Name!.GetHashCode();

		if (!AllocatedHandles.TryGetValue(assemblyId, out var handles))
		{
			AllocatedHandles.Add(assemblyId, []);
			handles = AllocatedHandles[assemblyId];
		}

		handles.Add(handle);
	}

}
