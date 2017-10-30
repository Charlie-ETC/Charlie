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

    float timer;

    public override async void OnEnter()
    {
        timer = secs;
    }

    public override void OnUpdate()
    {
        if (!Enabled)
            return;
        timer -= Time.deltaTime;
        if (timer < 0)
            Finish();
    }
}
