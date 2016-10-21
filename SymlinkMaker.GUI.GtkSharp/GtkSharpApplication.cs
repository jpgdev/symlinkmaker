using SymlinkMaker.Core;
using System.Collections.Generic;
using Gtk;

namespace SymlinkMaker.GUI.GtkSharp
{
    public class GtkSharpApplication : RunnableApplicationBase
    {
        private readonly IMainWindowView _mainWindowView;
        private readonly MainWindowController _mainWindowController;

        public GtkSharpApplication(
            AppSettings settings, 
            IDictionary<CommandType, CommandAdapter> commands,
            IDialogHelper dialogHelper,
            IGtkIconNameConverter iconNameConverter
        )
            : base(settings, commands)
        {
            Application.Init();

            // TODO : Set this as the params instead of the commands
            ICommandsManager<CommandType> commandsManager = new CommandsManager(commands);

            _mainWindowView = new MainWindowView(iconNameConverter);

            _mainWindowController = new MainWindowController(
                settings, 
                _mainWindowView,
                commandsManager,
                dialogHelper
            );

            _mainWindowView.Closed += (sender, e) =>
            {
                _mainWindowController.Dispose();
                Application.Quit();
            };
        }

        public override void Run(string[] args)
        {
            _mainWindowView.Show();
            Application.Run();
        }
    }
}
