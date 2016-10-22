using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public interface IDirectory
    {
        bool Create(string path);
        bool Move(string from, string to);
        bool Delete(string path);
        bool Exists(string path);

        IEnumerable<string> GetFiles(string path);
        IEnumerable<string> GetDirectories(string path);
    }
}

