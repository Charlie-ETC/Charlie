using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Net;
using Asyncoroutine;

namespace Charlie.Remote
{
    public class RootModule : WebModuleBase
    {
        public override string Name => nameof(RootModule);

        private static Dictionary<string, byte[]> preloadedWebUI = new Dictionary<string, byte[]>();

        public RootModule()
        {
            PreloadWebUI();
            AddHandler("/dist/bundle.js", HttpVerbs.Get, GetBundle);
            AddHandler(ModuleMap.AnyPath, HttpVerbs.Get, GetRoot);
        }

        public Task<bool> GetRoot(HttpListenerContext context, CancellationToken cts)
        {
            if (context.RequestPath().StartsWith("/dist/bundle.js") ||
                context.RequestPath().StartsWith("/api")) {
                return Task.FromResult(false);
            }

            byte[] response = preloadedWebUI["index.html"];
            context.Response.ContentType = "text/html";
            context.Response.OutputStream.Write(response, 0, response.Length);
            return Task.FromResult(true);
        }

        public Task<bool> GetBundle(HttpListenerContext context, CancellationToken cts)
        {
            byte[] response = preloadedWebUI["bundle.js"];
            context.Response.ContentType = "text/javascript";
            context.Response.OutputStream.Write(response, 0, response.Length);
            return Task.FromResult(true);
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
    }
}
