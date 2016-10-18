namespace SymlinkMaker.GUI
{
    public interface IDialogHelper
    {
        bool ShowDialog(DialogType type, string content, string title = null);

        string ShowFileChooserDialog(string basePath,
                                     ChooserDialogAction action,
                                     string title = null,
                                     string acceptButtonContent = null) ;
    }
}

