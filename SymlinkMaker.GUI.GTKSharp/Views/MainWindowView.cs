using Gtk;
using System;

namespace SymlinkMaker.GUI.GTKSharp
{
    public partial class MainWindowView : GtkSharpWindow, IMainWindowView
    {

        #region IMainWindowView Properties

        public IImage SourceStatusImage { get; private set; }

        public IImage TargetStatusImage { get; private set; }

        public ITextSource SourcePath { get; private set; }

        public ITextSource TargetPath { get; private set; }

        public IButton CopySourceButton { get; private set; }

        public IButton MoveSourceButton { get; private set; }

        public IButton DeleteSourceButton { get; private set; }

        public IButton DeleteTargetButton { get; private set; }

        public IButton CreateSymlinkButton { get; private set; }

        public IButton DoAllButton { get; private set; }

        public IButton FindSourceButton { get; private set; }

        public IButton OpenSourceButton { get; private set; }

        public IButton FindTargetButton { get; private set; }

        public IButton OpenTargetButton { get; private set; }

        public IToggle RequireConfirmToggleButton { get; private set; }

        #endregion

        public MainWindowView(IGtkIconNameConverter iconNameConverter)
            : base(WindowType.Toplevel)
        {
            if (iconNameConverter == null)
                throw new ArgumentNullException(nameof(iconNameConverter));

            this.Build();

            // Image
            SourceStatusImage = new GtkSharpImage(imgSourcePath, "yes", iconNameConverter);
            TargetStatusImage = new GtkSharpImage(imgTargetPath, "yes", iconNameConverter);

            // Text Entries
            SourcePath = new GtkSharpTextEntry(txtBoxSource);
            TargetPath = new GtkSharpTextEntry(txtBoxTarget);

            // Command Buttons
            CopySourceButton = new GtkSharpButton(btnCopySource);
            MoveSourceButton = new GtkSharpButton(btnMoveSource);
            DeleteSourceButton = new GtkSharpButton(btnDeleteSource);
            DeleteTargetButton = new GtkSharpButton(btnDeleteTarget);
            CreateSymlinkButton = new GtkSharpButton(btnCreateSymlink);
            DoAllButton = new GtkSharpButton(btnDoWhole);

            // Find / Open path Buttons
            FindSourceButton = new GtkSharpButton(btnFindSource);
            FindTargetButton = new GtkSharpButton(btnFindTarget);
            OpenSourceButton = new GtkSharpButton(btnOpenSource);
            OpenTargetButton = new GtkSharpButton(btnOpenTarget);

            // Toggle Actions
            RequireConfirmToggleButton = new GtkSharpToggleAction(toggleRequireConfirmAction);
        }
    }
}
