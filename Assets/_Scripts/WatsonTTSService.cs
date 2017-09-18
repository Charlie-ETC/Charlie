using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Asyncoroutine;
using IBM.Watson.DeveloperCloud.Utilities;

public class WatsonTTSService : MonoBehaviour
{
    private string ibmWatsonTtsUrl;
    private string ibmWatsonUsername;
    private string ibmWatsonPassword;

    private string ibmWatsonToken;

    private void Start()
    {
        ConfigService service = GetComponent<ConfigService>();
        ibmWatsonTtsUrl = service.SelectedConfig().ibmWatsonTtsUrl;
        ibmWatsonUsername = service.SelectedConfig().ibmWatsonUsername;
        ibmWatsonPassword= service.SelectedConfig().ibmWatsonPassword;

        // Retrieve the token from Watson before starting.
        Initialize();
    }

    public async void Initialize()
    {
        string encodedCredentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{ibmWatsonUsername}:{ibmWatsonPassword}"));
        string encodedUrl = WWW.EscapeURL(ibmWatsonTtsUrl, Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Get(
            $"https://stream.watsonplatform.net/authorization/api/v1/token?url={encodedUrl}");
        request.SetRequestHeader("Authorization", $"Basic {encodedCredentials}");
        await request.Send();

        ibmWatsonToken = request.downloadHandler.text;
    }

    public async Task<AudioClip> Synthesize(string text)
    {
        UnityWebRequest request = UnityWebRequest.Get(
            $"{ibmWatsonTtsUrl}/v1/synthesize?text={text}");
        request.SetRequestHeader("X-Watson-Authorization-Token", ibmWatsonToken);
        request.SetRequestHeader("Accept", "audio/wav");
        await request.Send();

        return WaveFile.ParseWAV("watson", request.downloadHandler.data);
    }
}
