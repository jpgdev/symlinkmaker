using Gdk;
using Gtk;
using System;

namespace SymlinkMaker.GUI.GtkSharp
{
    public class FromWidgetIconLoader : IIconLoader<Pixbuf>
    {
        private readonly Widget _widget;
        private readonly IconSize _iconSize;

        public FromWidgetIconLoader(
            Widget widget,
            IconSize iconSize = IconSize.Menu)
        {
            if (widget == null)
                throw new ArgumentNullException(nameof(widget));

            _widget = widget;
            _iconSize = iconSize;
        }

        public Pixbuf Load(string name)
        {
            return Stetic.IconLoader.LoadIcon(_widget, name, _iconSize);
        }
    }
}

