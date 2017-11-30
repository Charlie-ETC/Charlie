using Charlie.Apiai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;

namespace Charlie
{
    public class FsmContextSync : MonoBehaviour
    {
        private List<PlayMakerFSM> fsms;
        private Dictionary<string, string> fsmsLastState = new Dictionary<string, string>();
        private Dictionary<string, bool> fsmsActiveState = new Dictionary<string, bool>();
        private Queue<Func<Task>> apiaiOperationQueue = new Queue<Func<Task>>();

        void Start()
        {
            fsms = GetComponents<PlayMakerFSM>().ToList();
            foreach (PlayMakerFSM fsm in fsms)
            {
                fsmsActiveState[fsm.FsmName] = false;
            }

            ProcessQueue();
        }

        void Update()
        {
            // Check the active state of FSMs and see if any of them have changed.
            foreach (PlayMakerFSM fsm in fsms)
            {
                // State changed from active -> inactive.
                // Remove all contexts associated with this state.
                if (!fsm.Active && fsmsActiveState[fsm.FsmName] && fsmsLastState.ContainsKey(fsm.FsmName))
                {
                    apiaiOperationQueue.Enqueue(() => {
                        string contextName = CreateContextName(fsm.FsmName, fsmsLastState[fsm.FsmName]);
                        Debug.Log($"[FsmContextSync] Deleting context {contextName} (reason: FSM deactivated)");
                        return ApiaiService.Instance.DeleteContext(
                            DictationMonitor.Instance.apiaiSessionId,
                            contextName
                        );
                    });
                }

                fsmsActiveState[fsm.FsmName] = fsm.Active;
            }

            List<PlayMakerFSM> activeFsms = fsms.Where(fsm => fsm.Active).ToList();
            foreach (PlayMakerFSM fsm in activeFsms)
            {
                // There was a previous state, check if the state has changed.
                if (fsmsLastState.ContainsKey(fsm.FsmName))
                {
                    var lastState = fsmsLastState[fsm.FsmName];
                    if (fsm.ActiveStateName != lastState)
                    {
                        Debug.Log($"[FsmContextSync] FSM {fsm.FsmName} state changed: {lastState} -> {fsm.ActiveStateName}");
                        SyncContext(fsm.FsmName, fsm.ActiveStateName, lastState);
                        fsmsLastState[fsm.FsmName] = fsm.ActiveStateName;
                    }
                }
                else
                {
                    // Completely new state, treat as if it has changed.
                    SyncContext(fsm.FsmName, fsm.ActiveStateName, null);
                    fsmsLastState[fsm.FsmName] = fsm.ActiveStateName;
                }
            }
        }

        private async void ProcessQueue()
        {
            while (true)
            {
                while (apiaiOperationQueue.Count > 0)
                {
                    Func<Task> operation = apiaiOperationQueue.Dequeue();
                    await operation.Invoke();
                }
                await new WaitForEndOfFrame();
            }
        }

        private void SyncContext(string fsmName, string newState, string oldState)
        {
            // Delete the old context.
            if (oldState != null)
            {
                apiaiOperationQueue.Enqueue(() => {
                    string oldContextName = CreateContextName(fsmName, oldState);
                    Debug.Log($"[FsmContextSync] Deleting context {oldContextName}");
                    return ApiaiService.Instance.DeleteContext(
                        DictationMonitor.Instance.apiaiSessionId,
                        oldContextName
                    );
                });
            }

            // Create the new context.
            apiaiOperationQueue.Enqueue(() => {
                string newContextName = CreateContextName(fsmName, newState);
                Context context = new Context {
                    name = newContextName,
                    lifespan = 99
                };

                Debug.Log($"[FsmContextSync] Creating context {context.name}");
                return ApiaiService.Instance.PostContext(
                    DictationMonitor.Instance.apiaiSessionId,
                    context
                );
            });
        }

        private string CreateContextName(string fsmName, string state)
        {
            Regex regex = new Regex("[^a-zA-Z0-9]");
            state = regex.Replace(state, "");
            return $"fsm_{fsmName}_{state}";
        }
    }
}
