using System.Collections.Generic;
using SymlinkMaker.Core;

namespace SymlinkMaker.CLI
{
    // TODO : Rename this? since it has almost no connection to the Command object?
    public struct CLICommandInfo
    {
        public CommandType Type;
        public IDictionary<string, string> Arguments;
        public bool RequiresConfirm; // TODO : Move into args?

        public CLICommandInfo (CommandType type, IDictionary<string, string> args, bool requiresConfirm){
            Type = type;
            Arguments = args;
            RequiresConfirm = requiresConfirm;
        }
    }
}

