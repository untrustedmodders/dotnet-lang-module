using Plugify;

namespace ExamplePlugin
{
    public class ExamplePlugin : Plugin
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