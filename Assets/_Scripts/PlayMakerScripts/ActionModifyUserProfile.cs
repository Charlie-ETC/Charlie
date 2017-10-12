using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using System.Text.RegularExpressions;

public class ActionModifyUserProfile : FsmStateAction
{

    public FsmString key;
    public FsmString value;

    public override void OnEnter()
    {
        UserProfile.Content[key.ToString()] = value.ToString();
        Finish();
    }
}

public class UserProfile
{
    public static Dictionary<string, string> Content = new Dictionary<string, string>();
}
