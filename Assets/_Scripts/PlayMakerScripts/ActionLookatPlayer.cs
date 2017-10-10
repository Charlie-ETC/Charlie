using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;

[ActionCategory(ActionCategory.Audio)]
public class ActionLookatPlayer : FsmStateAction
{
    [UIHint(UIHint.TextArea)]
    public FsmString speech;

    public FsmOwnerDefault audioSourceObj;

    public override async void OnEnter()
    {
        Debug.Log($"ActionSpeak, start speech:{speech}");
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("talk", true);

        AudioClip clip = await WatsonTTSService.Instance.Synthesize(speech.ToString());

        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<AudioSource>().PlayOneShot(clip);
        //CharlieManager.Instance.SpeakAnimation(clip.length);

        await new WaitForSeconds(clip.length);

        Debug.Log($"ActionSpeak, finish speech:{speech}");
        Finish();
    }

    public override void OnExit()
    {
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("talk", false);
    }
}
