using System;
using Gtk;

namespace SymlinkMaker.GUI.GtkSharp
{
    public class GtkSharpToggleAction : IToggle 
    {
        public event ButtonToggleEventHandler StatusChanged;

        #region Properties

        protected ToggleAction BaseAction { get; }

        public bool IsActive
        {
            get { return BaseAction.Active; }
            set { BaseAction.Active = value; }
        }

        public string Tooltip
        {
            get { return BaseAction.Tooltip; }
            set { BaseAction.Tooltip = value; }
        }

        #endregion

        public GtkSharpToggleAction(ToggleAction toggleAction)
        {
            if (toggleAction == null)
                throw new ArgumentNullException(nameof(toggleAction));

            BaseAction = toggleAction;

            BaseAction.Toggled += ToggleButton_Toggled;
        }

        public void Dispose()
        {
            BaseAction.Toggled -= ToggleButton_Toggled;
        }

        public void Toggle()
        {
            BaseAction.Toggle();
        }

        private void ToggleButton_Toggled(object sender, EventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged(this, new ToggleEventArgs(IsActive));
        }
    }
}

