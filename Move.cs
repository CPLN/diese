using System;

namespace diese
{
    public class Move {
        private double length;
        private double x;
        private double y;

        public double Length { get { return length; } }

        public double X { get { return x; } }

        public double Y { get { return y; } }

        public Move(int dx, int dy) {
            length = Math.Sqrt(dx * dx + dy * dy);
            x = dx / Length;
            y = dy / Length;
        }

        public void Shorten(int dl) {
            length = Math.Max(0, Length - dl);
        }
    }
}

