using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Asyncoroutine;

[RequireComponent(typeof(ConfigService))]
public class ApiaiService : MonoBehaviour {

    private string accessToken;

    void Start () {
        ConfigService service = GetComponent<ConfigService>();
        accessToken = service.SelectedConfig().apiaiClientAccessToken;
    }

    public string CreateSession()
    {
        return Guid.NewGuid().ToString();
    }

    public async Task<Response> Query(string sessionId, string text)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", $"Bearer {accessToken}");
        headers.Add("Content-Type", "application/json");

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

        string request = JsonConvert.SerializeObject(query, settings);
        WWW www = await new WWW("https://api.api.ai/v1/query",
            Encoding.UTF8.GetBytes(request), headers);
        Debug.Log(www.text);
        return JsonConvert.DeserializeObject<Response>(www.text, settings);
    }
}
