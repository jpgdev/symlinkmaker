using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public interface ICommandsLoader
    {
        IDictionary<CommandType, ICommand> Load();
    }
}