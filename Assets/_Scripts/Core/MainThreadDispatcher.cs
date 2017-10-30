using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Charlie
{
    class MainThreadDispatcher : Singleton<MainThreadDispatcher>
    {
        private Queue<Task> mainThreadQueue = new Queue<Task>();
        private Queue<Tuple<Func<object>, TaskCompletionSource<object>>> mainThreadQueueWithResult =
            new Queue<Tuple<Func<object>, TaskCompletionSource<object>>>();

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

            lock (mainThreadQueueWithResult)
            {
                foreach (Tuple<Func<object>, TaskCompletionSource<object>> item in mainThreadQueueWithResult)
                {
                    try
                    {
                        Debug.Log("[MainThreadDispatcher] Invoking action");
                        item.Item2.SetResult(item.Item1.Invoke());
                        Debug.Log("[MainThreadDispatcher] Done invoking action");
                    }
                    catch (Exception e)
                    {
                        Debug.Log("[MainThreadDispatcher] Invoking action failed with exception");
                        Debug.Log(e);
                        item.Item2.SetException(e);
                    }
                }

                mainThreadQueueWithResult.Clear();
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

        public async Task<T> DispatchWithResult<T>(Func<T> action)
        {
            TaskCompletionSource<object> source = new TaskCompletionSource<object>();
            Func<object> actionWrapper = () => action();
            lock (mainThreadQueueWithResult)
            {
                Debug.Log("[MainThreadDispatcher] DispatchWithResult adding to queue");
                mainThreadQueueWithResult.Enqueue(
                    new Tuple<Func<object>, TaskCompletionSource<object>>(
                        actionWrapper, source)
                );
            }
            Debug.Log("[MainThreadDispatcher] Awaiting completion");
            T result = (T) await source.Task;
            Debug.Log("[MainThreadDispatcher] Done");
            return result;
        }
    }
}
