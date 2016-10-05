using System;

namespace SymlinkMaker.CLI
{
    public static class ConsoleHelper
    {
        /// <summary>
        /// Reads the key entered by the user.
        /// </summary>
        /// <returns>The key info.</returns>
        public static ConsoleKeyInfo ReadKey(){
            return Console.ReadKey();
        }

        /// <summary>
        /// Writes the line colored.
        /// </summary>
        /// <param name="message">The text to write.</param>
        /// <param name="fgColor">The color of the text.</param>
        /// <param name="parameters">The message parameters.</param>
        public static void WriteLineColored(
            string message, 
            ConsoleColor? fgColor, 
            params string[] parameters)
        {
            WriteLineColored(message, fgColor, null, parameters);
        }

        /// <summary>
        /// Writes the line colored.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="fgColor">Fg color.</param>
        /// <param name="bgColor">The background color of the text.</param>
        /// <param name="parameters">The message parameters.</param>
        public static void WriteLineColored(
            string message, 
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor, 
            params string[] parameters)
        {
            WriteColored(message, fgColor, bgColor, Console.WriteLine, parameters);
        }

        /// <summary>
        /// Writes the text colored.
        /// </summary>
        /// <param name="message">The text to write.</param>
        /// <param name="fgColor">The color of the text.</param>
        /// <param name="parameters">The message parameters.</param>
        public static void WriteColored(
            string message, 
            ConsoleColor? fgColor, 
            params string[] parameters)
        {            
            WriteColored(message, fgColor, null, parameters);
        }

        /// <summary>
        /// Writes the text colored.
        /// </summary>
        /// <param name="message">The text to write.</param>
        /// <param name="fgColor">The color of the text.</param>
        /// <param name="bgColor">The background color of the text.</param>
        /// <param name="parameters">The message parameters.</param>
        public static void WriteColored(
            string message, 
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor, 
            params string[] parameters)
        {            
            WriteColored(message, fgColor, bgColor, Console.Write, parameters);
        }

        /// <summary>
        /// Writes using the delegate colored.
        /// </summary>
        /// <param name="message">The text to write.</param>
        /// <param name="fgColor">The color of the text.</param>
        /// <param name="bgColor">The background color of the text.</param>
        /// <param name="consoleWriteAction">The delegate action to write to.</param>
        /// <param name="parameters">The message parameters.</param>
        private static void WriteColored(
            string message, 
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor, 
            Action<string, string[]> consoleWriteAction, 
            params string[] parameters)
        {
            if (consoleWriteAction == null)
                consoleWriteAction = Console.Write;
            
            if (fgColor.HasValue)
                Console.ForegroundColor = fgColor.Value;
            if (bgColor.HasValue)
                Console.BackgroundColor = bgColor.Value;
            
            consoleWriteAction(message, parameters);

            Console.ResetColor();
        }
    }
}

