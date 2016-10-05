using System;
using System.IO;

namespace SymlinkMaker.Core
{
	public class BasicFileOperations : IFileOperations
	{
		public virtual void CreateSymbolicLink(string sourceDirName, string targetDirName)
		{
			throw new NotImplementedException("CreateSymbolicLink");
		}

		public virtual void CopyDirectory(string sourceDirName, string targetDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo directory = new DirectoryInfo(sourceDirName);
			if (!directory.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: " + sourceDirName);
			}
	
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(targetDirName))
			{
				Directory.CreateDirectory(targetDirName);
			}
	
			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = directory.GetFiles();
			foreach (FileInfo file in files)
			{
				file.CopyTo(Path.Combine(targetDirName, file.Name), false);
			}
	
			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				DirectoryInfo[] subDirectories = directory.GetDirectories();
				foreach (DirectoryInfo subdir in subDirectories)
				{
					string tempPath = Path.Combine(targetDirName, subdir.Name);
					CopyDirectory(subdir.FullName, tempPath, copySubDirs);
				}
			}
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

