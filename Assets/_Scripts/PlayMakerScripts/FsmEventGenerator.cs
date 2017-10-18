using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmEventGenerator : MonoBehaviour {

    PlayMakerFSM[] FSMArray;
    public string LastSpeech;
    public Dictionary<string, string> LastParam;

    void Start()
    {
        FSMArray = gameObject.GetComponents<PlayMakerFSM>();
    }

    public void HandleDictationResult(string text)
    {
        if (isActiveAndEnabled == false)
            return;
        LastSpeech = text;
        foreach (var fsm in FSMArray)
        {
            fsm.SendEvent("SpeechFinish");
        }
    }

    public void HandleResponse(Response resp)
    {
        //var intentName = resp?.result?.metadata?.intentName ?? "hello";
        LastParam = resp.result.parameters;
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
