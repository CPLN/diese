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

			win.Resize (640, 480);
			var menubar = new HButtonBox ();
			var exit = new Button ();
			exit.Label = "Exit";
			exit.Clicked += new EventHandler (on_exit);
			menubar.PackEnd (exit, false, false, 0);

			var vbox = new VBox ();
			vbox.PackStart (menubar, false, true, 0);

			var hbox = new HBox ();
			vbox.Add (hbox);

			var scroll0 = new ScrolledWindow ();
			var editor = new TextEditor ();
			scroll0.Add (editor);
			hbox.Add (scroll0);

			//var scroll1 = new ScrolledWindow ();
			var drawing = new DrawingArea ();
			drawing.ExposeEvent += new ExposeEventHandler (on_expose);
			//scroll1.Add (drawing);
			hbox.Add (drawing);

			win.Add (vbox);

			win.ShowAll ();

			Application.Run ();
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

			Cairo.Context g = Gdk.CairoHelper.Create(area.GdkWindow);

			g.MoveTo (new PointD (10, 10));
			g.LineTo (new PointD (50, 50));
			g.LineTo (new PointD (50, 10));
			g.LineTo (new PointD (10, 10));

			g.Color = new Color (1, 0, 0);
			g.Stroke ();

			g.GetTarget ().Dispose ();
			g.Dispose ();
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
