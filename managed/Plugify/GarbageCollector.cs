using System;
using System.Runtime.InteropServices;

namespace Plugify;

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
			Logger.Log(Severity.Error, "Error collecting garbage: {0}", e);
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
			Logger.Log(Severity.Error, "Error waiting for pending finalizers: {0}", e);
		}
	}
}
