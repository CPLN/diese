using System;

namespace diese
{
    public abstract class Operation
    {
    }

    public class Move : Operation {
        public double Length;

        public double X;

        public double Y;

        public Move() {
        }

        public Move(int dx, int dy)
        {
            Length = Math.Sqrt(dx * dx + dy * dy);
            X = dx / Length;
            Y = dy / Length;
        }

        public void Shorten(int dl) {
            Length = Math.Max(0, Length - dl);
        }
    }

    public class Shot : Operation
    {
        public int X;
        public int Y;
        public double Angle;
        public double Length;
        public bool HasShot;

        public Shot()
        {
            HasShot = false;
            Length = 20;
        }

        public void Shorten(int dt)
        {
            Length = Math.Max(0, Length - dt);
        }
    }
}

