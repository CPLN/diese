using System;
using System.Drawing;
using System.Collections.Generic;

namespace diese
{
    public class Diese
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Image Image { get; set; }

        public int Speed { get; set; }

        private readonly List<Move> moves;

        public Diese(int x, int y, Image image)
        {
            X = x;
            Y = y;
            Image = image;
            Speed = 5;
            moves = new List<Move>();
        }

        public void Move(int dx, int dy)
        {
            moves.Add(new Move(dx, dy));
        }

        public void Tick() {
            if (moves.Count == 0)
            {
                return;
            }

            var move = moves[0];
            var speed = Math.Min(Speed, move.Length);
            move.Shorten(Speed);

            X += move.X * speed;
            Y += move.Y * speed;
            // Remove useless events.
            if (move.Length <= 0)
            {
                moves.RemoveAt(0);
            }
        }

        public void Draw(Graphics gr) {
            gr.DrawImage(Image, (int) X, (int) Y);
        }
    }
}