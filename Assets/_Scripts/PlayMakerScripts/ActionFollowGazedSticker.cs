using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;
using Charlie.WatsonTTS;

public class ActionFollowGazedSticker : FsmStateAction
{
    public FsmOwnerDefault objSelf;
    bool ended = false;
    Animator anim = null;
    GameObject currentTarget = null;

    public override async void OnEnter()
    {
        ended = false;
        Debug.Log($"ActionWalkTo, start");
        var go = Fsm.GetOwnerDefaultTarget(objSelf);
       

        anim = go.GetComponentInChildren<Animator>();
        int i = UnityEngine.Random.Range(1, 3);
        Debug.Log(i);
        anim.SetInteger("toWalk", i);
        anim.SetBool("pointUp", false);

        GameObject nextTarget = null;
        int nextTargetCounter = 0;

        AudioClip clip = await WatsonTTSService.Instance.Synthesize("This one?");
        AudioSource audioSource = go.GetComponent<AudioSource>();

        while (ended == false)
        {

            //World

            // go.transform.LookAt(target.position);

            var newTarget = WorldCursor.Instance.GazeHoveringObject;

            if (nextTarget == newTarget)
            {
                nextTargetCounter++;
            }
            else
            {
                nextTargetCounter = 0;
                nextTarget = newTarget;
            }

            if (nextTargetCounter > 10)
            {
                currentTarget = nextTarget;
            }

            if (currentTarget != null)
            {
                var realTarget = currentTarget.transform.position;
                var diff = (realTarget - go.transform.position);
                var pos = diff - Vector3.Dot(diff, go.transform.up.normalized) * go.transform.up.normalized + go.transform.position;

                if ((pos - go.transform.position).magnitude < 0.05f)
                {
                    anim.SetInteger("toWalk", 0);
                    if (!anim.GetBool("pointUp"))
                    {
                        if (audioSource.isPlaying)
                        {
                            audioSource.Stop();
                        }

                        audioSource.clip = clip;
                        audioSource.Play();
                    
                        anim.SetBool("pointUp", true);
                    }
                }
                else
                {
                    anim.SetInteger("toWalk", 2);
                    anim.SetBool("pointUp", false);
                    go.transform.LookAt(pos);
                    go.transform.Translate(Vector3.forward * Time.deltaTime * 0.16f, Space.Self);
                }
            }
            else
            {
                anim.SetInteger("toWalk", 0);
                anim.SetBool("pointUp", false);
            }


            await new WaitForNextFrame();

        }

        Finish();
    }

    public override void OnExit()
    {
        ended = true;
        anim.SetInteger("toWalk", 0);
        anim.SetBool("pointUp", false);
        Charlie.GestureManager.Instance.VoiceSelectObject(currentTarget);
    }



    public override bool Event(FsmEvent fsmEvent)
    {
        base.Event(fsmEvent);
        if (fsmEvent.Name == "Action:smalltalk.confirmation.yes" && currentTarget != null)
        {
             Finish();
        }
        return false;
    }

}
