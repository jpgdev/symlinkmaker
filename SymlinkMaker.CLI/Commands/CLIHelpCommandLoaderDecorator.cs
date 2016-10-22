using System;
using SymlinkMaker.Core;
using System.Collections.Generic;

namespace SymlinkMaker.CLI
{
    public class CLIHelpCommandLoaderDecorator : ICommandsLoader
    {
        private readonly ICommandsLoader _baseCommandsLoader;
        private readonly IConsoleHelper _consoleHelper;

        private const string APP_NAME = "symlinkmaker";

        public CLIHelpCommandLoaderDecorator(
            ICommandsLoader fileCommandsLoader, 
            IConsoleHelper consoleHelper)
        {
            if (fileCommandsLoader == null)
                throw new ArgumentNullException(nameof(fileCommandsLoader));

            if (consoleHelper == null)
                throw new ArgumentNullException(nameof(consoleHelper));

            _baseCommandsLoader = fileCommandsLoader;
            _consoleHelper = consoleHelper;
        }

        public IDictionary<CommandType, ICommand> Load()
        {
            var baseCommands = _baseCommandsLoader.Load();

            // Set the help command
            baseCommands.Add(
                CommandType.ShowHelp, 
                new Command(
                    args =>
                    {
                        _consoleHelper.WriteLineColored(
                            GenerateHelpMessage(),
                            // TODO : Use global/app settings to get the color, like a Theme?
                            ConsoleColor.Yellow
                        );

                        return true;
                    }
                )
            );

            return baseCommands;
        }


        private static string GenerateHelpMessage()
        {
            // TODO : Find a better way to build this (with string.PadRight, etc..)

            // TODO : Generate this using the _baseCommandsLoader's commands
            //        Would need a title or summary? Maybe add a summary to the ICommand object?
            //        We could parse the RequiredArguments also easily enough
            return string.Format(
                string.Concat(
                    "Usage: {1} <command> <SOURCE> [<TARGET>] [OPTIONS]{0}",
                    "Available commands : {0}" +
                    "\tcopy            = Copy the <SOURCE> to the <TARGET>.{0}",
                    "\tdelete          = Delete the <SOURCE>.{0}",
                    "\tmove            = Move the <SOURCE> to the <TARGET>.{0}",
                    "\tlink            = Create a symbolic link from <SOURCE> to the <TARGET>.{0}",
                    "\tall             = Move the <SOURCE> to the <TARGET> then create a {0}" +
                    "\t                  symbolic link from the <SOURCE> to the <TARGET>.{0}",
                    "\thelp            = Shows this command.{0}" +
                    "Options:{0}",
                    "\t-c,--confirm    = requires confirmation{0}",
                    "\t-n,--no-confirm = no confirmation"
                ),
                Environment.NewLine, 
                APP_NAME);
        }
    }
}

