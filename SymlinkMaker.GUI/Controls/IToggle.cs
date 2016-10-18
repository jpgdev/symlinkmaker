namespace SymlinkMaker.GUI
{
    public interface IToggle : IControl
    {
        //        event ButtonToggleEventHandler OnToggle;
        event ButtonToggleEventHandler StatusChanged;

        bool IsActive { get; set; }

        void Toggle();
    }
}

