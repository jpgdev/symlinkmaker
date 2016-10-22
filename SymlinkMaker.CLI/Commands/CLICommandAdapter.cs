using System;
using System.Collections.Generic;
using SymlinkMaker.Core;

namespace SymlinkMaker.CLI
{
    public class CLICommandAdapter : CommandAdapter
    {
        #region Attributes

        private readonly IConsoleHelper _consoleHelper;
        private string _title;
        private string[] _titleArgs;

        #endregion

        #region Properties

        public ConsoleColor ConfirmColor { get; set; } = ConsoleColor.DarkGreen;

        public ConsoleColor DoneColor { get; set; }  =  ConsoleColor.DarkGreen;

        public ConsoleColor TitleColor { get; set; } = ConsoleColor.Yellow;

        public ConsoleColor ErrorColor { get; set; }  = ConsoleColor.DarkRed;

        #endregion

        public CLICommandAdapter(
            ICommand command,
            IConsoleHelper consoleHelper,
            string title = null,
            string[] titleArgsNames = null)
            : base(command)
        {            

            if (consoleHelper == null)
                throw new ArgumentNullException(nameof(consoleHelper));
            
            _consoleHelper = consoleHelper;
            _title = title;
            _titleArgs = titleArgsNames;

            // Add delegate to show the command title before the command is executed
            Command.RegisterPreExecutionValidation(ShowTitle);
        }

        public override void Dispose()
        {
            Command.UnregisterPreExecutionValidation(ShowTitle);

            base.Dispose();
        }

        #region Event Handlers

        protected override void OnFinish(ICommand sender, CommandEventArgs eventArgs)
        {
        }

        protected override void OnSuccess(ICommand sender, CommandEventArgs eventArgs)
        {
            _consoleHelper.WriteLineColored("DONE", DoneColor);
        }

        protected override void OnFailure(ICommand sender, CommandEventArgs eventArgs)
        {
            _consoleHelper.WriteLineColored("FAILED", ErrorColor);
        }

        protected override void OnException(ICommand sender, CommandExceptionEventArgs eventArgs)
        {
            _consoleHelper.WriteColored(
                string.Format(
                    "FAILED{0}\t{1}",
                    Environment.NewLine,
                    eventArgs.Exception.Message
                ),
                ErrorColor,
                null,
                Console.Error.WriteLine);
        }

        #endregion

        protected bool ShowTitle(IDictionary<string, string> args)
        {
            if (string.IsNullOrEmpty(_title))
                return true;

            string message = _title;           
            if (_titleArgs != null)
            {
                string[] argValues = GetArgsValuesFromArgsName(args, _titleArgs);
                message = string.Format(message, argValues);
            }

            _consoleHelper.WriteColored(message, TitleColor);

            return true;
        }

        protected override bool ConfirmationHandler(
            IDictionary<string, string> arguments)
        {
            _consoleHelper.WriteColored(" (Y/n)?", ConfirmColor);
            return (_consoleHelper.ReadKey().Key != ConsoleKey.N);
        }
    }
}

