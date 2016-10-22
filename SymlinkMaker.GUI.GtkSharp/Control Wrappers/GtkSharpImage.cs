using Gtk;
using System;
using Gdk;
using Image = Gtk.Image;

namespace SymlinkMaker.GUI.GtkSharp
{
    public class GtkSharpImage : GtkSharpControl, IImage
    {
        #region Fields

        private string _gtkName;
        private readonly IGtkIconNameConverter _iconNameConverter;
        private readonly IIconLoader<Pixbuf> _iconLoader;

        #endregion

        #region Properties

        protected new Image BaseWidget
        {
            get { return base.BaseWidget as Image; }
        }

        public string Name
        {
            get
            {
                return _iconNameConverter.GetImageNameFromGtkName(_gtkName);
            }
            set
            {
                var newGtkName = _iconNameConverter.GetGtkNameFromImageName(value);
                if (_gtkName == newGtkName)
                    return;
                
                SetImagePixbuf(newGtkName);
                _gtkName = newGtkName;
            }
        }

        #endregion

        public GtkSharpImage(
            Image image, 
            string name, 
            IGtkIconNameConverter iconNameConverter,
            IIconLoader<Pixbuf> iconLoader
        )
            : base(image)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (iconNameConverter == null)
                throw new ArgumentNullException(nameof(iconNameConverter));

            if (iconLoader == null)
                throw new ArgumentNullException(nameof(iconLoader));
            
            _iconNameConverter = iconNameConverter;
            _iconLoader = iconLoader;

            _gtkName = _iconNameConverter.GetGtkNameFromImageName(name);
            SetImagePixbuf(_gtkName);
        }

        private void SetImagePixbuf(string gtkName)
        {
            BaseWidget.Pixbuf = _iconLoader.Load(gtkName);
        }
    }
}
