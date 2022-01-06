using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleThreadPool
{
    public sealed class SimpleThreadPool 
    {
        public SimpleThreadPool(int size)
        {
            StartWorkerThreads(size);
        }

        public void Submit(Action action)
        {
            _tasks.Put(action);
        }

        public void Join()
        {
            foreach (var worker in this._workers)
            {
                this._tasks.Put(() => {}); //Poison pill
            }
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
                Action task_ = () => { };
                var task = _tasks.Take();
                if (!Equate(task, task_))
                {
                    break;
                }
                task();
            }
        }
        private LinkedList<Thread> _workers; // queue of worker threads ready to process actions
        private UnboundedBlockingMPMCQueue<Action> _tasks = new UnboundedBlockingMPMCQueue<Action>(4); // actions to be processed by worker threads
        
        
        public static bool Equate(System.Delegate a, System.Delegate b)
        {
            // standard equality
            if (a == b)
                return true;

            // null
            if (a == null || b == null)
                return false;

            // compiled method body
            if (a.Target != b.Target)
                return false;
            byte[] a_body = a.Method.GetMethodBody().GetILAsByteArray();
            byte[] b_body = b.Method.GetMethodBody().GetILAsByteArray();
            if (a_body.Length != b_body.Length)
                return false;
            for (int i = 0; i < a_body.Length; i++)
            {
                if (a_body[i] != b_body[i])
                    return false;
            }
            return true;
        }
    }
    
    
}