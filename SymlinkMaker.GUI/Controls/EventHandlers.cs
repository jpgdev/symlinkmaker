using System;

namespace SymlinkMaker.GUI
{
    public delegate void ButtonToggleEventHandler(IToggle button,ToggleEventArgs eventArgs);
    public delegate void ButtonClickEventHandler(IButton button,ButtonEventArgs eventArgs);
    public delegate void TextChangedEventHandler(ITextSource sender,EventArgs eventArgs);
}
