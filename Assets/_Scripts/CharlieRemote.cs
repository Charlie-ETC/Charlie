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

namespace Charlie
{
    class CharlieRemote : Singleton<CharlieRemote>
    {
        public int port = 24275;

        private WebServer server;

        private CancellationTokenSource cts;

        private Task task;

        public class RootController : WebApiController
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

            [WebApiHandler(HttpVerbs.Get, "/api/scenes")]
            public async Task<bool> getScenes(WebServer server, HttpListenerContext context)
            {
                string scenesJson = null;
                await MainThreadDispatcher.Instance.Dispatch(() =>
                {
                    Scene[] scenes = SceneManager.GetAllScenes();
                    scenesJson = JsonUtility.ToJson(scenes);
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
            Terminal.OnLogMessageReceived += HandleLogMessageReceived;

            Debug.Log($"[CharlieRemote] new WebServer {port}");
            server = new WebServer(port);
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

        void HandleLogMessageReceived(object source, LogMessageReceivedEventArgs e) {
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
