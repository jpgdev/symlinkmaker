using System;

namespace SymlinkMaker.Core
{
    public class DefaultAppSettingsLoader : IAppSettingsLoader
    {
        private readonly  ILocator<PlatformID, IFileSystemOperations> _fileSystemOperationsLocator;

        public DefaultAppSettingsLoader(ILocator<PlatformID, IFileSystemOperations> fileOperationsLocator)
        {
            if (fileOperationsLocator == null)
                throw new ArgumentNullException(nameof(fileOperationsLocator));

            _fileSystemOperationsLocator = fileOperationsLocator;
        }

        public AppSettings Load(string path)
        {
            string HOME_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var fileOperations = _fileSystemOperationsLocator.Get(Environment.OSVersion.Platform);

            return new AppSettings()
            {
                RequiresConfirmation = true,
                SourcePath = HOME_FOLDER + "/Sandbox/TestSymlink/TestFolder",
                TargetPath = HOME_FOLDER + "/Sandbox/TestSymlink/TestFolder_COPY",
                FileOperations = fileOperations 
            };
        }
    }
}

