using System;
using System.Collections.Generic;

namespace SymlinkMaker.CLI
{
    // TODO : Rename this? since it has no connection to the Command object?
    internal struct CommandInfo
    {
        public CommandType? Type;
        public IDictionary<string, string> Arguments;
        public bool RequireConfirm; // TODO : Move into args?

        public CommandInfo (CommandType? type, IDictionary<string, string> args, bool requireConfirm){
            Type = type;
            Arguments = args;
            RequireConfirm = requireConfirm;
        }
    }
}

