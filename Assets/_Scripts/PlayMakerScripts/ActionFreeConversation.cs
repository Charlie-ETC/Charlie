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
    [UIHint(UIHint.TextArea)]
    public FsmString speakWhenSilence;
    public bool speakSequencal;

    bool wasLookingAtPlayer;
    GameObject go;

    float stateStartTime;
    float lastSpeakEnd;
    Queue<string> speechQueue = new Queue<string>();

    public override void OnEnter()
    {
        go = Fsm.GetOwnerDefaultTarget(owner);
        //wasLookingAtPlayer = go.GetComponent<LookatPlayer>().enabled;
        wasLookingAtPlayer = go.GetComponentInChildren<LookAtPlayerIKControl>().isActive;
        //go.GetComponent<LookatPlayer>().enabled = true;
        go.GetComponentInChildren<LookAtPlayerIKControl>().isActive = true;

        stateStartTime = Time.time;
        lastSpeakEnd = Time.time;



        string[] speechArr = speakWhenSilence.ToString().Split('\n');

        if (!speakSequencal)
        {
            int n = speechArr.Length;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, speechArr.Length);
                var value = speechArr[k];
                speechArr[k] = speechArr[n];
                speechArr[n] = value;
            }
        }

        foreach (var line in speechArr)
        {
            if (!string.IsNullOrEmpty(line))
            {
                speechQueue.Enqueue(line);
            }
        }
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
        
        string actualSpeech = speechQueue.Dequeue();
        speechQueue.Enqueue(actualSpeech);

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
