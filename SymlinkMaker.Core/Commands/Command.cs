
/*
 *  IDEAs/TODOs: 
 * 
 *         Add a Undo/Revert() function. 
 *              Either with the command pattern 
 *              OR 
 *              a simple RevertFunc(args) which does the opposite operation (might be the right one, since we change the FileSystem).
 * 
 *         Add an exception parser in the Run()->catch? 
 *              So it would be parsed per command, not per wrapper? 
 *              This way the exception parsing is not repeated for each wrapper.
 *              Note : Might want to create it out of the commands, 
 *                     since the errors can be the same for mutliple commands.
 *
 *             IExceptionParser? Creates a message from the exception?
 *                EX. UnixIOException.ErrorCode = EEXIST, ...NativeErrorCode = 17,
 *                    Change the message to : "The file already exists." (i18n too)
 */


using System;
using System.Collections.Generic;
using System.Linq;

namespace SymlinkMaker.Core
{
    public class Command
    {
        /// <summary>
        /// Gets the function to run as the actual command.
        /// </summary>
        /// <value>The command func.</value>
        protected Func<IDictionary<string, string>, bool> CommandFunc { get; private set; }

        /// <summary>
        /// Gets or sets the function ran before the actual command.
        /// </summary>
        /// <remarks>
        /// If the function returns false, the command will not be ran.
        /// </remarks>
        /// <value>The before run func.</value>
        /// 
        public Func<IDictionary<string, string>, bool> BeforeRunFunc // TODO : Find a better name
        { 
            get; 
            set; 
        }

        protected string[] RequiredArguments { get; set; }

        public delegate void CommandEventHandler(Command command,CommandEventArgs cmdEventArgs);

        public delegate void CommandExceptionEventHandler(Command command,CommandExceptionEventArgs cmdEventArgs);

        public event CommandEventHandler OnSuccess;
        public event CommandEventHandler OnFailure;
        public event CommandEventHandler OnFinish;
        public event CommandExceptionEventHandler OnException;

        public Command(
            Func<IDictionary<string, string>, bool> commandFunc,
            string[] requiredArguments = null)
            : this(commandFunc, null, requiredArguments)
        {
        }

        public Command(
            Func<IDictionary<string, string>, bool> commandFunc,
            Func<IDictionary<string, string>, bool> beforeRunFunc,
            string[] requiredArguments = null)
        {
            if (commandFunc == null)
                throw new ArgumentNullException("commandFunc");

            CommandFunc = commandFunc;
            BeforeRunFunc = beforeRunFunc;
            RequiredArguments = requiredArguments;
        }

        /// <summary>
        /// Run the command without any arguments.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown by <see cref="SymlinkMaker.Core.Command.ValidateRequiredArguments"/> if required arguments are missing</exception> 
        public bool Run()
        {
            return Run(null);
        }

        /// <summary>
        /// Run the command with the specified args.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown by <see cref="SymlinkMaker.Core.Command.ValidateRequiredArguments"/> if required arguments are missing</exception> 
        /// <param name="args">Arguments for the command.</param>
        public bool Run(IDictionary<string, string> args)
        {
            try
            {
                ValidateRequiredArguments(args, RequiredArguments);

                // If we need another validation (Can be used to confirm)
                if (BeforeRunFunc != null && !BeforeRunFunc(args))
                    return false;

                bool result = CommandFunc(args);
                if (result)
                {
                    if (OnSuccess != null)
                        OnSuccess(this, new CommandEventArgs(args, CommandStatus.Success));
                }
                else
                {
                    if (OnFailure != null)
                        OnFailure(this, new CommandEventArgs(args, CommandStatus.Failed));
                }

                // Add the result boolean to the EventArgs params?
                if (OnFinish != null)
                    OnFinish(this, new CommandEventArgs(args, CommandStatus.Success));

                return result;
            }
            catch (Exception ex)
            {
                if (OnException == null)
                    throw;

                OnException(this, new CommandExceptionEventArgs(args, ex));

                // TODO : also call OnFailure????
                return false;
            }
            // finally
            // {                    
            //     HandleCleanup(args);
            // }

        }

        /// <summary>
        /// Validates that all the required arguments are provided. 
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when there are missing required arguments.</exception>
        /// <param name="args">The arguments provided.</param>
        /// <param name="requiredArguments">Required arguments.</param>
        public static void ValidateRequiredArguments(IDictionary<string, string> args, IEnumerable<string> requiredArguments = null)
        {
            if (requiredArguments == null)
                return;

            IEnumerable<string> missingArgs = requiredArguments.Where(arg =>
                !args.ContainsKey(arg));

            if (missingArgs.Any())
            {
                throw new ArgumentException(
                    string.Format("Missing command arguments : {0}",
                        string.Join(",", missingArgs)));
            }
        }

    }
}
