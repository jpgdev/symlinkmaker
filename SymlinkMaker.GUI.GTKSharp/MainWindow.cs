using Gtk;
using System;
using System.Diagnostics;
using SymlinkMaker.Core;
using System.IO;
using System.Collections.Generic;
using Stetic;
using Action = System.Action;
using System.Reflection;
using System.ComponentModel;

namespace SymlinkMaker.GUI.GTKSharp
{
    public partial class MainWindow : Gtk.Window
    {
        private readonly string HOME_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        private IDictionary<CommandType, GtkSharpCommandWrapper> _commands;
        private AppSettings _settings;

        public MainWindow()
            : base(Gtk.WindowType.Toplevel)
        {           
            this.Build();

            Initialize();
        }

        private void Initialize()
        {
            // Note : The order here is important

            BindEvents();

            InitializeSettings();

            InitializeCommands(_settings.FileOperations);
        }

        private void InitializeSettings()
        {
            _settings = new AppSettings();
            _settings.PropertyChanged += AppSettings_PropertyChanged;

            // TODO : Load these from a (JSON, YAML?) config file (most of these)

            _settings.FileOperations = FileOperationsFactory.GetFileOperationsObject();
            _settings.RequiresConfirmation = true;
            // TODO : Change these, test values
            _settings.SourcePath = HOME_FOLDER + "/Sandbox/TestSymlink/TestFolder";
            _settings.TargetPath = HOME_FOLDER + "/Sandbox/TestSymlink/TestFolder_COPY";
        }

        private void BindEvents()
        {
            // NOTE: This is done here, since binding it with the Designer (& auto generated code)
            //       will sometimes undo itself. 

            btnCopySource.Clicked += OnBtnCopySourceClicked;
            btnMoveSource.Clicked += OnBtnMoveSourceClicked;
            btnCreateSymlink.Clicked += OnBtnCreateSymlinkClicked;
            btnDeleteSource.Clicked += OnBtnDeleteSourceClicked;
            btnDeleteTarget.Clicked += OnBtnDeleteTargetClicked;
            btnDoWhole.Clicked += OnBtnDoWholeClicked;

            btnFindSource.Clicked += OnBtnFindSourceClicked;
            btnOpenSource.Clicked += OnBtnOpenSourceClicked;

            btnFindTarget.Clicked += OnBtnFindTargetClicked;
            btnOpenTarget.Clicked += OnBtnOpenTargetClicked;

            txtBoxSource.Changed += OnTxtBoxSourceChanged;
            txtBoxTarget.Changed += OnTxtBoxTargetChanged;

            toggleRequireConfirmAction.Toggled += OnToggleConfirmToggled;
        }

        private void InitializeCommands(IFileOperations fileOperations){
            FileCommands.InitializeCommands(fileOperations);

            _commands = new Dictionary<CommandType, GtkSharpCommandWrapper>()
            {
                {
                    CommandType.Copy,
                    new GtkSharpCommandWrapper(
                        FileCommands.Commands[CommandType.Copy],
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Are you sure you want to copy?</b>{0}{0}{1}",
                                Environment.NewLine,
                                "'{0}' => '{1}'"
                            ),
                            new string[] { "sourcePath", "targetPath" },
                            "Copying {0} ...",
                            new string[] { "sourcePath" }
                        )
                    )
                },
                {
                    CommandType.Move,
                    new GtkSharpCommandWrapper(
                        FileCommands.Commands[CommandType.Move],
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Are you sure you want to move?</b>{0}{0}{1}",
                                Environment.NewLine,
                                "'{0}' => '{1}'"
                            ),
                            new string[] { "sourcePath", "targetPath" },
                            "Moving {0} ...",
                            new string[] { "sourcePath"}
                        )
                    )
                },
                {
                    CommandType.Delete,
                    new GtkSharpCommandWrapper(
                        FileCommands.Commands[CommandType.Delete],
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Are you sure you want to delete?</b>{0}{0}{1}",
                                Environment.NewLine,
                                "'{0}'"
                            ),
                            new string[] { "sourcePath" },
                            "Deleting {0} ...",
                            new string[] { "sourcePath" }
                        )
                    )
                },
                {
                    CommandType.CreateSymLink,
                    new GtkSharpCommandWrapper(
                        FileCommands.Commands[CommandType.CreateSymLink],
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Create a symbolic link?</b>{0}{0}{1}",
                                Environment.NewLine,
                                "'{0}' => '{1}'"
                            ),
                            new string[] { "sourcePath", "targetPath" },
                            "Linking {0} to {1} ...",
                            new string[] { "sourcePath", "targetPath" }
                        )
                    )
                },
                {
                    CommandType.All,
                    new GtkSharpCommandWrapper(
                        FileCommands.Commands[CommandType.All],
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Are you sure you want to launch the replacement process?</b>{0}{0}{1}{0}{2}{0}{0}{3}",
                                Environment.NewLine,
                                "<b>Source:</b> '{0}'",
                                "<b>Target:</b> '{1}'",
                                "This will move the <b>source</b> directory to the <b>target</b> directory then create a " +
                                "symlink from the original <b>source</b> directory to the <b>target</b> directory."
                            ),
                            new string[] { "sourcePath", "targetPath" },
                            "Launch replacement process ...",
                            new string[] { "sourcePath", "targetPath" }
                        )
                    )
                }
            };
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
            string path = ShowDirectoryChooserDialog(_settings.SourcePath, "Select the source directory");
            if (!string.IsNullOrEmpty(path))
            {
                _settings.SourcePath = path;
            }
        }

        protected void OnBtnFindTargetClicked(object sender, EventArgs e)
        {
            string path = ShowDirectoryChooserDialog(_settings.TargetPath, "Select the target directory");
            if (!string.IsNullOrEmpty(path))
            {
                _settings.TargetPath = path;
            }
        }

        protected void OnBtnOpenSourceClicked(object sender, EventArgs e)
        {
            Process.Start(_settings.SourcePath);
        }

        protected void OnBtnOpenTargetClicked(object sender, EventArgs e)
        {
            Process.Start(_settings.TargetPath);
        }

        protected void OnBtnCopySourceClicked(object sender, EventArgs e)
        {  
            _commands[CommandType.Copy].Run(
                new Dictionary<string, string>()
                {
                    { "sourcePath", _settings.SourcePath },
                    { "targetPath", _settings.TargetPath }
                }, 
                _settings.RequiresConfirmation
            );
            
//            ShowInfoDialog(string.Format("{0} successfully copied to {1}.", _settings.SourcePath, _settings.TargetPath), "Success");
            AfterCommands();
        }

        protected void OnBtnMoveSourceClicked(object sender, EventArgs e)
        {  
            _commands[CommandType.Move].Run(
                new Dictionary<string, string>()
                {
                    { "sourcePath", _settings.SourcePath },
                    { "targetPath", _settings.TargetPath }
                }, 
                _settings.RequiresConfirmation
            );

//            ShowInfoDialog(string.Format("{0} successfully copied to {1}.", _settings.SourcePath, _settings.TargetPath), "Success");
            AfterCommands();
        }

        protected void OnBtnDeleteTargetClicked(object sender, EventArgs e)
        {
            _commands[CommandType.Delete].Run(
                new Dictionary<string, string>()
                {
                    { "sourcePath", _settings.TargetPath }
                },
                true // Since it may be dangerous always confirm?
                //_settings.RequiresConfirmation
            );

            //            ShowInfoDialog(string.Format("{0} successfully deleted.", _settings.SourcePath), "Success");
            AfterCommands();
        }

        protected void OnBtnDeleteSourceClicked(object sender, EventArgs e)
        {
            _commands[CommandType.Delete].Run(
                new Dictionary<string, string>()
                {
                    { "sourcePath", _settings.SourcePath }
                }, 
                _settings.RequiresConfirmation
            );

//            ShowInfoDialog(string.Format("{0} successfully deleted.", _settings.SourcePath), "Success");
            AfterCommands();
        }

        protected void OnBtnCreateSymlinkClicked(object sender, EventArgs e)
        {
            _commands[CommandType.CreateSymLink].Run(
                new Dictionary<string, string>()
                {
                    { "sourcePath", _settings.SourcePath },
                    { "targetPath", _settings.TargetPath }
                }, 
                _settings.RequiresConfirmation
            );

//            ShowInfoDialog(string.Format("Link to {0} successfully created.", _settings.TargetPath), "Success");
            AfterCommands();
        }

        protected void OnBtnDoWholeClicked(object sender, EventArgs e)
        {
            _commands[CommandType.All].Run(
                new Dictionary<string, string>()
                {
                    { "sourcePath", _settings.SourcePath },
                    { "targetPath", _settings.TargetPath }
                }, 
                _settings.RequiresConfirmation
            );

//            ShowInfoDialog("Operation Successful!", "Success");
            AfterCommands();
        }

        protected void OnToggleConfirmToggled(object sender, EventArgs e)
        {
            ToggleAction toggleAction = (ToggleAction)sender;

            _settings.RequiresConfirmation = toggleAction.Active;
        }

        protected void OnAboutActionActivated(object sender, EventArgs e)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            ShowInfoDialog(
                string.Format(
                    "SymlinkMaker {1}{0}", 
                    Environment.NewLine, 
                    version
                ), "Symlink Maker");
        }

        #endregion

        #region Text Entry Events

        protected void OnTxtBoxSourceChanged(object sender, EventArgs e)
        {
            _settings.SourcePath = ((Entry)sender).Text;
            UpdateSourceTextStatus();
        }

        protected void OnTxtBoxTargetChanged(object sender, EventArgs e)
        {
            _settings.TargetPath = ((Entry)sender).Text;
            UpdateTargetTextStatus();
        }

        #endregion

        private void AppSettings_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            AppSettings settings = (AppSettings)sender;

            switch (e.PropertyName)
            {
                case "RequiresConfirmation":
                    toggleRequireConfirmAction.Active = settings.RequiresConfirmation;
                    break;

                case "SourcePath":
                    txtBoxSource.Text = settings.SourcePath;
                    break;

                case "TargetPath":
                    txtBoxTarget.Text = settings.TargetPath;
                    break;

                case "FileOperations":
                    break;
            }
        }


        // NOTE : TEMP method used to refresh the icons after a button is clicked, so the files may change
        // Until I find a better way
        protected void AfterCommands()
        {
            UpdateSourceTextStatus();
            UpdateTargetTextStatus();
        }

        protected void UpdateSourceTextStatus(){
            UpdateImageFromFileExistance(imgSourcePath, _settings.SourcePath, FileOperationsFactory.GetFileOperationsObject());
        }

        protected void UpdateTargetTextStatus(){
            UpdateImageFromFileExistance(imgTargetPath, _settings.TargetPath, FileOperationsFactory.GetFileOperationsObject(), true);
        }

        protected void UpdateImageFromFileExistance(
            Image image, 
            string path, 
            IFileOperations fileOperations, 
            bool invertImages = false)
        {
            bool exists = fileOperations.DirectoryExists(path);

            string imgTooltip = exists ? "This location is occupied" : "This location is free";

            if (invertImages) 
                exists = !exists;            

            string imageName = exists ? "gtk-yes" : "gtk-no";

            image.TooltipText = imgTooltip;
            image.Pixbuf = IconLoader.LoadIcon (this, imageName, IconSize.Menu);
        }


        #region Show dialogs

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


       
        #endregion

    }
}

