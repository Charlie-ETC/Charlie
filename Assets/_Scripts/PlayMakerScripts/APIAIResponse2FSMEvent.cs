using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIAIResponse2FSMEvent : MonoBehaviour {

    PlayMakerFSM[] FSMArray;

    void Start()
    {
        FSMArray = gameObject.GetComponents<PlayMakerFSM>();
    }

    public void HandleResponse(Response resp)
    {
        if (string.IsNullOrEmpty(resp.result.metadata.intentName))
            return;

        foreach (var fsm in FSMArray)
        {
            fsm.SendEvent("Intent:" + resp.result.metadata.intentName);
        }
    }
}
