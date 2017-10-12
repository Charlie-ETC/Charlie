using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;

[ActionCategory(ActionCategory.Audio)]
public class ActionTimeout : FsmStateAction
{
    public float secs;

    public override async void OnEnter()
    {

        await new WaitForSeconds(secs);
        Finish();
    }
}
