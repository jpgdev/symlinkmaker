﻿namespace SymlinkMaker.GUI.GtkSharp
{
    public struct GtkSharpCommandConfirmationInfo
    {
        public string Message { get; set; }
        public string[] MessageArgsNames { get; set; }
        public string DialogTitle { get; set; }
        public string[] DialogTitleArgsNames { get; set; }

        /// <summary>
        /// Stores infos for the <see cref="SymlinkMaker.GUI.GtkSharp.GtkSharpCommandLoader"/> confirmation message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="messageArgsNames">Message arguments names.</param>
        /// <param name="dialogTitle">Dialog title.</param>
        /// <param name="dialogTitleArgsNames">Dialog title arguments names.</param>
        public GtkSharpCommandConfirmationInfo(
            string message, 
            string[] messageArgsNames, 
            string dialogTitle, 
            string[] dialogTitleArgsNames)
        {
            Message = message;
            MessageArgsNames = messageArgsNames;
            DialogTitle = dialogTitle;
            DialogTitleArgsNames = dialogTitleArgsNames;
        }        
    }
}

