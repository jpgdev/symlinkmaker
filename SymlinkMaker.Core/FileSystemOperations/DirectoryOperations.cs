using System;
using System.IO;

namespace SymlinkMaker.Core
{
    public abstract class DirectoryOperations : IFileSystemOperations
    {
        private readonly IDirectory _directoryManager;
        private readonly IFile _fileManager;

        protected DirectoryOperations(IDirectory directoryManager,
                                      IFile fileManager)
        {
            if (directoryManager == null)
                throw new ArgumentNullException(nameof(directoryManager));

            _directoryManager = directoryManager;
            _fileManager = fileManager;
        }

        public abstract bool CreateSymbolicLink(string from, string to);

        public virtual bool Copy(string sourceDirName, string targetDirName)
        {
            var isCopied = true;

            // FIXME : Is it a good idea? 
            if (_fileManager == null)
                throw new NullReferenceException("An IFile instance is required to Copy a Directory.");

            if (!_directoryManager.Exists(sourceDirName))
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: " + sourceDirName);
            }

            bool created = _directoryManager.Create(targetDirName);
            if (!created)
                return false;
            
            // Get the files in the directory and copy them to the new location.
            var files = _directoryManager.GetFiles(sourceDirName);
            if (files != null)
            {
                foreach (var fileName in files)
                {
                    var oldPath = Path.Combine(sourceDirName, fileName);
                    var newPath = Path.Combine(targetDirName, fileName);
                    isCopied = isCopied && _fileManager.Copy(oldPath, newPath);
                }
            }

            // Copy the subdirectories and their contents to new location.
            var subDirectories = _directoryManager.GetDirectories(sourceDirName);
            if (subDirectories != null)
            {
                foreach (var subDirName in subDirectories)
                {
                    var oldPath = Path.Combine(sourceDirName, subDirName);
                    var newPath = Path.Combine(targetDirName, subDirName);

                    isCopied = isCopied && Copy(oldPath, newPath);
                }
            }

            return isCopied;
        }

        public virtual bool Move(string sourceDirName, string targetDirName)
        {
            return _directoryManager.Move(sourceDirName, targetDirName);
        }

        public virtual bool Delete(string path)
        {
            return _directoryManager.Delete(path);
        }

        public virtual bool Exists(string path)
        {
            return _directoryManager.Exists(path);
        }
    }
}
