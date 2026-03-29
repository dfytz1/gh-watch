using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;

namespace gh
{
    // WatchAttributes controls how the Watch component looks and behaves on the
    // Grasshopper canvas. Every GH component has an "Attributes" object; by
    // subclassing GH_ResizableAttributes we get free drag-to-resize corner handles
    // while still being able to paint the component however we like.
    public class WatchAttributes : GH_ResizableAttributes<ghComponent>
    {
        // Default pixel dimensions of the component when first placed on canvas.
        // HeaderHeight is the thin strip at the top that shows the name and grip.
        private const float DefaultWidth = 400f;
        private const float DefaultHeight = 324f; // 300 body + 24 header
        private const float HeaderHeight = 24f;

        // How far the white body fill is inset from the capsule border so the
        // rounded border remains visible around the WebView panel.
        private const float BorderInset = 5f;

        // The WinForms UserControl that holds the WebView2 browser.
        // It is a real Win32 window parented to the GH canvas, so it can render
        // hardware-accelerated web content that GDI+ cannot draw itself.
        private WatchPanel _panel;

        public WatchAttributes(ghComponent owner) : base(owner)
        {
            // Set an initial Bounds so the component has a sensible size before
            // GH calls Layout() for the first time.
            Bounds = new RectangleF(0, 0, DefaultWidth, DefaultHeight);
        }

        // ── GH_ResizableAttributes required overrides ─────────────────────────
        // These three properties tell the resize logic how small/large the user
        // can drag the component and how wide the draggable edge zones are.
        protected override Size MinimumSize => new Size(120, 80);
        protected override Size MaximumSize => new Size(4000, 4000);
        protected override Padding SizingBorders => new Padding(10);

        // ── Layout ────────────────────────────────────────────────────────────
        // GH calls Layout() whenever the component needs to reposition itself
        // (on load, after a move, after a resize, etc.). We must:
        //   1. Pin Bounds to the current Pivot (the anchor point GH manages).
        //   2. Tell the input param exactly where its wire grip lives so GH can
        //      draw the connecting wire to the right screen position.
        protected override void Layout()
        {
            SizeF size = Bounds.Size;
            if (size.IsEmpty) size = new SizeF(DefaultWidth, DefaultHeight);

            // Keep the component's top-left corner at the Pivot that GH controls.
            Bounds = new RectangleF(Pivot.X, Pivot.Y, size.Width, size.Height);

            // Position the single input param's grip at the vertical centre of
            // the header strip, flush with the left edge of the capsule.
            // Pivot  = the exact point where connecting wires terminate.
            // Bounds = the clickable rectangle that GH hit-tests for wire drops.
            if (Owner.Params.Input.Count > 0)
            {
                var p = Owner.Params.Input[0];
                float y = Bounds.Y + HeaderHeight * 0.5f;
                p.Attributes.Pivot = new PointF(Bounds.Left, y);
                p.Attributes.Bounds = new RectangleF(Bounds.Left - 20f, y - 5f, 20f, 10f);
            }
        }

        // ── Wire connectivity ─────────────────────────────────────────────────
        // GH builds a flat list of every IGH_Attributes object on the canvas
        // (the "attribute tree") and uses it for hit-testing, selection, and
        // wire routing. GH_ResizableAttributes only adds 'this' to the list by
        // default — it does NOT automatically walk the component's input/output
        // params the way GH_ComponentAttributes does. We must add them manually
        // so GH can find the input grip when the user drops a wire onto it.
        public override void AppendToAttributeTree(List<IGH_Attributes> attributes)
        {
            attributes.Add(this);
            foreach (var p in Owner.Params.Input)
                if (p.Attributes != null)
                    attributes.Add(p.Attributes);
        }

        // ── Rendering ─────────────────────────────────────────────────────────
        // GH calls Render() for each channel (background, objects, overlay, …).
        // We only draw during the Objects channel, which is the main content pass.
        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel != GH_CanvasChannel.Objects) return;

            // Step 1 — Draw the GH capsule shape (the rounded rectangle with the
            // standard colour palette, selection highlight, lock/hide indicators).
            // AddInputGrip registers the y-position of the grip nub so the capsule
            // renderer knows where to draw the circle on the left edge.
            GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Normal);
            capsule.AddInputGrip(Bounds.Y + HeaderHeight * 0.5f);

            // Render the capsule body FIRST, then draw the grip nubs ON TOP.
            // If you call RenderGrips before Render the body paints over the nubs
            // making the input grip invisible and users cannot connect wires.
            capsule.Render(graphics, Selected, Owner.Locked, Owner.Hidden);
            capsule.RenderEngine.RenderGrips(graphics);
            capsule.Dispose();

            // Step 2 — Paint the body area white so the WebView2 panel blends in.
            // Without this you would see the default grey capsule fill behind the
            // browser content whenever the panel is smaller than the body.
            RectangleF body = BodyRect;
            using (var b = new SolidBrush(Color.White))
                graphics.FillRectangle(b, body);

            // Step 3 — Draw the component's NickName centred in the header strip.
            RectangleF headerRect = new RectangleF(
                Bounds.X, Bounds.Y, Bounds.Width, HeaderHeight);
            using (var brush = new SolidBrush(Color.FromArgb(50, 50, 50)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                graphics.DrawString(Owner.NickName, SystemFonts.DefaultFont, brush, headerRect, sf);

            // Step 4 — Reposition the WinForms panel so it always lines up with
            // the body area in screen space (accounts for pan and zoom changes).
            SyncPanel(canvas);
        }

        // Returns the rectangle for the body area: everything below the header,
        // inset by BorderInset so the capsule's rounded border stays visible.
        private RectangleF BodyRect => new RectangleF(
            Bounds.X + BorderInset,
            Bounds.Y + HeaderHeight,
            Bounds.Width - BorderInset * 2f,
            Bounds.Height - HeaderHeight - BorderInset);

        // ── Size persistence ──────────────────────────────────────────────────
        // GH serialises component state when saving a .gh file. We save the
        // current width/height so the component reopens at the size the user
        // left it rather than reverting to the defaults.
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetSingle("BW", Bounds.Width);
            writer.SetSingle("BH", Bounds.Height);
            return base.Write(writer);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            bool ok = base.Read(reader);
            try
            {
                float w = reader.GetSingle("BW");
                float h = reader.GetSingle("BH");
                if (w > 0 && h > 0)
                    Bounds = new RectangleF(Pivot.X, Pivot.Y, w, h);
            }
            catch { /* first load — no saved size yet, use defaults */ }
            return ok;
        }

        // ── WinForms overlay ──────────────────────────────────────────────────
        // WebView2 is a Win32 HWND window, not a GDI+ drawing. It must live as a
        // real WinForms Control parented to the GH canvas Control. SyncPanel
        // converts the body rectangle from document (canvas) coordinates to screen
        // pixel coordinates and repositions/resizes the panel to match, so the
        // browser content stays aligned when the user pans or zooms the canvas.
        private void SyncPanel(GH_Canvas canvas)
        {
            EnsurePanel(canvas);

            RectangleF body = BodyRect;
            PointF tl = canvas.Viewport.ProjectPoint(new PointF(body.Left, body.Top));
            PointF br = canvas.Viewport.ProjectPoint(new PointF(body.Right, body.Bottom));

            _panel.Location = new Point((int)tl.X, (int)tl.Y);
            _panel.Size = new Size(
                Math.Max(1, (int)(br.X - tl.X)),
                Math.Max(1, (int)(br.Y - tl.Y)));
            _panel.Visible = true;
        }

        // Creates the WatchPanel (which hosts WebView2) the first time it is
        // needed and adds it to the canvas's WinForms control collection.
        // The DocumentChanged subscription ensures we clean up the panel when
        // the user switches to a different GH document — otherwise the browser
        // window would remain visible over the new document's canvas.
        private void EnsurePanel(GH_Canvas canvas)
        {
            if (_panel != null && !_panel.IsDisposed) return;

            _panel = new WatchPanel();
            canvas.Controls.Add(_panel);
            _panel.BringToFront();
            canvas.DocumentChanged += OnDocumentChanged;
        }

        // Triggered when the active GH document changes (e.g. the user opens
        // another file). Unsubscribes immediately to avoid firing again, then
        // destroys the panel so it doesn't appear over an unrelated document.
        private void OnDocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            sender.DocumentChanged -= OnDocumentChanged;
            DestroyPanel(sender);
        }

        // Public overload called by ghComponent.RemovedFromDocument so the panel
        // is cleaned up when the component is deleted from the canvas.
        public void DestroyPanel()
        {
            DestroyPanel(Instances.ActiveCanvas);
        }

        // Removes the WinForms panel from the canvas and disposes it.
        // Failing to do this would leave a visible browser window floating on
        // the canvas even after the component is gone.
        private void DestroyPanel(GH_Canvas canvas)
        {
            if (_panel == null || _panel.IsDisposed) return;
            canvas?.Controls.Remove(_panel);
            _panel.Dispose();
            _panel = null;
        }

        // Called by ghComponent.SolveInstance whenever new data arrives on the
        // input. Currently a stub — the TODO in ghComponent notes that this
        // should forward the geometry/data into the WebView2 so it can display it.
        public void UpdateWebView(IGH_Goo _)
        {
            if (_panel == null || _panel.IsDisposed) return;
        }
    }
}
