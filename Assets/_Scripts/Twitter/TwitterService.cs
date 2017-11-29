using System;
using System.Collections.Generic;
#if NETFX_CORE
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
#else
using System.Security.Cryptography;
#endif
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Asyncoroutine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Charlie.Twitter
{
    public class TwitterService : Singleton<TwitterService>
    {

        private string consumerKey;
        private string consumerSecret;
        private string accessToken;
        private string accessTokenSecret;

        void Start()
        {
            ConfigService service = ConfigService.Instance;
            Config config = service.SelectedConfig();
            consumerKey = config.twitterConsumerKey;
            consumerSecret = config.twitterConsumerSecret;
            accessToken = config.twitterAccessToken;
            accessTokenSecret = config.twitterAccessTokenSecret;
        }

        private string GenerateNonce()
        {
            byte[] bytes = new byte[32];
            System.Random random = new System.Random();
            random.NextBytes(bytes);
            return Regex.Replace(Convert.ToBase64String(bytes), @"[^\w@-]", "");
        }

        private string GetSigningKey()
        {
            return $"{Uri.EscapeDataString(consumerSecret)}&" +
                $"{Uri.EscapeDataString(accessTokenSecret)}";
        }

        private string CreateParameterString(
            Dictionary<string, string> oauthParams,
            Dictionary<string, string> requestParams)
        {
            StringBuilder builder = new StringBuilder();
            Dictionary<string, string> allParams =
                new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> param in oauthParams)
            {
                allParams.Add(
                    Uri.EscapeDataString(param.Key),
                    Uri.EscapeDataString(param.Value)
                );
            }

            foreach (KeyValuePair<string, string> param in requestParams)
            {
                allParams.Add(
                    Uri.EscapeDataString(param.Key),
                    Uri.EscapeDataString(param.Value)
                );
            }

            List<string> keys = new List<string>(allParams.Keys);
            keys.Sort();

            foreach (string key in keys)
            {
                builder.AppendFormat("&{0}={1}", key, allParams[key]);
            }

            return builder.ToString().Substring(1);
        }

        private string CreateBaseString(string method, string url,
            Dictionary<string, string> oauthParams,
            Dictionary<string, string> requestParams)
        {
            string parameterString = CreateParameterString(oauthParams, requestParams);
            return $"{method.ToUpper()}&{Uri.EscapeDataString(url)}&" +
                $"{Uri.EscapeDataString(parameterString)}";
        }

        private string CreateSignature(string method, string url,
            Dictionary<string, string> oauthParams,
            Dictionary<string, string> requestParams)
        {
            string key = GetSigningKey();
            string baseString = CreateBaseString(method, url, oauthParams,
                requestParams);
            byte[] baseStringBytes = Encoding.UTF8.GetBytes(baseString);

#if NETFX_CORE
        MacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);
        CryptographicHash hmac = provider.CreateHash(
            CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8));
        hmac.Append(WindowsRuntimeBufferExtensions.AsBuffer(baseStringBytes));
        byte[] hash = WindowsRuntimeBufferExtensions.ToArray(hmac.GetValueAndReset());
#else
            HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key));
            byte[] hash = hmac.ComputeHash(baseStringBytes);
#endif

            return Convert.ToBase64String(hash);
        }

        UnityWebRequest BuildRequest(string method, string url,
            Dictionary<string, string> requestParams)
        {
            // Prepare the Authorization header.
            Dictionary<string, string> oauthParams = new Dictionary<string, string>() {
            { "oauth_consumer_key", consumerKey },
            { "oauth_nonce", GenerateNonce() },
            { "oauth_signature_method", "HMAC-SHA1" },
            { "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() },
            { "oauth_token", accessToken },
            { "oauth_version", "1.0" }
        };

            string signature = CreateSignature(method, url, oauthParams,
                requestParams);
            oauthParams.Add("oauth_signature", signature);

            StringBuilder oauthBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> param in oauthParams)
            {
                oauthBuilder.AppendFormat("{0}=\"{1}\", ", Uri.EscapeDataString(param.Key),
                    Uri.EscapeDataString(param.Value));
            }

            string authorization = $"OAuth {oauthBuilder.ToString(0, oauthBuilder.Length - 2)}";

            // Prepare to include the request parameters in the body.
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> param in requestParams)
            {
                builder.AppendFormat("&{0}={1}", Uri.EscapeDataString(param.Key),
                    Uri.EscapeDataString(param.Value));
            }

            String requestBody = builder.ToString();
            UnityWebRequest request = new UnityWebRequest(url)
            {
                method = method,
                uploadHandler = requestBody.Length > 0 ?
                    new UploadHandlerRaw(
                        Encoding.UTF8.GetBytes(requestBody.Substring(1))) :
                    null,
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Authorization", authorization);
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            return request;
        }

        public async void Tweet(string text)
        {
            UnityWebRequest request = BuildRequest(UnityWebRequest.kHttpVerbPOST,
                "https://api.twitter.com/1.1/statuses/update.json",
                new Dictionary<string, string>()
                {
                { "status", text }
                });
            await request.SendWebRequest();
            
            if (request.isHttpError)
            {
                throw new TwitterException($"Request failed with HTTP status code {request.responseCode}");
            }
        }

        public async void TweetWithMedia(string text, string[] mediaIds)
        {
            UnityWebRequest request = BuildRequest(UnityWebRequest.kHttpVerbPOST,
                "https://api.twitter.com/1.1/statuses/update.json",
                new Dictionary<string, string>()
                {
                { "status", text },
                { "media_ids", String.Join(",", mediaIds) }
                });
            await request.SendWebRequest();
            
            if (request.isHttpError)
            {
                throw new TwitterException($"Request failed with HTTP status code {request.responseCode}");
            }
        }

        public async Task<Media> UploadMedia(byte[] data)
        {
            UnityWebRequest request = BuildRequest(UnityWebRequest.kHttpVerbPOST,
                "https://upload.twitter.com/1.1/media/upload.json",
                new Dictionary<string, string>() { });

            // Since this is a multipart/form-data request, we need to have a boundary.
            byte[] boundary = UnityWebRequest.GenerateBoundary();

            // Construct the request body.
            List<byte> requestBody = new List<byte>();
            requestBody.AddRange(Encoding.UTF8.GetBytes("--"));
            requestBody.AddRange(boundary);
            requestBody.AddRange(Encoding.UTF8.GetBytes("\r\n"));
            requestBody.AddRange(Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"media\""));
            requestBody.AddRange(Encoding.UTF8.GetBytes("\r\n\r\n"));

            requestBody.AddRange(data);
            requestBody.AddRange(Encoding.UTF8.GetBytes("\r\n"));

            requestBody.AddRange(Encoding.UTF8.GetBytes("--"));
            requestBody.AddRange(boundary);
            requestBody.AddRange(Encoding.UTF8.GetBytes("--\r\n"));
            request.uploadHandler = new UploadHandlerRaw(requestBody.ToArray());

            // Add the boundary information into the Content-Type header.
            List<byte> contentType = new List<byte>();
            contentType.AddRange(Encoding.UTF8.GetBytes("multipart/form-data; boundary="));
            contentType.AddRange(boundary);

            request.SetRequestHeader("Content-Type", Encoding.UTF8.GetString(contentType.ToArray()));
            await request.SendWebRequest();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            if (request.isHttpError)
            {
                throw new TwitterException($"Request failed with HTTP status code {request.responseCode}");
            }

            return JsonConvert.DeserializeObject<Media>(
                request.downloadHandler.text, settings);
        }
    }
}
