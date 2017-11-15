using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Swan;

namespace Charlie.Remote
{
    public class CharlieRemote : Singleton<CharlieRemote>
    {
        public int port = 24275;

        private WebServer server;

        private CancellationTokenSource cts;

        private Task task;

        protected override void Awake()
        {
            base.Awake();
            Terminal.OnLogMessageReceived += HandleLogMessageReceived;

            Debug.Log($"[CharlieRemote] new WebServer {port}");
            server = new WebServer(port);
            server = server.EnableCors();
            server.RegisterModule(new WebApiModule());
            server.RegisterModule(new RootModule());
            server.Module<WebApiModule>().RegisterController<ApiController>(MakeApiController);

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

        private ApiController MakeApiController()
        {
            return new ApiController();
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
