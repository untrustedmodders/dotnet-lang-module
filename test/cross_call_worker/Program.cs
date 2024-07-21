using Plugify;

namespace cross_call_worker;

public class CrossCallWorker : Plugin
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