using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace Symlink_Maker
{
    public partial class MainWindow : Window
    {
        private enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        [DllImport("kernel32.dll")]
        private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);
        
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            TxtSymLinkSource.Text = @"C:\Users\JP\";
            TxtSymLinkDestination.Text = @"G:\Google Drive\Save Games\";
        }
       
        private static string GetPathFromPicker(string intialPath)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog() { SelectedPath = intialPath };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                intialPath = dialog.SelectedPath.ToString();

            return intialPath;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        
        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            TxtSymLinkSource.Text = GetPathFromPicker(TxtSymLinkSource.Text);
        }

        private void BtnBrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            TxtSymLinkDestination.Text = GetPathFromPicker(TxtSymLinkDestination.Text);
        }

        private void SymLinkButton_Click(object sender, RoutedEventArgs e)
        {
            string confirmMessage = String.Format("Making a syslink: {0} ==> {1} ?", TxtSymLinkSource.Text, TxtSymLinkDestination.Text);
            MessageBoxResult result = System.Windows.MessageBox.Show(confirmMessage, "Sym link validation", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                bool created = CreateSymbolicLink(TxtSymLinkSource.Text, TxtSymLinkDestination.Text, SymbolicLink.Directory);

                System.Windows.MessageBox.Show(created ? "Symbolic link created." : "Symbolic link creation failed");
            }
        }

        private void CopySavesButton_Click(object sender, RoutedEventArgs e)
        {
            string confirmMessage = String.Format("Copying the save games from '{0}' to '{1}' ?", TxtSymLinkSource.Text, TxtSymLinkDestination.Text);
            MessageBoxResult result = System.Windows.MessageBox.Show(confirmMessage, "Save games copy validation", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                DirectoryCopy(TxtSymLinkSource.Text, TxtSymLinkDestination.Text, true);
            }
        }

        private void DeleteSourceDirButton_Click(object sender, RoutedEventArgs e)
        {
            string confirmMessage = String.Format("Are you sure you want to delete '{0}' and all contained directories?", TxtSymLinkSource.Text);
            MessageBoxResult result = System.Windows.MessageBox.Show(confirmMessage, "Directory deletion validation", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Directory.Delete(TxtSymLinkSource.Text, true);
            }
        }
    }
}
