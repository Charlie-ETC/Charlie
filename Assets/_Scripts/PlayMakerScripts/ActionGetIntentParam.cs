using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;

public class ActionGetIntentParam : FsmStateAction
{
    public string intentParamKey;
    public FsmVar textVar;
    public FsmOwnerDefault owner;

    public override void OnEnter()
    {
        string text = Fsm.GetOwnerDefaultTarget(owner).GetComponent<FsmEventGenerator>().LastResponse.result.parameters[intentParamKey];
        Debug.Log($"Action Get Intent Param: {intentParamKey}, value: {text}");
        textVar.SetValue(text);
        Finish();
    }
}
