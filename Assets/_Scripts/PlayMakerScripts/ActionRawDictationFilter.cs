using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;
using Charlie.WatsonTTS;

public class ActionRawDictationFilter : FsmStateAction
{
    public FsmOwnerDefault owner;
    public string containText;

    bool wasLookingAtPlayer;
    GameObject go;

    public override void OnEnter()
    {
        go = Fsm.GetOwnerDefaultTarget(owner);
    }


    public override void OnExit()
    {
    }

    public override bool Event(FsmEvent fsmEvent)
    {
        base.Event(fsmEvent);
        if (fsmEvent.Name == "Dictation:Hypothesis")
        {
            if (go.GetComponent<FsmEventGenerator>().LastHypothesis.ToLower().Contains(containText.ToLower()))
            {
                Finish();
            }
        }
        return false;
    }
}
