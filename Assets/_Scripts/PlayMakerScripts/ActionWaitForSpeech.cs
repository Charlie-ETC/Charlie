using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;

public class ActionWaitForSpeech : FsmStateAction
{
    public bool isActive;

    public override void OnEnter()
    {
        GameObject.FindGameObjectWithTag("TargetRoot").transform.Find("WindowAnim").GetComponent<WindowAnimControl>().ChangeBG(UserProfile.Content["FavoriteView"]);
        Finish();
    }
}
