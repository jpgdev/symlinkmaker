namespace SymlinkMaker.GUI
{
    public interface IMainWindowView  : IWindow
    {
        IImage SourceStatusImage { get; }

        IImage TargetStatusImage { get; }

        ITextSource SourcePath { get; }

        ITextSource TargetPath { get; }

        IButton CopySourceButton { get; }

        IButton MoveSourceButton { get; }

        IButton DeleteSourceButton { get; }

        IButton DeleteTargetButton { get; }

        IButton CreateSymlinkButton { get; }

        IButton DoAllButton { get; }

        IButton FindSourceButton { get; }

        IButton OpenSourceButton { get; }

        IButton FindTargetButton { get; }

        IButton OpenTargetButton { get; }

        IToggle RequireConfirmToggleButton { get; }
      
    }
}
