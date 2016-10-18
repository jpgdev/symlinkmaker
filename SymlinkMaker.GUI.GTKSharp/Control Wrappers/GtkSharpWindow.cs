using Gtk;
using System;

namespace SymlinkMaker.GUI.GTKSharp
{
    public class GtkSharpWindow : Window, IWindow
    {
        public event EventHandler Closed;

        public GtkSharpWindow(WindowType type)
            :base(type)
        {
            Destroyed += MainWindowView_DestroyEvent;
        }

        ~GtkSharpWindow()
        {
            Destroyed -= MainWindowView_DestroyEvent;
        }

        public void MainWindowView_DestroyEvent(object o, EventArgs args)
        {
            if (Closed != null)
                Closed(this, args);
        }

        public void Close()
        {
            Destroy();
        }
    }
}

