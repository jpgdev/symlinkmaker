namespace SymlinkMaker.Core
{
    public interface IFileSystemOperations
    {
        bool Move(string from, string to);
        bool Delete(string path);
        bool Copy(string from, string to);
        bool Exists(string path);
        bool CreateSymbolicLink(string from, string to);
    }
}
