using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;

[ActionCategory(ActionCategory.Audio)]
public class ActionAnimationTrigger : FsmStateAction
{
    public FsmString trigger;
    Animator anim;
    public FsmOwnerDefault objSelf;

    public override void OnEnter()
    {
        var go = Fsm.GetOwnerDefaultTarget(objSelf);
        anim = go.GetComponentInChildren<Animator>();

        anim.SetTrigger(trigger.ToString());

        Finish();
    }
}

[ActionCategory(ActionCategory.Audio)]
public class ActionAnimationBool : FsmStateAction
{
    public FsmString boolvar;
    Animator anim;
    public FsmOwnerDefault objSelf;

    public override void OnEnter()
    {
        Debug.Log("enter");
        var go = Fsm.GetOwnerDefaultTarget(objSelf);
        anim = go.GetComponentInChildren<Animator>();

        anim.SetBool(boolvar.ToString(), true);
    }

    public override void OnExit()
    {
        Debug.Log("exit");
        anim.SetBool(boolvar.ToString(), false);
    }
}


[ActionCategory(ActionCategory.Audio)]
public class ActionAnimationIntTimeOut : FsmStateAction
{
    public FsmString intvar;
    public int intValue;
    public int timeOutValue;
    public float secs; 
    Animator anim;
    public FsmOwnerDefault objSelf;

    public override async void OnEnter()
    {
        var go = Fsm.GetOwnerDefaultTarget(objSelf);
        anim = go.GetComponentInChildren<Animator>();

        anim.SetInteger(intvar.ToString(), intValue);

        await new WaitForSeconds(secs);
        anim.SetInteger(intvar.ToString(), timeOutValue);
        //Finish();
    }

    public override void OnExit()
    {
        anim.SetInteger(intvar.ToString(), timeOutValue);
    }

}


[ActionCategory(ActionCategory.Audio)]
public class ActionAnimationTriggerTimeOut : FsmStateAction
{
    public FsmString timeOutTrigger;
    public FsmOwnerDefault objSelf;
    public float secs;
    Animator anim;

    public override async void OnEnter()
    {
        var go = Fsm.GetOwnerDefaultTarget(objSelf);
        anim = go.GetComponentInChildren<Animator>();

        await new WaitForSeconds(secs);
        anim.SetTrigger(timeOutTrigger.ToString());
        
        Finish();
    }

}
