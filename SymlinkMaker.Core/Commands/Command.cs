/*
 *  IDEAs/TODOs: 
 * 
 *         Add a Undo/Revert() function. 
 *              Either with the command pattern 
 *              OR 
 *              a simple RevertFunc(args) which does the opposite operation (might be the right one, since we change the FileSystem).
 * 
 *         Add an exception parser in the Run()->catch? 
 *              So it would be parsed per command, not per adapter? 
 *              This way the exception parsing is not repeated for each adapter.
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
    // TODO : Rename to Operation OR Job OR something else??

    //        Since it does not implement the Command Pattern,
    //        it is a weird name to use.

    public class Command : ICommand
    {
        #region Properties

        /// <summary>
        /// Gets the delegate to execute. 
        /// </summary>
        /// <value>The command func.</value>
        protected Operation Operation { get; private set; }

        /// <summary>
        /// Gets or sets the function ran before the actual command.
        /// </summary>
        /// <remarks>
        /// If the function returns false, the command will not be ran.
        /// </remarks>
        /// <value>The function to execute before executing the actual command.</value>
        protected Operation PreExecutionValidation { get; private set; }

        protected string[] RequiredArguments { get; set; }

        #endregion

        #region Events

        public event CommandEventHandler Succeeded;
        public event CommandEventHandler Failed;
        public event CommandEventHandler Finished;
        public event CommandExceptionEventHandler ExceptionThrown;

        #endregion

        #region Constructors

        public Command(
            Operation commandFunc,
            string[] requiredArguments = null)
            : this(commandFunc, null, requiredArguments)
        {
        }

        public Command(
            Operation operation,
            Operation preExecuteValidator,
            string[] requiredArguments = null)
        {

            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            Operation = operation;
            RequiredArguments = requiredArguments;

            InitializePreExecutionValidation(preExecuteValidator);
        }

        #endregion

        private void InitializePreExecutionValidation(Operation preRunValidator)
        {
            if (RequiredArguments != null)
                PreExecutionValidation = GetRequiredArgsValidator(RequiredArguments);
            
            PreExecutionValidation += preRunValidator;
        }

        #region Notify Event Subscribers

        // Note:
        // I don't know how I can find a parameter type that can encapsulate
        // both the CommandEventHandler & CommandExceptionEventHandler
        // to merge these functions

        protected void NotifyEventSubscribers(CommandEventHandler eventHandler,
                                              IDictionary<string, string> args,
                                              CommandStatus status)
        {
            if (eventHandler == null)
                return;

            NotifyEventSubscribers(
                eventHandler,
                new CommandEventArgs(args, status)
            );
        }

        protected void NotifyEventSubscribers(CommandEventHandler eventHandler,
                                              CommandEventArgs commandEventArgs)
        {
            if (eventHandler == null)
                return;
            
            eventHandler(this, commandEventArgs);
        }


        protected void NotifyEventSubscribers(CommandExceptionEventHandler eventHandler,
                                              IDictionary<string, string> args,
                                              Exception exception,
                                              CommandStatus status)
        {
            if (eventHandler == null)
                return;

            NotifyEventSubscribers(
                eventHandler,
                new CommandExceptionEventArgs(args, exception, status)
            );
        }

        protected void NotifyEventSubscribers(CommandExceptionEventHandler eventHandler,
                                              CommandExceptionEventArgs commandEventArgs)
        {
            if (eventHandler == null)
                return;
            
            eventHandler(this, commandEventArgs);
        }

        #endregion

        #region Methods

        public void RegisterPreExecutionValidation(Operation preRunValidation)
        {
            if (preRunValidation == null)
                return;

            PreExecutionValidation += preRunValidation;
        }

        public void UnregisterPreExecutionValidation(Operation preRunValidation)
        {
            if (PreExecutionValidation != null &&
                PreExecutionValidation.GetInvocationList().Contains(preRunValidation))
            {
                PreExecutionValidation -= preRunValidation;
            }
        }

        public bool Execute(IDictionary<string, string> args)
        {
            CommandStatus currentStatus = CommandStatus.PreRun;
            try
            {
                if (!ExecuteValidationsFunctions(args))
                    return false;

                currentStatus = CommandStatus.Running;
                bool result = Operation(args);
                if (result)
                {
                    currentStatus = CommandStatus.Succeeded;
                    NotifyEventSubscribers(Succeeded, args, currentStatus);
                }
                else
                {
                    currentStatus = CommandStatus.Failed;
                    NotifyEventSubscribers(Failed, args, currentStatus);
                }

                NotifyEventSubscribers(Finished, args, currentStatus);

                return result;
            }
            catch (Exception ex)
            {
                if (ExceptionThrown == null)
                    throw;

                NotifyEventSubscribers(
                    ExceptionThrown,
                    args,
                    ex,
                    currentStatus);

                return false;
            }
        }

        private bool ExecuteValidationsFunctions(IDictionary<string, string> args)
        {
            if (PreExecutionValidation == null)
                return true;
            
            var validationFuncs = PreExecutionValidation.GetInvocationList();

            return validationFuncs.All(
                func => (func as Operation)(args)
            );
        }

        private static Operation GetRequiredArgsValidator(IEnumerable<string> requiredArguments)
        {
            return args =>
            {
                if (requiredArguments == null)
                    return true;
                
                if (args == null)
                    throw new ArgumentNullException(nameof(args));

                var missingArgs = requiredArguments.Where(arg => !args.ContainsKey(arg));
                if (missingArgs.Any())
                {
                    throw new ArgumentException(
                        string.Format(
                            "Missing command arguments : {0}",
                            string.Join(",", missingArgs)));
                }
                return true;
            };
        }

        #endregion
    }
}
