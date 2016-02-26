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
        protected readonly List<Actor> actors;
        protected readonly List<Actor> futureActors;

        public Canvas()
        {
            SetStyle(
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            BackColor = Color.Transparent;

            actors = new List<Actor>();
            futureActors = new List<Actor>();
        }

        public void AddActor(Actor actor)
        {
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
            futureActors.Clear();

            actors.RemoveAll(a => a.Dead);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = e.ClipRectangle;
            var state = e.Graphics.Save();

            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            // Centre l'affichage.
            e.Graphics.TranslateTransform(rect.Right / 2f, rect.Bottom / 2f);

            foreach (var actor in actors)
            {
                actor.Draw(e.Graphics);
            }

            e.Graphics.Restore(state);
        }
    }

    public class Background : Canvas
    {
        public Image Image;
        private TextureBrush brush;

        public Background()
            : base()
        {
            Disposed += onDispose;
        }

        private void onDispose(object sender, EventArgs e)
        {
            // Free the memory of the brush before leaving.
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