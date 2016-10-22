using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public interface ICommandsManager<TKey>// : ILocator<TKey, ICommand>
    {
        bool Execute(TKey type,
                 string sourcePath = null,
                 string targetPath = null,
                 bool requiresConfirmation = true);

        bool Execute(TKey type,
                 Dictionary<string, string> args,
                 bool requiresConfirmation = true);
    }
}
