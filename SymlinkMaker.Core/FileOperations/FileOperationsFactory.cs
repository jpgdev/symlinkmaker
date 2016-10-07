using System;

namespace SymlinkMaker.Core
{
	public static class FileOperationsFactory
	{
		/// <summary>
		/// Gets the file operations object from the current OS.
		/// </summary>
		/// <returns>The file operations object.</returns>
		public static IFileOperations GetFileOperationsObject ()
		{
			switch (Environment.OSVersion.Platform) {
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
					return new WindowsFileOperations ();
                case PlatformID.Unix:
                    return new UnixFileOperations();
                default:
                    return new BasicFileOperations();
			}
		}
	}
}
