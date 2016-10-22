using System;

namespace SymlinkMaker.Core
{
    public sealed class FileSystemOperationsLocator : FirstMatchLocator<PlatformID, IFileSystemOperations>
    {
        public FileSystemOperationsLocator(IDirectory directoryManager,
                                          IFile fileManager)
        {
            Initialize(directoryManager, fileManager);
        }

        private void Initialize(IDirectory directoryManager, IFile fileManager)
        {
            Register(
                platform => platform == PlatformID.Unix,
                new UnixDirectoryOperations(directoryManager, fileManager));

            Register(
                platform => (
                    platform == PlatformID.Win32NT ||
                    platform == PlatformID.Win32S ||
                    platform == PlatformID.Win32Windows ||
                    platform == PlatformID.WinCE), 
                new WindowsDirectoryOperations(directoryManager, fileManager)
            );

            Register(
                platform =>
                {
                    throw new PlatformNotSupportedException(
                        string.Format(
                            "{0} is not a supported platform.", 
                            platform
                        )
                    ); 
                }, 
                null
            );
        }

    }
}
