using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;

//[ActionCategory(ActionCategory.Audio)]
public class ActionLookatPlayer : FsmStateAction
{
    private static int instanceMAxIndex = 0; 
    private int instanceIndex;
    //[UIHint(UIHint.TextArea)]
    //public FsmString speech;

    public FsmOwnerDefault objSelf;
    public bool lookAtPlayerOn;

    public override async void OnEnter()
    {
        instanceMAxIndex++;
        Debug.Log("instanceMaxIndex" + instanceMAxIndex);
        instanceIndex = instanceMAxIndex;
        // if we want to control her neck
        Fsm.GetOwnerDefaultTarget(objSelf).GetComponentInChildren<LookAtPlayerIKControl>().isActive = lookAtPlayerOn;
        //Fsm.GetOwnerDefaultTarget(objSelf).GetComponent<LookatPlayer>().enabled = lookAtPlayerOn;

        // pause the eye contact when active and not speaking
        // check the look-away(0.5-2s) possibility(1%) every 1 second 
        while (lookAtPlayerOn && instanceIndex == instanceMAxIndex) {
            if (!Fsm.GetOwnerDefaultTarget(objSelf).GetComponent<AudioSource>().isPlaying) {
                if (UnityEngine.Random.Range(0f, 1f) < 0.20f) {
                    Fsm.GetOwnerDefaultTarget(objSelf).GetComponentInChildren<LookAtPlayerIKControl>().isActive = false;
                    await new WaitForSeconds(UnityEngine.Random.Range(1.5f, 5f));
                    Fsm.GetOwnerDefaultTarget(objSelf).GetComponentInChildren<LookAtPlayerIKControl>().isActive = true;
                }
            } 
         
            await new WaitForSeconds(1f);
        }

        Finish();
    }

    //public override void OnExit()
    //{
        
    //}
}
