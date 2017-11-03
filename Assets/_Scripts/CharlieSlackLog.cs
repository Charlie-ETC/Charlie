using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Asyncoroutine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using Charlie;


public class CharlieSlackLog : MonoBehaviour {

    private string url;
    private string charlieEmoji;
    private string userEmoji;
    private string channel;
    private string apiaiSessionId;

    // Use this for initialization
    void Start()
    {
        // prepare token, parameters and connection if needed. Get it from local configuration
        ConfigService configService = ConfigService.Instance;
        Config config = configService.SelectedConfig();
        url = config.slackWebhookUrl;
        charlieEmoji = config.slackCharlieIcon;
        userEmoji = config.slackUserIcon;
        channel = config.slackChannel;
    }


    public void SetAPISessionID(string id) {
        apiaiSessionId = id;
    }

    // used in ActinoSpeak and DictationMonitor scripts
    // @username, name shown on slack, including api.ai sessionID 
    public async Task SlackLog(string username, string message)
    {
        //Debug.LogWarning(username + message);
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(message)) { return; }

        //Debug.LogWarning("[CharlieSlackLog] 00 Slack Log Sent.");

        // construct JSON
        SlackMessage slackMessage = new SlackMessage();
        slackMessage.text = message;
        slackMessage.username = username + " " + apiaiSessionId;
        slackMessage.channel = channel;

        if (username.ToLower().Contains("charlie"))
        {
            // Tmake it a converted json string from object?
            slackMessage.iconEmoji = charlieEmoji;
        }
        if (username.ToLower().Contains("user")) {
            slackMessage.iconEmoji = userEmoji;
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

        await request.SendWebRequest();
        //Debug.LogWarning(request.downloadHandler.text);
        //Debug.LogWarning("[CharlieSlackLog] Slack Log Sent.");       
    }

    class SlackMessage
    {
        public string text;
        public string username;
        public string iconEmoji;
        public string channel;
    }
}
