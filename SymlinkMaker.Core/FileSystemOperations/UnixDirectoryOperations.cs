using Mono.Unix;

namespace SymlinkMaker.Core
{
    public class UnixDirectoryOperations : DirectoryOperations
    {
        public UnixDirectoryOperations(IDirectory directoryManager, IFile fileManager)
            :base(directoryManager, fileManager)
        { }

        public override bool CreateSymbolicLink(string from, string to)
        {
            var dir = new UnixDirectoryInfo(to);
            dir.CreateSymbolicLink(from);

            return true;
        }
    }
}
