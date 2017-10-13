using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;

public class ActionChangeBG : FsmStateAction
{
    public FsmVar speechVar;

    public override void OnEnter()
    {
        speechVar.UpdateValue();
        Debug.Log("text == " + speechVar.GetValue());
        GameObject.FindGameObjectWithTag("TargetRoot").transform.Find("WindowAnim").GetComponent<WindowAnimControl>().ChangeBG(speechVar.GetValue() as string);
        Finish();
    }

}


