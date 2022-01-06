using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleThreadPool
{
    // Unbounded Blocking Multi-Producer/Multi-Consumer (MPMC) Queue
    public class UnboundedBlockingMPMCQueue<T>
    {
        private readonly Queue<T> buffer_ = new Queue<T>();
        private readonly Mutex mutex_ = new Mutex();
        CountdownEvent cde_;
        //Thread role: producer
        public UnboundedBlockingMPMCQueue (int size_)
        {
            this.cde_ = new CountdownEvent(size_);
        }
        public void Put(T value)
        {
            mutex_.WaitOne();
            this.buffer_.Enqueue(value);
            cde_.Signal();
            mutex_.ReleaseMutex();
        }
        //Thread role: consumer
        public T Take()
        {
            mutex_.WaitOne();
            while (this.buffer_.Count != 0)
            {
                cde_.Wait();
            }
            return TakeLocked();
        }
        private T TakeLocked()
        {
            var item = buffer_.Dequeue();
            mutex_.ReleaseMutex();
            return item;
        }
    }
}