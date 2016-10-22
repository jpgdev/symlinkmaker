namespace SymlinkMaker.Core
{
    public class AppSettings : Notifier
    {
        #region Attributes

        private bool _requiresConfirmation;
        private string _sourcePath;
        private string _targetPath;
        private IFileSystemOperations _fileOperations;

        #endregion

        #region Properties

        public bool RequiresConfirmation
        { 
            get { return _requiresConfirmation; }
            set
            {
                if (value != _requiresConfirmation)
                {
                    _requiresConfirmation = value;
                    NotifyPropertyChanged();                    
                }
            }
        }

        public string SourcePath
        { 
            get { return _sourcePath; }
            set
            {
                if (value != _sourcePath)
                {
                    _sourcePath = value;
                    NotifyPropertyChanged();                    
                }
            }
        }

        public string TargetPath
        { 
            get { return _targetPath; }
            set
            {
                if (value != _targetPath)
                {
                    _targetPath = value;
                    NotifyPropertyChanged();                    
                }
            }
        }

        public IFileSystemOperations FileOperations
        { 
            get { return _fileOperations; }
            set
            {
                if (value != _fileOperations)
                {
                    _fileOperations = value;
                    NotifyPropertyChanged();                    
                }
            }
        }

        #endregion
    }
}
