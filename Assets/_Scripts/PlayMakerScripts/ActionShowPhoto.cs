using HutongGames.PlayMaker;
using System;
using UnityEngine;
using UnityEngine.UI;


public class ActionShowPhoto : FsmStateAction
{

    public override void OnEnter()
    {

        GameObject.Find("PhotoTaken").GetComponent<RawImage>().color = Color.white;
    }
}

public class ActionHidePhoto : FsmStateAction
{

    public override void OnEnter()
    {

        GameObject.Find("PhotoTaken").GetComponent<RawImage>().color = Color.clear;
    }
}

