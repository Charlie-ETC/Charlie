using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using System.Text.RegularExpressions;
using Charlie.Apiai;

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

    public static async Task<string> TryGetApiaiContext(string key)
    {
        List<Context> contexts = await ApiaiService.Instance.GetContexts(DictationMonitor.Instance.apiaiSessionId);
        Context userContext = contexts.Find(context => context.name == "user");
        // Perform null check for userContext.
        Debug.Log(userContext);
        Debug.Log(userContext.parameters);
        Debug.Log(userContext.parameters.ContainsKey(key));
        if (userContext != null && userContext.parameters != null && userContext.parameters.ContainsKey(key))
        {
            var value = userContext.parameters[key] as string;
            if (value != null)
            {

                Debug.Log($"key {key} , value{value}");
                return value;
            }
        }

        Debug.Log($"key {key} doesn't exist");
        return null;
    }
}
