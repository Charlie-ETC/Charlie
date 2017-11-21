using HutongGames.PlayMaker;
using System;
using UnityEngine;
using UnityEngine.UI;


public class ActionShowPhoto : FsmStateAction
{

    public override void OnEnter()
    {
        GameObject.Find("PhotoTaken").GetComponent<RawImage>().color = Color.white;
        GameObject.Find("PhotoFrameModelParent").transform.Find("photoframe_model").gameObject.SetActive(true);
        GameObject.Find("PhotoStage").transform.Find("Sticker").gameObject.SetActive(true);

        Finish();
    }
}

public class ActionHidePhoto : FsmStateAction
{
    GameObject photoFrameModel = GameObject.Find("PhotoFrameModelParent");

    public override void OnEnter()
    {

        GameObject.Find("PhotoTaken").GetComponent<RawImage>().color = Color.clear;
        GameObject.Find("PhotoFrameModelParent").transform.Find("photoframe_model").gameObject.SetActive(false);
        GameObject.Find("PhotoStage").transform.Find("Sticker").gameObject.SetActive(false);

        Finish();
    }
}

