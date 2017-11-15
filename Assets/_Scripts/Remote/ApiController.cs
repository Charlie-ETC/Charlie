using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Net;

namespace Charlie.Remote
{
    public class ApiController : WebApiController
    {
        [WebApiHandler(HttpVerbs.Get, "/api/reloadScene")]
        public bool GetReloadScene(WebServer server, HttpListenerContext context)
        {
            Unosquare.Labs.EmbedIO.Extensions.JsonResponse(context, "{}");
            MainThreadDispatcher.Instance.Dispatch(() =>
            {
                SceneManager.LoadScene("Main");
            });
            return true;
        }

        [WebApiHandler(HttpVerbs.Get, "/api/finishMapping")]
        public bool GetFinishMapping(WebServer server, HttpListenerContext context)
        {
            Unosquare.Labs.EmbedIO.Extensions.JsonResponse(context, "{}");
            MainThreadDispatcher.Instance.Dispatch(() =>
            {
                SpatialMapper mapper = GameObject.FindObjectOfType<SpatialMapper>();
                if (mapper != null && mapper.CanFinishMapping)
                {
                    mapper.FinishMapping();
                }
                else
                {
                    Debug.Log("[CharlieRemote] Cannot finish spatial mapping");
                }
            });
            return true;
        }

        [WebApiHandler(HttpVerbs.Get, "/api/testCharliePosition")]
        public async Task<bool> GetTestCharliePosition(WebServer server, HttpListenerContext context)
        {
            string position = null;
            await MainThreadDispatcher.Instance.Dispatch(() =>
            {
                Debug.Log("[CharlieRemote] Getting GameObject and transform position");
                GameObject charlie = GameObject.Find("Charlie");
                position = JsonUtility.ToJson(charlie.transform.position);
            });

            byte[] response = Encoding.UTF8.GetBytes(position);
            context.Response.OutputStream.Write(response, 0, response.Length);
            return true;
        }

        [WebApiHandler(HttpVerbs.Get, "/api/gameObjects")]
        public async Task<bool> getGameObjects(WebServer server, HttpListenerContext context)
        {
            string gameObjectsJson = null;
            await MainThreadDispatcher.Instance.Dispatch(() =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                StringWriter stringWriter = new StringWriter(stringBuilder);
                using (JsonWriter writer = new JsonTextWriter(stringWriter)) {
                    Scene scene = SceneManager.GetActiveScene();
                    GameObject[] gameObjects = scene.GetRootGameObjects();
                    writer.WriteStartArray();

                    int sceneCount = SceneManager.sceneCountInBuildSettings;
                    foreach (GameObject gameObject in gameObjects)
                    {
                        writer.WriteRawValue(SerializationUtils.Serialize(gameObject));
                    }

                    writer.WriteEndArray();
                    gameObjectsJson = stringBuilder.ToString();
                }
            });

            byte[] response = Encoding.UTF8.GetBytes(gameObjectsJson);
            context.Response.ContentType = "application/json";
            context.Response.OutputStream.Write(response, 0, response.Length);
            return true;
        }

        [WebApiHandler(HttpVerbs.Get, "/api/scenes")]
        public async Task<bool> getScenes(WebServer server, HttpListenerContext context)
        {
            string scenesJson = null;
            await MainThreadDispatcher.Instance.Dispatch(() =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                StringWriter stringWriter = new StringWriter(stringBuilder);
                using (JsonWriter writer = new JsonTextWriter(stringWriter)) {
                    writer.WriteStartArray();

                    int sceneCount = SceneManager.sceneCountInBuildSettings;
                    for (int i = 0; i < sceneCount; i++)
                    {
                        Scene scene = SceneManager.GetSceneByBuildIndex(i);
                        writer.WriteStartObject();
                        writer.WritePropertyName("name");
                        writer.WriteValue(scene.name);
                        writer.WritePropertyName("path");
                        writer.WriteValue(SceneUtility.GetScenePathByBuildIndex(i));
                        writer.WritePropertyName("isLoaded");
                        writer.WriteValue(scene.isLoaded);
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                    scenesJson = stringBuilder.ToString();
                }
            });

            byte[] response = Encoding.UTF8.GetBytes(scenesJson);
            context.Response.OutputStream.Write(response, 0, response.Length);
            return true;
        }

        [WebApiHandler(HttpVerbs.Get, "/api/debug/on")]
        public bool GetDebugOn(WebServer server, HttpListenerContext context)
        {
            Unosquare.Labs.EmbedIO.Extensions.JsonResponse(context, "{}");
            MainThreadDispatcher.Instance.Dispatch(() =>
            {
                DebugManager manager = GameObject.FindObjectOfType<DebugManager>();
                manager.Activate();
            });
            return true;
        }
    }
}
