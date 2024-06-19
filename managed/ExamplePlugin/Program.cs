using System.Runtime.InteropServices;

namespace Plugify 
{
	internal class ExamplePlugin(IntPtr pluginHandle) : Plugin(pluginHandle)
	{
		public override void OnStart()
		{
			Console.WriteLine(".NET: OnStart");
		}

		public override void OnEnd()
		{
			Console.WriteLine(".NET: OnEnd");
		}

	}
	
	public static class Main
	{
		public static Plugin? Plugin { get; private set; }
		
		[UnmanagedCallersOnly]
		public static int OnInit(IntPtr pluginHandle)
		{
			// This function is executed the first time the Plugin is init
			Plugin = new ExamplePlugin(pluginHandle);
			return 0;
		}
		
		[UnmanagedCallersOnly]
		public static void OnStart()
		{
			// This function is executed the first time the Plugin is started
			Plugin?.OnStart();
		}
		
		[UnmanagedCallersOnly]
		public static void OnEnd()
		{
			// This function is executed the first time the Plugin is ended
			Plugin?.OnEnd();
		}
	}
}