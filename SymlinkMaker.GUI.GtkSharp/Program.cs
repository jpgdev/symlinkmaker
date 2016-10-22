using SymlinkMaker.Core;

namespace SymlinkMaker.GUI.GtkSharp
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // Load the AppSettings using the IAppSettingsLoader
            var fsOpsLocator = new FileSystemOperationsLocator(
                                   new SystemDirectory(),
                                   new SystemFileManager()
                               );
            IAppSettingsLoader settingsLoader = new DefaultAppSettingsLoader(fsOpsLocator);
            var settings = settingsLoader.Load(null);

            // Load the Commands using the ICommandLoader
            ICommandsLoader commandsLoader = new FileCommandsLoader(settings.FileOperations);
            var commands = commandsLoader.Load();


            // Load the CommandsAdapters using the ICommandAdapterLoader 
            IDialogHelper dialogHelper = new GtkSharpDialogHelper();
            ICommandAdaptersLoader commandAdapterLoader = new GtkSharpCommandAdaptersLoader(
                commands, 
                dialogHelper
            );
            var commandsAdapters = commandAdapterLoader.Load();

            IGtkIconNameConverter iconNameConverter = new GtkIconNameConverter();
            IRunnableApplication app = new GtkSharpApplication(
                settings, 
                commandsAdapters,
                dialogHelper,
                iconNameConverter
            );

            app.Run(args);
        }
    }
}
