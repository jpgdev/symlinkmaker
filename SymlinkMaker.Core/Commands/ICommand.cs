using System;
using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public interface ICommand
    {
        bool Run (IDictionary<string, string> args);
    }
}

