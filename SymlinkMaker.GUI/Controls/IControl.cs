using System;

namespace SymlinkMaker.GUI
{
    public interface IControl : IDisposable
    {
        string Tooltip { get; set; }
    }
}

