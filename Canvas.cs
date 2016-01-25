using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace diese
{
    public class Canvas : Control
    {
        private readonly List<Diese> actors;

        public Canvas()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;

            actors = new List<Diese>();

            Paint += onPaint;
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

        protected void onPaint(object sender, PaintEventArgs e)
        {
            foreach (var actor in actors)
            {
                actor.Draw(e.Graphics);
            }
        }
    }

    public class Background : Control
    {
        public Background()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Paint += onPaint;
        }

        protected void onPaint(object sender, PaintEventArgs e)
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
        }
    }
}