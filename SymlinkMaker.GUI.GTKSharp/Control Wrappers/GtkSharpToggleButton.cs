using System;
using Gtk;

namespace SymlinkMaker.GUI.GTKSharp
{
    public class GtkSharpToggleButton : GtkSharpButton, IToggle
    {
        public event ButtonToggleEventHandler StatusChanged;

        protected ToggleButton BaseWidget
        {
            get { return base.BaseWidget as ToggleButton; }
        }

        public bool IsActive
        {
            get { return BaseWidget.Active; }
            set { BaseWidget.Active = value; }
        }

        public GtkSharpToggleButton(ToggleButton toggleButton)
            : base(toggleButton)
        {
            BaseWidget.Toggled += ToggleButton_Toggled;
        }

        public override void Dispose()
        {
            BaseWidget.Toggled -= ToggleButton_Toggled;
        }

        public void Toggle()
        {
            BaseWidget.Toggle();
        }

        private void ToggleButton_Toggled(object sender, EventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged(this, new ToggleEventArgs(IsActive));
        }
    }
}

