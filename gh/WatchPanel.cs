using gh.Dtos;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gh
{
    public class WatchPanel : UserControl
    {
        private CoreWebView2Controller _controller;
        private CoreWebView2 _webView;

        public WatchPanel()
        {
            BackColor = Color.White;
            DoubleBuffered = true;
        }

        // OnHandleCreated fires once the native HWND exists, which is required
        // for CoreWebView2Environment.CreateCoreWebView2ControllerAsync.
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _ = InitWebViewAsync();
        }

        private async Task InitWebViewAsync()
        {
            try
            {
                var userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "gh-watch-webview");

                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);

                // CreateCoreWebView2ControllerAsync takes our WinForms HWND directly —
                // no WinForms WebView2 wrapper required, so no version conflict with
                // the Core DLL that Rhino already has loaded in the process.
                _controller = await env.CreateCoreWebView2ControllerAsync(Handle);
                _controller.Bounds = ClientRectangle;

                _webView = _controller.CoreWebView2;
                _webView.Navigate("http://localhost:5173");

                Resize += (s, args) =>
                {
                    if (_controller != null)
                        _controller.Bounds = ClientRectangle;
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 init failed: {ex}");
            }
        }

        public void SendGeometry(SendDataDto dto)
        {
            if (_webView == null) return;

            var json = JsonConvert.SerializeObject(dto);

            // PostWebMessageAsJson must run on the UI thread.
            if (InvokeRequired)
                BeginInvoke(() => _webView.PostWebMessageAsJson(json));
            else
                _webView.PostWebMessageAsJson(json);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _controller?.Close();
                _controller = null;
            }
            base.Dispose(disposing);
        }
    }
}
