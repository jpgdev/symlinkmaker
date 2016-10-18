using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public interface ICommandsManager<TKey>// : ILocator<TKey, ICommand>
    {
        bool Run(TKey type,
                 string sourcePath = null,
                 string targetPath = null,
                 bool requiresConfirmation = true);

        bool Run(TKey type,
                 Dictionary<string, string> args,
                 bool requiresConfirmation = true);
    }
}
