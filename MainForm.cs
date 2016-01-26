using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
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
            background.Cursor = Cursors.Default;
            background.Location = new Point(0, 0);
            background.Size = new Size(640, 480);
            background.Anchor = (AnchorStyles)(
                AnchorStyles.Top |
                AnchorStyles.Right |
                AnchorStyles.Bottom |
                AnchorStyles.Left
            );

            canvas.Cursor = Cursors.Default;
            canvas.Location = new Point(0, 0);
            canvas.Size = new Size(640, 480);
            canvas.Anchor = (AnchorStyles)(
                AnchorStyles.Top |
                AnchorStyles.Right |
                AnchorStyles.Bottom |
                AnchorStyles.Left
            );

            // Les poupées russes.
            background.Controls.Add(canvas);
            Controls.Add(background);
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