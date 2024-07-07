
using Plugify;

namespace SampleApp
{
    public class BasePlugin : Plugin
    {
        public void OnStart()
        {
            Console.WriteLine("BasePlugin OnStart");
        }
    }
    
    public class ExportClass
    {
        public static void MyExportFunction(int a, string b)
        {
            Console.WriteLine("SampleApp: " + b);
        }
    }
}