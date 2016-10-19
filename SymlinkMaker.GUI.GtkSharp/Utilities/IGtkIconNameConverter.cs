namespace SymlinkMaker.GUI.GtkSharp
{
    public interface IGtkIconNameConverter
    {
        string GetImageNameFromGtkName(string gtkName);
        string GetGtkNameFromImageName(string name);
    }
}
