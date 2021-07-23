using System;
using System.Collections.Generic;
using System.Threading;

namespace MedianService
{
    class TaskQueue : IDisposable
    {
        object locker = new object();

        Thread[] workers;

        Queue<int> taskQ = new Queue<int>();

        public TaskQueue(int workerCount)
        {
            workers = new Thread[workerCount];

            for (int i = 0; i < workerCount; i++)
                (workers[i] = new Thread(Consume)).Start();
        }

        public void Dispose()
        {
            foreach (Thread worker in workers) EnqueueTask(0);
        }

        public void EnqueueTask(int task)
        {
            lock (locker)
            {
                taskQ.Enqueue(task);

                Monitor.PulseAll(locker);
            }
        }

        public void Consume()
        {
            while (true)
            {
                int task;

                lock (locker)
                {
                    while (taskQ.Count == 0) Monitor.Wait(locker);

                    task = taskQ.Dequeue();
                }
                if (task == 0) return;

                ServerClient server = new ServerClient();

                server.connect();

                server.getNumbers(task.ToString() + '\n');   
            }
        }
    }
}
