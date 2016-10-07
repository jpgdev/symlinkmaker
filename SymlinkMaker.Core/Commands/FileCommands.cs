using System;
using System.Collections.Generic;
using System.Linq;
using CommandType = SymlinkMaker.Core.CommandType;

namespace SymlinkMaker.Core
{

    public static class FileCommands
    {
        /// <summary>
        /// Gets the available FileOperations commands.
        /// </summary>
        /// <value>The commands.</value>
        public static IDictionary<CommandType, Command> Commands { get; private set; }

        /// <summary>
        /// Initializes and generates the FileOperations commands.
        /// </summary>
        /// <param name="fileOperations">File operations.</param>
        public static void InitializeCommands(IFileOperations fileOperations)
        {
            if (fileOperations == null)
                throw new ArgumentNullException("fileOperations");

            Commands = new Dictionary<CommandType, Command>()
            {
                {
                 CommandType.Copy,
                 new Command(
                    args =>
                    {
                        if (args["sourcePath"] == args["targetPath"])
                            throw new Exception("The source cannot be the same path as the target.");

                        fileOperations.CopyDirectory(
                            args["sourcePath"],
                            args["targetPath"],
                            true);

                        return true;
                    },
                    new[] { "sourcePath", "targetPath" }
                 )
            },
            {
                CommandType.Move,
                new Command(
                    args =>
                    {
                        if (args["sourcePath"] == args["targetPath"])
                            throw new Exception("The source cannot be the same path as the target.");

                        fileOperations.MoveDirectory(
                            args["sourcePath"],
                            args["targetPath"]
                        );

                        return true;
                    },
                    new[] { "sourcePath", "targetPath" }
                )
            },
            {
                CommandType.Delete,
                new Command(
                    args =>
                    {
                        fileOperations.DeleteDirectory(
                            args["sourcePath"],
                            true);

                        return true;
                    },
                    new[] { "sourcePath" }
                )
            },
            {
                CommandType.CreateSymLink,
                new Command(
                    args =>
                    {
                        // TODO : Validation
                        if (args["sourcePath"] == args["targetPath"])
                            throw new Exception("The source cannot be the same path as the target.");

                        fileOperations.CreateSymbolicLink(
                            args["sourcePath"],
                            args["targetPath"]);

                        return true;
                    },
                    new[] { "sourcePath", "targetPath" }
                )
            },
            {
                CommandType.All,
                new Command(
                    args =>
                    {
                        var commands = new CommandType[]{
                            CommandType.Move,
                            CommandType.CreateSymLink
                        };

                        return commands.All(cmd => Commands[cmd].Run(args));
                    },
                    new[] { "sourcePath", "targetPath" }
                )
            }
           };
        }
    }
}
