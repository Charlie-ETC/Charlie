using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;

public class ActionWaitForSpeech : FsmStateAction
{
    public FsmVar textVar;
    public FsmOwnerDefault owner;

    public override void OnExit()
    {
        string text = Fsm.GetOwnerDefaultTarget(owner).GetComponent<FsmEventGenerator>().LastSpeech;
        Debug.Log("HIHIHIIHIHIHI" + text);
        textVar.SetValue(text);
    }
}
