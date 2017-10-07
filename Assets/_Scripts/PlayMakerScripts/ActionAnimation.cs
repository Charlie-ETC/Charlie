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
        var go = Fsm.GetOwnerDefaultTarget(objSelf);
        anim = go.GetComponentInChildren<Animator>();

        anim.SetBool(boolvar.ToString(), true);
    }

    public override void OnExit()
    {
        anim.SetBool(boolvar.ToString(), false);
    }
}
