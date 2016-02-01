using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
#if WINDOWS 
using ScintillaNET;
#endif
//using ExpressionEvaluator;


namespace diese
{
    public class MainForm : Form
    {
        public const string VERSION = "0.0.1";

        private Diese diese;
        private Background background;
        private Canvas canvas;
        private long frame;

        public MainForm(Diese diese)
        {
            this.diese = diese;

            background = new Background();

            canvas = new Canvas();
            canvas.AddActor(diese);

            Name = "Dièse";
            Text = "Apprendre avec Dièse";

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();
            InitializeEvents();
        }

        private void InitializeComponent()
        {
            var zero = new Point(0, 0);
            var size = new Size(640, 480);

            var splitter = new SplitContainer();
            splitter.Location = zero;
            splitter.Size = size;
            splitter.Anchor = (AnchorStyles)(
                AnchorStyles.Top |
                AnchorStyles.Right |
                AnchorStyles.Bottom |
                AnchorStyles.Left
            );
#if WINDOWS 
            var scintilla = new Scintilla();
            scintilla.Margins[0].Width = 0;

            // TODO.
#else
            var scintilla = new TextBox();
            scintilla.Multiline = true;
            scintilla.ScrollBars = ScrollBars.Both;
            scintilla.Font = new Font("Mono", 15, FontStyle.Regular);
#endif

            scintilla.Location = zero;
            scintilla.Size = size;
            scintilla.Anchor = (AnchorStyles)(
                AnchorStyles.Top |
                AnchorStyles.Right |
                AnchorStyles.Bottom |
                AnchorStyles.Left
            );


            background.Cursor = Cursors.Default;
            background.Location = zero;
            background.Size = size;
            background.Anchor = (AnchorStyles)(
                AnchorStyles.Top |
                AnchorStyles.Right |
                AnchorStyles.Bottom |
                AnchorStyles.Left
            );

            canvas.Cursor = Cursors.Default;
            canvas.Location = zero;
            canvas.Size = size;
            canvas.Anchor = (AnchorStyles)(
                AnchorStyles.Top |
                AnchorStyles.Right |
                AnchorStyles.Bottom |
                AnchorStyles.Left
            );

            // Les poupées russes.
            background.Controls.Add(canvas);
            splitter.Panel1.Controls.Add(scintilla);
            splitter.Panel2.Controls.Add(background);
            Controls.Add(splitter);
        }

        private void InitializeEvents()
        {
            background.KeyUp += onKeyUp;

            var tick = new Timer();
            tick.Interval = 1000 / 60; // 60 fps.
            tick.Tick += onTick;

            frame = 0;
            tick.Start();
        }

        private void onKeyUp(object sender, KeyEventArgs e) {
            var pas = 100;
   
            switch (e.KeyCode)
            {
                case Keys.Left:
                    diese.Move(-pas, 0);
                    break;
                case Keys.Right:
                    diese.Move(pas, 0);
                    break;
                case Keys.Down:
                    diese.Move(0, pas);
                    break;
                case Keys.Up:
                    diese.Move(0, -pas);
                    break;
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
            // L'utilisation de ressources sous GNU/Linux est compliqué...
            /*var resourceManager = new System.Resources.ResourceManager(typeof(MainForm));
            var image = (Image) (resourceManager.GetObject("Avatar"));*/
            var image = Image.FromFile(
                "Resources" +
                Path.DirectorySeparatorChar +
                "diese.png");

            var diese = new Diese(0, 0, image);
            var form = new MainForm(diese);

            Application.Run(form);
        }
    }
}