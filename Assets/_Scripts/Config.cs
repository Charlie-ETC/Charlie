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
    public string unsplashAppId;
    public string unsplashImageSize = "regular";
    public string slackWebhookUrl = "https://hooks.slack.com/services/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";
    public string slackCharlieIcon = ":girl:";
    public string slackUserIcon = ":dark_sunglasses:";
    public string slackChannel = "#charlie_history";
    public string slackDebugLogWebhookUrl = "https://hooks.slack.com/services/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";
    public string slackDebugIcon = ":package:";
    public string slackDebugChannel = "#charlie_debug_log";
    public bool giphyDebug;
    public string giphyApiKey;
}
