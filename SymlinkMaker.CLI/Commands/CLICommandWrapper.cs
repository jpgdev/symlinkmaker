using SymlinkMaker.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymlinkMaker.CLI
{
    internal class CLICommandWrapper : CommandWrapper
    {
        #region Attributes

        private ConsoleColor _confirmColor = ConsoleColor.DarkGreen;
        private ConsoleColor _doneColor = ConsoleColor.DarkGreen;
        private ConsoleColor _titleColor = ConsoleColor.Yellow;
        private ConsoleColor _errorColor = ConsoleColor.DarkRed;

        private string _title;
        private string[] _titleArgs;

        #endregion

        #region Properties

        public ConsoleColor ConfirmColor
        {
            get { return _confirmColor; }
            set { _confirmColor = value; }
        }

        public ConsoleColor DoneColor
        {
            get { return _doneColor; }
            set { _doneColor = value; }
        }

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

        public CLICommandWrapper(
            Command command,
            string title = null,
            string[] titleArgsNames = null)
            : base(command)
        {            
            _title = title;
            _titleArgs = titleArgsNames;
        }

        #region Event Handlers

        protected override void OnFinish(object sender, CommandEventArgs eventArgs)
        {
        }

        protected override void OnSuccess(object sender, CommandEventArgs eventArgs)
        {
            ConsoleHelper.WriteLineColored("DONE", DoneColor);
        }

        protected override void OnFailure(object sender, CommandEventArgs eventArgs)
        {
            ConsoleHelper.WriteLineColored("FAILED", ErrorColor);
        }

        protected override void OnException(object sender, CommandExceptionEventArgs eventArgs)
        {
            ConsoleHelper.WriteLineColored(
                string.Concat(
                    "FAILED{0}",
                    "\t{1}"
                ),
                ErrorColor,
                Environment.NewLine,
                eventArgs.Exception.Message);
        }

        #endregion

        public override bool Run(IDictionary<string, string> args, bool needConfirmation = true)
        {
            // Note : If I want to use a ternary IF (?:), I need to type cast the null... so this is cleaner
            // Note : Always need to reset it to null, since if we run it again WITHOUT the needConfirmation
            //        There should'nt be a BeforeRunFunc.
            //        This design seems dumb, need to change something.
            if (needConfirmation)
                Command.BeforeRunFunc = arguments =>
                {
                    ShowTitle(arguments);
                    return GetConfirmation(arguments);
                };
            else
                Command.BeforeRunFunc = null;
           

            return Command.Run(args);
        }

        protected void ShowTitle(IDictionary<string, string> args)
        {
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

        private  bool GetConfirmation(IDictionary<string, string> args)
        {
            ConsoleHelper.WriteColored(" (Y/n)?", _confirmColor);
            return (ConsoleHelper.ReadKey().Key != ConsoleKey.N);
        }
    }
}

