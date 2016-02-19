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
        private readonly List<Actor> actors;
        private readonly List<Actor> futureActors;

        public Canvas()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;

            SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);

            actors = new List<Actor>();
            futureActors = new List<Actor>();
        }

        public void AddActor(Actor actor)
        {
            futureActors.Add(actor);
        }

        public void Tick()
        {
            foreach (var actor in actors)
            {
                actor.Tick();
            }
            foreach (var actor in futureActors)
            {
                actors.Add(actor);
            }
            actors.RemoveAll(a => a.X > 1000 || a.Y > 1000);
            futureActors.Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for(var i = actors.Count - 1; i >= 0; i--)
            {
                actors[i].Draw(e.Graphics);
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
