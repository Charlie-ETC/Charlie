using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Unosquare.Net;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Swan;
using Asyncoroutine;

namespace Charlie
{
    class CharlieRemote : Singleton<CharlieRemote>
    {
        public int port = 24275;

        private WebServer server;

        private CancellationTokenSource cts;

        private Task task;

        private static Dictionary<string, byte[]> preloadedWebUI = new Dictionary<string, byte[]>();

        public class RootController : WebApiController
        {
            [WebApiHandler(HttpVerbs.Get, "/")]
            public bool GetRoot(WebServer server, HttpListenerContext context)
            {
                byte[] response = preloadedWebUI["index.html"];
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.Write(response, 0, response.Length);
                return true;
            }

            [WebApiHandler(HttpVerbs.Get, "/dist/bundle.js")]
            public bool GetBundle(WebServer server, HttpListenerContext context)
            {
                byte[] response = preloadedWebUI["bundle.js"];
                context.Response.ContentType = "text/javascript";
                context.Response.OutputStream.Write(response, 0, response.Length);
                return true;
            }

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
                    SpatialMapper mapper = FindObjectOfType<SpatialMapper>();
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
                            writer.WriteStartObject();
                            writer.WritePropertyName("id");
                            writer.WriteValue(gameObject.GetHashCode());

                            Type gameObjectType = gameObject.GetType();
                            FieldInfo[] fieldInfos = gameObjectType.GetFields(
                                BindingFlags.Instance | BindingFlags.Public |
                                BindingFlags.FlattenHierarchy);
                            PropertyInfo[] propertyInfos = gameObjectType.GetProperties(
                                BindingFlags.Instance | BindingFlags.Public |
                                BindingFlags.FlattenHierarchy);

                            foreach (PropertyInfo propertyInfo in propertyInfos) {
                                string propertyValue = null;
                                bool gotException = false;
                                try {
                                    propertyValue = propertyInfo.GetValue(gameObject).ToString();
                                } catch (Exception err) {
                                    // Some properties are not meant to be read...
                                    gotException = true;
                                }

                                if (!gotException) {
                                    writer.WritePropertyName(propertyInfo.Name);
                                    writer.WriteValue(propertyValue);
                                }
                            }

                            foreach (FieldInfo fieldInfo in fieldInfos) {
                                string fieldValue = null;
                                bool gotException = false;
                                try {
                                    fieldValue = fieldInfo.GetValue(gameObject).ToString();
                                } catch (Exception err) {
                                    // Some properties are not meant to be read...
                                    gotException = true;
                                }

                                if (!gotException) {
                                    writer.WritePropertyName(fieldInfo.Name);
                                    writer.WriteValue(fieldValue);
                                }
                            }

                            writer.WriteEndObject();
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
                    DebugManager manager = FindObjectOfType<DebugManager>();
                    manager.Activate();
                });
                return true;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            PreloadWebUI();
            Terminal.OnLogMessageReceived += HandleLogMessageReceived;

            Debug.Log($"[CharlieRemote] new WebServer {port}");
            server = new WebServer(port);
            server = server.EnableCors();
            server.RegisterModule(new WebApiModule());
            server.Module<WebApiModule>().RegisterController<RootController>();

            cts = new CancellationTokenSource();
            task = server.RunAsync(cts.Token);
            Debug.Log($"[CharlieRemote] Running on port {port}");
        }

        protected override void OnDestroy()
        {
            Debug.Log("[CharlieRemote] Shutting down HTTP server");
            cts.Cancel();
            server.Dispose();
            server = null;
            Terminal.OnLogMessageReceived -= HandleLogMessageReceived;
            base.OnDestroy();
        }

        async void PreloadWebUI()
        {
            ResourceRequest request = Resources.LoadAsync("Web/index");
            await request;
            TextAsset asset = request.asset as TextAsset;
            preloadedWebUI.Add("index.html", asset.bytes);

            request = Resources.LoadAsync("Web/bundle.js");
            await request;
            asset = request.asset as TextAsset;
            preloadedWebUI.Add("bundle.js", asset.bytes);
        }

        void HandleLogMessageReceived(object source, LogMessageReceivedEventArgs e)
        {
            switch (e.MessageType)
            {
                case LogMessageType.Debug:
                case LogMessageType.Info:
                case LogMessageType.Trace:
                    Debug.Log(e.Source.Length == 0 ? e.Message : $"[{e.Source}] {e.Message}");
                    break;
                case LogMessageType.Warning:
                    Debug.LogWarning(e.Source.Length == 0 ? e.Message : $"[{e.Source}] {e.Message}");
                    break;
                case LogMessageType.Error:
                    Debug.LogError(e.Source.Length == 0 ? e.Message : $"[{e.Source}] {e.Message}");
                    break;
            }
        }
    }
}
