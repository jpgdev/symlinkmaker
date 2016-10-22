using System;
using System.Collections.Generic;
using SymlinkMaker.Core;

namespace SymlinkMaker.GUI.GtkSharp
{
    public class GtkSharpCommandAdaptersLoader : ICommandAdaptersLoader
    {
        public IDictionary<CommandType, ICommand> BaseCommands { get; private set; }

        private readonly IDialogHelper _dialogHelper;
        
        public GtkSharpCommandAdaptersLoader(
            IDictionary<CommandType, ICommand> baseCommands,
            IDialogHelper dialogHelper)
        {
            if (baseCommands == null)
                throw new ArgumentNullException(nameof(baseCommands));

            if (dialogHelper == null)
                throw new ArgumentNullException(nameof(dialogHelper));
            
            BaseCommands = baseCommands;
            _dialogHelper = dialogHelper;
        }

        public IDictionary<CommandType, CommandAdapter> Load()
        {
            return new Dictionary<CommandType, CommandAdapter>()
            {
                {
                    CommandType.Copy,
                    new GtkSharpCommandAdapter(
                        BaseCommands[CommandType.Copy],
                        _dialogHelper,
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Are you sure you want to copy?</b>{0}{0}{1}",
                                Environment.NewLine,
                                "'{0}' => '{1}'"
                            ),
                            new [] { "sourcePath", "targetPath" },
                            "Copying {0} ...",
                            new [] { "sourcePath" }
                        )
                    )
                },
                {
                    CommandType.Move,
                    new GtkSharpCommandAdapter(
                        BaseCommands[CommandType.Move],
                        _dialogHelper,
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Are you sure you want to move?</b>{0}{0}{1}",
                                Environment.NewLine,
                                "'{0}' => '{1}'"
                            ),
                            new [] { "sourcePath", "targetPath" },
                            "Moving {0} ...",
                            new [] { "sourcePath" }
                        )
                    )
                },
                {
                    CommandType.Delete,
                    new GtkSharpCommandAdapter(
                        BaseCommands[CommandType.Delete],
                        _dialogHelper,
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Are you sure you want to delete?</b>{0}{0}{1}",
                                Environment.NewLine,
                                "'{0}'"
                            ),
                            new [] { "sourcePath" },
                            "Deleting {0} ...",
                            new [] { "sourcePath" }
                        )
                    )
                },
                {
                    CommandType.CreateSymLink,
                    new GtkSharpCommandAdapter(
                        BaseCommands[CommandType.CreateSymLink],
                        _dialogHelper,
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Create a symbolic link?</b>{0}{0}{1}",
                                Environment.NewLine,
                                "'{0}' => '{1}'"
                            ),
                            new [] { "sourcePath", "targetPath" },
                            "Linking {0} to {1} ...",
                            new [] { "sourcePath", "targetPath" }
                        )
                    )
                },
                {
                    CommandType.All,
                    new GtkSharpCommandAdapter(
                        BaseCommands[CommandType.All],
                        _dialogHelper,
                        new GtkSharpCommandConfirmationInfo(
                            string.Format(
                                "<b>Are you sure you want to launch the replacement process?</b>{0}{0}{1}{0}{2}{0}{0}{3}",
                                Environment.NewLine,
                                "<b>Source:</b> '{0}'",
                                "<b>Target:</b> '{1}'",
                                "This will move the <b>source</b> directory to the <b>target</b> directory then create a " +
                                "symlink from the original <b>source</b> directory to the <b>target</b> directory."
                            ),
                            new [] { "sourcePath", "targetPath" },
                            "Launch replacement process ...",
                            new [] { "sourcePath", "targetPath" }
                        )
                    )
                }
            };
   
        }
    }
}

