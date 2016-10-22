using System;
using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public class CommandExceptionEventArgs : CommandEventArgs
    {
        public Exception Exception { get; set; }

        public CommandExceptionEventArgs(
            IDictionary<string, string> args, 
            Exception ex, 
            CommandStatus status = CommandStatus.Running)
            :base (args, status)
        {
            Exception = ex;
        }
    }
}