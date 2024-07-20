﻿using Plugify;

namespace CrossCallWorker
{
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
}