using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Charlie
{
    class MainThreadDispatcher : Singleton<MainThreadDispatcher>
    {
        private Queue<Task> mainThreadQueue = new Queue<Task>();

        void Update()
        {
            lock (mainThreadQueue)
            {
                foreach (Task task in mainThreadQueue)
                {
                    task.RunSynchronously();
                }

                mainThreadQueue.Clear();
            }
        }

        public Task Dispatch(Action action)
        {
            lock (mainThreadQueue)
            {
                Task task = new Task(action);
                mainThreadQueue.Enqueue(task);
                return task;
            }
        }
    }
}
