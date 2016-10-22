using System.IO;

namespace SymlinkMaker.Core
{
    public class SystemFileManager : IFile
    {
        public bool Copy(string from, string to)
        {
            File.Copy(from, to);

            return true;
        }
    }    
}
