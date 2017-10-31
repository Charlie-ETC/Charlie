using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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

        private async Task<string> CallAPI(string path, NameValueCollection parameters)
        {
            string url = BuildURL(path, parameters);
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

            return request.downloadHandler.text;
        }

        // <summary>
        // Searches the GIPHY Stickers API for stickers.
        // </summary>
        // <param name="q">Search query term or phrase.</param>
        // <param name="limit">Maximum number of records to return.</param>
        // <param name="offset">An optional results offset.</param>
        // <param name="rating">Filter the search results by content rating.</param>
        // <param name="lang">Specify default country for regional content.</param>
        public async Task<Response<List<Sticker>>> Search(string q, int limit = 25,
            int offset = 0, string rating = "G", string lang = "en")
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

            string response = await CallAPI("/stickers/search", parameters);
            return JsonConvert.DeserializeObject<Response<List<Sticker>>>(
                response, settings);
        }

        // <summary>
        // Gets the trending stickers.
        // </summary>
        // <param name="limit">Maximum number of records to return.</param>
        // <param name="rating">Filter the search results by content rating.</param>
        public async Task<Response<List<Sticker>>> Trending(int limit = 25, string rating = "G")
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            NameValueCollection parameters = new NameValueCollection
            {
                { "limit", limit.ToString() },
                { "rating", rating }
            };

            string response = await CallAPI("/stickers/trending", parameters);
            return JsonConvert.DeserializeObject<Response<List<Sticker>>>(
                response, settings);
        }

        // <summary>
        // Gets a random sticker.
        // </summary>
        // <param name="tag">Filter results by specified tag.</param>
        // <param name="rating">Filter results by rating.</param>
        public async Task<Response<Sticker>> Random(string tag, string rating = "G")
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            NameValueCollection parameters = new NameValueCollection
            {
                { "tag", tag },
                { "rating", rating }
            };

            string response = await CallAPI("/stickers/random", parameters);
            return JsonConvert.DeserializeObject<Response<Sticker>>(
                response, settings);
        }

        // <summary>
        // Gets a sticker by ID.
        // </summary>
        // <param name="id">Filter results by specified GIF ID.</param>
        public async Task<Response<Sticker>> GetByID(string id)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            NameValueCollection parameters = new NameValueCollection();
            string response = await CallAPI($"/gifs/{id}", parameters);
            return JsonConvert.DeserializeObject<Response<Sticker>>(
                response, settings);
        }

        // <summary>
        // Gets sticker packs.
        // </summary>
        public async Task<Response<List<StickerPack>>> ListPacks()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            NameValueCollection parameters = new NameValueCollection();
            string response = await CallAPI("/stickers/packs", parameters);
            return JsonConvert.DeserializeObject<Response<List<StickerPack>>>(
                response, settings);
        }

        // <summary>
        // Gets stickers in pack.
        // </summary>
        // <param name="id">Filters resilts by specified Sticker Pack ID.</param>
        // <param name="limit">Maximum number of records to return.</param>
        // <param name="offset">An optional results offset.</param>
        public async Task<Response<List<Sticker>>> StickersInPack(int id,
            int limit = 25, int offset = 0)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            NameValueCollection parameters = new NameValueCollection
            {
                { "limit", limit.ToString() },
                { "offset", offset.ToString() }
            };

            string response = await CallAPI($"/stickers/packs/{id}/stickers", parameters);
            return JsonConvert.DeserializeObject<Response<List<Sticker>>>(
                response, settings);
        }
    }
}
