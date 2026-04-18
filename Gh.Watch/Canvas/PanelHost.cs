using Gh.Watch.Dtos;
using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Gh.Watch.Canvas
{
    // Manages the WatchPanel's lifetime on the Grasshopper canvas and owns the
    // geometry buffer so the last solved data can be replayed whenever a new panel
    // initialises (document open race, document switch and return).
    internal sealed class PanelHost
    {
        private WatchPanel _panel;
        private List<SendDataDto> _lastData;

        // Called every render frame from WatchAttributes.Render().
        // Creates the panel on the first call, then keeps it positioned and sized
        // to match the component's body rectangle in screen space.
        public void Sync(GH_Canvas canvas, RectangleF bodyRect)
        {
            EnsurePanel(canvas);

            PointF tl = canvas.Viewport.ProjectPoint(new PointF(bodyRect.Left, bodyRect.Top));
            PointF br = canvas.Viewport.ProjectPoint(new PointF(bodyRect.Right, bodyRect.Bottom));

            _panel.Location = new Point((int)tl.X, (int)tl.Y);
            _panel.Size = new Size(
                Math.Max(1, (int)(br.X - tl.X)),
                Math.Max(1, (int)(br.Y - tl.Y)));
            _panel.Visible = true;
        }

        // Stores the latest geometry batch and tries to push it immediately.
        // If the WebView isn't ready yet Flush() is a no-op; OnWebViewReady will
        // call it again once the JS signals it's listening.
        public void Store(List<SendDataDto> data)
        {
            _lastData = data;
            Flush();
        }

        public void Destroy(GH_Canvas canvas)
        {
            if (_panel == null || _panel.IsDisposed) return;
            canvas?.Controls.Remove(_panel);
            _panel.WebViewReady -= OnWebViewReady;
            _panel.Dispose();
            _panel = null;
        }

        private void EnsurePanel(GH_Canvas canvas)
        {
            if (_panel != null && !_panel.IsDisposed) return;

            _panel = new WatchPanel();
            _panel.WebViewReady += OnWebViewReady;
            canvas.Controls.Add(_panel);
            _panel.BringToFront();

            // Clean up when the user switches to a different GH document so the
            // browser window doesn't float over an unrelated canvas.
            canvas.DocumentChanged += OnDocumentChanged;
        }

        private void OnDocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            sender.DocumentChanged -= OnDocumentChanged;
            Destroy(sender);
        }

        private void OnWebViewReady(object sender, EventArgs e) => Flush();

        private void Flush()
        {
            if (_lastData == null || _panel == null || _panel.IsDisposed || !_panel.IsReady) return;
            foreach (var dto in _lastData)
                _panel.SendGeometry(dto);
        }
    }
}
