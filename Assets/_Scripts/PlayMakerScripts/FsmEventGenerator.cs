using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmEventGenerator : MonoBehaviour {

    PlayMakerFSM[] FSMArray;

    void Start()
    {
        FSMArray = gameObject.GetComponents<PlayMakerFSM>();
    }

    public void HandleSpeech(string text)
    {
        foreach (var fsm in FSMArray)
        {
            fsm.SendEvent("Speech:" + text);
        }
    }

    public void HandleResponse(Response resp)
    {
        //var intentName = resp?.result?.metadata?.intentName ?? "hello";
        if (!string.IsNullOrEmpty(resp.result.metadata.intentName))
        {
            foreach (var fsm in FSMArray)
            {
                fsm.SendEvent("Intent:" + resp.result.metadata.intentName);
            }
        }
        else if (!string.IsNullOrEmpty(resp.result.action))
        {
            foreach (var fsm in FSMArray)
            {
                fsm.SendEvent("Action:" + resp.result.action);
            }
        }
    }
}
