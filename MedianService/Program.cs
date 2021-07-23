using System;

namespace MedianService
{
    class Program
    {
        public static TaskQueue taskQueue = new TaskQueue(500);
        static void Main(string[] args)
        {
            for (int i = 1; i < 2019; i++)
            {
                taskQueue.EnqueueTask(i);
            }
        }
    }
}
