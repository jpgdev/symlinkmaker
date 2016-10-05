using System;
using System.Collections.Generic;
using System.Linq;
using SymlinkMaker.Core;

namespace SymlinkMaker.CLI
{
    internal class CLICommand : ConfirmableCommand
    {
        #region Attributes

        public const ConsoleColor CONFIRM_COLOR = ConsoleColor.DarkGreen;

        private ConsoleColor _titleColor = ConsoleColor.Yellow;
        private ConsoleColor _errorColor = ConsoleColor.DarkRed;

//        private Action<IDictionary<string, string>> _titleFunc;
        private string _title;
        private string[] _titleArgs;

        #endregion

        #region Properties

        public ConsoleColor TitleColor
        {
            get { return _titleColor; }
            set { _titleColor = value; }
        }

        public ConsoleColor ErrorColor
        {
            get { return _errorColor; }
            set { _errorColor = value; }
        }

        #endregion

        public CLICommand(Func<IDictionary<string, string>, bool> commandFunc, string title, string[] titleArgs = null, string[] requiredArgs = null)
            : base(commandFunc, GetConfirmation, requiredArgs)
        {                         
            _title = title;
            _titleArgs = titleArgs;            
        }

//        public CLICommand(Func<IDictionary<string, string>, bool> commandFunc, Action<IDictionary<string, string>> titleFunc, string[] requiredArgs = null)
//            : base(commandFunc, GetConfirmation, requiredArgs)
//        {
//            if (titleFunc == null)
//                throw new ArgumentNullException("titleFunc");
//
//            _titleFunc = titleFunc;
//        }

        public override bool Run(IDictionary<string, string> args, bool needConfirmation = true)
        {
            try
            {
                             
                ValidateRequiredArguments(args);
                ShowTitle(args);
            }
            catch (Exception e)
            {
                return HandleException(e, args);
            }

            return base.Run(args, needConfirmation);
        }

        protected void ShowTitle(IDictionary<string, string> args){
            if (string.IsNullOrEmpty(_title))
                return;
            
            string message = _title;
            if (_titleArgs != null)
            {
                string[] argValues = _titleArgs.Select((argName) => args[argName]).ToArray();
                message = string.Format(message, argValues);
            }

            ConsoleHelper.WriteColored(message, TitleColor);
        }

        public override bool HandleException(Exception ex, IDictionary<string, string> args)
        {
            // TODO : Manage this better?
            ConsoleHelper.WriteLineColored(
                string.Concat(
                    "FAILED{0}", 
                    "\t{1}"
                ),
                ErrorColor,
                Environment.NewLine,
                ex.Message);
            
            return false;
        }

        private static bool GetConfirmation(IDictionary<string, string> args)
        {
            ConsoleHelper.WriteColored(" (Y/n)?", CONFIRM_COLOR);
            return (ConsoleHelper.ReadKey().Key != ConsoleKey.N);
        }
    }
}

