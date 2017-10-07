using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmEventGenerator : MonoBehaviour {

    PlayMakerFSM[] FSMArray;

    void Start()
    {
        FSMArray = gameObject.GetComponents<PlayMakerFSM>();
    }

    public void HandleResponse(Response resp)
    {
        //var intentName = resp?.result?.metadata?.intentName ?? "hello";
        if (string.IsNullOrEmpty(resp.result.metadata.intentName))
            return;

        foreach (var fsm in FSMArray)
        {
            fsm.SendEvent("Intent:" + resp.result.metadata.intentName);
        }
    }
}
