using System;
using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public abstract class ConfirmableCommand : Command
    {
        protected Func<IDictionary<string, string>, bool> ConfirmFunc { set; get; }

        public ConfirmableCommand(Func<IDictionary<string, string>, bool> commandFunc, Func<IDictionary<string, string>, bool> confirmFunc,  string[] requiredArgs = null)
            :base (commandFunc, requiredArgs)
        {
            if (confirmFunc == null)
                throw new ArgumentNullException("confirmFunc");

            ConfirmFunc = confirmFunc;
        }

        public virtual bool Run(IDictionary<string, string> args, bool requireConfirm = true)
        {
            try
            {
                ValidateRequiredArguments(args);

                if (requireConfirm && !ConfirmFunc(args))
                    return false;
            }
            catch (Exception e)
            {
                return HandleException(e, args);
            }

            return base.Run(args);
        }
    }
}

