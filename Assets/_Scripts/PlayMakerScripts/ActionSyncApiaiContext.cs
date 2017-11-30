using HutongGames.PlayMaker;
using UnityEngine;
using Charlie.Apiai;

namespace Charlie
{
    public class ActionSyncApiaiContext : FsmStateAction
    {
        public FsmVar oldState;
        public FsmVar newState;
        public FsmString contextPrefix;

        public override async void OnEnter()
        {
            oldState.UpdateValue();
            newState.UpdateValue();

            // Delete the old context.
            if (oldState.stringValue.Length != 0) {
                Debug.Log($"Deleting context {oldState.stringValue}");
                await ApiaiService.Instance.DeleteContext(
                    DictationMonitor.Instance.apiaiSessionId,
                    $"{contextPrefix.Value}_{oldState.stringValue}"
                );
            }

            // Create the new context.
            Context context = new Context();
            context.name = $"{contextPrefix.Value}_{newState.stringValue}";
            context.lifespan = 99;

            Debug.Log($"Creating context {context.name}");
            await ApiaiService.Instance.PostContext(
                DictationMonitor.Instance.apiaiSessionId,
                context
            );

            oldState.SetValue(newState.stringValue);
            Finish();
        }
    }
}
