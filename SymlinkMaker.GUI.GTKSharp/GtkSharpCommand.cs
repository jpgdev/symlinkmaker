using System;
using SymlinkMaker.Core;
using System.Collections.Generic;
using Gtk;
using System.Linq;

namespace SymlinkMaker.GUI.GTKSharp
{    
    public class GtkSharpCommand : ConfirmableCommand
    {      
        #region Constructors

        public GtkSharpCommand(
            Func<IDictionary<string, string>, bool> commandFunc, 
            string[] requiredArgs = null)
            : this(commandFunc, new GtkSharpCommandConfirmationInfo(), requiredArgs)
        {            
        }

        public GtkSharpCommand(
            Func<IDictionary<string, string>, bool> commandFunc, 
            GtkSharpCommandConfirmationInfo confirmInfo,
            string[] requiredArgs = null)
            : this(commandFunc, (args) => ShowConfirmationDialog(confirmInfo, args), requiredArgs)
        {  
        }

        private GtkSharpCommand(
            Func<IDictionary<string, string>, bool> commandFunc, 
            Func<IDictionary<string, string>, bool> confirmFunc, 
            string[] requiredArgs = null)
            : base(commandFunc, confirmFunc, requiredArgs)
        {
        }

        #endregion

//        protected static bool ShowConfirmationDialog(
//            string message = "Are you sure?", 
//            string[] messageArgs = null,
//            string title = null, 
//            string[] titleArgs = null)
//        {
        protected static bool ShowConfirmationDialog(GtkSharpCommandConfirmationInfo confirmInfo, IDictionary<string, string> args)
        {            
            string content = confirmInfo.Message;
            if (confirmInfo.MessageArgs != null)
            {
                string[] argValues = confirmInfo.MessageArgs.Select((argName) => args[argName]).ToArray();
                content = string.Format(content, argValues);
            }

            string dialogTitle = confirmInfo.DialogTitle;
            if (confirmInfo.DialogTitleArgs != null)
            {
                string[] argValues = confirmInfo.DialogTitleArgs.Select((argName) => args[argName]).ToArray();
                dialogTitle = string.Format(dialogTitle, argValues);
            }

            // TO TEST : What happens if content is null????
            var confirmDialog = new MessageDialog(
                                    null,
                                    DialogFlags.DestroyWithParent,
                                    MessageType.Warning,
                                    ButtonsType.YesNo,
                                    content);

            if (!string.IsNullOrEmpty(dialogTitle))
                confirmDialog.Title = dialogTitle;

            bool returnedValue = false;
            if (confirmDialog.Run() == (int)ResponseType.Yes)
                returnedValue = true;

            confirmDialog.Destroy();

            return returnedValue;
        }

        public override bool HandleException(Exception ex, IDictionary<string, string> args)
        {
            string message = string.Format(
                                 "{0}{1}{1}",
                                 ex.Message,
                                 Environment.NewLine
                             );

            if (args.ContainsKey("sourcePath"))
            {
                message = string.Format(
                    "{0}Source: {1}{2}",
                    message,
                    args["sourcePath"],
                    Environment.NewLine
                );
            }

            if (args.ContainsKey("targetPath"))
            {
                message = string.Format(
                    "{0}Target: {1}{2}",
                    message,
                    args["targetPath"],
                    Environment.NewLine
                );
            }

            var errorDialog = new MessageDialog(
                                  null,
                                  DialogFlags.Modal,
                                  MessageType.Error,
                                  ButtonsType.Ok,
                                  message);
            
            errorDialog.Title = "Error";

            errorDialog.Run();
            errorDialog.Destroy();

            return false;
        }
    }
}

