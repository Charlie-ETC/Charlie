using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using System.Text.RegularExpressions;

[ActionCategory(ActionCategory.Audio)]
public class ActionSpeak : FsmStateAction
{
    [UIHint(UIHint.TextArea)]
    public FsmString speech;

    //public FsmVar[] formatVariable;

    public FsmOwnerDefault audioSourceObj;

    bool prevLookatPlayerState;

    public override async void OnEnter()
    {
        //string[] s = new string[formatVariable.Length];
        //int idx = 0;
        //foreach (var v in formatVariable)
        //{
        //    v.UpdateValue();
        //    s[idx] = v.GetValue().ToString();
        //    idx++;
        //}
        string actualSpeech = "";
        //if (idx == 0)
            actualSpeech = speech.ToString();
        //else
        //    actualSpeech = String.Format(speech.ToString(), s);

        //Regex r = new Regex("{.*}");
        //Match m = r.Match(actualSpeech);
        actualSpeech = actualSpeech.Replace("{PlayerName}", FsmVariables.GlobalVariables.GetFsmString("PlayerName").ToString());
        

        Debug.Log($"ActionSpeak, start speech:{actualSpeech}");
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("talk", true);

        prevLookatPlayerState = Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<LookatPlayer>().enabled;
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<LookatPlayer>().enabled = true;
        AudioClip clip = await WatsonTTSService.Instance.Synthesize(actualSpeech);
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<AudioSource>().PlayOneShot(clip);
        //CharlieManager.Instance.SpeakAnimation(clip.length);

        await new WaitForSeconds(clip.length);
   
        Debug.Log($"ActionSpeak, finish speech:{actualSpeech}");
        Finish();
    }

    public override void OnExit()
    {
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("talk", false);
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<LookatPlayer>().enabled = prevLookatPlayerState;
    }
}
