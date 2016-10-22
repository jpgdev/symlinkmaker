using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public delegate void CommandEventHandler(ICommand command, CommandEventArgs cmdEventArgs);

    public delegate void CommandExceptionEventHandler(ICommand command,CommandExceptionEventArgs cmdEventArgs);

    public delegate bool Operation(IDictionary<string, string> args);

    // TODO : rename this, it collides with another ICommand (System.Windows.Input.ICommand)
    //        Ideas : IOperation?
    public interface ICommand
    {
        event CommandEventHandler Succeeded;
        event CommandEventHandler Failed;
        event CommandEventHandler Finished;
        event CommandExceptionEventHandler ExceptionThrown;

        void RegisterPreExecutionValidation(Operation preExecuteValidation);

        void UnregisterPreExecutionValidation(Operation preExecuteValidation);

        bool Execute(IDictionary<string, string> args);
    }
}
