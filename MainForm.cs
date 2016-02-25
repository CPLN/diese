using System;
using System.Dynamic;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

#if !LINUX
using ScintillaNET;
#endif
using ExpressionEvaluator;


namespace diese
{
    public class MainForm : Form
    {
        public const string VERSION = "0.0.1";

        private Diese joueur;
        private Background fond;
        private Canvas canvas;
        private Control éditeur;
        private long frame;
        private IDictionary<string, Image> images;

        // System.Windows.Forms.Timer is not great for good fps.
        private System.Threading.Timer tick;
        private List<Asteroid> asteroids;
        private bool started;

        public MainForm(IDictionary<string, string[]> resources)
        {
            images = new Dictionary<string, Image>(resources.Count);
            asteroids = new List<Asteroid>();
            fond = new Background();
            canvas = new Canvas();

            foreach (var item in resources)
            {
                images.Add(item.Key, loadImage(item.Key, item.Value));
            }
            fond.Image = images["fond"];

            joueur = new Diese(canvas, images)
            {
                X = 0,
                Y = 0,
                Width = images["dièse"].Width,
                Height = images["dièse"].Height,
                Image = images["dièse"],
            };
            canvas.AddActor(joueur);

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            Name = "Dièse";
            Text = "Apprendre avec Dièse";

            InitializeComponent();
            InitializeEvents();
        }

        private Image loadImage(string name, string[] path)
        {
            var root = String.Join(Path.DirectorySeparatorChar.ToString(), new string[] { "..", "..", "Resources", "" });
            var p = String.Join(Path.DirectorySeparatorChar.ToString(), path);
            return Image.FromFile(root + p);
        }

        private void InitializeComponent()
        {
            var splitter = new SplitContainer();
            splitter.Dock = DockStyle.Fill;
            splitter.Parent = this;

            var fontFamily = "Consolas";
            var fontSize = 14;
#if !(LINUX)
            var scintilla = new Scintilla();
            scintilla.Margins[0].Width = 0;

            scintilla.Lexer = Lexer.Cpp;
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = fontFamily;
            scintilla.Styles[Style.Default].Size = fontSize;
            scintilla.StyleClearAll();

            scintilla.SetKeywords(0, "abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using var virtual while");
            scintilla.SetKeywords(1, "bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void");

            var Green = Color.FromArgb(0, 128, 0);
            var Gray = Color.FromArgb(128, 128, 128);
            var Red = Color.FromArgb(163, 21, 21);
            scintilla.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Cpp.Comment].ForeColor = Green;
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Green;
            scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Gray;
            scintilla.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.String].ForeColor = Red;
            scintilla.Styles[Style.Cpp.Character].ForeColor = Red;
            scintilla.Styles[Style.Cpp.Verbatim].ForeColor = Red;
            scintilla.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
#else
            var scintilla = new TextBox();
            scintilla.WordWrap = true;
            scintilla.Multiline = true;
            scintilla.ScrollBars = ScrollBars.Both;
            scintilla.Font = new Font(fontFamily, fontSize, FontStyle.Regular);
#endif
            scintilla.Dock = DockStyle.Fill;
            scintilla.Parent = splitter.Panel1;
            scintilla.Text = "// Bienvenue dans l'éditeur C#\n\n" +
                "var asteroids = (Asteroid[]) Radar();\n\n" +
                "foreach (var asteroid in asteroids) {\n" +
                "    var angle = Math.Atan(asteroid.Y / asteroid.X);\n" +
                "    var deg = angle * 180 / 3.1415923;\n\n" +
                "    if (asteroid.X < 0) {\n" +
                "        deg += 180;\n" +
                "    }\n\n" +
                "    Tir(deg);\n" +
                "}\n";

            fond.Cursor = Cursors.Default;
            fond.Dock = DockStyle.Fill;
            fond.Parent = splitter.Panel2;

            canvas.Cursor = Cursors.Default;
            canvas.Dock = DockStyle.Fill;

            // lien.
            éditeur = scintilla;

            // Les poupées russes.
            fond.Controls.Add(canvas);
            splitter.Panel1.Controls.Add(éditeur);
            splitter.Panel2.Controls.Add(fond);
            Controls.Add(splitter);
        }

        private void InitializeEvents()
        {
            éditeur.KeyUp += onKeyUp;
            fond.KeyUp += onKeyUp;
            FormClosing += onFormClosing;

            tick = new System.Threading.Timer(_ => onTick(), null, 0, 1000 / 60);
            frame = 0;
        }

        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            tick.Change(Timeout.Infinite, Timeout.Infinite);
            tick.Dispose();
            tick = null;
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            var pas = 100;
            var rnd = new Random();

            if (sender == fond)
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        joueur.Move(-pas, 0);
                        break;
                    case Keys.Right:
                        joueur.Move(pas, 0);
                        break;
                    case Keys.Down:
                        joueur.Move(0, pas);
                        break;
                    case Keys.Up:
                        joueur.Move(0, -pas);
                        break;
                    case Keys.Space:
                        joueur.Shot(rnd.NextDouble() * 360);
                        break;
                    case Keys.F5:
                        evaluate(éditeur.Text);
                        break;
                }
            if (sender == éditeur)
                switch (e.KeyCode)
                {
                    case Keys.F5:
                        evaluate(éditeur.Text);
                        break;
                }
        }

        private void evaluate(string code)
        {
            var registry = new TypeRegistry();
            registry.RegisterDefaultTypes();
            registry.RegisterType("Console", typeof(Console));
            registry.RegisterType("Asteroid", typeof(Asteroid));
            registry.RegisterType("Asteroid[]", typeof(Asteroid[]));
            dynamic scope = new ExpandoObject();
            scope.Gauche = (Func<int, ExpandoObject>)(x =>
            {
                joueur.Move(-x, 0);
                return scope;
            });
            scope.Droite = (Func<int, ExpandoObject>)(x =>
            {
                joueur.Move(x, 0);
                return scope;
            });
            scope.Haut = (Func<int, ExpandoObject>)(x =>
            {
                joueur.Move(0, -x);
                return scope;
            });
            scope.Bas = (Func<int, ExpandoObject>)(x =>
            {
                joueur.Move(0, x);
                return scope;
            });
            scope.Tir = (Func<double, ExpandoObject>)(x =>
            {
                joueur.Shot(x);
                return scope;
            });
            Func<Asteroid[]> radar = () =>
            {
                return asteroids.ToArray();
            };
            scope.Radar = radar;

            var expression = new CompiledExpression(code)
            {
                TypeRegistry = registry
            };
            expression.ExpressionType = CompiledExpressionType.StatementList;

            // Préparation du niveau en cours.
            var rnd = new Random();
            started = true;
            asteroids.ForEach(asteroid =>
                {
                    asteroid.Dead = true;
                });
            asteroids.RemoveAll(a => { return a.Dead; });

            for (var i = 0; i < rnd.Next(10, 30); i++)
            {
                var angle = rnd.NextDouble() * Math.PI * 2f;
                var distance = 300 + (10 * i);
                var asteroid = new Asteroid()
                {
                    X = -distance * Math.Cos(angle),
                    Y = -distance * Math.Sin(angle),
                    Angle = angle,
                    Width = images["météorite"].Width,
                    Height = images["météorite"].Height,
                    Image = images["météorite"]
                };
                canvas.AddActor(asteroid);
                asteroids.Add(asteroid);
            }

            try
            {
                var f = expression.ScopeCompile<ExpandoObject>();
                f(scope);
            }
            catch (NullReferenceException)
            {
                // Do nothing, no code where entered.
                //MessageBox.Show("Vous n'avez pas saisi de code à exécuter.");
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (ExpressionEvaluator.Parser.ExpressionParseException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void onUpdate()
        {
            canvas.Tick();
            canvas.Invalidate();
            canvas.Update();
            frame += 1;

            if (started && asteroids.Count > 0)
            {
                foreach (var asteroid in asteroids)
                {
                    if (asteroid.CollisionWith(joueur))
                    {
                        asteroids.ForEach(a => { a.Dead = true; });
                        started = false;
                        MessageBox.Show("Vous venez d'exploser dans l'hyper-espace...");
                        break;
                    }
                }

                foreach (var bullet in canvas.Bullets)
                {
                    foreach (var asteroid in asteroids)
                    {
                        if (!bullet.Dead && asteroid.CollisionWith(bullet))
                        {
                            bullet.Dead = true;
                            asteroid.Dead = true;
                        }
                    }
                }

                asteroids.RemoveAll(asteroid => { return asteroid.Dead; });

                if (started && asteroids.Count == 0)
                {
                    started = false;
                    MessageBox.Show("Vous avez gagné! Bravo.");
                }
            }
        }

        private void onTick()
        {
            try
            {
                Invoke(new MethodInvoker(onUpdate), null);
            }
            catch (ObjectDisposedException)
            {
                // do nothing.
            }

        }

        [STAThread]
        public static void Main(string[] args)
        {
            var images = new Dictionary<string, string[]>();
            // Poulpe verdâtre.
            images.Add("dièse", new string[] { "space-shooter", "ships", "9.png" });
            // Fond étoilé simple.
            images.Add("fond", new string[] { "space-shooter", "backgrounds", "1.png" });
            // Tir simple
            images.Add("tir", new string[] { "space-shooter", "shots", "7.png" });
            // Grosse météorite
            images.Add("météorite", new string[] { "space-shooter", "backgrounds", "meteor-1.png" });

            Application.Run(new MainForm(images));
        }
    }
}