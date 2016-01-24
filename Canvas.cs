using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace diese
{
    public class Canvas : Control
    {
        private readonly List<Diese> actors;

        public Canvas()
        {
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
}