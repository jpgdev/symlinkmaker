using System;
using Gtk;
using System.Diagnostics;
using SymlinkMaker.Core;
using System.IO;
using Action = System.Action;
using System.Collections.Generic;
using Stetic;

namespace SymlinkMaker.GUI.GTKSharp
{
    public partial class MainWindow : Gtk.Window
    {
        private string _homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private IFileOperations _fileOperations;
        private Dictionary<string, GtkSharpCommand> _commands;

        // TODO : Use the CommandType instead?
        private const string COPY_COMMAND_NAME = "COPY";
        private const string DELETE_COMMAND_NAME = "DELETE";
        private const string SYMLINK_COMMAND_NAME = "CREATE_SYMLINK";
        private const string ALL_COMMAND_NAME = "ALL";

        public MainWindow()
            : base(Gtk.WindowType.Toplevel)
        {           
            this.Build();

            Initialize();

            BindEvents();
        }

        private void Initialize()
        {
            _fileOperations = FileOperationsFactory.GetFileOperationsObject();

            // TODO : Change these, test values
            txtBoxSource.Text = _homeFolder + "/Sandbox/TestSymlink/TestFolder";
            txtBoxTarget.Text = _homeFolder + "/Sandbox/TestSymlink/TestFolder_COPY";

            _commands = new Dictionary<string, GtkSharpCommand>();

            _commands.Add(COPY_COMMAND_NAME, new GtkSharpCommand((args) =>
                    {
                        // TODO : Validation
                        if (args["sourcePath"] == args["targetPath"])
                            throw new Exception("The source cannot be the same path as the target.");

                        _fileOperations.CopyDirectory(args["sourcePath"], args["targetPath"], true);

                        return true;
                    }, 
                    new GtkSharpCommandConfirmationInfo(
                        "Are you sure you want to copy '{0}' to '{1}' ?",
                        new string[] { "sourcePath", "targetPath" },
                        "Copying {0} ...",
                        new string[] { "sourcePath" }
                    ),
                    new string[] { "sourcePath", "targetPath" }
                ));

            _commands.Add(DELETE_COMMAND_NAME, new GtkSharpCommand((args) =>
                    {
                        // TODO : Validation
                        _fileOperations.DeleteDirectory(args["sourcePath"], true);

                        return true;
                    }, 
                    new GtkSharpCommandConfirmationInfo(
                        "Are you sure you want to delete '{0}' ?",
                        new string[] { "sourcePath" },
                        "Deleting {0} ...",
                        new string[] { "sourcePath" }
                    ),
                    new string[] { "sourcePath" }
                ));

            string symlinkCommandConfirmMessage = string.Format(
                                                      "Create a symbolic link?{0}{0}{1}",
                                                      Environment.NewLine,
                                                      "'{0}' => '{1}'"
//                "Target: '{1}'"
                                                  );

            _commands.Add(SYMLINK_COMMAND_NAME, new GtkSharpCommand((args) =>
                    {
                        // TODO : Validation
                        if (args["sourcePath"] == args["targetPath"])
                            throw new Exception("The source cannot be the same path as the target.");

                        _fileOperations.CreateSymbolicLink(args["sourcePath"], args["targetPath"]);

                        return true;
                    }, 
                    new GtkSharpCommandConfirmationInfo(
                        symlinkCommandConfirmMessage,
                        new string[] { "sourcePath", "targetPath" },
                        "Linking {0} to {1} ...",
                        new string[] { "sourcePath", "targetPath" }
                    ),
                    new string[] { "sourcePath", "targetPath" }
                ));

            string allCommandConfirmMessage = string.Format(
                                                  "Are you sure you want to launch the whole process?{0}{0}{1}{0}{2}{0}{0}{3}",
                                                  Environment.NewLine,
                                                  "Source: '{0}'",
                                                  "Target: '{1}'",
                                                  "This will copy the source directory to the target directory then replace the source directory with a link to the target directory."
                                              );

            _commands.Add(ALL_COMMAND_NAME, new GtkSharpCommand((args) =>
                    {                   
                        // TODO : Validation
                        if (args["sourcePath"] == args["targetPath"])
                            throw new Exception("The source cannot be the same path as the target.");

                        string[] commands = new string[]
                        {
                            COPY_COMMAND_NAME, 
                            DELETE_COMMAND_NAME,
                            SYMLINK_COMMAND_NAME
                        };

                        foreach (string cmd in commands)
                        {
                            if (!_commands[cmd].Run(args))
                            {
                                return false;
                            }
                        }

                        return true;
                    }, 
                    new GtkSharpCommandConfirmationInfo(
                        allCommandConfirmMessage,
                        new string[] { "sourcePath", "targetPath" },
                        "Launch whole process ...",
                        new string[] { "sourcePath", "targetPath" }
                    ),
                    new string[] { "sourcePath", "targetPath" }
                ));

        }

        private void BindEvents()
        {
            // NOTE: This is done here, since binding it with the Designer (& auto generated code)
            //       will sometimes undo itself. 

            btnCopySource.Clicked += OnBtnCopySourceClicked;
            btnCreateSymlink.Clicked += OnBtnCreateSymlinkClicked;
            btnDeleteSource.Clicked += OnBtnDeleteSourceClicked;
            btnDoWhole.Clicked += OnBtnDoWholeClicked;

            btnFindSource.Clicked += OnBtnFindSourceClicked;
            btnOpenSource.Clicked += OnBtnOpenSourceClicked;

            btnFindTarget.Clicked += OnBtnFindTargetClicked;
            btnOpenTarget.Clicked += OnBtnOpenTargetClicked;

            txtBoxSource.Changed += OnTxtBoxSourceChanged;
            txtBoxTarget.Changed += OnTxtBoxTargetChanged;
        }

        #region Window Events

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }

        #endregion

        #region Buttons Events

        protected void OnBtnFindSourceClicked(object sender, EventArgs e)
        {            
            string path = ShowDirectoryChooserDialog(txtBoxSource.Text, "Select the source directory");
            if (!string.IsNullOrEmpty(path))
            {
                txtBoxSource.Text = path;
            }
        }

        protected void OnBtnFindTargetClicked(object sender, EventArgs e)
        {
            string path = ShowDirectoryChooserDialog(txtBoxTarget.Text, "Select the target directory");
            if (!string.IsNullOrEmpty(path))
            {
                txtBoxTarget.Text = path;
            }
        }

        protected void OnBtnOpenSourceClicked(object sender, EventArgs e)
        {
            Process.Start(txtBoxSource.Text);
        }

        protected void OnBtnOpenTargetClicked(object sender, EventArgs e)
        {
            Process.Start(txtBoxTarget.Text);
        }

        protected void OnBtnCopySourceClicked(object sender, EventArgs e)
        {  
            bool result = _commands[COPY_COMMAND_NAME].Run(new Dictionary<string, string>()
                {
                    { "sourcePath", txtBoxSource.Text },
                    { "targetPath", txtBoxTarget.Text }
                }, true);
            
            if (result)
                ShowInfoDialog(string.Format("{0} successfully copied to {1}.", txtBoxSource.Text, txtBoxTarget.Text), "Success");
        }


        protected void OnBtnDeleteSourceClicked(object sender, EventArgs e)
        {
            bool result = _commands[DELETE_COMMAND_NAME].Run(new Dictionary<string, string>()
                {
                    { "sourcePath", txtBoxSource.Text }
                }, true);

            if (result)
                ShowInfoDialog(string.Format("{0} successfully deleted.", txtBoxSource.Text), "Success");
        }

        protected void OnBtnCreateSymlinkClicked(object sender, EventArgs e)
        {
            bool result = _commands[SYMLINK_COMMAND_NAME].Run(new Dictionary<string, string>()
                {
                    { "sourcePath", txtBoxSource.Text },
                    { "targetPath", txtBoxTarget.Text }
                }, true);

            if (result)
                ShowInfoDialog(string.Format("Link to {0} successfully created.", txtBoxTarget.Text), "Success");
        }

        protected void OnBtnDoWholeClicked(object sender, EventArgs e)
        {
            bool result = _commands[ALL_COMMAND_NAME].Run(new Dictionary<string, string>()
                {
                    { "sourcePath", txtBoxSource.Text },
                    { "targetPath", txtBoxTarget.Text }
                }, true);

            if (result)
                ShowInfoDialog("Operation Successful!", "Success");
        }

        //        protected void OnTextBoxSourceChanged(object sender, EventArgs e)
        //        {
        //            sourceFileChooser.SetCurrentFolder((sender as Entry).Text);
        //        }
        //
        //        protected void OnSourceFileChooserCurrentFolderChanged(object sender, EventArgs e)
        //        {
        //            txtBoxSource.Text = ((sender as FileChooserWidget).CurrentFolder);
        //        }



        #endregion

        #region Text Entry Events

        protected void OnTxtBoxSourceChanged(object sender, EventArgs e)
        {
            Entry entry = sender as Entry;
            string text = entry.Text;

            // TODO : Merge this with the other one (the exists cond is inverted)

            bool exists = _fileOperations.DirectoryExists(text);
            string imgTooltip = exists ? "This already exists." : "This does not exist.";
            string imageName = exists ? "gtk-apply" : "gtk-dialog-error";

            imgSourcePath.TooltipText = imgTooltip;

            SetImageFromGtkIcon(imgSourcePath, imageName);
        }

        protected void OnTxtBoxTargetChanged(object sender, EventArgs e)
        {
            Entry entry = sender as Entry;
            string text = entry.Text;

            // TODO : Merge this with the other one (the exists cond is inverted)

            bool exists = _fileOperations.DirectoryExists(text);
            string imgTooltip = exists ? "This already exists." : "This does not exist.";
            string imageName = exists ? "gtk-dialog-error" : "gtk-apply";

            imgTargetPath.TooltipText = imgTooltip;

            SetImageFromGtkIcon(imgTargetPath, imageName);
        }

        #endregion

        protected void ShowInfoDialog(string text, string title = "Info")
        {
            var infoDialog = new MessageDialog(
                                 this,
                                 DialogFlags.DestroyWithParent,
                                 MessageType.Info,
                                 ButtonsType.Ok,
                                 text);

            if (!string.IsNullOrEmpty(title))
                infoDialog.Title = title;

            infoDialog.Run();
            infoDialog.Destroy();
        }

        protected string ShowDirectoryChooserDialog(string basePath = "", string title = "Choose a directory")
        {
            var chooser = new FileChooserDialog(
                              title,
                              this,
                              FileChooserAction.SelectFolder,
                              "Cancel", ResponseType.Cancel,
                              "Select", ResponseType.Accept);
            
            chooser.SetCurrentFolder(basePath);

            string returnedPath = null;
            if (chooser.Run() == (int)ResponseType.Accept)
            {
                returnedPath = chooser.CurrentFolder;
            }

            chooser.Destroy();

            return returnedPath;
        }

        protected void SetImageFromGtkIcon(Image image, string imageName)
        {
            image.Pixbuf = IconLoader.LoadIcon (this, imageName, IconSize.Menu);

        }
    }
}

