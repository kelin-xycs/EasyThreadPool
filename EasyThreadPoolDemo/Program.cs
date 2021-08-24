using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using System.Threading;

using EasyThreadPool;

namespace EasyThreadPoolDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("第一轮任务执行完后，不要关闭程序，5 秒后会再次添加任务，唤醒上次剩下的一个工作线程和 Routine 线程 。");
            Console.WriteLine("第一轮任务执行完是指第一次添加的任务全部执行完，此时，会显示最后一个工作线程和 Routine 线程挂起 。");
            Console.WriteLine("\r\n添加第一轮任务 。\r\n");

            ThreadPool threadPool = new ThreadPool(100, 100, 4);

            for (int i=0; i<10; i++)
            {
                Task task = new Task(Test);

                threadPool.AddTask(task);
            }

            System.Threading.Thread.Sleep(5000);

            Console.WriteLine("\r\n添加第二轮任务 。\r\n");

            for (int i = 0; i < 10; i++)
            {
                Task task = new Task(Test);

                threadPool.AddTask(task);
            }

        }

        private static void Test()
        {
            Console.WriteLine("线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] Test Begin .");

            System.Threading.Thread.Sleep(400);

            Console.WriteLine("线程 [ ManagedThreadId : " + System.Threading.Thread.CurrentThread.ManagedThreadId + " ] Test End .");
        }
    }
}
