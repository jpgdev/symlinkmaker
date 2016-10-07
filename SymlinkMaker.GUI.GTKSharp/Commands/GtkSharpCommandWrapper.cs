using Gtk;
using SymlinkMaker.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymlinkMaker.GUI.GTKSharp
{
    public class GtkSharpCommandWrapper : CommandWrapper
    {

        private GtkSharpCommandConfirmationInfo _confirmInfo;

        public GtkSharpCommandWrapper(
            Command command)
            : this(command, new GtkSharpCommandConfirmationInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymlinkMaker.GUI.GTKSharp.GtkSharpCommandWrapper"/> class.
        /// </summary>
        /// <param name="command">Command object to wrap.</param>
        /// <param name="confirmInfo">Confirmation dialog informations.</param>
        /// 
        /// <remarks>
        /// The <paramref name="confirmInfo"/> .Message and .DialogTitle both will be parsed with <see cref="String.Format"/> 
        /// using on the corresponding arguments with the provided names 
        /// (<see cref="SymlinkMaker.GUI.GTKSharp.GTKSharpCommandConfirmationInfo"/> MessageArgsNames & DialogTitleArgsNames)
        /// </remarks>
        public GtkSharpCommandWrapper(
            Command command,
            GtkSharpCommandConfirmationInfo confirmInfo)
            : base(command)
        {
            _confirmInfo = confirmInfo;
        }

        public override bool Run(IDictionary<string, string> args, bool needConfirmation = true)
        {          
            // Note : If I want to use a ternary IF (?:), I need to type cast the null... so this is cleaner
            // Note : Always need to reset it to null, since if we run it again WITHOUT the needConfirmation
            //        There should'nt be a BeforeRunFunc.
            //        This design seems dumb, need to change something.
            if (needConfirmation)
                Command.BeforeRunFunc = (arguments) => ShowConfirmationDialog(_confirmInfo, arguments);
            else
                Command.BeforeRunFunc = null;

            return Command.Run(args);
        }

        #region Event handlers

        protected override void OnFinish(object sender, CommandEventArgs eventArgs)
        {
        }

        protected override void OnSuccess(object sender, CommandEventArgs eventArgs)
        {
            // TODO : Change the message to something with the Arguments? More information?
            ShowInfoDialog("Operation successful!", "Success!");
        }

        protected override void OnFailure(object sender, CommandEventArgs eventArgs)
        {            
            ShowFailureDialog("Operation failed.", eventArgs.Arguments);
        }

        protected override void OnException(object sender, CommandExceptionEventArgs eventArgs)
        {
            string message = string.Format(
                                 "<b>Error : {0}</b>{1}", 
                                 eventArgs.Exception.Message, 
                                 Environment.NewLine
                             );

            ShowFailureDialog(message, eventArgs.Arguments);
        }

        #endregion

        #region Show dialog methods

        protected static void ShowInfoDialog(string message, string title = "Info")
        {
            
            var doneDialog = new MessageDialog(
                                 null,
                                 DialogFlags.Modal,
                                 MessageType.Info,
                                 ButtonsType.Ok,
                                 message);

            if (!string.IsNullOrEmpty(title))
                doneDialog.Title = title;
            
            doneDialog.IconName = "gtk-dialog-info";

            doneDialog.Run();
            doneDialog.Destroy();
        }


        protected static void ShowFailureDialog(
            string message, 
            IDictionary<string, string> arguments = null)
        {

            if (arguments != null)
            {
                if (arguments.ContainsKey("sourcePath"))
                {
                    message = string.Format(
                        "{0}{2}<i>Source:</i> {1}{2}",
                        message,
                        arguments["sourcePath"],
                        Environment.NewLine
                    );
                }

                if (arguments.ContainsKey("targetPath"))
                {
                    message = string.Format(
                        "{0}<i>Target</i>: {1}{2}",
                        message,
                        arguments["targetPath"],
                        Environment.NewLine
                    );
                }
            }

            var failureDialog = new MessageDialog(
                                    null,
                                    DialogFlags.Modal,
                                    MessageType.Error,
                                    ButtonsType.Ok,
                                    message);

            failureDialog.Title = "Error";
            failureDialog.IconName = "gtk-dialog-error";

            failureDialog.Run();
            failureDialog.Destroy();
        }

        protected static bool ShowConfirmationDialog(
            GtkSharpCommandConfirmationInfo confirmInfo, 
            IDictionary<string, string> args)
        {
            string content = confirmInfo.Message;
            if (confirmInfo.MessageArgsNames != null)
            {
                string[] argValues = confirmInfo.MessageArgsNames.Select((argName) => args[argName]).ToArray();
                content = string.Format(content, argValues);
            }

            string dialogTitle = confirmInfo.DialogTitle;
            if (confirmInfo.DialogTitleArgsNames != null)
            {
                string[] argValues = confirmInfo.DialogTitleArgsNames.Select((argName) => args[argName]).ToArray();
                dialogTitle = string.Format(dialogTitle, argValues);
            }

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

        #endregion
    }
}
