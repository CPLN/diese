using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;


// TODO: refactor those controls as on "drawable layer" with the nice stuff from
//       game design. E.g. Tick.
namespace diese
{
    public class Canvas : Control
    {
        private readonly List<Diese> actors;

        public Canvas()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;

            SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);

            actors = new List<Diese>();
        }

        public void AddActor(Diese diese)
        {
            actors.Add(diese);
        }

        public void Tick()
        {
            foreach (var actor in actors)
            {
                actor.Tick();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            foreach (var actor in actors)
            {
                actor.Draw(e.Graphics);
            }
        }
    }

    public class Background : Control
    {
        public Image Image;

        public Background()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.WhiteSmoke;

            SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = e.ClipRectangle;
            e.Graphics.DrawLine(
                Pens.Blue,
                new PointF(rect.Left, rect.Top),
                new PointF(rect.Right, rect.Bottom));
            e.Graphics.DrawLine(
                Pens.Blue,
                new PointF(rect.Right, rect.Top),
                new PointF(rect.Left, rect.Bottom));

            e.Graphics.DrawImage(Image, 0, 0);

            using (var brush = new TextureBrush(Image, WrapMode.Tile))
            {
                e.Graphics.FillRectangle(brush, 0, 0, rect.Right, rect.Bottom);
            }
        }
    }
}
