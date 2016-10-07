using System;
using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public abstract class CommandWrapper
    {
        public Command Command { get; private set; }

        protected CommandWrapper(Command command)
        {
            if(command == null)
                throw new ArgumentNullException("command");
                
            Command = command;

            Command.OnFinish += OnFinish;
            Command.OnSuccess += OnSuccess;
            Command.OnFailure += OnFailure;
            Command.OnException += OnException;
        }

        protected abstract void OnFailure(object sender, CommandEventArgs eventArgs);
        protected abstract void OnSuccess(object sender, CommandEventArgs eventArgs);
        protected abstract void OnFinish(object sender, CommandEventArgs eventArgs);
        protected abstract void OnException(object sender, CommandExceptionEventArgs eventArgs);

        public abstract bool Run(IDictionary<string, string> arguments, bool requireConfirmation = true);
    }
}