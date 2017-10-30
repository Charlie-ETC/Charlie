using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Asyncoroutine;
using IBM.Watson.DeveloperCloud.Utilities;

namespace Charlie.WatsonTTS
{
    public class WatsonTTSService : Singleton<WatsonTTSService>
    {
        public string voice = "en-US_LisaVoice";

        private string ibmWatsonTtsUrl;
        private string ibmWatsonUsername;
        private string ibmWatsonPassword;

        private string ibmWatsonToken;

        public static WatsonTTSService Instance;

        private void Start()
        {
            Instance = this;

            ConfigService service = GetComponent<ConfigService>();
            ibmWatsonTtsUrl = service.SelectedConfig().ibmWatsonTtsUrl;
            ibmWatsonUsername = service.SelectedConfig().ibmWatsonUsername;
            ibmWatsonPassword = service.SelectedConfig().ibmWatsonPassword;

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
            await request.SendWebRequest();

            ibmWatsonToken = request.downloadHandler.text;
        }

        public async Task<AudioClip> Synthesize(string text)
        {
            // Define a default voice transformation.
            // TODO: Turn this into a custom voice instead.
            text = WWW.EscapeURL($"<voice-transformation type=\"Young\" strength=\"80%\">{text}</voice-transformation>", Encoding.UTF8);

            UnityWebRequest request = UnityWebRequest.Get(
                $"{ibmWatsonTtsUrl}/v1/synthesize?text={text}&voice={voice}");
            request.SetRequestHeader("X-Watson-Authorization-Token", ibmWatsonToken);
            request.SetRequestHeader("Accept", "audio/wav");
            await request.SendWebRequest();

            return WaveFile.ParseWAV("watson", request.downloadHandler.data);
        }
    }
}
