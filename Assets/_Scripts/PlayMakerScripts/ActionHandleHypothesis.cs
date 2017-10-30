using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;

public class ActionHandleHypothesis : FsmStateAction
{
    public FsmOwnerDefault owner;

    bool wasLookingAtPlayer;
    GameObject go;

    public override void OnEnter()
    {
        go = Fsm.GetOwnerDefaultTarget(owner);
        wasLookingAtPlayer = go.GetComponent<LookatPlayer>().enabled;
        go.GetComponent<LookatPlayer>().enabled = true;
    }


    public override void OnExit()
    {
        go.GetComponent<LookatPlayer>().enabled = wasLookingAtPlayer;
    }

    public override bool Event(FsmEvent fsmEvent)
    {
        base.Event(fsmEvent);
        Debug.Log($"ActionHandleHypothesis get event : {fsmEvent.Name}");
        aEvent(fsmEvent);
        Debug.Log($"Finish ActionHandleHypothesis get event : {fsmEvent.Name}");
        return false;
    }

    bool waitingForSpeech = false;
    async void aEvent(FsmEvent fsmEvent)
    {
        if (fsmEvent.Name == "Apiai:Speech")
        {
            waitingForSpeech = true;
            await new WaitForNextFrame();
            if (Enabled)
            {
                Debug.Log($"Speak start");
                await DictationMonitor.Instance.SpeakApiaiResponse(go.GetComponent<FsmEventGenerator>().LastResponse);
                waitingForSpeech = false;
                Debug.Log($"Speak End");
                Finish();
            }
        }
        else if ((fsmEvent.Name == "Complete" && go.GetComponent<FsmEventGenerator>().LastCompleteMessage != "Complete")
            || fsmEvent.Name == "Dictation:Error"
            || (!waitingForSpeech && fsmEvent.Name == "Apiai:Response"))
        {
            await new WaitForNextFrame();
            if (Enabled) { 
                Finish();
            }
        }
    }
}
