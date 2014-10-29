using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace Symlink_Maker
{
    public partial class MainWindow : Window
    {
        #region External calls

        private enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        [DllImport("kernel32.dll")]
        private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        #endregion

        public MainWindow()
        {
             Loaded += OnWindowLoad;
        }

        private void OnWindowLoad(object sender, RoutedEventArgs e)
        {
            TxtSymLinkSource.Text = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).FullName;
            TxtSymLinkDestination.Text = @"G:\Google Drive\Save Games\";
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
   
        #region Methods

        private static string GetPathFromPicker(string intialPath)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog() { SelectedPath = intialPath };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                intialPath = dialog.SelectedPath.ToString();

            return intialPath;
        }

        private static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            try
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

                return true;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "There was an error copying the directories", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;            
        }

        private bool CreateSymLink()
        {
            string confirmMessage = String.Format("Making a syslink: {0} ==> {1} ?", TxtSymLinkSource.Text, TxtSymLinkDestination.Text);
            MessageBoxResult result = System.Windows.MessageBox.Show(confirmMessage, "Sym link validation", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                bool created = CreateSymbolicLink(TxtSymLinkSource.Text, TxtSymLinkDestination.Text, SymbolicLink.Directory);

                System.Windows.MessageBox.Show(created ? "Symbolic link created." : "Symbolic link creation failed.");

                return created;
            }

            return false;
        }

        private bool CopySaves()
        {
            string confirmMessage = String.Format("Copying the save games from '{0}' to '{1}' ?", TxtSymLinkSource.Text, TxtSymLinkDestination.Text);
            MessageBoxResult result = System.Windows.MessageBox.Show(confirmMessage, "Save games copy validation", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                return DirectoryCopy(TxtSymLinkSource.Text, TxtSymLinkDestination.Text, true);
            }

            return false;
        }

        private bool DeleteSourceDirectory()
        {
            string confirmMessage = String.Format("Are you sure you want to delete '{0}' and all contained directories?", TxtSymLinkSource.Text);
            MessageBoxResult result = System.Windows.MessageBox.Show(confirmMessage, "Directory deletion validation", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    Directory.Delete(TxtSymLinkSource.Text, true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "There was an error deleting the source directory", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return false;
        }

        #endregion

        #region Events Handler

        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            TxtSymLinkSource.Text = GetPathFromPicker(TxtSymLinkSource.Text);
        }

        private void BtnBrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            TxtSymLinkDestination.Text = GetPathFromPicker(TxtSymLinkDestination.Text);
        }               

        private void CopySavesButton_Click(object sender, RoutedEventArgs e)
        {
            CopySaves();
        }

        private void DeleteSourceDirButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSourceDirectory();
        }
        private void SymLinkButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSymLink();
        }

        private void DoWholeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CopySaves()) return;
            if (!DeleteSourceDirectory()) return;
            if (!CreateSymLink()) return;

            System.Windows.MessageBox.Show("The whole process was successfull.");
        }

        #endregion

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            e.Handled = true;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(System.Windows.MessageBox.Show("Are you sure you want to quit?", "Quit", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                e.Cancel = true;
        }
    }
}
