using HutongGames.PlayMaker;
using System;
using UnityEngine;
using Asyncoroutine;

using Charlie.WatsonTTS;

[ActionCategory(ActionCategory.Audio)]
public class ActionSpeak : FsmStateAction
{
    [UIHint(UIHint.TextArea)]
    public FsmString speech;
    public bool ignoredApiResponse;

    //public FsmVar[] formatVariable;

    public FsmOwnerDefault audioSourceObj;

    //bool prevLookatPlayerState;

    public override async void OnEnter()
    {
        if (!String.IsNullOrEmpty(speech.ToString()))
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

            string[] speechArr = speech.ToString().Split('\n');

            if (String.IsNullOrEmpty(speechArr[speechArr.Length - 1]))
            {
                actualSpeech = speechArr[UnityEngine.Random.Range(0, speechArr.Length - 1)];
            }
            else
            {
                actualSpeech = speechArr[UnityEngine.Random.Range(0, speechArr.Length)];
            }

            //else
            //    actualSpeech = String.Format(speech.ToString(), s);

            //Regex r = new Regex("{.*}");
            //Match m = r.Match(actualSpeech);
            if (actualSpeech.Contains("{PlayerName}"))
            {
                actualSpeech = actualSpeech.Replace("{PlayerName}", UserProfile.Content["PlayerName"]);
            }

            if (actualSpeech.Contains("{FavoriteCity}"))
            {
                Debug.Log("getting favorite city");
                string FavoriteCity = await UserProfile.TryGetApiaiContext("favorite_city");
                Debug.Log($"got favorite city, {FavoriteCity}");
                actualSpeech = actualSpeech.Replace("{FavoriteCity}", FavoriteCity);
            }


            Debug.Log($"ActionSpeak, start speech:{actualSpeech}");
            Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("toTalk", true); // for facial animation
            Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetInteger("toTalkBody", UnityEngine.Random.Range(1, 9)); // for body talk animation

            //prevLookatPlayerState = Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<LookatPlayer>().enabled;
            //Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<LookatPlayer>().enabled = true;
            AudioClip clip = await WatsonTTSService.Instance.Synthesize(actualSpeech);

            //Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<AudioSource>().PlayOneShot(clip);
            AudioSource audioSource = Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<AudioSource>();
            CharlieSlackLog charlieSlackLog = Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<CharlieSlackLog>();

            if (audioSource != null) // make sure only play single audioclip at one time
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }

                audioSource.clip = clip;
                audioSource.Play();
                // log what charlie says to Slack
                charlieSlackLog.SlackLog("charlie", actualSpeech);
                DictationMonitor.Instance.plotSpeaking = true;
            }
            else
            {
                Debug.Log("No audioSource attached to Charlie");
            }
            //CharlieManager.Instance.SpeakAnimation(clip.length);

            await new WaitForSeconds(clip.length);

            Debug.Log($"ActionSpeak, finish speech:{actualSpeech}");

            Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("toTalk", false); // for facial animation
            Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetInteger("toTalkBody", 0); // for body talk animation

            DictationMonitor.Instance.plotSpeaking = false;
            if (DictationMonitor.Instance.MissedQ)
            {
                if (!ignoredApiResponse) {
                    DictationMonitor.Instance.TriggerApiaiEvent(Charlie.Apiai.ApiaiEventNames.INPUT_UNKNOWN);
                }
                DictationMonitor.Instance.MissedQ = false;
            }
        }

        Finish();
    }

    public override void OnExit()
    {
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponent<AudioSource>().Stop();
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetBool("toTalk", false); // for facial animation
        Fsm.GetOwnerDefaultTarget(audioSourceObj).GetComponentInChildren<Animator>().SetInteger("toTalkBody", 0); // for body talk animation
    }

    //public override void OnExit()
    //{

    //}
}
