using Plugify;

namespace ExamplePlugin
{
    public class ExamplePlugin : Plugin
    {
        public void OnStart()
        {
            Console.WriteLine(".NET: OnStart");
        }

        public void OnEnd()
        {
            Console.WriteLine(".NET: OnEnd");
        }
    }
}