using System.Collections.Generic;
using System;

namespace SymlinkMaker.Core
{
    public class CommandsManager : ICommandsManager<CommandType>
    {
        private readonly IDictionary<CommandType, CommandAdapter> _commands;

        public CommandsManager(IDictionary<CommandType, CommandAdapter> commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));
            
            _commands = commands;
        }

        public bool Run(
            CommandType type, 
            string sourcePath = null, 
            string targetPath = null, 
            bool requiresConfirmation = true)
        {
            var args = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(sourcePath))
                args["sourcePath"] = sourcePath; 

            if (!string.IsNullOrEmpty(targetPath))
                args["targetPath"] = targetPath; 

            return Run(type, args, requiresConfirmation);
        }

        public bool Run(
            CommandType type, 
            Dictionary<string, string> args,
            bool requiresConfirmation = true)
        {
            // TODO : When the Get is changed, use it to retrieve the command
            return _commands[type].Run(args, requiresConfirmation);
        }

        // TODO : Change this when the ICommandAdapter are changed to ICommandDecorator
        public ICommand Get(CommandType key)
        {
            return _commands[key].Command;
        }
    }
}

