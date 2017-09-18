using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Asyncoroutine;

[RequireComponent(typeof(ConfigService))]
public class ApiaiService : MonoBehaviour {

    private string accessToken;

    void Start ()
    {
        ConfigService service = GetComponent<ConfigService>();
        accessToken = service.SelectedConfig().apiaiClientAccessToken;
    }

    public string CreateSession()
    {
        return Guid.NewGuid().ToString();
    }

    public async Task<Response> Query(string sessionId, string text)
    {
        Query query = new Query
        {
            v = "20150910",
            lang = "en",
            query = text,
            sessionId = sessionId
        };

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
        await request.Send();

        return JsonConvert.DeserializeObject<Response>(
            request.downloadHandler.text, settings);
    }
}
