using System;
using System.Collections.Generic;
using System.Linq;

namespace SymlinkMaker.Core
{
    public abstract class Command : ICommand
    {
        protected Func<IDictionary<string, string>, bool> CommandFunc { set; get; }
        protected string[] RequiredArguments { set; get; }

        public Command(Func<IDictionary<string, string>, bool> commandFunc, string[] requiredArguments = null)
        {
            if (commandFunc == null)
                throw new ArgumentNullException("commandFunc");
            
            CommandFunc = commandFunc;
            RequiredArguments = requiredArguments;
        }

        // NOTE: Possible flaw, can only accept string arguments
        public virtual bool Run(IDictionary<string, string> args)
        {
            try
            {                
                ValidateRequiredArguments(args);

                return CommandFunc(args);
            }
            catch (Exception ex)
            {                
                return HandleException(ex, args);
            }
        }

        public virtual void ValidateRequiredArguments(IDictionary<string, string> args)
        {
            if (RequiredArguments == null)
                return;

            IEnumerable<string> missingArgs = RequiredArguments.Where((arg) => {
                return !args.ContainsKey(arg);
            });

            if (missingArgs.Count() > 0)
            {
                throw new ArgumentException(
                    string.Format("Missing command arguments : {0}", 
                        string.Join(",", missingArgs)));
            }
        }

        public abstract bool HandleException(Exception ex, IDictionary<string, string> args);
    }
}

