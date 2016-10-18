using System.Collections.Generic;
using System.Linq;

namespace SymlinkMaker.GUI.GTKSharp
{
    public class GtkIconNameConverter : IGtkIconNameConverter
    {
        private readonly Dictionary<string, string> _gtkNameFromImageName;

        public GtkIconNameConverter()
        {
            // TODO : Add an IconLoader?
            //        intead of hardcoding them in the contructor
            _gtkNameFromImageName = new Dictionary<string, string>()
            {
                { "yes", "gtk-yes" },
                { "no", "gtk-no" },
//                { "ok", "gtk-ok" }
            };
        }

        public string GetImageNameFromGtkName(string gtkName)
        {
            // FIXME : Might return the wrong value if there is multiple imageName 
            //         which are the same, since values are not unique like keys
            return _gtkNameFromImageName.First(x => x.Value == gtkName).Key;
        }

        public string GetGtkNameFromImageName(string name)
        {
            return _gtkNameFromImageName[name];
        }
    }
}

