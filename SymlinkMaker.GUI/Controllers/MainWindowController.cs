using System;
using SymlinkMaker.Core;
using System.ComponentModel;

namespace SymlinkMaker.GUI
{
    public class MainWindowController : IDisposable
    {
        #region Constants

        private const string GREEN_STATUS_IMAGE_NAME = "yes";
        private const string RED_STATUS_IMAGE_NAME = "no";

        #endregion

        #region Fields

        private readonly AppSettings _settings;
        private readonly IMainWindowView _view;
        private readonly ICommandsManager<CommandType> _commandManager;
        private readonly IDialogHelper _dialogHelper;

        #endregion

        #region Contructors

        public MainWindowController(
            AppSettings settings, 
            IMainWindowView view,
            ICommandsManager<CommandType> commandManager,
            IDialogHelper dialogHelper)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (commandManager == null)
                throw new ArgumentNullException(nameof(commandManager));

            if (dialogHelper == null)
                throw new ArgumentNullException(nameof(dialogHelper));

            _settings = settings;
            _view = view;
            _commandManager = commandManager;
            _dialogHelper = dialogHelper;

            SubscribeToEvents();

            // HACK : For now, to trigger the changes "manually" in the UI to bind the controls values
            var props = new []
            { 
                nameof(_settings.RequiresConfirmation), 
                nameof(_settings.SourcePath), 
                nameof(_settings.TargetPath) 
            };

            foreach (string propName in props)
                AppSettings_PropertyChanged(
                    _settings,
                    new PropertyChangedEventArgs(propName));
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }

        #endregion

        #region Events Un/Subscriptions

        private void SubscribeToEvents()
        {
            
            // Commands buttons
            _view.CopySourceButton.Triggered += View_CopySourceTriggered;
            _view.MoveSourceButton.Triggered += View_MoveSourceTriggered;
            _view.CreateSymlinkButton.Triggered += View_CreateSymlinkTriggered;
            _view.DeleteSourceButton.Triggered += View_DeleteSourceTriggered;
            _view.DeleteTargetButton.Triggered += View_DeleteTargetTriggered;
            _view.DoAllButton.Triggered += View_DoAllTriggered;

            // Find / Open location buttons
            _view.FindSourceButton.Triggered += View_FindSourcePathTriggered;
            _view.FindTargetButton.Triggered += View_FindTargetPathTriggered;
            _view.OpenSourceButton.Triggered += View_OpenSourcePathTriggered;
            _view.OpenTargetButton.Triggered += View_OpenTargetPathTriggered;

            // Text changed
            _view.SourcePath.TextChanged += View_SourcePathTextChanged;
            _view.TargetPath.TextChanged += View_TargetPathTextChanged;

            // View Settings changed
            _view.RequireConfirmToggleButton.StatusChanged += View_RequireConfirmationStatusChanged;

            // Settings changed
            _settings.PropertyChanged += AppSettings_PropertyChanged;
        }

        private void UnsubscribeFromEvents()
        {
            // Commands buttons
            _view.CopySourceButton.Triggered -= View_CopySourceTriggered;
            _view.MoveSourceButton.Triggered -= View_MoveSourceTriggered;
            _view.CreateSymlinkButton.Triggered -= View_CreateSymlinkTriggered;
            _view.DeleteSourceButton.Triggered -= View_DeleteSourceTriggered;
            _view.DeleteTargetButton.Triggered -= View_DeleteTargetTriggered;
            _view.DoAllButton.Triggered -= View_DoAllTriggered;

            // Find / Open location buttons
            _view.FindSourceButton.Triggered -= View_FindSourcePathTriggered;
            _view.FindTargetButton.Triggered -= View_FindTargetPathTriggered;
            _view.OpenSourceButton.Triggered -= View_OpenSourcePathTriggered;
            _view.OpenTargetButton.Triggered -= View_OpenTargetPathTriggered;

            // Text changed
            _view.SourcePath.TextChanged -= View_SourcePathTextChanged;
            _view.TargetPath.TextChanged -= View_TargetPathTextChanged;

            // View Settings changed
            _view.RequireConfirmToggleButton.StatusChanged -= View_RequireConfirmationStatusChanged;

            // Settings changed
            _settings.PropertyChanged -= AppSettings_PropertyChanged;
        }

        #endregion

        #region App Settings PropertyChanged

        private void AppSettings_PropertyChanged(object sender,
                                                 PropertyChangedEventArgs e)
        {
            var settings = (AppSettings)sender;

            switch (e.PropertyName)
            {
                case nameof(settings.RequiresConfirmation):
                    _view.RequireConfirmToggleButton.IsActive = settings.RequiresConfirmation;
                    break;

                case nameof(settings.SourcePath):
                    _view.SourcePath.Text = settings.SourcePath;
                    UpdateImageFromFileExistance(
                        _view.SourceStatusImage,
                        settings.SourcePath,
                        settings.FileOperations);
                    break;

                case nameof(settings.TargetPath):
                    _view.TargetPath.Text = settings.TargetPath;
                    UpdateImageFromFileExistance(
                        _view.TargetStatusImage,
                        settings.TargetPath,
                        settings.FileOperations,
                        true);
                    break;
            }
        }

        #endregion

        #region Button Event Handlers

        private void View_CopySourceTriggered(IButton button,
                                              ButtonEventArgs eventArgs)
        {
            _commandManager.Execute(
                CommandType.Copy, 
                _settings.SourcePath,
                _settings.TargetPath,
                _settings.RequiresConfirmation
            );

            UpdatePathStatusImages();
        }

        protected void View_CreateSymlinkTriggered(IButton sender, EventArgs e)
        {
            _commandManager.Execute(
                CommandType.CreateSymLink, 
                _settings.SourcePath, 
                _settings.TargetPath, 
                _settings.RequiresConfirmation);

            UpdatePathStatusImages();
        }

        protected void View_DeleteSourceTriggered(IButton sender, EventArgs e)
        {
            _commandManager.Execute(
                CommandType.Delete,
                _settings.SourcePath,
                null,
                true);

            UpdatePathStatusImages();
        }

        protected void View_DeleteTargetTriggered(IButton sender, EventArgs e)
        {
            _commandManager.Execute(
                CommandType.Delete,
                _settings.TargetPath,
                null,
                true);

            UpdatePathStatusImages();
        }

        protected void View_DoAllTriggered(IButton sender, EventArgs e)
        {
            _commandManager.Execute(
                CommandType.All, 
                _settings.SourcePath, 
                _settings.TargetPath, 
                _settings.RequiresConfirmation);

            UpdatePathStatusImages();
        }

        protected void View_MoveSourceTriggered(IButton sender, EventArgs e)
        {  
            _commandManager.Execute(
                CommandType.Move, 
                _settings.SourcePath, 
                _settings.TargetPath, 
                _settings.RequiresConfirmation);

            UpdatePathStatusImages();
        }

        protected void View_FindSourcePathTriggered(IButton sender,
                                                    EventArgs e)
        {            
            string path = _dialogHelper.ShowFileChooserDialog(
                              _settings.SourcePath, 
                              ChooserDialogAction.FindDirectory,
                              "Select the source directory",
                              "Select"
                          );

            if (!string.IsNullOrEmpty(path))
                _settings.SourcePath = path;
        }

        protected void View_FindTargetPathTriggered(IButton sender,
                                                    EventArgs e)
        {
            string path = _dialogHelper.ShowFileChooserDialog(
                              _settings.TargetPath, 
                              ChooserDialogAction.FindDirectory,
                              "Select the target directory",
                              "Select"
                          );

            if (!string.IsNullOrEmpty(path))
                _settings.TargetPath = path;
        }

        protected void View_OpenSourcePathTriggered(IButton sender,
                                                    EventArgs e)
        {
            // TODO : Replace with Interface
            // Process.Start(_settings.SourcePath);
        }

        protected void View_OpenTargetPathTriggered(IButton sender,
                                                    EventArgs e)
        {
            // TODO : Replace with Interface
            // Process.Start(_settings.TargetPath);
        }

        private void View_RequireConfirmationStatusChanged(IToggle button,
                                                           ToggleEventArgs eventArgs)
        {
            _settings.RequiresConfirmation = eventArgs.IsActive; 
        }

        #endregion

        #region Text Event Handlers

        private void View_SourcePathTextChanged(ITextSource sender,
                                                EventArgs eventArgs)
        {
            _settings.SourcePath = sender.Text;
        }

        private void View_TargetPathTextChanged(ITextSource sender,
                                                EventArgs eventArgs)
        {
            _settings.TargetPath = sender.Text;
        }

        #endregion

        #region Methods

        protected static void UpdateImageFromFileExistance(
            IImage image, 
            string path, 
            IFileSystemOperations fileOperations, 
            bool invertImages = false)
        {
            bool exists = fileOperations.Exists(path);

            string imgTooltip = exists ? "This location is occupied" : "This location is free";

            // This is used since the Target path & Source path expect different icons
            // for the same status. 
            // Ex: If a directory exists, the source is green, but the target is red
            if (invertImages)
                exists = !exists;

            image.Path = exists ? GREEN_STATUS_IMAGE_NAME : RED_STATUS_IMAGE_NAME;
            image.Tooltip = imgTooltip;
        }

        // FIXME : This is until I find a better way to do this, that is bound to the commands directly
        private void UpdatePathStatusImages()
        {
            UpdateImageFromFileExistance(
                _view.SourceStatusImage,
                _settings.SourcePath,
                _settings.FileOperations);

            UpdateImageFromFileExistance(
                _view.TargetStatusImage,
                _settings.TargetPath,
                _settings.FileOperations,
                true);
        }

        #endregion
    }
}
