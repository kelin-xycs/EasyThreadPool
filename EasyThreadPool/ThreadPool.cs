using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace EasyThreadPool
{
    public class ThreadPool
    {
        private int routineInterval;
        private int maxThreadCount;
        private int addThreadInterval;
        
        private int threadCount = 0;

        private Queue<Task> queue = new Queue<Task>();

        private DateTime lastFetchedTaskTime;

        private Thread firstWorkThread = null;
        private Thread routineThread = null;

        public ThreadPool(int routineInterval, int addThreadInterval, int maxThreadCount)
        {
            this.routineInterval = routineInterval;
            this.addThreadInterval = addThreadInterval;
            this.maxThreadCount = maxThreadCount;
        }

        public void AddTask(Task task)
        {
            bool ifCreateWork = false;
            bool ifWakeWork = false;
            bool ifCreateRoutine = false;
            bool ifWakeRoutine = false;

            lock (this)
            {
                this.queue.Enqueue(task);

                if (this.queue.Count == 1)
                {
                    if (this.threadCount == 0)
                    {
                        ifCreateWork = true;
                        this.lastFetchedTaskTime = DateTime.MinValue;
                    }
                    else
                    {
                        ifWakeWork = true;
                    }
                    
                }
                    

                if (this.routineThread == null)
                {
                    ifCreateRoutine = true;
                }
                else if (this.routineThread.ThreadState == ThreadState.Suspended)
                {
                    ifWakeRoutine = true;
                }

            }

            if (ifCreateWork)
            {
                AddThread();
            }
            else if (ifWakeWork)
            {
                this.firstWorkThread.Resume();
            }

            if (ifCreateRoutine)
            {
                this.routineThread = new Thread(Routine);
                this.routineThread.Start();
            }
            else if (ifWakeRoutine)
            {
                this.routineThread.Resume();
            }
           
            
        }

        private void Routine()
        {

            Console.WriteLine("Routine 线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] 创建 .");

            while (true)
            {
                Console.WriteLine("Routine 线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] 工作 .");

                bool ifSuspend = false;
                bool ifAddThread = false;

                lock (this)
                {

                    if (this.queue.Count == 0)
                    {
                        ifSuspend = true;
                    }
                    else if (this.lastFetchedTaskTime != DateTime.MinValue
                        && (DateTime.Now - this.lastFetchedTaskTime).TotalMilliseconds >= this.addThreadInterval
                        && this.threadCount < this.maxThreadCount
                        && this.queue.Count > 0)
                    {
                        ifAddThread = true;
                    }
                }

                if (ifSuspend)
                {
                    Console.WriteLine("Routine 线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] 挂起 .");
                    this.routineThread.Suspend();
                    Console.WriteLine("Routine 线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] 唤醒 .");
                    continue;
                }

                if (ifAddThread)
                {
                    AddThread();
                }

                Thread.Sleep(routineInterval);
               
            }
           
        }

        private void AddThread()
        {
            lock(this)
            {
                this.threadCount++;
            }

            Thread thread = new Thread(Work);
            thread.Start();
        }

        private void Work()
        {
            Console.WriteLine("线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] 创建 .");

            while (true)
            {
                bool ifSuspend = false;
                Task task = null;

                lock (this)
                {
                    if (queue.Count > 0)
                    {
                        task = queue.Dequeue();
                        this.lastFetchedTaskTime = DateTime.Now;
                    }
                    else
                    {
                        if (this.threadCount == 1)
                        {
                            ifSuspend = true;
                        }
                        else
                        {
                            this.threadCount--;

                            Console.WriteLine("线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] 退出 .");

                            return;
                        }
                        
                    }
                    
                }

                if (ifSuspend)
                {
                    Console.WriteLine("线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] 挂起 .");
                    this.firstWorkThread = Thread.CurrentThread;
                    this.firstWorkThread.Suspend();
                    Console.WriteLine("线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] 唤醒 .");
                    continue;
                }
       

                task.Execute();
                    
            }
        }
    }
}
