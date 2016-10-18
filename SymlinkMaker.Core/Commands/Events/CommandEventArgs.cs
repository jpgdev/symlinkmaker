using System;
using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public class CommandEventArgs : EventArgs
    {
        public IDictionary<string, string> Arguments { get; set; }
        public CommandStatus Status { get; set; }
        
        public CommandEventArgs(IDictionary<string, string> args, CommandStatus status) 
        {
            Arguments = args;
            Status = status;
        }
    }
}