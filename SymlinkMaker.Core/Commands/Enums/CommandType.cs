namespace SymlinkMaker.Core
{
    public enum CommandType
    {

        None = 0,
        Delete,
        Copy,
        CreateSymLink,
        Move,
        // TODO : Rename this, it does no longer do it all
        All,
        ShowHelp,
    }
}

