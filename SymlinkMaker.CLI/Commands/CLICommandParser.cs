using System;
using System.Collections.Generic;
using SymlinkMaker.Core;
using Mono.Options;

namespace SymlinkMaker.CLI
{
    public class CLICommandParser : ICLICommandParser
    {
        public CLICommandInfo ParseArgs(IEnumerable<string> arguments)
        {
            var commandInfo = new CLICommandInfo(
                                  CommandType.None,
                                  new Dictionary<string, string>(),
                                  true);

            var commandFlags = new OptionSet
            {
                {
                    "c|confirm",
                    "If we want to confirm before doing an action.",
                    (c) => commandInfo.RequiresConfirm = true

                },
                {
                    "n|no-confirm",
                    "If we want don't want to confirm before doing an action.",
                    (c) => commandInfo.RequiresConfirm = false

                }
            };

            var extraArgs = commandFlags.Parse(arguments);

            if (extraArgs.Count == 0)
                throw new ArgumentException("This cannot work without a command.");

            // TODO : Refactor the switch, Dictionary maybe?
            var commandName = extraArgs[0];
            switch (commandName.ToLower())
            {
                case "--help":
                case "help":
                    commandInfo.Type = CommandType.ShowHelp;
                    commandInfo.RequiresConfirm = false;
                    break;

                case "copy":
                    commandInfo.Type = CommandType.Copy;

                    if (extraArgs.Count < 3)
                        throw new ArgumentException("This command requires the source and target.");

                    commandInfo.Arguments["sourcePath"] = extraArgs[1];
                    commandInfo.Arguments["targetPath"] = extraArgs[2];

                    break;
                case "delete":
                    commandInfo.Type = CommandType.Delete;

                    if (extraArgs.Count < 2)
                        throw new ArgumentException("This command requires the source.");

                    commandInfo.Arguments["sourcePath"] = extraArgs[1];

                    break;

                case "move":
                    commandInfo.Type = CommandType.Move;

                    if (extraArgs.Count < 3)
                        throw new ArgumentException("This command requires the source and target.");

                    commandInfo.Arguments["sourcePath"] = extraArgs[1];
                    commandInfo.Arguments["targetPath"] = extraArgs[2];

                    break;
                case "link":
                    commandInfo.Type = CommandType.CreateSymLink;

                    if (extraArgs.Count < 3)
                        throw new ArgumentException("This command requires the source and target.");

                    commandInfo.Arguments["sourcePath"] = extraArgs[1];
                    commandInfo.Arguments["targetPath"] = extraArgs[2];

                    break;

                case "all":
                    commandInfo.Type = CommandType.All;

                    if (extraArgs.Count < 3)
                        throw new ArgumentException("This command requires the source and target.");

                    commandInfo.Arguments["sourcePath"] = extraArgs[1];
                    commandInfo.Arguments["targetPath"] = extraArgs[2];

                    break;

                default:
                    throw new ArgumentException(string.Format("'{0}' is not an existing command.", commandName));
            }

            return commandInfo;
        }
    }
}

