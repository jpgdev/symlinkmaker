using System;

namespace SymlinkMaker.CLI
{
    public interface IConsoleHelper
    {
        ConsoleKeyInfo ReadKey();

        void WriteLineColored(
            string message, 
            ConsoleColor? fgColor);

        void WriteLineColored(
            string message, 
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor);

        void WriteColored(
            string message, 
            ConsoleColor? fgColor);

        void WriteColored(
            string message, 
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor);

        void WriteColored(
            string message,
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor, 
            Action<string> consoleWriteAction);
    }
}
