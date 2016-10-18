namespace SymlinkMaker.GUI.GTKSharp
{
    public interface IGtkIconNameConverter
    {
        string GetImageNameFromGtkName(string gtkName);
        string GetGtkNameFromImageName(string name);
    }
}
