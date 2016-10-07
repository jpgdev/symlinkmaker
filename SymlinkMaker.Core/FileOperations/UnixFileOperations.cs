using Mono.Unix;

namespace SymlinkMaker.Core
{
    public class UnixFileOperations : BasicFileOperations
    {
        /// <summary>
        /// Creates a symbolic link from the source to the target.
        /// </summary>
        /// <param name="sourceDirName">Source dir name.</param>
        /// <param name="targetDirName">Target dir name.</param>
        public override void CreateSymbolicLink(string sourceDirName, string targetDirName)
        {
            var dir = new UnixDirectoryInfo(targetDirName);
            dir.CreateSymbolicLink(sourceDirName);
        }
    }
}
