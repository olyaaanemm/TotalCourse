using System;
using System.Threading;

namespace SimpleThreadPool
{
    public static class Program
    {
        static int  shared_counter = 0;
        static void Main()
        {
            SimpleThreadPool pool = new SimpleThreadPool(4);
            
            var random = new Random();
            Action<int> randomizer = (index =>
            {
                Console.WriteLine("{0}: Working on index {1}", Thread.CurrentThread.Name, index);
                Thread.Sleep(random.Next(20, 400));
                Console.WriteLine("{0}: Ending {1}", Thread.CurrentThread.Name, index);
            });

            
            for (int i = 0; i < 100500; ++i)
            {
                var i1 = i;
                pool.Submit( () => ++shared_counter);
            }
            
            pool.Join();
            Thread.Sleep(3);
            Console.WriteLine(shared_counter);
            
            Thread.Sleep(3);
            Console.WriteLine(shared_counter);
        }
    }
}