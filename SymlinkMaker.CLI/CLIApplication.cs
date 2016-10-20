using System;
using System.Collections.Generic;
using SymlinkMaker.Core;

namespace SymlinkMaker.CLI
{
    public class CLIApplication : RunnableApplicationBase
    {
        #region Fields

        private ConsoleColor _errorColor = ConsoleColor.DarkRed;

        private readonly IConsoleHelper _consoleHelper;
        private readonly ICLICommandParser _commandParser;

        #endregion

        #region Properties

        public ConsoleColor ErrorColor
        {
            get { return _errorColor; }
            set { _errorColor = value; }
        }

        #endregion

        #region Constructors

        public CLIApplication(
            AppSettings settings, 
            IDictionary<CommandType, CommandAdapter> commands,
            IConsoleHelper consoleHelper,
            ICLICommandParser commandParser
        )
            : base(settings, commands)
        { 
            if (consoleHelper == null)
                throw new ArgumentNullException(nameof(consoleHelper));

            if (commandParser == null)
                throw new ArgumentNullException(nameof(commandParser));

            _consoleHelper = consoleHelper;
            _commandParser = commandParser;
        }

        #endregion

        #region Methods

        public override void Run(string[] args)
        {
            try
            {
                CLICommandInfo info = _commandParser.ParseArgs(args);
                RunCommandFromInfo(info);
            }
            catch (Exception e)
            {
                _consoleHelper.WriteColored(
                    string.Format(
                        "ERROR: {0}\t{1}",
                        Environment.NewLine,
                        e.Message),
                    ErrorColor,
                    null,
                    Console.Error.WriteLine);
            }
        }

        internal bool RunCommandFromInfo(CLICommandInfo info)
        {
            if (!Commands.ContainsKey(info.Type))
                throw new ArgumentException(string.Format("'{0}' is not an existing command.", info.Type));

            return Commands[info.Type].Execute(info.Arguments, info.RequiresConfirm);
        }

        #endregion

    }
}
