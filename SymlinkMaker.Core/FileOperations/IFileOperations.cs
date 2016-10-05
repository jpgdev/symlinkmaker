﻿namespace SymlinkMaker.Core
{
    public interface IFileOperations
    {
        void CreateSymbolicLink (string sourceDirName, string targetDirName);

		void CopyDirectory (string sourceDirName, string targetDirName, bool copySubDirs);

        // TODO : Find a better name
        bool DirectoryExists(string path);

		void DeleteDirectory (string path, bool recursive);
	}
}
