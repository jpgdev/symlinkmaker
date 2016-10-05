using System;
using System.Runtime.InteropServices;

namespace SymlinkMaker.Core
{
    public class WindowsFileOperations : BasicFileOperations
    {
        private enum SymbolicLinkType
        {
            File = 0,
            Directory = 1
        }

        [DllImport("kernel32.dll")]
        private static extern bool CreateSymbolicLink(string symlinkFileName, string targetFileName, SymbolicLinkType symLinkType);

        public override void CreateSymbolicLink(string sourceDirName, string targetDirName)
        {
            bool created = CreateSymbolicLink(sourceDirName, targetDirName, SymbolicLinkType.Directory);
            if (!created)
            {
                // TODO : Change the Exception
                throw new Exception("There was an error creating the symlink");
            }
        
        }
    }
}