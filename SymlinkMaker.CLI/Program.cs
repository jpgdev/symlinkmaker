using Mono.Unix;
using Mono.Options;
using System;

//using System.Globalization;
//using System.Resources;
using SymlinkMaker.Core;
using System.Collections.Generic;

namespace SymlinkMaker.CLI
{
    class MainClass
    {
        // Console colors
        private const ConsoleColor TITLE_COLOR = ConsoleColor.Yellow;
        private const ConsoleColor COMMAND_COLOR = ConsoleColor.Cyan;
        private const ConsoleColor DONE_COLOR = ConsoleColor.DarkGreen;
        private const ConsoleColor ERROR_COLOR = ConsoleColor.DarkRed;

        private const string APP_NAME = "symlinkmaker";

        private static Dictionary<CommandType, CLICommand> _commands;

        public static void Main(string[] args)
        {       
//            AppResources.Culture = CultureInfo.CurrentCulture;
//            string str = AppResources.ErrorNoArguments;
//            Catalog.Init("SymlinkMaker.CLI", "./locale");

            try
            {
                InitializeCommands(FileOperationsFactory.GetFileOperationsObject());

                CommandInfo info = GetCommandInfoFromArgs(args);
                if (!info.Type.HasValue)
                    return;                

                CommandType type = info.Type.Value;
                if (!_commands.ContainsKey(type))
                    return;
                
                _commands[type].Run(info.Arguments, info.RequireConfirm);
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLineColored(e.Message, ERROR_COLOR);
            }
        }

        public static void InitializeCommands(IFileOperations fileOperations)
        {
            _commands = new Dictionary<CommandType, CLICommand>()
            {
                {
                    CommandType.Copy, 
                    new CLICommand(
                        (args) =>
                        {
                            fileOperations.CopyDirectory(
                                args["sourcePath"],
                                args["targetPath"],
                                true);
                            

                            ConsoleHelper.WriteLineColored("DONE", DONE_COLOR);
                            
                            return true;
                        }, 
                        "Copy {0} to {1}.",
                        new string[] { "sourcePath", "targetPath" },
                        new string[] { "sourcePath", "targetPath" }
                    )            
                },
                {
                    CommandType.Delete,
                    new CLICommand(
                        (args) =>
                        {
                            fileOperations.DeleteDirectory(
                                args["targetPath"],
                                true);
                            
                            ConsoleHelper.WriteLineColored("DONE", DONE_COLOR);

                            return true;
                        }, 
                        "Delete {0}.",
                        new string[] { "targetPath" },
                        new string[] { "targetPath" }
                    )
                },
                {
                    CommandType.CreateSymLink, 
                    new CLICommand(
                        (args) =>
                        {
                            fileOperations.CreateSymbolicLink(
                                args["sourcePath"],
                                args["targetPath"]);

                            ConsoleHelper.WriteLineColored("DONE", DONE_COLOR);

                            return true;
                        }, 
                        "Create symbolic link from {0} to {1}.",
                        new string[] { "sourcePath", "targetPath" },
                        new string[] { "sourcePath", "targetPath" }
                    )            
                },
                {
                    CommandType.All, 
                    new CLICommand(
                        (args) =>
                        {
                            CommandType[] commands = new CommandType[]{
                                CommandType.Copy, 
                                CommandType.Delete,
                                CommandType.CreateSymLink
                            };

                            foreach(CommandType cmd in commands)
                            {
                                if(!_commands[cmd].Run(args, true)) 
                                    return false;
                            }

                            ConsoleHelper.WriteLineColored("DONE", DONE_COLOR);

                            return true;
                        }, 

//                        "This will copy '{0}' to '{1}' then replace '{0}' with a link to '{1}'.",
                        "Launch the whole process '{0}' => '{1}'? ",
                        new string[] { "sourcePath", "targetPath" },
                        new string[] { "sourcePath", "targetPath" }
                    )            
                },

            };
        }

        // BUG : The currentCommand can be set multiple times... if the user enter multiple options
        public static CommandInfo GetCommandInfoFromArgs(string[] args)
        {
            bool shouldShowHelp = false;

            CommandInfo commandInfo = new CommandInfo(
                null, 
                new Dictionary<string, string>(), 
                true);
            
            OptionSet _commandFlags = new OptionSet()
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
                    "If we want to confirm before doing an action. (DEFAULT)", 
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
                ConsoleHelper.WriteLineColored(
                    string.Concat(
                        "Usage: {1} <command> <SOURCE> [<TARGET>] [OPTIONS]{0}",
                        "Available commands : copy, delete, link, all.{0}",
                        "Options: "
                    ),
                    ConsoleColor.Yellow,
                    Environment.NewLine,
                    APP_NAME
                );
                _commandFlags.WriteOptionDescriptions(Console.Out);

                return commandInfo;
            }


            if(extraArgs.Count == 0)
                throw new ArgumentException("This cannot work without a command.");
            
            var commandName = extraArgs[0];
            switch (commandName.ToLower())
            {
//                case "cp":
                case "copy":
                    commandInfo.Type = CommandType.Copy;

                    if (extraArgs.Count < 3)
                        throw new ArgumentException("This command requires the source and target.");

                    commandInfo.Arguments["sourcePath"] = extraArgs[1];
                    commandInfo.Arguments["targetPath"] = extraArgs[2];

                    break;
//                case "rm":
                case "remove":
                    commandInfo.Type = CommandType.Delete;

                    if (extraArgs.Count < 2)
                        throw new ArgumentException("This command requires the target.");

                    commandInfo.Arguments["sourcePath"] = extraArgs[1];
                    break;

//                case "ln":
                case "link":
//                case "symlink":
                    commandInfo.Type = CommandType.CreateSymLink;

                    if (extraArgs.Count < 3)
                        throw new ArgumentException("This command requires the source and target.");

                    commandInfo.Arguments["sourcePath"] = extraArgs[1];
                    commandInfo.Arguments["targetPath"] = extraArgs[2];

                    break;

//                case "a":
                case "all":
                    commandInfo.Type = CommandType.All;

                    if (extraArgs.Count < 3)
                        throw new ArgumentException("This command requires the source and target.");

                    commandInfo.Arguments["sourcePath"] = extraArgs[1];
                    commandInfo.Arguments["targetPath"] = extraArgs[2];

                    break;

                default:
                    throw new NotImplementedException(string.Format("'{0}' is not an implemented command.", commandName));
            }

            return commandInfo;
        }
      
               
        //        private static bool RunCommandFunc(
        //            string commandTitle,
        //            Action commandFunc,
        //            bool requireConfirm = true)
        //        {
        //
        //            ConsoleHelper.WriteColored(commandTitle, TITLE_COLOR);
        //
        //            if (requireConfirm && !GetConfirmation())
        //                return false;
        //
        //            Console.WriteLine(); // End the line here
        //
        //            commandFunc();
        //
        //            return true;
        //        }

//
//        private static bool RunCommand(
//            CommandType commandType,
//            IDictionary<string, string> args,
//            IFileOperations fileOperations,
//            bool requireConfirm = true
//        )
//        {   
//            return RunCommand(
//                new CommandInfo(commandType, args, requireConfirm),
//                fileOperations);
//        }

//        private static bool RunCommand(CommandInfo commandInfo, IFileOperations fileOperations)
//        {
//
//            CLICommand cmd;
//            switch (commandInfo.Type)
//            {
//                case CommandType.Copy:
//                    cmd = new CLICommand(
//                        (args) =>
//                        {
//                            fileOperations.CopyDirectory(
//                                args["sourcePath"],
//                                args["targetPath"],
//                                true);
//
//                            return true;
//                        }, 
//                        "Copy {0} to {1}.",
//                        new string[] { "sourcePath", "targetPath" },
//                        new string[] { "sourcePath", "targetPath" }
//                    );
//                    break;
//                
//                case CommandType.Delete:
//                    cmd = new CLICommand(
//                        (args) =>
//                        {
//                            fileOperations.DeleteDirectory(
//                                args["targetPath"],
//                                true);
//
//                            return true;
//                        }, 
//                        "Delete {0}.",
//                        new string[] { "targetPath" },
//                        new string[] { "targetPath" }
//                    );
//                    break;
//
//
//                default  :
//                    throw new NotImplementedException("This command is not yet implemented");
//            }
//
//            return cmd.Run(commandInfo.Arguments, commandInfo.RequireConfirm);


//            var commands = new Dictionary<CommandType, CLICommand>();
//
//            commands.Add(
//                CommandType.Copy, 
//                new CLICommand(
//                    (args) =>
//                    {
//                        string source = args["sourcePath"];
//                        string target = args["targetPath"];
//
//                        fileOperations.CopyDirectory(
//                            source,
//                            target,
//                            true);
//
//                        return true;
//                    }, 
//                    (args) =>
//                    {
//                        ConsoleHelper.WriteColored(
//                            "Copy {0} to {1}.",
//                            TITLE_COLOR,
//                            args["sourcePath"], 
//                            args["targetPath"]
//                        );
//                    }
//                )
//            );

//            if (!commandInfo.Type.HasValue)
//            {
//                //  TODO
//                throw new Exception();
//            }
//
//            _commands[commandInfo.Type.Value].Run(commandInfo.Arguments, true);



//            var cmd = (string[] args) => 
//                {
//                    string source = args[0];
//                    string target = args[1];
//
//                    fileOperations.CopyDirectory(
//                        source,
//                        target,
//                        true);
//                };

//            cmd.Run(new string[] { "/asdfsa/asfd", "/asdf" });


//            var commands = new Dictionary<CommandType, Func<string[], bool>>();
//
//            commands.Add(CommandType.Copy, (string[] args) => 
//                {
//                    string source = args[0];
//                    string target = args[1];
//
//                    fileOperations.CopyDirectory(
//                        source,
//                        target,
//                        true);
//
//                    return true;
//                });
                

            // TODO : Clean this up
//            try
//            {
//                switch (commandInfo.Type)
//                {
//                    case CommandType.Copy:
//                        return RunCommandFunc(
//                            string.Format("Copy {0} to {1}.", 
//                                commandInfo.Arguments["sourcePath"], 
//                                commandInfo.Arguments["targetPath"]
//                            ),
//                            () =>
//                            {
//                                fileOperations.CopyDirectory(
//                                    commandInfo.Arguments["sourcePath"], 
//                                    commandInfo.Arguments["targetPath"], 
//                                    true);
//                            }, 
//                            commandInfo.RequireConfirm);
//
//                    case CommandType.Delete:
//                        return RunCommandFunc(
//                            string.Format("Delete {0}.", 
//                                commandInfo.Arguments["targetPath"]
//                            ),
//                            () =>
//                            {
//                                fileOperations.DeleteDirectory(commandInfo.Arguments["targetPath"], true);
//                            }, 
//                            commandInfo.RequireConfirm);
//
//                    case CommandType.CreateSymLink:
//
//                        return RunCommandFunc(
//                            string.Format("Create symbolic link from {0} to {1}.", 
//                                commandInfo.Arguments["sourcePath"], 
//                                commandInfo.Arguments["targetPath"]
//                            ),
//                            () =>
//                            {
//                                fileOperations.CreateSymbolicLink(
//                                    commandInfo.Arguments["sourcePath"], 
//                                    commandInfo.Arguments["targetPath"]);
//                            }, 
//                            commandInfo.RequireConfirm);
//
//                    case CommandType.All:
//
//                        return RunCommandFunc(
//                            string.Format("Replace {0} with a symlink to the copy at {1}.",
//                                commandInfo.Arguments["sourcePath"], 
//                                commandInfo.Arguments["targetPath"]
//                            ),
//                            () =>
//                            {
//                                if (!RunCommand(
//                                        new CommandInfo(
//                                            CommandType.Copy, 
//                                            commandInfo.Arguments, 
//                                            commandInfo.RequireConfirm),
//                                        FileOperationsFactory.GetFileOperationsObject()))
//                                {
//                                    return;
//                                }
//
//                                if (!RunCommand(
//                                        new CommandInfo(
//                                            CommandType.Delete, 
//                                            new Dictionary<string, string>()
//                                        { 
//                                            { "targetPath",  commandInfo.Arguments["sourcePath"] }
//                                        }, 
//                                            commandInfo.RequireConfirm),                           
//                                        FileOperationsFactory.GetFileOperationsObject()))
//                                {
//                                    return;
//                                }
//
//                                if (!RunCommand(
//                                        new CommandInfo(
//                                            CommandType.CreateSymLink, 
//                                            commandInfo.Arguments, 
//                                            commandInfo.RequireConfirm),                           
//                                        FileOperationsFactory.GetFileOperationsObject()))
//                                {
//                                    return;
//                                }
//
//                            }
//
//
//
//                        );
//
//                    default:
//                        throw new ArgumentException(
//                            string.Concat(
//                                "{0} is not a valid command type",
//                                commandInfo.Type.ToString()));
//                    
//                }
//            }
//            catch (Exception ex)
//            {
//                // TODO : Manage this better?
//                ConsoleHelper.WriteLineColored(
//                    string.Concat(
//                        "FAILED{0}", 
//                        "\t{1}"
//                    ),
//                    ERROR_COLOR,
//                    Environment.NewLine,
//                    ex.Message);
//
//                return false;
//            }

//            try
//            {
//                switch (commandInfo.Type)
//                {
//                    case CommandType.Copy:
//
//                        ConsoleHelper.WriteColored("Copy {0} to {1}.", TITLE_COLOR,
//                            commandInfo.Arguments["sourcePath"], 
//                            commandInfo.Arguments["targetPath"]
//                        );
//
//                        if (commandInfo.RequireConfirm && !GetConfirmation())
//                            return false;
//
//                        Console.WriteLine(); // End the line here
//
//                        fileOperations.CopyDirectory(
//                            commandInfo.Arguments["sourcePath"], 
//                            commandInfo.Arguments["targetPath"], 
//                            true);
//
//                        break;
//
//                    case CommandType.Delete:
//
//                        ConsoleHelper.WriteColored(
//                            "Delete {0}.", 
//                            TITLE_COLOR,
//                            commandInfo.Arguments["targetPath"]);
//
//                        if (commandInfo.RequireConfirm && !GetConfirmation())
//                            return false;
//
//                        Console.WriteLine(); // End the line here
//
//                        fileOperations.DeleteDirectory(commandInfo.Arguments["targetPath"], true);
//                        break;
//
//                    case CommandType.CreateSymLink:
//
//                        ConsoleHelper.WriteColored(
//                            "Create symbolic link from {0} to {1}.", 
//                            TITLE_COLOR,
//                            commandInfo.Arguments["sourcePath"], 
//                            commandInfo.Arguments["targetPath"]);
//
//                        if (commandInfo.RequireConfirm && !GetConfirmation())
//                            return false;
//
//                        Console.WriteLine(); // End the line here
//
//                        fileOperations.CreateSymbolicLink(
//                            commandInfo.Arguments["sourcePath"], 
//                            commandInfo.Arguments["targetPath"]);
//                        
//                        break;
//
//                    case CommandType.All:
//
//                        ConsoleHelper.WriteColored(
//                            "Replace {0} with a symlink to the copy at {1}.", 
//                            TITLE_COLOR,
//                            commandInfo.Arguments["sourcePath"], 
//                            commandInfo.Arguments["targetPath"]);
//
//                        if (commandInfo.RequireConfirm && !GetConfirmation())
//                            return false;
//
//                        Console.WriteLine(); // End the line here
//
//                        if(!RunCommand(
//                            new CommandInfo(
//                                CommandType.Copy, 
//                                commandInfo.Arguments, 
//                                commandInfo.RequireConfirm),
//                            FileOperationsFactory.GetFileOperationsObject()))
//                        {
//                            return false;
//                        }
//
//                        if(!RunCommand(
//                            new CommandInfo(
//                                CommandType.Delete, 
//                                new Dictionary<string, string>() 
//                                { 
//                                    { "targetPath",  commandInfo.Arguments["sourcePath"]}
//                                }, 
//                                commandInfo.RequireConfirm),                           
//                            FileOperationsFactory.GetFileOperationsObject()))
//                        {
//                            return false;
//                        }
//
//                        if(!RunCommand(
//                            new CommandInfo(
//                                CommandType.CreateSymLink, 
//                                commandInfo.Arguments, 
//                                commandInfo.RequireConfirm),                           
//                            FileOperationsFactory.GetFileOperationsObject()))
//                        {
//                            return false;
//                        }
//
//                        break;
//
//                    default:
//                        throw new ArgumentException(
//                            string.Concat(
//                                "{0} is not a valid command type",
//                                commandInfo.Type.ToString()));
//                }
//
//                ConsoleHelper.WriteLineColored("DONE", DONE_COLOR);
//            }
//            catch(Exception ex){
//                // TODO : Manage this better?
//                ConsoleHelper.WriteLineColored(
//                    string.Concat(
//                        "FAILED{0}", 
//                        "\t{1}"
//                    ),
//                    ERROR_COLOR,
//                    Environment.NewLine,
//                    ex.Message);
//
//                return false;
//            }
//
//            return true;
//        }

//        // TODO : Rename? switch logic to requiring Y instead?
//        private static bool GetConfirmation()
//        {
//            ConsoleHelper.WriteColored(" (Y/n)?", DONE_COLOR);
//
//            return (ConsoleHelper.ReadKey().Key != ConsoleKey.N);
//        }
    }
}
