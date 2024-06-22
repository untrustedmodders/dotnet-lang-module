using Plugify;

namespace ExamplePlugin
{
    public class ExamplePlugin(nint pluginHandle) : Plugin(pluginHandle)
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
}