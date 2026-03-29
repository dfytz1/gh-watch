using System.Drawing;
using System.Windows.Forms;

namespace gh
{
    /// <summary>
    /// WinForms UserControl overlaid on the GH canvas as the component's viewer area.
    /// WebView2 will be added here in the next phase.
    /// </summary>
    public class WatchPanel : UserControl
    {
        public WatchPanel()
        {
            BackColor      = Color.White;
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Placeholder text — replaced by WebView2 later
            const string text = "Three.js viewer";
            using (var font  = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (var brush = new SolidBrush(Color.FromArgb(180, 180, 180)))
            {
                SizeF sz = e.Graphics.MeasureString(text, font);
                e.Graphics.DrawString(
                    text, font, brush,
                    (Width  - sz.Width)  / 2f,
                    (Height - sz.Height) / 2f);
            }
        }
    }
}
