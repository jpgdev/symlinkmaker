namespace SymlinkMaker.GUI
{
    public interface IButton : IControl
    {
        event ButtonClickEventHandler Triggered;

        void Trigger();
    }
}

