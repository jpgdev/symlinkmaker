using System;

namespace SymlinkMaker.GUI
{
    public class ToggleEventArgs : EventArgs
    {
        public bool IsActive { get; set; }

        public ToggleEventArgs(bool isActive)
        {
            IsActive = isActive;
        }
    }
}

