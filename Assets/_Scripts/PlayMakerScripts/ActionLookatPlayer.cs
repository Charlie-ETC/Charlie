using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//[ActionCategory(ActionCategory.Audio)]
public class ActionLookatPlayer : FsmStateAction
{

    //[UIHint(UIHint.TextArea)]
    //public FsmString speech;

    public FsmOwnerDefault objSelf;
    public bool lookAtPlayerOn;
    public bool lookAway;

    public override void OnEnter()
    {

        // if we want to control her neck
        Fsm.GetOwnerDefaultTarget(objSelf).GetComponentInChildren<LookAtPlayerIKControl>().isActive = lookAtPlayerOn;
        Fsm.GetOwnerDefaultTarget(objSelf).GetComponentInChildren<LookAtPlayerIKControl>().lookAwayOn = lookAway;
        //Fsm.GetOwnerDefaultTarget(objSelf).GetComponent<LookatPlayer>().enabled = lookAtPlayerOn;



        Finish();
    }

    public override void OnExit()
    {

    }
}
