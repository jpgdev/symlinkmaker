namespace SymlinkMaker.GUI
{
    public interface ITextSource : IControl
    {
        event TextChangedEventHandler TextChanged;

        string Text { get; set; }
    }
}

