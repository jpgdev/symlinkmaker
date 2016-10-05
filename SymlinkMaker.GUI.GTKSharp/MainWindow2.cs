using System;
using Gtk;

public partial class MainWindow2: Gtk.Window
{
	public MainWindow2 () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
