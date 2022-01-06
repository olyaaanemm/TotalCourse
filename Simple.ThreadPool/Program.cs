using System;
using System.Threading;

namespace SimpleThreadPool
{
    public static class Program
    {
        static volatile int   shared_counter = 0;
        static void Main()
        {
            SimpleThreadPool pool = new SimpleThreadPool();
            
            for (int i = 0; i < 100500; ++i)
            {
                var i1 = i;
                pool.Submit( () => Interlocked.Increment(ref shared_counter));
            }
            
            pool.Join();
            Console.WriteLine(shared_counter);
        }
    }
}
