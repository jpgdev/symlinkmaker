using System;
using Gtk;

namespace SymlinkMaker.GUI.GTKSharp
{
    public class GtkSharpDialogHelper : IDialogHelper
    {
        #region IDialogHelper implementation

        public bool ShowDialog(DialogType type, string content, string title = null)
        {
            return ShowDialog(
                type,
                content,
                title,
                null,
                true
            );
        }

        public string ShowFileChooserDialog(
            string basePath, 
            ChooserDialogAction action, 
            string title = null,
            string acceptButtonContent = null)
        {

            return ShowFileChooserDialog(
                basePath,
                title,
                GetFileChooserAction(action),
                acceptButtonContent,
                null
            );
        }

        #endregion

        #region Private Show Dialog methods

        private static bool ShowDialog(
            DialogType type, 
            string content, 
            string title = null, 
            Window parent = null,
            bool useMarkup = true)
        {
            
            MessageDialog dialog = null;
            try
            {      
                MessageType messageType;
                ButtonsType buttonsType;
                string iconName;

                switch (type)
                {
                    case DialogType.Information:                       

                        messageType = MessageType.Info;
                        buttonsType = ButtonsType.Ok;
                        iconName = "gtk-dialog-info";

                        break;
                    case DialogType.Error:

                        messageType = MessageType.Error;
                        buttonsType = ButtonsType.Ok;
                        iconName = "gtk-dialog-error";
                        break;

                    case DialogType.Warning:

                        messageType = MessageType.Warning;
                        buttonsType = ButtonsType.Ok;
                        iconName = "gtk-dialog-warning";                       
                        break;
                    
                    case DialogType.Confirmation:   
                        if (string.IsNullOrEmpty(title))
                            title = "Are you sure?";
                        
                        messageType = MessageType.Warning;
                        buttonsType = ButtonsType.YesNo;
                        iconName = "gtk-dialog-warning";
                        break;

                    default:
                        throw new NotImplementedException(string.Concat("{0} is not an implemented dialog type", type.ToString()));
                }              

                if (string.IsNullOrEmpty(title))
                    title = type.ToString();

                dialog = new MessageDialog(
                    parent, 
                    DialogFlags.DestroyWithParent, 
                    messageType, 
                    buttonsType,
                    useMarkup,
                    content
                );
                
                dialog.Title = title;   
                
                if (!string.IsNullOrEmpty(iconName))
                    dialog.IconName = iconName;

                var responseType = (ResponseType)dialog.Run();

                bool returnedValue = false;

                if (type == DialogType.Confirmation && responseType == ResponseType.Yes)
                    returnedValue = true;

                return returnedValue;
            }
            finally
            {
                if (dialog != null)
                    dialog.Destroy();
            }
        }

        private static string ShowFileChooserDialog(
            string basePath = null, 
            string title = "Choose", 
            FileChooserAction fileChooseAction = FileChooserAction.Open,
            string acceptButtonContent = "Select",
            Window parent = null)
        {
            FileChooserDialog chooser = null;
            try
            {
                chooser = new FileChooserDialog(
                    title,
                    parent,
                    fileChooseAction,
                    "Cancel", ResponseType.Cancel,
                    acceptButtonContent, ResponseType.Accept);

                if (!string.IsNullOrEmpty(basePath))
                    chooser.SetCurrentFolder(basePath);

                ResponseType result = (ResponseType)chooser.Run();

                string returnedPath = null;
                switch (fileChooseAction)
                {
                    case FileChooserAction.CreateFolder:
                    case FileChooserAction.SelectFolder:
                        returnedPath = chooser.CurrentFolder;
                        break;

                    case FileChooserAction.Open:
                    case FileChooserAction.Save:
                        returnedPath = chooser.Filename;
                        break;
                }

                return result == ResponseType.Accept ? returnedPath : null;
            }
            finally
            {
                if (chooser != null)
                    chooser.Destroy();
            }
        }

        #endregion

        #region Conversion Methods

        private static FileChooserAction GetFileChooserAction(ChooserDialogAction action)
        {
            switch (action)
            {
                case ChooserDialogAction.CreateDirectory:
                    return FileChooserAction.CreateFolder;

                case ChooserDialogAction.CreateFile:
                    return FileChooserAction.Save;

                case ChooserDialogAction.FindDirectory:
                    return FileChooserAction.SelectFolder;

                case ChooserDialogAction.FindFile:
                    return FileChooserAction.Open;
                
                default :
                    throw new NotSupportedException($"{action} is not a supported ChooseDialogAction.");
            }
        }

        #endregion
    }
}

