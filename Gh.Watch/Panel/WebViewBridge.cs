using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Gh.Watch.Panel
{
    // Owns the WebView2 process: creates the environment and controller, navigates
    // to the app URL, and handles the JS-initiated ready handshake. Nothing here
    // knows about WinForms controls or Grasshopper — it only speaks WebView2.
    internal sealed class WebViewBridge : IDisposable
    {
        private const string AppUrl            = "http://localhost:5173";
        private const string UserDataFolder    = "gh-watch-webview";

        private CoreWebView2Controller _controller;
        private CoreWebView2 _webView;
        private bool _isReady;

        public bool IsReady => _isReady;

        // Fires once: when JS posts { type:"ready" } confirming its message listener is live.
        public event EventHandler Ready;

        public async Task InitAsync(IntPtr hwnd, Rectangle initialBounds)
        {
            try
            {
                var folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    UserDataFolder);

                var env = await CoreWebView2Environment.CreateAsync(null, folder);

                // CoreWebView2ControllerAsync takes our raw HWND — no WinForms wrapper needed.
                _controller = await env.CreateCoreWebView2ControllerAsync(hwnd);
                _controller.Bounds = initialBounds;

                _webView = _controller.CoreWebView2;
                _webView.WebMessageReceived += OnWebMessageReceived;
                _webView.Navigate(AppUrl);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"[WebViewBridge] init failed: {ex}");
#endif
            }
        }

        public void UpdateBounds(Rectangle bounds)
        {
            if (_controller != null)
                _controller.Bounds = bounds;
        }

        // Caller is responsible for ensuring this runs on the UI thread.
        public void Send(string json) => _webView?.PostWebMessageAsJson(json);

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (!e.WebMessageAsJson.Contains("\"ready\"")) return;

            _webView.WebMessageReceived -= OnWebMessageReceived;
            _isReady = true;
            Ready?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _controller?.Close();
            _controller = null;
        }
    }
}
