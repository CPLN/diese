using Gtk;
using Cairo;
using System;
using ExpressionEvaluator;
using Mono.TextEditor;


namespace diese
{
	class MainClass
	{
		public const string VERSION = "0.0.1";
		public static void Main (string[] args)
		{
			Application.Init ();
			var win = new Window (string.Format("Dièse v{0}", VERSION));

			win.Resize (1024, 768);
			var menubar = new HButtonBox ();
			var run = new Button ();
			run.Label = "Exécuter";
			run.Clicked += new EventHandler (on_run);
			menubar.PackStart (run, false, false, 0);
	
			var exit = new Button ();
			exit.Label = "Quitter";
			exit.Clicked += new EventHandler (on_exit);
			menubar.PackEnd (exit, false, false, 0);

			var vbox = new VBox ();
			vbox.PackStart (menubar, false, true, 0);

			var hpaned = new HPaned ();

			var scroll0 = new ScrolledWindow ();
			var editor = new TextEditor ();
			var options = new TextEditorOptions ();
			options.EnableSyntaxHighlighting = true;
			options.ShowFoldMargin = true;
			options.ShowWhitespaces = ShowWhitespaces.Selection;
			options.DrawIndentationMarkers = true;
			options.EnableAnimations = true;
			options.ColorScheme = "Visual Studio";
			editor.Text = "avance(10);\ntourne(90);\navance(10);";
			editor.Document.MimeType = "text/x-csharp";
			scroll0.Add (editor);
			hpaned.Pack1 (scroll0, true, true);

			var scroll1 = new ScrolledWindow ();
			var drawing = new DrawingArea ();
			drawing.SetSizeRequest(500, 500);
			drawing.ExposeEvent += new ExposeEventHandler (on_expose);
			scroll1.AddWithViewport (drawing);
			hpaned.Pack2 (scroll1, true, true);

			vbox.Add (hpaned);

			win.Add (vbox);
			win.ShowAll ();

			Application.Run ();
		}

		public static void on_run(object o, EventArgs e) {
			// TODO
			/*
			// https://csharpeval.codeplex.com/
			var t = new ExpressionEvaluator.TypeRegistry();
			var john = new Diese (0, 0);
			t.RegisterSymbol ("john", john);

			var expression = new CompiledExpression {
				StringToParse = "john.Move();john.Move();john.Move();true;",
				TypeRegistry = t
			};
			expression.ExpressionType = CompiledExpressionType.StatementList;

			var result = expression.Eval ();

			Console.WriteLine ("{0}, ({1},{2})", result, john.X, john.Y);
			*/
		}

		public static void on_exit(object o, EventArgs e) {
			Application.Quit ();
		}

		public static void on_expose(object o, ExposeEventArgs e) {
			DrawingArea area = (DrawingArea)o;

			var width = 0;
			var height = 0;
			var size = 40;
			area.GetSizeRequest(out width, out height);

			using(Cairo.Context g = Gdk.CairoHelper.Create(area.GdkWindow)) {
				g.SetSourceRGB (1, 1, 1);
				g.Rectangle (0, 0, width, height);
				g.Fill ();

				g.Translate (width / 2, height / 2);

				g.Save ();
				g.SetSourceColor(new Color (0, 0, 1));
				g.LineWidth = 2;
				g.LineCap = LineCap.Round;

				g.MoveTo (0, 0);
				g.LineTo (0, size);
				g.Stroke ();

				//g.MoveTo (0, 0);
				g.Arc (0, 0, size, 0, 2 * Math.PI);
				g.Stroke ();

				g.MoveTo (size, -size);
				g.SetSourceRGB (.2, .2, .2);
				g.ShowText ("Hello world!");

				g.Restore ();

				g.Stroke ();
				g.Restore ();

				//g.GetTarget ().Dispose ();
			//g.Dispose ();
			}
		}
	}

	class Diese {
		public int X { get; set; }
		public int Y { get; set; }

		public Diese(int x, int y) {
			X = x;
			Y = y;
		}

		public void Move() {
			this.X += 1;
			this.Y += 1;
		}
	}
}
