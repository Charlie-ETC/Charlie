using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Asyncoroutine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

public class CharlieSlackLog : MonoBehaviour {
    // TODO: get url from configuration
    string url = "https://hooks.slack.com/services/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";

	// Use this for initialization
	void Start () {
        // prepare token and connection if needed


    }

    public async Task SlackLog(string username, string message)
    {
        Debug.LogWarning(username + message);
        //Debug.LogWarning("[CharlieSlackLog] 1.");
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(message)) { return; }
        
        Debug.LogWarning("[CharlieSlackLog] 00 Slack Log Sent.");

        // construct JSON
        SlackMessage slackMessage = new SlackMessage();
        if (username.ToLower() == "charlie")
        {
            // replace with girl icon and username
            // TODO make it a converted json string from object?
            slackMessage.text = message;
            slackMessage.username = username;
            slackMessage.iconEmoji = ":girl:";
            slackMessage.channel = "#charlie_history";
        }
        if (username.ToLower() == "user") {
            Debug.LogWarning("[CharlieSlackLog] 00 Slack Log Sent.");
            // replace with girl icon and username
            slackMessage.text = message;
            slackMessage.username = username;
            slackMessage.iconEmoji = ":dark_sunglasses:";
            slackMessage.channel = "#charlie_history";
        }


        // post request
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
        
        string JSONstring = JsonConvert.SerializeObject(slackMessage, settings);
        UnityWebRequest request = new UnityWebRequest(url)
        {
            method = UnityWebRequest.kHttpVerbPOST,
            uploadHandler = new UploadHandlerRaw(
                Encoding.UTF8.GetBytes(JSONstring)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-type", "application/json");

        // TODO change this to await?
        await request.SendWebRequest();
        Debug.LogWarning(request.downloadHandler.text);
        Debug.LogWarning("[CharlieSlackLog] Slack Log Sent.");
        
    }

    class SlackMessage
    {
        public string text;
        public string username;
        public string iconEmoji;
        public string channel;
    }
}
