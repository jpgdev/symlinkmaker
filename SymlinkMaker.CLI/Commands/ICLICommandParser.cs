using System.Collections.Generic;

namespace SymlinkMaker.CLI
{
    public interface ICLICommandParser
    {
        CLICommandInfo ParseArgs(IEnumerable<string> arguments);
    }
}

