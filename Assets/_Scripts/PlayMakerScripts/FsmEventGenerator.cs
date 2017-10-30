using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmEventGenerator : MonoBehaviour {

    PlayMakerFSM[] FSMArray;
    public string LastHypothesis;
    public string LastSpeech;
    public string LastCompleteMessage;
    public Response LastResponse;
    //public Dictionary<string, string> LastParam;

    void Start()
    {
        FSMArray = gameObject.GetComponents<PlayMakerFSM>();
    }

    public void HandleDictationHypothesis(string text)
    {
        if (isActiveAndEnabled == false)
            return;
        LastHypothesis = text;
        foreach (var fsm in FSMArray)
        {
            fsm.SendEvent("Dictation:Hypothesis");
        }
    }

    public void HandleDictationComplete(string text)
    {
        if (isActiveAndEnabled == false)
            return;

        LastCompleteMessage = text;

        foreach (var fsm in FSMArray)
        {
            fsm.SendEvent("Dictation:Complete");
        }
    }

    public void HandleDictationError(string text)
    {
        if (isActiveAndEnabled == false)
            return;

        foreach (var fsm in FSMArray)
        {
            fsm.SendEvent("Dictation:Error");
        }
    }

    public void HandleDictationResult(string text)
    {
        if (isActiveAndEnabled == false)
            return;
        LastSpeech = text;
        foreach (var fsm in FSMArray)
        {
            fsm.SendEvent("Dictation:Finish");
        }
    }

    public void HandleResponse(Response resp)
    {
        //var intentName = resp?.result?.metadata?.intentName ?? "hello";
        LastResponse = resp;
        if (!string.IsNullOrEmpty(resp.result.metadata.intentName))
        {
            foreach (var fsm in FSMArray)
            {
                fsm.SendEvent("Intent:" + resp.result.metadata.intentName);
            }
        }

        if (!string.IsNullOrEmpty(resp.result.action))
        {
            foreach (var fsm in FSMArray)
            {
                fsm.SendEvent("Action:" + resp.result.action);
            }
        }

        if (resp.result.speech.Length != 0)
        {
            foreach (var fsm in FSMArray)
            {
                fsm.SendEvent("Apiai:Speech");
            }
        }

        foreach (var fsm in FSMArray)
        {
            fsm.SendEvent("Apiai:Response");
        }
    }
}
