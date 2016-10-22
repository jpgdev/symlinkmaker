using System;
using System.Collections.Generic;
using System.Linq;
using CommandType = SymlinkMaker.Core.CommandType;

namespace SymlinkMaker.Core
{
    public class FileCommandsLoader : ICommandsLoader
    {
        public IFileSystemOperations FileOperations { get; private set; }

        public FileCommandsLoader(IFileSystemOperations fileOperations)
        {
            if (fileOperations == null)
                throw new ArgumentNullException(nameof(fileOperations));

            FileOperations = fileOperations;
        }

        public IDictionary<CommandType, ICommand> Load()
        {
            var commands = new Dictionary<CommandType, ICommand>()
            {
                {
                    CommandType.Copy,
                    new Command(
                        args =>
                        {
                            if (args["sourcePath"] == args["targetPath"])
                                throw new Exception("The source cannot be the same path as the target.");

                            return FileOperations.Copy(
                                args["sourcePath"],
                                args["targetPath"]);
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

                            return FileOperations.Move(
                                args["sourcePath"],
                                args["targetPath"]
                            );
                        },
                        new[] { "sourcePath", "targetPath" }
                    )
                },
                {
                    CommandType.Delete,
                    new Command(
                        args => FileOperations.Delete(args["sourcePath"]),
                        new[] { "sourcePath" }
                    )
                },
                {
                    CommandType.CreateSymLink,
                    new Command(
                        args =>
                        {
                            if (args["sourcePath"] == args["targetPath"])
                                throw new Exception("The source cannot be the same path as the target.");

                            return FileOperations.CreateSymbolicLink(
                                args["sourcePath"],
                                args["targetPath"]);
                        },
                        new[] { "sourcePath", "targetPath" }
                    )
                }
            };

            // Note : Since it uses commands from the commands Dictionary, it need to be added afterwards
            commands.Add(
                CommandType.All,
                new Command(
                    args =>
                    {
                        var commandTypes = new[]
                        {
                            CommandType.Move,
                            CommandType.CreateSymLink
                        };

                        return commandTypes.All(cmd => commands[cmd].Execute(args));
                    },
                    new[] { "sourcePath", "targetPath" }
                )
            );

            return commands;
        }
    }
}
