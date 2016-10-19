using Gtk;
using System;
using Gdk;
using Image = Gtk.Image;
using System.Diagnostics;

namespace SymlinkMaker.GUI.GTKSharp
{
    public class GtkSharpImage : GtkSharpControl, IImage
    {
        #region Fields

        private readonly string _gtkName;
        private readonly IGtkIconNameConverter _iconNameConverter;

        #endregion

        #region Properties

        protected Image BaseWidget
        {
            get { return base.BaseWidget as Image; }
        }

        public string Path
        {
            get
            {
                return _iconNameConverter.GetImageNameFromGtkName(_gtkName);
            }
            set
            {
                var newPath = _iconNameConverter.GetGtkNameFromImageName(value);
                if (_gtkName == newPath)
                    return;
                
                SetImagePixbuf(newPath);
            }
        }

        #endregion

        public GtkSharpImage(
            Image image, 
            string name, 
            IGtkIconNameConverter iconNameConverter)
            : base(image)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (iconNameConverter == null)
                throw new ArgumentNullException(nameof(iconNameConverter));

            _iconNameConverter = iconNameConverter;
            _gtkName = _iconNameConverter.GetGtkNameFromImageName(name);

            SetImagePixbuf(_gtkName);
        }

        private void SetImagePixbuf(string gtkName)
        {
            BaseWidget.Pixbuf = GetIconPixbuf(gtkName);
        }

        private Pixbuf GetIconPixbuf(string name, IconSize size = IconSize.Menu)
        {
            return Stetic.IconLoader.LoadIcon(BaseWidget, name, size);
        }
    }
}

