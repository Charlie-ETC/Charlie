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

    //bool prevLookatPlayerState;

    public override async void OnEnter()
    {
        if (!String.IsNullOrEmpty(speech.ToString())) {
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
            if (actualSpeech.Contains("{PlayerName}"))
            {
                actualSpeech = actualSpeech.Replace("{PlayerName}", UserProfile.Content["PlayerName"]);
            }


            Debug.Log($"ActionSpeak, start speech:{actualSpeech}");
            Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("toTalk", true); // for facial animation
            Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetInteger("toTalkBody", UnityEngine.Random.Range(1, 9)); // for body talk animation

            //prevLookatPlayerState = Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<LookatPlayer>().enabled;
            //Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<LookatPlayer>().enabled = true;
            AudioClip clip = await WatsonTTSService.Instance.Synthesize(actualSpeech);
            Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<AudioSource>().PlayOneShot(clip);
            //CharlieManager.Instance.SpeakAnimation(clip.length);

            await new WaitForSeconds(clip.length);

            Debug.Log($"ActionSpeak, finish speech:{actualSpeech}");

            Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("toTalk", false); // for facial animation
            Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetInteger("toTalkBody", 0); // for body talk animation
                                                                                                                     
        }

        Finish();
    }

    //public override void OnExit()
    //{
        
    //}
}
