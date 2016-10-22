using SymlinkMaker.Core;

namespace SymlinkMaker.CLI
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

            IConsoleHelper consoleHelper = new ConsoleHelper();

            // Load the Commands using the ICommandLoader
            ICommandsLoader commandsLoader = new CLIHelpCommandLoaderDecorator(
                                                new FileCommandsLoader(settings.FileOperations),
                                                consoleHelper
                                            );
            var commands = commandsLoader.Load();

            // Load the CommandsAdapters using the ICommandAdapterLoader 
            ICommandAdaptersLoader commandAdapterLoader = new CLICommandAdaptersLoader(
                                                             commands, 
                                                             consoleHelper);
            var commandAdapters = commandAdapterLoader.Load();

            ICLICommandParser commandParser = new CLICommandParser();
            IRunnableApplication app = new CLIApplication(
                                           settings, 
                                           commandAdapters, 
                                           consoleHelper,
                                           commandParser);
            app.Run(args);
        }
    }
}
