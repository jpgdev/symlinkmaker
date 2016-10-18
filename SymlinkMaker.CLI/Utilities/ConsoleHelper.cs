using System;

namespace SymlinkMaker.CLI
{

    // TODO : Change from '' implementation to an Interface implementation
    //        So it would be Injectable, instead of having  behavior
    public class ConsoleHelper : IConsoleHelper
    {
        /// <summary>
        /// Reads the key entered by the user.
        /// </summary>
        /// <returns>The key info.</returns>
        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        /// <summary>
        /// Writes the line colored.
        /// </summary>
        /// <param name="message">The text to write.</param>
        /// <param name="fgColor">The color of the text.</param>
        public void WriteLineColored(
            string message, 
            ConsoleColor? fgColor)
        {
            WriteLineColored(message, fgColor, null);
        }

        /// <summary>
        /// Writes the line colored.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="fgColor">Fg color.</param>
        /// <param name="bgColor">The background color of the text.</param>
        public void WriteLineColored(
            string message, 
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor)
        {
            WriteColored(message, fgColor, bgColor, Console.WriteLine);
        }

        /// <summary>
        /// Writes the text colored.
        /// </summary>
        /// <param name="message">The text to write.</param>
        /// <param name="fgColor">The color of the text.</param>
        public void WriteColored(
            string message, 
            ConsoleColor? fgColor)
        {            
            WriteColored(message, fgColor, null);
        }

        /// <summary>
        /// Writes the text colored.
        /// </summary>
        /// <param name="message">The text to write.</param>
        /// <param name="fgColor">The color of the text.</param>
        /// <param name="bgColor">The background color of the text.</param>
        public  void WriteColored(
            string message, 
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor)
        {            
            WriteColored(message, fgColor, bgColor, Console.Write);
        }

        /// <summary>
        /// Writes using the delegate colored.
        /// </summary>
        /// <param name="message">The text to write.</param>
        /// <param name="fgColor">The color of the text.</param>
        /// <param name="bgColor">The background color of the text.</param>
        /// <param name="consoleWriteAction">The delegate action to write to.</param>
        public void WriteColored(
            string message, 
            ConsoleColor? fgColor, 
            ConsoleColor? bgColor, 
            Action<string> consoleWriteAction)
        {
            if (consoleWriteAction == null)
                throw new ArgumentNullException(nameof(consoleWriteAction));
            
            if (fgColor.HasValue)
                Console.ForegroundColor = fgColor.Value;

            if (bgColor.HasValue)
                Console.BackgroundColor = bgColor.Value;
            
            consoleWriteAction(message);

            Console.ResetColor();
        }
    }
}
