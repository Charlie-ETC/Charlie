using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;

public class ActionFreeConversation : FsmStateAction
{
    public FsmOwnerDefault owner;

    public float wholeConversationTimeout;
    public float playerResponseTimeout;
    public string speakWhenSilence;

    bool wasLookingAtPlayer;
    GameObject go;

    float stateStartTime;
    float lastSpeakEnd;

    public override void OnEnter()
    {
        go = Fsm.GetOwnerDefaultTarget(owner);
        //wasLookingAtPlayer = go.GetComponent<LookatPlayer>().enabled;
        wasLookingAtPlayer = go.GetComponentInChildren<LookAtPlayerIKControl>().isActive;
        //go.GetComponent<LookatPlayer>().enabled = true;
        go.GetComponentInChildren<LookAtPlayerIKControl>().isActive = true;

        stateStartTime = Time.time;
    }


    public override void OnExit()
    {
        //go.GetComponent<LookatPlayer>().enabled = wasLookingAtPlayer;
        go.GetComponentInChildren<LookAtPlayerIKControl>().isActive = wasLookingAtPlayer;
    }

    public override bool Event(FsmEvent fsmEvent)
    {
        base.Event(fsmEvent);
        Debug.Log($"ActionHandleHypothesis get event : {fsmEvent.Name}");
        aEvent(fsmEvent);
        Debug.Log($"Finish ActionHandleHypothesis get event : {fsmEvent.Name}");
        return false;
    }

    bool waitingForSpeech = false;
    bool playerSpeaking = false;
    bool waitingRandomQuestion = false;
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!waitingForSpeech && !playerSpeaking && !waitingRandomQuestion) {
            if (Time.time - stateStartTime > wholeConversationTimeout)
            {
                Finish();
            }
            else if (Time.time - lastSpeakEnd > playerResponseTimeout)
            {
                speakRandomQuestion();
            }
        }


        if (Time.time - stateStartTime > wholeConversationTimeout + 10)
        {
            // just in case
            Finish();
        }
    }

    async void speakRandomQuestion()
    {
        waitingRandomQuestion = true;


        string actualSpeech = "";

        string[] speechArr = speakWhenSilence.Split('\n');

        if (speechArr.Length == 0)
            return;

        if (String.IsNullOrEmpty(speechArr[speechArr.Length - 1]))
        {
            actualSpeech = speechArr[UnityEngine.Random.Range(0, speechArr.Length - 1)];
        }
        else
        {
            actualSpeech = speechArr[UnityEngine.Random.Range(0, speechArr.Length)];
        }
        
        if (speechArr.Length == 0)
            return;

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


        await DictationMonitor.Instance.SpeakText(actualSpeech);
        waitingRandomQuestion = false;
        lastSpeakEnd = Time.time;
    }

    async void aEvent(FsmEvent fsmEvent)
    {
        if (fsmEvent.Name == "Apiai:Speech")
        {
            waitingForSpeech = true;
            await new WaitForNextFrame();
            if (Enabled)
            {
                Debug.Log($"Speak start");
                await DictationMonitor.Instance.SpeakApiaiResponse(go.GetComponent<FsmEventGenerator>().LastResponse);
                waitingForSpeech = false;
                lastSpeakEnd = Time.time;
                Debug.Log($"Speak End");
            }
        }
        else if (fsmEvent.Name == "Dictation:Hypothesis")
        {
            playerSpeaking = true;
        }
        else if ((fsmEvent.Name == "Dictation:Complete" && go.GetComponent<FsmEventGenerator>().LastCompleteMessage != "Complete")
            || fsmEvent.Name == "Dictation:Error")
        {
            playerSpeaking = false;
            lastSpeakEnd = Time.time;
        }
    }
}
