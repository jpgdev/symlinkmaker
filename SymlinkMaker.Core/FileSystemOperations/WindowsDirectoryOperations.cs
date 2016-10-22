using System.Runtime.InteropServices;

namespace SymlinkMaker.Core
{
    public class WindowsDirectoryOperations : DirectoryOperations
    {
        private enum SymbolicLinkType
        {
            File = 0,
            Directory = 1
        }

        public WindowsDirectoryOperations(IDirectory directoryManager, IFile fileManager)
            :base(directoryManager, fileManager)
        {}

        [DllImport("kernel32.dll")]
        private static extern bool CreateSymbolicLink(string source, string target, SymbolicLinkType symLinkType);

        public override bool CreateSymbolicLink(string from, string to)
        {
            return CreateSymbolicLink(from, to, SymbolicLinkType.Directory);
        }
    }
}