using System;
using System.Linq;
using System.IO;

namespace SymlinkMaker.Core
{
    public class BasicFileOperations : IFileOperations
    {
//
//        private static BasicFileOperations _instance;
//        private static readonly object instanceLock = new object();
//
//        public static BasicFileOperations Instance
//        {
//            get
//            {
//                lock (instanceLock)
//                {
//                    if (_instance == null)
//                        _instance = new BasicFileOperations();
//                
//                    return _instance;
//                }
//            }
//        }
//
//        private BasicFileOperations() 
//        {
//            
//        }
        
        public virtual void CreateSymbolicLink(string sourceDirName, string targetDirName)
        {
            throw new NotImplementedException("CreateSymbolicLink");
        }

        // Source : https://msdn.microsoft.com/en-us/library/bb762914.aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-1
        public virtual void CopyDirectory(string sourceDirName, string targetDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            var directory = new DirectoryInfo(sourceDirName);
            if (!directory.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: " + sourceDirName);
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(targetDirName))
                Directory.CreateDirectory(targetDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
                file.CopyTo(Path.Combine(targetDirName, file.Name), false);

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                DirectoryInfo[] subDirectories = directory.GetDirectories();
                foreach (DirectoryInfo subdir in subDirectories)
                {
                    string newPath = Path.Combine(targetDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, newPath, copySubDirs);
                }
            }
        }

        public virtual void MoveDirectory(string sourceDirName, string targetDirName)
        {
            // Get the subdirectories for the specified directory.
            var directory = new DirectoryInfo(sourceDirName);
            if (!directory.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: " + sourceDirName);
            }

            directory.MoveTo(targetDirName);
        }


        public virtual void DeleteDirectory(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        public virtual bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
