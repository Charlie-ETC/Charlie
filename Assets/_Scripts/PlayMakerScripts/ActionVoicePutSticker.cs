using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;
using Charlie.WatsonTTS;

public class ActionVoicePutSticker : FsmStateAction
{

    public override async void OnEnter()
    {

        Charlie.GestureManager.Instance.VoicePutObject();
        Finish();
    }
}
