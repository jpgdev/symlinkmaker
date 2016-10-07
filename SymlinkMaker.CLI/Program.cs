using Mono.Options;
using System;
using SymlinkMaker.Core;
using System.Collections.Generic;
//using System.Globalization;
//using System.Resources;
using System.ComponentModel;

namespace SymlinkMaker.CLI
{
    class MainClass
    {
        private const ConsoleColor ERROR_COLOR = ConsoleColor.DarkRed;
        private const string APP_NAME = "symlinkmaker";

        private static AppSettings _settings;
        private static Dictionary<CommandType, CommandWrapper> _commands;

        public static void Main(string[] args)
        {
            //            AppResources.Culture = CultureInfo.CurrentCulture;
            //            string str = AppResources.ErrorNoArguments;
            //            Catalog.Init("SymlinkMaker.CLI", "./locale");

            try
            {
                InitializeSettings();
                InitializeCommands(_settings.FileOperations);

                CLICommandInfo info = GetCommandInfoFromArgs(args);
                
                RunCommand(info);
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLineColored(
                    "ERROR: {0}\t{1}",
                    ERROR_COLOR,
                    Environment.NewLine,
                    e.Message);
            }
        }

        public static void InitializeSettings()
        {
            _settings = new AppSettings();

            _settings.PropertyChanged += AppSettings_PropertyChanged;

            // TODO : Load these from a (JSON, YAML?) config file (most of these)

            _settings.FileOperations = FileOperationsFactory.GetFileOperationsObject();
            _settings.RequiresConfirmation = true;

        }

        public static void InitializeCommands(IFileOperations fileOperations)
        {
            FileCommands.InitializeCommands(fileOperations);

            _commands = new Dictionary<CommandType, CommandWrapper>()
            {
                {
                    CommandType.Copy,
                    new CLICommandWrapper(
                        FileCommands.Commands[CommandType.Copy],
                        "Copy {0} to {1}.",
                        new[] { "sourcePath", "targetPath" }
                    )
                },
                {
                    CommandType.Move,
                    new CLICommandWrapper(
                        FileCommands.Commands[CommandType.Move],
                        "Move {0} to {1}.",
                        new[] { "sourcePath", "targetPath" }
                    )
                },
                {
                    CommandType.Delete,
                    new CLICommandWrapper(
                        FileCommands.Commands[CommandType.Delete],
                        "Delete {0}.",
                        new[] { "sourcePath" }
                    )
                },
                {
                    CommandType.CreateSymLink,
                    new CLICommandWrapper(
                        FileCommands.Commands[CommandType.CreateSymLink],
                        "Create symbolic link from {0} to {1}.",
                        new[] { "sourcePath", "targetPath" }
                    )
                },
                {
                    CommandType.All,
                    new CLICommandWrapper(
                        FileCommands.Commands[CommandType.All],
                        "Launch the whole process '{0}' => '{1}'? ",
                        // "This will copy '{0}' to '{1}' then replace '{0}' with a link to '{1}'.",
                        new[] { "sourcePath", "targetPath" }
                    )
                }
            };
        }

        public static CLICommandInfo GetCommandInfoFromArgs(IEnumerable<string> args)
        {
            bool shouldShowHelp = false;

            var commandInfo = new CLICommandInfo(
                                  CommandType.None,
                                  new Dictionary<string, string>(),
                                  _settings.RequiresConfirmation);

            var _commandFlags = new OptionSet()
            {
//                { 
//                    "cp|copy=", 
//                    "Copy a {SOURCE_PATH} directory to a {TARGET_PATH}.", 
//                    (source, target) =>
//                    { 
//                        currentCommand = CommandType.Copy; 
//
//                        argsMap["sourcePath"] = source;
//                        argsMap["targetPath"] = target;
//                    }
//                },
//                { 
//                    "l|link=", 
//                    "Create a symbolic link from a {SOURCE} to a {TARGET} directory.", 
//                    (source, target) =>
//                    { 
//                        currentCommand = CommandType.CreateSymLink; 
//
//                        argsMap["sourcePath"] = source;
//                        argsMap["targetPath"] = target;
//                    }
//                },
//                { 
//                    "d|delete=", 
//                    "Delete a {TARGET} directory.", 
//                    (target) =>
//                    { 
//                        currentCommand = CommandType.CreateSymLink; 
//
//                        argsMap["targetPath"] = target;
//                    }
//                },
//                { 
//                    "a|all=", 
//                    "Copy a {SOURCE} directory to a {TARGET} location, then delete the {SOURCE} directory & create a symbolic link from the {SOURCE} location to the {TARGET}.", 
//                    (source, target) =>
//                    { 
//                        currentCommand = CommandType.All; 
//
//                        argsMap["sourcePath"] = source;
//                        argsMap["targetPath"] = target;
//                    }
//                },
                {
                    "c|confirm",
                    "If we want to confirm before doing an action.",
                    (c) => commandInfo.RequireConfirm = true

                },
                {
                    "n|no-confirm",
                    "If we want don't want to confirm before doing an action.",
                    (c) => commandInfo.RequireConfirm = false

                },
                { "h|help", "Show this message.", h => shouldShowHelp = (h != null) }
            };

            var extraArgs = _commandFlags.Parse(args);

            if (shouldShowHelp)
            {
                // TODO : Add more description for the available commands
                ConsoleHelper.WriteLineColored(
                    string.Concat(
                        "Usage: {1} <command> <SOURCE> [<TARGET>] [OPTIONS]{0}",
                        "Available commands : copy, delete, move, link, all.{0}",
                        "Options: "
                    ),
                    ConsoleColor.Yellow,
                    Environment.NewLine,
                    APP_NAME
                );
                _commandFlags.WriteOptionDescriptions(Console.Out);

                return commandInfo;
            }

            if (extraArgs.Count == 0)
                throw new ArgumentException("This cannot work without a command.");

            var commandName = extraArgs[0];
            switch (commandName.ToLower())
            {
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

        public static bool RunCommand(CLICommandInfo info)
        {            
//            if (info.Type == CommandType.None)
//                return false;
//
            if (!_commands.ContainsKey(info.Type))
                throw new ArgumentException(string.Format("'{0}' is not an existing command.", info.Type.ToString()));

            return _commands[info.Type].Run(info.Arguments, info.RequireConfirm);
        }

        private static void AppSettings_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
//            AppSettings settings = (AppSettings)sender;
//
//            switch (e.PropertyName)
//            {
//                case "RequiresConfirmation":
//                    break;
//
//                case "SourcePath":
//                    break;
//
//                case "TargetPath":
//                    break;
//
//                case "FileOperations":
//                    break;
//            }
        }
    }
}
