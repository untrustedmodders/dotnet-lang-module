using System.Runtime.InteropServices;

namespace Plugify;

using static ManagedHost;

internal static class GarbageCollector
{
	[UnmanagedCallersOnly]
	internal static void CollectGarbage(int generation, GCCollectionMode collectionMode, Bool32 blocking, Bool32 compacting)
	{
		try
		{
			if (generation < 0)
				GC.Collect();
			else
				GC.Collect(generation, collectionMode, blocking, compacting);
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}

	[UnmanagedCallersOnly]
	internal static void WaitForPendingFinalizers()
	{
		try
		{
			GC.WaitForPendingFinalizers();
		}
		catch (Exception e)
		{
			HandleException(e);
		}
	}
}
