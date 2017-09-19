using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "default", menuName = "Charlie/Configuration")]
public class Config : ScriptableObject {
    public string apiaiClientAccessToken;
    public string ibmWatsonTtsUrl = "https://stream.watsonplatform.net/text-to-speech/api";
    public string ibmWatsonUsername;
    public string ibmWatsonPassword;
    public string twitterConsumerKey;
    public string twitterConsumerSecret;
    public string twitterAccessToken;
    public string twitterAccessTokenSecret;
}
