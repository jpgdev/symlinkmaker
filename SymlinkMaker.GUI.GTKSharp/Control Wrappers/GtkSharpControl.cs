using Gtk;
using System;

namespace SymlinkMaker.GUI.GTKSharp
{
    public abstract class GtkSharpControl  : IControl
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
    }
}

