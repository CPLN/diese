using System;
using System.Drawing;
using System.Collections.Generic;

namespace diese
{
    public abstract class Actor
    {
        public double X;

        public double Y;

        public double Width;

        public double Height;

        public int Speed;

        public Image Image;

        public double Angle;

        public bool Dead = false;

        public double Degree
        {
            get { return Angle * 180 / Math.PI; }
            set { Angle = value * Math.PI / 180.0; }
        }

        public virtual void Draw(Graphics gr)
        {
            var state = gr.Save();
            gr.TranslateTransform((float)X, (float)Y);
            gr.RotateTransform((float)(Degree));
            gr.DrawImage(Image, (int)(-Width / 2.0), (int)(-Height / 2.0), (int)Width, (int)Height);
            gr.Restore(state);
        }

        public bool CollisionWith(Actor other)
        {
            // Dummy algorith using the distance method.
            var min = Math.Min(Width, Height);
            var minOther = Math.Min(other.Width, other.Height);
            var distance = Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2));

            return distance < (min + minOther) / 2f;
        }

        public abstract void Tick();
    }

    public class Diese : Actor
    {
        private readonly List<Operation> moves;
        private readonly Canvas parent;
        private readonly IDictionary<string,Image> images;

        public Diese(Canvas canvas, IDictionary<string,Image> assets)
            : base()
        {
            Speed = 5;
            Angle = 0;
            moves = new List<Operation>();
            parent = canvas;
            images = assets;
        }

        public void Move(int dx, int dy)
        {
            moves.Add(new Move(dx, dy));
        }

        public void Shot(int angle)
        {
            moves.Add(new Shot()
                {
                    X = (int)X, Y = (int)Y, Angle = angle
                });
        }

        public override void Tick()
        {
            if (moves.Count == 0)
            {
                return;
            }

            var move = moves[0];
            if (move is Move)
            {
                var m = (Move)move;
                var speed = Math.Min(Speed, m.Length);
                m.Shorten(Speed);

                X += m.X * speed;
                Y += m.Y * speed;
                // Remove useless events.
                if (m.Length <= 0)
                {
                    moves.RemoveAt(0);
                }
            }
            else if (move is Shot)
            {
                var b = (Shot)move;
                b.Shorten(Speed);

                if (!b.HasShot)
                {
                    var image = images["tir"];
                    parent.AddActor(new Bullet
                        {
                            X = X,
                            Y = Y,
                            Degree = b.Angle,
                            Image = image,
                            Width = image.Width,
                            Height = image.Height
                        });
                    b.HasShot = true;
                }

                if (b.Length <= 0)
                {
                    moves.RemoveAt(0);
                }
            }
        }
    }

    public class Bullet : Actor
    {
        public Bullet()
            : base()
        {
            Speed = 5;
        }

        public override void Tick()
        {
            X += Math.Cos(Angle) * Speed;
            Y += Math.Sin(Angle) * Speed;
        }

        public override void Draw(Graphics g)
        {
            Angle += Math.PI / 2f;
            base.Draw(g);
            Angle -= Math.PI / 2f;
        }
    }

    public class Asteroid : Actor
    {
        public double Rotation;
        public double AngularRotation;

        public Asteroid()
            : base()
        {
            Speed = 2;
            Rotation = 0;
            var rnd = new Random();
            AngularRotation = rnd.NextDouble() + .5;
        }

        public override void Tick()
        {
            Rotation = (Rotation + AngularRotation * Speed) % 360;

            X += Math.Cos(Angle) * Speed;
            Y += Math.Sin(Angle) * Speed;
        }

        public override void Draw(Graphics g)
        {
            Degree += Rotation;
            base.Draw(g);
            Degree -= Rotation;
        }
    }
}