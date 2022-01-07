using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleThreadPool
{
    public sealed class SimpleThreadPool 
    {
        public SimpleThreadPool()
        {
            StartWorkerThreads(4);
        }

        public void Submit(Action action)
        {
            _tasks.Enqueue(action);
        }

       public void Shutdown()
       {
           state_ = false; 
            foreach (var worker in this._workers)
            {
                worker.Join();
            }
            this._workers.Clear();
        }

        private void StartWorkerThreads(int count)
        {
            this._workers = new LinkedList<Thread>();
            for (int i = 0; i < count; ++i)
            {
                var thread = new Thread(_ =>  WorkerRoutine() );
                _workers.AddLast(thread);
                thread.Start();
            }
        }

        private void WorkerRoutine()
        {
            while (true)
            {
                Action task;
                var state = _tasks.TryDequeue(out task);
                if (state)
                {
                    task();
                }

                if (!state_)
                {
                    break;
                }
                else
                {
                   
                }
            }

        }
        private LinkedList<Thread> _workers; // queue of worker threads ready to process actions
        private ConcurrentQueue<Action> _tasks = new ConcurrentQueue<Action>(); // actions to be processed by worker threads
        private volatile bool state_ = true;

    }
    
    
}
