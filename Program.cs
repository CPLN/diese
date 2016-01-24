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
        private Canvas canvas;
        
        public MainForm(Diese diese)
        {
            canvas = new Canvas();
            canvas.AddActor(diese);
            this.diese = diese;

            Name = "Dièse";
            Text = "Apprendre avec Dièse";

            InitializeComponent();
            InitializeEvents();
        }

        private void InitializeComponent()
        {
            canvas.Cursor = Cursors.Default;
            canvas.Location = new Point(0, 0);
            canvas.Size = new Size(640, 480);
            canvas.Anchor = (AnchorStyles)(
                AnchorStyles.Top |
                AnchorStyles.Right |
                AnchorStyles.Bottom |
                AnchorStyles.Left
            );
            canvas.TabIndex = 0;

            Controls.Add(canvas);
        }

        private void InitializeEvents()
        {
            KeyUp += onKeyUp;
            canvas.KeyUp += onKeyUp;

            var tick = new Timer();
            tick.Interval = 1000 / 60; // 60 fps.
            tick.Tick += onTick;

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

            canvas.Invalidate();
        }

        private void onTick(object sender, EventArgs e)
        {
            canvas.Tick();
            canvas.Invalidate();
        }

        [STAThread]
        public static void Main(string[] args)
        {
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