using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;


public class ActionResetDictation : FsmStateAction
{
    public override async void OnEnter()
    {
        GameObject.FindObjectOfType<DictationRecognizer>().ResetDictationRecognizer();
        await new WaitForNextFrame();
        Finish();
    }
}
