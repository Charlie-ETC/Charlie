using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Asyncoroutine;

namespace Charlie.Giphy
{
    public class GiphyService : Singleton<GiphyService>
    {
        private bool giphyDebug;
        private string giphyApiKey;
        private string giphyApiBaseUrl;

        private void Start()
        {
            ConfigService service = ConfigService.Instance;
            giphyApiKey = service.SelectedConfig().giphyApiKey;
            giphyDebug = service.SelectedConfig().giphyDebug;
        }

        private string BuildQueryString(NameValueCollection parameters)
        {
            return string.Join("&",
                parameters.AllKeys.Select(key => $"{WWW.EscapeURL(key)}={WWW.EscapeURL(parameters[key])}"));
        }

        private string BuildURL(string path, NameValueCollection parameters)
        {
            parameters.Add("api_key", giphyApiKey);
            return $"https://api.giphy.com/v1{path}?{BuildQueryString(parameters)}";
        }

        // <summary>
        // Searches the GIPHY Stickers API for stickers.
        // </summary>
        // <param name="q">The query string.</param>
        // <param name="limit">Number of results to request for.</param>
        // <param name="offset">Number of results to skip.</param>
        // <param name="rating">Filter the search results by content rating.</param>
        // <param name="lang">Filter the search results by language.</param>
        public async Task<Response<List<Sticker>>> Search(string q, int limit,
            int offset, string rating, string lang)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            NameValueCollection parameters = new NameValueCollection
            {
                { "q", q },
                { "limit", limit.ToString() },
                { "offset", offset.ToString() },
                { "rating", rating },
                { "lang", lang }
            };

            string url = BuildURL("/stickers/search", parameters);
            UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (giphyDebug)
            {
                Debug.Log($"[GiphyService] Made request to {url}");
                Debug.Log($"[GiphyService] {request.downloadHandler.text}");
            }

            if (request.isHttpError)
            {
                throw new GiphyException($"Request failed with HTTP status code {request.responseCode}");
            }

            return JsonConvert.DeserializeObject<Response<List<Sticker>>>(
                request.downloadHandler.text);
        }

        public Task<Response<List<Sticker>>> Search(string q)
        {
            return Search(q, 25, 0);
        }

        public Task<Response<List<Sticker>>> Search(string q, int limit, int offset)
        {
            return Search(q, limit, offset, "G");
        }

        public Task<Response<List<Sticker>>> Search(string q, int limit, int offset,
            string rating)
        {
            return Search(q, limit, offset, rating, "en");
        }
    }
}
