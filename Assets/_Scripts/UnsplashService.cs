using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Asyncoroutine;

namespace Unsplash
{

    [RequireComponent(typeof(ConfigService))]
    public class UnsplashService : MonoBehaviour
    {
        private string unsplashAppId;
        private string unsplashImageSize;

        private void Start()
        {
            ConfigService service = GetComponent<ConfigService>();
            unsplashAppId = service.SelectedConfig().unsplashAppId;
            unsplashImageSize = service.SelectedConfig().unsplashImageSize;
        }

        public async Task<Response> SearchPhotos(string query)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            UnityWebRequest request = new UnityWebRequest($"https://api.unsplash.com/search/photos?query={query}")
            {
                method = UnityWebRequest.kHttpVerbGET,
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Authorization", $"Client-ID {unsplashAppId}");
            request.SetRequestHeader("Accept-Version", "v1");
            await request.SendWebRequest();

            return JsonConvert.DeserializeObject<Response>(
                request.downloadHandler.text, settings);
        }

        public async Task<Texture> GetRandomPhoto(string query)
        {
            Response response = await SearchPhotos(query);
            if (response?.results?.Length == 0)
            {
                return null;
            }

            UnityWebRequest request = new UnityWebRequest(response?.results?[0]?.urls[unsplashImageSize])
            {
                method = UnityWebRequest.kHttpVerbGET,
                downloadHandler = new DownloadHandlerTexture()
            };
            await request.SendWebRequest();

            return DownloadHandlerTexture.GetContent(request);
        }
    }
}
