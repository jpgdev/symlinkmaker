namespace SymlinkMaker.Core
{
    public interface IAppSettingsLoader
    {
        AppSettings Load(string path);
    }
}