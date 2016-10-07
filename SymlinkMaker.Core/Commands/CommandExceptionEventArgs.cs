using System;
using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public class CommandExceptionEventArgs : CommandEventArgs
    {
        public Exception Exception { get; set; }

        public CommandExceptionEventArgs(IDictionary<string, string> args, Exception ex)
            :base (args, CommandStatus.Running)
        {
            Exception = ex;
        }
    }
}