using Gtk;
using System;

namespace SymlinkMaker.GUI.GtkSharp
{
    public abstract class GtkSharpControl : IControl, IDisposable
    {
        protected Widget BaseWidget { get; private set; }

        public string Tooltip
        {
            get { return BaseWidget.TooltipText; }
            set { BaseWidget.TooltipText = value; }
        }

        protected GtkSharpControl(Widget widget)
        {
            if (widget == null)
                throw new ArgumentNullException(nameof(widget));

            BaseWidget = widget;
        }

        #region IDisposable implementation

        public virtual void Dispose() { }

        #endregion
    }
}

