using System;

namespace SymlinkMaker.GUI.GTKSharp
{
    public struct GtkSharpCommandConfirmationInfo
    {
        public string Message { get; set; }
        public string[] MessageArgs { get; set; }
        public string DialogTitle { get; set; }
        public string[] DialogTitleArgs { get; set; }

        public GtkSharpCommandConfirmationInfo(
            string message, 
            string[] messageArgs, 
            string dialogTitle, 
            string[] dialogTitleArgs)
        {
            Message = message;
            MessageArgs = messageArgs;
            DialogTitle = dialogTitle;
            DialogTitleArgs = dialogTitleArgs;
        }        
    }
}

