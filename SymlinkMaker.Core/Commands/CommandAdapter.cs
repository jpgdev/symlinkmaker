using System;
using System.Collections.Generic;
using System.Linq;

namespace SymlinkMaker.Core
{
    public abstract class CommandAdapter : IDisposable
    {
        public ICommand Command { get; private set; }

        protected CommandAdapter(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            Command = command;

            Command.Finished += OnFinish;
            Command.Succeeded += OnSuccess;
            Command.Failed += OnFailure;
            Command.ExceptionThrown += OnException;
        }

        public virtual void Dispose()
        {
            Command.Finished -= OnFinish;
            Command.Succeeded -= OnSuccess;
            Command.ExceptionThrown -= OnFailure;
            Command.ExceptionThrown -= OnException;

            Command.UnregisterPreExecutionValidation(ConfirmationHandler);
        }

        #region Abstract methods

        protected abstract void OnFailure(ICommand sender,
                                          CommandEventArgs eventArgs);

        protected abstract void OnSuccess(ICommand sender,
                                          CommandEventArgs eventArgs);

        protected abstract void OnFinish(ICommand sender,
                                         CommandEventArgs eventArgs);

        protected abstract void OnException(ICommand sender,
                                            CommandExceptionEventArgs eventArgs);

        protected abstract bool ConfirmationHandler(IDictionary<string, string> arguments);

        #endregion

        public virtual bool Execute(
            IDictionary<string, string> arguments, 
            bool requireConfirmation = true)
        {
            // FIXME : Precaution to not register each time we call the Command
            //         There is a problem here obviously, need some change.
            Command.UnregisterPreExecutionValidation(ConfirmationHandler);

            if (requireConfirmation)
                Command.RegisterPreExecutionValidation(ConfirmationHandler);

            return Command.Execute(arguments);
        }


        /// <summary>
        /// Gets the values of the arguments from arguments names dictionary.
        /// </summary>
        /// <returns>The arguments values from arguments name.</returns>
        /// <param name="argsValues">Arguments values.</param>
        /// <param name="argsNames">Arguments names.</param>
        public static string[] GetArgsValuesFromArgsName(IDictionary<string, string> argsValues,
                                                         string[] argsNames)
        {
            return argsNames
                .Select(argName => argsValues[argName])
                .ToArray();
        }

    }
}
