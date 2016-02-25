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

        public readonly List<Bullet> Bullets;

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
            Bullets = new List<Bullet>();
        }

        public void AddActor(Actor actor)
        {
            if (actor is Bullet)
            {
                Bullets.Add((Bullet)actor);
            }
            futureActors.Add(actor);
        }

        public void RemoveActor(Actor actor)
        {
            actor.Dead = true;
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
            Bullets.ForEach(bullet =>
                {
                    if (bullet.X > 1000 || bullet.X < -1000 || bullet.Y > 1000 || bullet.Y < -1000)
                    {
                        bullet.Dead = true;
                    }
                });
            actors.RemoveAll(a => a.Dead);
            futureActors.Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = e.ClipRectangle;
            var state = e.Graphics.Save();

            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.TranslateTransform(rect.Right / 2f, rect.Bottom / 2f);


            for (var i = actors.Count - 1; i >= 0; i--)
            {
                actors[i].Draw(e.Graphics);
            }

            e.Graphics.Restore(state);
        }
    }

    public class Background : Control
    {
        public Image Image;
        private TextureBrush brush;

        public Background()
        {
            SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);

            Disposed += onDispose;
        }

        private void onDispose(object sender, EventArgs e)
        {
            brush.Dispose();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (brush == null)
            {
                brush = new TextureBrush(Image, WrapMode.Tile);
            }

            var rect = e.ClipRectangle;
            e.Graphics.FillRectangle(brush, 0, 0, rect.Right, rect.Bottom);

#if DEBUG
            e.Graphics.DrawLine(
                Pens.Blue,
                new PointF(rect.Left, rect.Top),
                new PointF(rect.Right, rect.Bottom));
            e.Graphics.DrawLine(
                Pens.Blue,
                new PointF(rect.Right, rect.Top),
                new PointF(rect.Left, rect.Bottom));
#endif
        }
    }
}
