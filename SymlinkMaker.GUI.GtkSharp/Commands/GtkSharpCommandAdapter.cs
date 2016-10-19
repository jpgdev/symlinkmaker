using SymlinkMaker.Core;
using System;
using System.Collections.Generic;

namespace SymlinkMaker.GUI.GtkSharp
{
    public class GtkSharpCommandAdapter : CommandAdapter
    {
        private GtkSharpCommandConfirmationInfo _confirmInfo;
        private readonly IDialogHelper _dialogHelper;

        public GtkSharpCommandAdapter(
            ICommand command,
            IDialogHelper dialogHelper)
            : this(command, dialogHelper, new GtkSharpCommandConfirmationInfo())
        {
        }

        public GtkSharpCommandAdapter(
            ICommand command,
            IDialogHelper dialogHelper,
            GtkSharpCommandConfirmationInfo confirmInfo)
            : base(command)
        {
            if (dialogHelper == null)
                throw new ArgumentNullException(nameof(dialogHelper));
            
            _confirmInfo = confirmInfo;
            _dialogHelper = dialogHelper;
        }

        #region CommandAdapter overrides

        protected override bool ConfirmationHandler(IDictionary<string, string> arguments)
        {
            return ShowConfirmationDialog(_confirmInfo, arguments);
        }

        protected override void OnFinish(ICommand sender, CommandEventArgs eventArgs)
        {
        }

        protected override void OnSuccess(ICommand sender, CommandEventArgs eventArgs)
        {
            // TODO : Change the message to something with the Arguments? More information?
            _dialogHelper.ShowDialog(DialogType.Information, "Operation successful.", "Success.");
        }

        protected override void OnFailure(ICommand sender, CommandEventArgs eventArgs)
        {            
            ShowFailureDialog("Operation failed.", eventArgs.Arguments);
        }

        protected override void OnException(ICommand sender, CommandExceptionEventArgs eventArgs)
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

        protected void ShowFailureDialog(
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
                        "{0}<i>Target:</i> {1}{2}",
                        message,
                        arguments["targetPath"],
                        Environment.NewLine
                    );
                }
            }

            _dialogHelper.ShowDialog(DialogType.Error, message);
        }

        protected bool ShowConfirmationDialog(
            GtkSharpCommandConfirmationInfo confirmInfo, 
            IDictionary<string, string> args)
        {
            string content = confirmInfo.Message;
            if (confirmInfo.MessageArgsNames != null)
            {
                string[] argValues = GetArgsValuesFromArgsName(args, confirmInfo.MessageArgsNames);
                content = string.Format(content, argValues);
            }

            string dialogTitle = confirmInfo.DialogTitle;
            if (confirmInfo.DialogTitleArgsNames != null)
            {
                string[] argValues = GetArgsValuesFromArgsName(args, confirmInfo.MessageArgsNames);
                dialogTitle = string.Format(dialogTitle, argValues);
            }

            return _dialogHelper.ShowDialog(
                DialogType.Confirmation, 
                content, 
                dialogTitle);
        }

        #endregion
    }
}
