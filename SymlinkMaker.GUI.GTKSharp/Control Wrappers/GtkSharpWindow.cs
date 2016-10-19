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
            Destroyed += Window_DestroyedEvent;
        }

        public override void Dispose()
        {
            Destroyed -= Window_DestroyedEvent;
        }

        public void Window_DestroyedEvent(object o, EventArgs args)
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

