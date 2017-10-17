using System;
using System.Collections.Generic;
using UnityEngine;

namespace Charlie
{
    class MainThreadDispatcher : Singleton<MainThreadDispatcher>
    {
        private Queue<Action> mainThreadQueue = new Queue<Action>();

        void Update()
        {
            lock (mainThreadQueue)
            {
                foreach (Action action in mainThreadQueue)
                {
                    action();
                }

                mainThreadQueue.Clear();
            }
        }

        public void Dispatch(Action action)
        {
            lock (mainThreadQueue)
            {
                mainThreadQueue.Enqueue(action);
            }
        }
    }
}
