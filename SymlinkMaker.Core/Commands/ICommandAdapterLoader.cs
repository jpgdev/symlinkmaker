using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public interface ICommandAdaptersLoader
    {
        IDictionary<CommandType, CommandAdapter> Load();
    }
}