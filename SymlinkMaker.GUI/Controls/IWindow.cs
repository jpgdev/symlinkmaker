using System;

namespace SymlinkMaker.GUI
{
    public interface IWindow
    {
        event EventHandler Closed;

        void Show();

        void Close();
    }
}

