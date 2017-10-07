using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;

[ActionCategory(ActionCategory.Audio)]
public class ActionSpeak : FsmStateAction
{
    [UIHint(UIHint.TextArea)]
    public FsmString speech;

    public FsmOwnerDefault audioSourceObj;

    public override async void OnEnter()
    {
        Debug.Log($"ActionSpeak, start speech:{speech}");
        
        AudioClip clip = await WatsonTTSService.Instance.Synthesize(speech.ToString());

        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<AudioSource>().PlayOneShot(clip);
        //CharlieManager.Instance.SpeakAnimation(clip.length);

        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("talk", true);
        await new WaitForSeconds(clip.length);

        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("talk", false);
   
        Debug.Log($"ActionSpeak, finish speech:{speech}");
        Finish();
    }
}
