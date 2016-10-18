using System.IO;
using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    // TODO : Rename this?
    public class SystemDirectory : IDirectory
    {
        public bool Create(string path)
        {
            if (Exists(path)) 
                return false;

            Directory.CreateDirectory(path);

            return true;
        }

        public bool Move(string from, string to)
        {
//            if (!Exists(from))
//            {
//                throw new DirectoryNotFoundException(
//                    "Source directory does not exist or could not be found: " + from);
//            }
//
//            if (Exists(to))
//                return false;

            Directory.Move(from, to);

            return true;
        }

        public bool Delete(string path)
        {
            Directory.Delete(path, true);

            return true;
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }


        public IEnumerable<string> GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}

