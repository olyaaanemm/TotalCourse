using System;
using System.Threading;

namespace SimpleThreadPool
{
    public static class Program
    {
        static volatile int shared_counter = 0;
        static void Main()
        {
            SimpleThreadPool pool = new SimpleThreadPool();
            CountdownEvent cde = new CountdownEvent(4);
            for (int i = 0; i < 100500; ++i)
            {
                var i1 = i;
                pool.Submit( () =>
                {
                    Interlocked.Increment(ref shared_counter);
                    cde.Signal();
                });
                
            }
            cde.Wait();
            pool.Shutdown();
            Thread.Sleep(3);
            Console.WriteLine(shared_counter);
        }
    }
}
