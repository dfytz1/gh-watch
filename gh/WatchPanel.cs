using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using gh.Dtos;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;

namespace gh
{
    public class WatchPanel : UserControl
    {
        private WebView2 _webView;

        public WatchPanel()
        {
            BackColor = Color.White;
            DoubleBuffered = true;

            _webView = new WebView2 { Dock = DockStyle.Fill };
            Controls.Add(_webView);

            InitAsync();
        }

        private async void InitAsync()
        {
            try
            {
                var userDatFolder = "gh-watch-webview";
                string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string path = Path.Combine(userDirectory, userDatFolder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path); // ensure it exists
                }

                var env = await CoreWebView2Environment.CreateAsync(
                    userDataFolder: path,
                    options: new CoreWebView2EnvironmentOptions()
                );
                await _webView.EnsureCoreWebView2Async(env);
                _webView.CoreWebView2.Navigate("http://localhost:5173");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string message)
        {
            _webView.Visible = false;
            Invalidate();
            using (var g = CreateGraphics())
            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(Color.FromArgb(180, 180, 180)))
            {
                SizeF sz = g.MeasureString(message, font);
                g.DrawString(message, font, brush,
                    (Width - sz.Width) / 2f,
                    (Height - sz.Height) / 2f);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _webView?.Dispose();
                _webView = null;
            }
            base.Dispose(disposing);
        }

        public void PostMessage(SendDataDto data)
        {
            if (_webView?.CoreWebView2 != null)
            {
                string json = JsonConvert.SerializeObject(data);
                _webView.CoreWebView2.PostWebMessageAsJson(json);
            }
        }
    }
}
