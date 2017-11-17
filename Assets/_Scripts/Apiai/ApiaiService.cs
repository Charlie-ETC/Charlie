using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Asyncoroutine;

namespace Charlie.Apiai
{
    [RequireComponent(typeof(ConfigService))]
    public class ApiaiService : Singleton<ApiaiService>
    {

        private string accessToken;

        void Start()
        {
            accessToken = ConfigService.Instance.SelectedConfig().apiaiClientAccessToken;
        }

        public string CreateSession()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<Response> Query(string sessionId, string text, bool sendAsEvent)
        {
            Query query = new Query
            {
                v = "20150910",
                lang = "en",
                sessionId = sessionId
            };

            if (sendAsEvent)
            {
                query.e = new Query.Event
                {
                    name = text
                };
            }
            else
            {
                query.query = text;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            string requestBody = JsonConvert.SerializeObject(query, settings);
            UnityWebRequest request = new UnityWebRequest("https://api.api.ai/v1/query")
            {
                method = UnityWebRequest.kHttpVerbPOST,
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();

            Debug.Log(request.downloadHandler.text);
            return JsonConvert.DeserializeObject<Response>(
                request.downloadHandler.text, settings);
        }

        public async Task<Response> Query(string sessionId, string text)
        {
            return await Query(sessionId, text, false);
        }

        public async Task<List<Context>> GetContexts(string sessionId)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            UnityWebRequest request = new UnityWebRequest($"https://api.api.ai/v1/contexts?sessionId={sessionId}")
            {
                method = UnityWebRequest.kHttpVerbGET,
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();

            return JsonConvert.DeserializeObject<List<Context>>(
                request.downloadHandler.text, settings);
        }

        public async Task PostContext(string sessionId, Context context)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            string requestBody = JsonConvert.SerializeObject(context, settings);
            UnityWebRequest request = new UnityWebRequest($"https://api.api.ai/v1/contexts?sessionId={sessionId}")
            {
                method = UnityWebRequest.kHttpVerbPOST,
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody))
            };
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();

            Debug.Log(sessionId);
            Debug.Log(request.downloadHandler.text);
        }

        public async Task DeleteContexts(string sessionId)
        {
            UnityWebRequest request = new UnityWebRequest($"https://api.api.ai/v1/contexts?sessionId={sessionId}")
            {
                method = UnityWebRequest.kHttpVerbDELETE
            };
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();
        }

        public async Task DeleteContext(string sessionId, string contextName)
        {
            UnityWebRequest request = new UnityWebRequest($"https://api.api.ai/v1/contexts/{contextName}?sessionId={sessionId}")
            {
                method = UnityWebRequest.kHttpVerbDELETE
            };
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();
        }
    }
}
