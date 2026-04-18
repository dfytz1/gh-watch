using Gh.Watch.Dtos;
using Gh.Watch.Panel;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gh.Watch
{
    // Thin WinForms shell: its sole job is to provide a native HWND that
    // WebViewBridge can attach its CoreWebView2Controller to.
    // All WebView2 logic lives in WebViewBridge; all canvas-lifecycle logic
    // lives in PanelHost. This class just bridges the two worlds.
    public sealed class WatchPanel : UserControl
    {
        private readonly WebViewBridge _bridge = new WebViewBridge();

        public bool IsReady => _bridge.IsReady;

        public event EventHandler WebViewReady
        {
            add    => _bridge.Ready += value;
            remove => _bridge.Ready -= value;
        }

        public WatchPanel()
        {
            BackColor = Color.White;
            DoubleBuffered = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _ = _bridge.InitAsync(Handle, ClientRectangle);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _bridge.UpdateBounds(ClientRectangle);
        }

        public void SendGeometry(SendDataDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            // PostWebMessageAsJson must run on the UI thread.
            if (InvokeRequired)
                BeginInvoke(() => _bridge.Send(json));
            else
                _bridge.Send(json);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _bridge.Dispose();
            base.Dispose(disposing);
        }
    }
}
