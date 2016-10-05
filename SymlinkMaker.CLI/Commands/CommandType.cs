using System;

namespace SymlinkMaker.CLI
{
    // TODO : Use flags instead?
    internal enum CommandType
    {
        None = 0,
        Delete = 1,
        Copy = 2,
        CreateSymLink = 3,
        All = 4
    }
}

