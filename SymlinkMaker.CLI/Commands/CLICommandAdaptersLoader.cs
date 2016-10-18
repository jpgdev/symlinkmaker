using System;
using SymlinkMaker.Core;
using System.Collections.Generic;

namespace SymlinkMaker.CLI
{
    public class CLICommandAdaptersLoader : ICommandAdaptersLoader
    {
        private readonly IDictionary<CommandType, ICommand> _baseCommands;
        private readonly IConsoleHelper _consoleHelper;

        public CLICommandAdaptersLoader(
            IDictionary<CommandType, ICommand> baseCommands,
            IConsoleHelper consoleHelper
        )
        {
            if (baseCommands == null)
                throw new ArgumentNullException(nameof(baseCommands));

            _baseCommands = baseCommands;
            _consoleHelper = consoleHelper;
        }

        public IDictionary<CommandType, CommandAdapter> Load()
        {
            return new Dictionary<CommandType, CommandAdapter>()
            {
                {
                    CommandType.Copy,
                    new CLICommandAdapter(
                        _baseCommands[CommandType.Copy],
                        _consoleHelper,
                        "Copy {0} to {1}.",
                        new[] { "sourcePath", "targetPath" }
                    )
                },
                {
                    CommandType.Move,
                    new CLICommandAdapter(
                        _baseCommands[CommandType.Move],
                        _consoleHelper,
                        "Move {0} to {1}.",
                        new[] { "sourcePath", "targetPath" }
                    )
                },
                {
                    CommandType.Delete,
                    new CLICommandAdapter(
                        _baseCommands[CommandType.Delete],
                        _consoleHelper,
                        "Delete {0}.",
                        new[] { "sourcePath" }
                    )
                },
                {
                    CommandType.CreateSymLink,
                    new CLICommandAdapter(
                        _baseCommands[CommandType.CreateSymLink],
                        _consoleHelper,
                        "Create symbolic link from {0} to {1}.",
                        new[] { "sourcePath", "targetPath" }
                    )
                },
                {
                    CommandType.All,
                    new CLICommandAdapter(
                        _baseCommands[CommandType.All],
                        _consoleHelper,
                        "Copy '{0}' to '{1}' then create a link from '{0}' to '{1}'.",
                        new[] { "sourcePath", "targetPath" }
                    )
                },
                {
                    CommandType.ShowHelp,
                    new CLICommandAdapter(
                        _baseCommands[CommandType.ShowHelp],
                        _consoleHelper
                    )
                }
            };
        }
    }     
}

