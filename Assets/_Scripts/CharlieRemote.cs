using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unosquare.Net;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Labs.EmbedIO.Constants;

namespace Charlie
{
    class CharlieRemote : MonoBehaviour
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
                MainThreadDispatcher.Instance.Dispatch(() =>
                {
                    SceneManager.LoadScene("Main");
                });
                return Extensions.JsonResponse(context, "{}");
            }
        }

        void Awake()
        {
            server = new WebServer(port);
            server.RegisterModule(new WebApiModule());
            server.Module<WebApiModule>().RegisterController<RootController>();

            cts = new CancellationTokenSource();
            task = server.RunAsync(cts.Token);
            Debug.Log($"[CharlieRemote] Running on port {port}");
        }

        void OnDestroy()
        {
            Debug.Log("[CharlieRemote] Shutting down HTTP server");
            cts.Cancel();
        }
    }
}
