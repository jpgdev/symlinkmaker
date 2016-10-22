using System;
using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public abstract class RunnableApplicationBase : IRunnableApplication
    {
        private readonly AppSettings _settings;
        private readonly IDictionary<CommandType, CommandAdapter> _commands;

        protected AppSettings Settings
        {
            get { return _settings; }
        }

        protected IDictionary<CommandType, CommandAdapter> Commands
        {
            get { return _commands; }
        }

        protected RunnableApplicationBase(
            AppSettings settings, 
            IDictionary<CommandType, CommandAdapter> commands)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            _settings = settings;
            _commands = commands;
        }

        public abstract void Run(string[] args);
    }
}
