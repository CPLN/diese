using System;
using System.Dynamic;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
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
        private IDictionary<string,Image> images;

        public MainForm(IDictionary<string,string[]> resources)
        {
            images = new Dictionary<string,Image>(resources.Count);
            fond = new Background();
            canvas = new Canvas();

            foreach (var item in resources)
            {
                Console.WriteLine(item.Key);
                images.Add(item.Key, loadImage(item.Key, item.Value));
            }
            fond.Image = images["fond"];

            joueur = new Diese (canvas, images) {
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
            var root = String.Join(Path.DirectorySeparatorChar.ToString(), new string[]{"..", "..", "Resources", ""});
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
            scintilla.Text = "// Bienvenue dans l'éditeur C#";

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

            var tick = new Timer();
            tick.Interval = 1000 / 60; // 60 fps.
            tick.Tick += onTick;

            frame = 0;
            tick.Start();
        }

        private void onKeyUp(object sender, KeyEventArgs e) {
            var pas = 100;
   
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
                        joueur.Shot(90);
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
            dynamic scope = new ExpandoObject();
            scope.Gauche = (Func<int,ExpandoObject>)(x =>
                {
                    joueur.Move(-x, 0);
                    return scope;
                });
            scope.Droite = (Func<int,ExpandoObject>) (x =>
                {
                    joueur.Move(x, 0);
                    return scope;
                });
            scope.Haut = (Func<int,ExpandoObject>)(x =>
                {
                    joueur.Move(0, -x);
                    return scope;
                });
            scope.Bas = (Func<int,ExpandoObject>) (x =>
                {
                    joueur.Move(0, x);
                    return scope;
                });
            scope.Tir = (Func<int,ExpandoObject>) (x => {
                    joueur.Shot(x);
                    return scope;
                });
            var expression = new CompiledExpression(code)
            {
                TypeRegistry = registry
            };
            expression.ExpressionType = CompiledExpressionType.StatementList;

            try {
                var f = expression.ScopeCompile<ExpandoObject>();
                f(scope);
            } catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException e) {
                MessageBox.Show(e.Message);
            } catch (ExpressionEvaluator.Parser.ExpressionParseException e) {
                MessageBox.Show(e.Message);
            }
        }

        private void onTick(object sender, EventArgs e)
        {
            canvas.Tick();
            canvas.Invalidate();

            frame += 1;
        }

        [STAThread]
        public static void Main(string[] args)
        {
            var images = new Dictionary<string,string[]>();
            // Poulpe verdâtre.
            images.Add("dièse", new string[]{"space-shooter", "ships", "9.png"});
            // Fond étoilé simple.
            images.Add("fond", new string[]{"space-shooter", "backgrounds", "1.png"});
            // Tir simple
            images.Add("tir", new string[]{"space-shooter", "shots", "1.png"});

            var form = new MainForm(images);

            Application.Run(form);
        }
    }
}