using System;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

namespace gh
{
    public class WatchAttributes : GH_ComponentAttributes
    {
        private const int ViewerWidth  = 400;
        private const int ViewerHeight = 300;

        // Set after base.Layout() — height of the standard GH header + param grips
        private float _headerHeight = 60f;

        private WatchPanel _panel;

        public WatchAttributes(ghComponent owner) : base(owner) { }

        protected override void Layout()
        {
            base.Layout();
            _headerHeight = Bounds.Height;
            float w = Math.Max(Bounds.Width, ViewerWidth);
            Bounds = new RectangleF(Bounds.X, Bounds.Y, w, _headerHeight + ViewerHeight);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                RectangleF vr = ViewerRect;

                // White fill over the extended capsule area
                using (var b = new SolidBrush(Color.White))
                    graphics.FillRectangle(b, vr);

                // Light gray border
                using (var p = new Pen(Color.FromArgb(180, 180, 180), 1f))
                    graphics.DrawRectangle(p, vr.X, vr.Y, vr.Width, vr.Height);

                SyncPanel(canvas);
            }
        }

        private RectangleF ViewerRect =>
            new RectangleF(Bounds.X, Bounds.Y + _headerHeight, Bounds.Width, ViewerHeight);

        private void SyncPanel(GH_Canvas canvas)
        {
            EnsurePanel(canvas);

            RectangleF vr = ViewerRect;
            PointF tl = canvas.Viewport.ProjectPoint(new PointF(vr.Left,  vr.Top));
            PointF br = canvas.Viewport.ProjectPoint(new PointF(vr.Right, vr.Bottom));

            _panel.Location = new Point((int)tl.X, (int)tl.Y);
            _panel.Size     = new Size(
                Math.Max(1, (int)(br.X - tl.X)),
                Math.Max(1, (int)(br.Y - tl.Y)));
            _panel.Visible  = true;
        }

        private void EnsurePanel(GH_Canvas canvas)
        {
            if (_panel != null && !_panel.IsDisposed) return;

            _panel = new WatchPanel();
            canvas.Controls.Add(_panel);
            _panel.BringToFront();
            canvas.DocumentChanged += OnDocumentChanged;
        }

        private void OnDocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            sender.DocumentChanged -= OnDocumentChanged;
            DestroyPanel(sender);
        }

        /// <summary>Called by the component when it is removed from the document.</summary>
        public void DestroyPanel()
        {
            DestroyPanel(Instances.ActiveCanvas);
        }

        private void DestroyPanel(GH_Canvas canvas)
        {
            if (_panel == null || _panel.IsDisposed) return;
            canvas?.Controls.Remove(_panel);
            _panel.Dispose();
            _panel = null;
        }
    }
}
