using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;

public class ActionActivitaGO : FsmStateAction
{
    public string targetName;
    public bool isActive;

    public override void OnEnter()
    {
        GameObject.FindGameObjectWithTag("TargetRoot").transform.Find(targetName).gameObject.SetActive(isActive);
        Finish();
    }

}


