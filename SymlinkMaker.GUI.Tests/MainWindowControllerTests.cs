using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SymlinkMaker.Core;
using System.Text.RegularExpressions;

namespace SymlinkMaker.GUI.Tests
{
    [TestFixture]
    public class MainWindowControllerTests
    {
        
        // TODO : Things left to tests
        /*
         *  - OpenSource (button)
         *  - OpenTarget (button)
         */

        #region Constants

        private readonly string SOURCE_PATH_DEFAULT = "/source/path/default/";
        private readonly string TARGET_PATH_DEFAULT = "/target/path/default/";
        private readonly string NEW_SOURCE_PATH = "/new/source/path";
        private readonly string NEW_TARGET_PATH = "/new/target/path";

        private const string BUTTON_COPY_SOURCE_KEY = "COPY_SOURCE";
        private const string BUTTON_MOVE_SOURCE_KEY = "MOVE_SOURCE";
        private const string BUTTON_CREATE_SYMLINK_KEY = "CREATESYMLINK";
        private const string BUTTON_DELETE_SOURCE_KEY = "DELETE_SOURCE";
        private const string BUTTON_DELETE_TARGET_KEY = "DELETE_TARGET";
        private const string BUTTON_DO_ALL_KEY = "DO_ALL";

        private const string BUTTON_FIND_SOURCE_KEY = "FIND_SOURCE";
        private const string BUTTON_FIND_TARGET_KEY = "FIND_TARGET";
        private const string BUTTON_OPEN_SOURCE_KEY = "OPEN_SOURCE";
        private const string BUTTON_OPEN_TARGET_KEY = "OPEN_TARGET";

        //        private const string BUTTON_TOGGLE_REQUIRE_CONFIRM_KEY = "REQUIRES_CONFIRM";

        #endregion

        #region Fields

        private AppSettings _settings;
        private Mock<IFileSystemOperations> _fileOperationsMock;
        private Mock<IMainWindowView> _viewMock;
        private Mock<ICommandsManager<CommandType>> _commandManagerMock;
        private Mock<IDialogHelper> _dialogHelperMock;
        private Dictionary<string, Mock<IButton>> _commandButtonsMocks;
        private MainWindowController _windowController;

        // Mocked view controls
        private Mock<IImage> _sourceStatusImage;
        private Mock<IImage> _targetStatusImage;
        private Mock<ITextSource> _sourcePathMock;
        private Mock<ITextSource> _targetPathMock;
        private Mock<IButton> _copySourceButtonMock;
        private Mock<IButton> _moveSourceButtonMock;
        private Mock<IButton> _deleteSourceButtonMock;
        private Mock<IButton> _deleteTargetButtonMock;
        private Mock<IButton> _createSymlinkButtonMock;
        private Mock<IButton> _doAllButtonMock;
        private Mock<IButton> _findSourceButtonMock;
        private Mock<IButton> _openSourceButtonMock;
        private Mock<IButton> _findTargetButtonMock;
        private Mock<IButton> _openTargetButtonMock;
        private Mock<IToggle> _requireConfirmToggleButtonMock;

        #endregion

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _commandButtonsMocks = new Dictionary<string, Mock<IButton>>(); 
            _fileOperationsMock = new Mock<IFileSystemOperations>();
            _commandManagerMock = new Mock<ICommandsManager<CommandType>>();
            _dialogHelperMock = new Mock<IDialogHelper>();
            _viewMock = CreateMockAndSetupProperties<IMainWindowView>();

            ResetAppSettings();
            CreateViewMockedControls();
            InitializeViewMockedProperties();
        }

        [SetUp]
        public void BeforeEachSetUp()
        {
            _windowController = new MainWindowController(
                _settings,
                _viewMock.Object,
                _commandManagerMock.Object,
                _dialogHelperMock.Object 
            );
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            ResetAppSettings();

            // Reset all View mocked controls
            ResetMockSetupProperties(_sourceStatusImage);
            ResetMockSetupProperties(_targetStatusImage);
            ResetMockSetupProperties(_sourcePathMock);
            ResetMockSetupProperties(_targetPathMock);
            ResetMockSetupProperties(_copySourceButtonMock);
            ResetMockSetupProperties(_deleteSourceButtonMock);
            ResetMockSetupProperties(_deleteTargetButtonMock);
            ResetMockSetupProperties(_createSymlinkButtonMock);
            ResetMockSetupProperties(_doAllButtonMock);
            ResetMockSetupProperties(_findSourceButtonMock);
            ResetMockSetupProperties(_openSourceButtonMock);
            ResetMockSetupProperties(_findTargetButtonMock);
            ResetMockSetupProperties(_openTargetButtonMock);
            ResetMockSetupProperties(_requireConfirmToggleButtonMock);

            // Only reset the calls, otherwise we lose the connection between the 
            // view and the mocked controls (which are Properties)
            _viewMock.ResetCalls();
            _fileOperationsMock.ResetCalls();
            _commandManagerMock.ResetCalls();
            _dialogHelperMock.ResetCalls();

            _windowController.Dispose();
            _windowController = null;
        }

        private void ResetAppSettings()
        {
            // TODO : Use an Interface or a Loader of some kind?
            _settings = new AppSettings()
            {
                FileOperations = _fileOperationsMock.Object,
                SourcePath = SOURCE_PATH_DEFAULT,
                TargetPath = TARGET_PATH_DEFAULT,
                RequiresConfirmation = true
            };
        }

        private void CreateViewMockedControls()
        {
            _sourceStatusImage = CreateMockAndSetupProperties<IImage>();
            _targetStatusImage = CreateMockAndSetupProperties<IImage>();

            _sourcePathMock = CreateMockAndSetupProperties<ITextSource>();
            _targetPathMock = CreateMockAndSetupProperties<ITextSource>();

            _copySourceButtonMock = CreateMockAndSetupProperties<IButton>();
            _moveSourceButtonMock = CreateMockAndSetupProperties<IButton>();
            _deleteSourceButtonMock = CreateMockAndSetupProperties<IButton>();
            _deleteTargetButtonMock = CreateMockAndSetupProperties<IButton>();
            _createSymlinkButtonMock = CreateMockAndSetupProperties<IButton>();
            _doAllButtonMock = CreateMockAndSetupProperties<IButton>();
            _findSourceButtonMock = CreateMockAndSetupProperties<IButton>();
            _openSourceButtonMock = CreateMockAndSetupProperties<IButton>();
            _findTargetButtonMock = CreateMockAndSetupProperties<IButton>();
            _openTargetButtonMock = CreateMockAndSetupProperties<IButton>();

            _requireConfirmToggleButtonMock = CreateMockAndSetupProperties<IToggle>();
//
//            _sourceStatusImage = new Mock<IImage>();
//            _targetStatusImage = new Mock<IImage>();
//
//            _sourcePathMock = new Mock<IText>();
//            _targetPathMock = new Mock<IText>();
//
//            _copySourceButtonMock = new Mock<IButton>();
//            _moveSourceButtonMock = new Mock<IButton>();
//            _deleteSourceButtonMock = new Mock<IButton>();
//            _deleteTargetButtonMock = new Mock<IButton>();
//            _createSymlinkButtonMock = new Mock<IButton>();
//            _doAllButtonMock = new Mock<IButton>();
//            _findSourceButtonMock = new Mock<IButton>();
//            _openSourceButtonMock = new Mock<IButton>();
//            _findTargetButtonMock = new Mock<IButton>();
//            _openTargetButtonMock = new Mock<IButton>();
//            _requireConfirmToggleButtonMock = new Mock<IToggleButton>();

            _commandButtonsMocks[BUTTON_COPY_SOURCE_KEY] = _copySourceButtonMock;
            _commandButtonsMocks[BUTTON_MOVE_SOURCE_KEY] = _moveSourceButtonMock;
            _commandButtonsMocks[BUTTON_DELETE_SOURCE_KEY] = _deleteSourceButtonMock;
            _commandButtonsMocks[BUTTON_DELETE_TARGET_KEY] = _deleteTargetButtonMock;
            _commandButtonsMocks[BUTTON_CREATE_SYMLINK_KEY] = _createSymlinkButtonMock;
            _commandButtonsMocks[BUTTON_DO_ALL_KEY] = _doAllButtonMock;
            _commandButtonsMocks[BUTTON_FIND_SOURCE_KEY] = _findSourceButtonMock;
            _commandButtonsMocks[BUTTON_FIND_TARGET_KEY] = _findTargetButtonMock;
            _commandButtonsMocks[BUTTON_OPEN_SOURCE_KEY] = _openSourceButtonMock;
            _commandButtonsMocks[BUTTON_OPEN_TARGET_KEY] = _openTargetButtonMock;
        }

        private void InitializeViewMockedProperties()
        {
            _viewMock
                .SetupGet(v => v.SourceStatusImage)
                .Returns(_sourceStatusImage.Object);

            _viewMock
                .SetupGet(v => v.TargetStatusImage)
                .Returns(_targetStatusImage.Object);

            _viewMock
                .SetupGet(v => v.SourcePath)
                .Returns(_sourcePathMock.Object);

            _viewMock
                .SetupGet(v => v.TargetPath)
                .Returns(_targetPathMock.Object);

            _viewMock
                .SetupGet(v => v.CopySourceButton)
                .Returns(_copySourceButtonMock.Object);

            _viewMock
                .SetupGet(v => v.MoveSourceButton)
                .Returns(_moveSourceButtonMock.Object);

            _viewMock
                .SetupGet(v => v.DeleteSourceButton)
                .Returns(_deleteSourceButtonMock.Object);

            _viewMock
                .SetupGet(v => v.DeleteTargetButton)
                .Returns(_deleteTargetButtonMock.Object);

            _viewMock
                .SetupGet(v => v.CreateSymlinkButton)
                .Returns(_createSymlinkButtonMock.Object);

            _viewMock
                .SetupGet(v => v.DoAllButton)
                .Returns(_doAllButtonMock.Object);

            _viewMock
                .SetupGet(v => v.FindSourceButton)
                .Returns(_findSourceButtonMock.Object);

            _viewMock
                .SetupGet(v => v.OpenSourceButton)
                .Returns(_openSourceButtonMock.Object);

            _viewMock
                .SetupGet(v => v.FindTargetButton)
                .Returns(_findTargetButtonMock.Object);

            _viewMock
                .SetupGet(v => v.OpenTargetButton)
                .Returns(_openTargetButtonMock.Object);

            _viewMock
                .SetupGet(v => v.RequireConfirmToggleButton)
                .Returns(_requireConfirmToggleButtonMock.Object);
        }

        #endregion

        #region Helpers

        private static Mock<T> CreateMockAndSetupProperties<T>()
            where T : class
        {
            var mock = new Mock<T>();
            mock.SetupAllProperties();

            return mock;
        }

        private static void ResetMockSetupProperties<T>(Mock<T> mock)
            where T : class
        {
            mock.Reset();
            mock.SetupAllProperties();
        }

        #endregion

        #region Contructor Tests

        [Test]
        public void Constructor_WithoutAppSettings_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                .And.Property("Message").Contains("settings"),
                () => new MainWindowController(
                    null, 
                    _viewMock.Object,
                    _commandManagerMock.Object,
                    _dialogHelperMock.Object
                )
            );
        }

        [Test]
        public void Constructor_WithoutView_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                .And.Property("Message").Contains("view"),
                () => new MainWindowController(
                    _settings,
                    null,
                    _commandManagerMock.Object,
                    _dialogHelperMock.Object
                )
            );
        }

        [Test]
        public void Constructor_WithoutCommandManager_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                .And.Property("Message").Contains("commandManager"),
                () => new MainWindowController(
                    _settings,
                    _viewMock.Object,
                    null,
                    _dialogHelperMock.Object
                )
            );
        }

        [Test]
        public void Constructor_WithoutDialogHelper_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                .And.Property("Message").Contains("dialogHelper"),
                () => new MainWindowController(
                    _settings,
                    _viewMock.Object,
                    _commandManagerMock.Object,
                    null
                )
            );
        }

        [Test]
        public void Constructor_WhenValid_ShouldSetSourcePathFromSettings()
        {
            _sourcePathMock.SetupProperty(
                p => p.Text
            );

            // We need to call the Dispose, otherwise EventHandler are left out
            using (new MainWindowController(
                       _settings,
                       _viewMock.Object,
                       _commandManagerMock.Object,
                       _dialogHelperMock.Object 
                   ))
            {

                Assert.AreEqual(
                    _settings.SourcePath,
                    _sourcePathMock.Object.Text);
            }
        }

        [Test]
        public void Constructor_WhenValid_ShouldSetTargetPathFromSettings()
        {
            _targetPathMock.SetupProperty(
                p => p.Text
            );

            // We need to call the Dispose, otherwise EventHandler are left out
            using (new MainWindowController(
                       _settings,
                       _viewMock.Object,
                       _commandManagerMock.Object,
                       _dialogHelperMock.Object 
                   ))
            {

                Assert.AreEqual(
                    _settings.TargetPath,
                    _targetPathMock.Object.Text);
            }
        }

        [Test]
        public void Constructor_WhenValid_ShouldSetRequiresConfirmFromSettings()
        {
            _requireConfirmToggleButtonMock.SetupProperty(
                t => t.IsActive
            );

            // We need to call the Dispose, otherwise EventHandler are left out
            using (new MainWindowController(
                       _settings,
                       _viewMock.Object,
                       _commandManagerMock.Object,
                       _dialogHelperMock.Object 
                   ))
            {

                Assert.AreEqual(
                    _settings.RequiresConfirmation,
                    _requireConfirmToggleButtonMock.Object.IsActive);
            }
        }

        #endregion

        #region Command Button Event Handlers Tests

        [Test]
        [TestCase(BUTTON_COPY_SOURCE_KEY, CommandType.Copy)]
        [TestCase(BUTTON_MOVE_SOURCE_KEY, CommandType.Move)]
        [TestCase(BUTTON_CREATE_SYMLINK_KEY, CommandType.CreateSymLink)]
        [TestCase(BUTTON_DO_ALL_KEY, CommandType.All)]
        public void TwoArgsCommandButtonEventHandler_WhenTriggered_ShouldRunTheCorrectCommand(
            string buttonMockName,
            CommandType commandType
        )
        {
            var buttonMock = _commandButtonsMocks[buttonMockName];
            buttonMock.Raise(
                e => e.Triggered += null,
                buttonMock.Object,
                new ButtonEventArgs()
            );

            _commandManagerMock.Verify
            (
                cmd => cmd.Execute(
                    commandType,
                    _settings.SourcePath,
                    _settings.TargetPath,
                    _settings.RequiresConfirmation
                ),
                Times.Once
            );
        }

        [Test]
        [TestCase(BUTTON_DELETE_SOURCE_KEY, CommandType.Delete)]
        public void OneArgCommandButtonEventHandler_WhenTriggered_ShouldRunTheCorrectCommand(
            string buttonMockName,
            CommandType commandType
        )
        {
            var buttonMock = _commandButtonsMocks[buttonMockName];
            buttonMock
                .Raise(
                e => e.Triggered += null,
                buttonMock.Object,
                new ButtonEventArgs());

            _commandManagerMock.Verify
            (
                cmd => cmd.Execute(
                    commandType,
                    _settings.SourcePath,
                    null,
                    _settings.RequiresConfirmation
                ),
                Times.Once
            );
        }

        [Test]
        public void DeleteTargetButtonEventHandler_WhenTriggered_ShouldRunDeleteCommand()
        {
            _deleteTargetButtonMock
                .Raise(
                e => e.Triggered += null,
                _deleteTargetButtonMock.Object,
                new ButtonEventArgs());

            _commandManagerMock.Verify
            (
                cmd => cmd.Execute(
                    CommandType.Delete, 
                    _settings.TargetPath,
                    null,
                    _settings.RequiresConfirmation
                ),
                Times.Once
            );
        }

        [Test]
        [TestCase(BUTTON_COPY_SOURCE_KEY)]
        [TestCase(BUTTON_MOVE_SOURCE_KEY)]
        [TestCase(BUTTON_CREATE_SYMLINK_KEY)]
        [TestCase(BUTTON_DELETE_SOURCE_KEY)]
        [TestCase(BUTTON_DELETE_TARGET_KEY)]
        [TestCase(BUTTON_DO_ALL_KEY)]
        public void SourceStatusImage_AfterCommandIsRan_ShouldUpdateImage(string buttonMockName)
        {
            var button = _commandButtonsMocks[buttonMockName];

            // Since it will be called in the constructor, we reset the calls
            _fileOperationsMock.ResetCalls();
            _fileOperationsMock
                .Setup(op => op.Exists(SOURCE_PATH_DEFAULT))
                .Returns(true);

            button.Raise(
                e => e.Triggered += null,
                button.Object,
                new ButtonEventArgs());

            _fileOperationsMock.Verify
            (
                op => op.Exists(SOURCE_PATH_DEFAULT),
                Times.Once
            );

            Assert.AreEqual("yes", _sourceStatusImage.Object.Name);
            Assert.IsTrue(Regex.IsMatch(
                    _sourceStatusImage.Object.Tooltip, 
                    "occupied"
                ));
        }

        [Test]
        [TestCase(BUTTON_COPY_SOURCE_KEY)]
        [TestCase(BUTTON_MOVE_SOURCE_KEY)]
        [TestCase(BUTTON_CREATE_SYMLINK_KEY)]
        [TestCase(BUTTON_DELETE_SOURCE_KEY)]
        [TestCase(BUTTON_DELETE_TARGET_KEY)]
        [TestCase(BUTTON_DO_ALL_KEY)]
        public void TargetStatusImage_AfterCommandIsRan_ShouldUpdateImage(string buttonMockName)
        {
            var button = _commandButtonsMocks[buttonMockName];

            // Since it will be called in the constructor, we reset the calls
            _fileOperationsMock.ResetCalls();
            _fileOperationsMock
                .Setup(op => op.Exists(TARGET_PATH_DEFAULT))
                .Returns(true);

            button.Raise(
                e => e.Triggered += null,
                button.Object,
                new ButtonEventArgs());

            _fileOperationsMock.Verify
            (
                op => op.Exists(TARGET_PATH_DEFAULT),
                Times.Once
            );

            Assert.AreEqual("no", _targetStatusImage.Object.Name);
            Assert.IsTrue(Regex.IsMatch(
                    _targetStatusImage.Object.Tooltip, 
                    "occupied"
                ));
        }

        #endregion

        #region Other Buttons Event Handlers Tests

        [Test]
        public void FindSourceButtonEventHandler_WhenTriggered_ShouldShowChooseFileDialog()
        {
            _findSourceButtonMock
                .Raise(
                e => e.Triggered += null,
                _findSourceButtonMock.Object,
                EventArgs.Empty);

            _dialogHelperMock.Verify
            (
                cmd => cmd.ShowFileChooserDialog(
                    _settings.SourcePath,
                    ChooserDialogAction.FindDirectory,
                    It.IsRegex("source"),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Test]
        public void FindTargetButtonEventHandler_WhenTriggered_ShouldShowChooseFileDialog()
        {
            _findTargetButtonMock
                .Raise(
                e => e.Triggered += null,
                _findTargetButtonMock.Object,
                EventArgs.Empty);

            _dialogHelperMock.Verify
            (
                cmd => cmd.ShowFileChooserDialog(
                    _settings.TargetPath,
                    ChooserDialogAction.FindDirectory,
                    It.IsRegex("target"),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Test]
        public void RequireConfrimButtonEventHandler_WhenStatusChanged_ShouldChangeSettings()
        {
            _settings.RequiresConfirmation = false;

            _requireConfirmToggleButtonMock
                .Raise(
                e => e.StatusChanged += null,
                _requireConfirmToggleButtonMock.Object,
                new ToggleEventArgs(true)
            );

            Assert.IsTrue(_settings.RequiresConfirmation);
        }

        #endregion

        #region Text Changed Event Handlers Tests

        [Test]
        public void SourcePathEventHandler_WhenTextChanged_ShouldSetTextInSettings()
        {
            _sourcePathMock
                .SetupGet(t => t.Text)
                .Returns(NEW_SOURCE_PATH);
                
            _sourcePathMock.Raise(
                e => e.TextChanged += null,
                _sourcePathMock.Object,
                EventArgs.Empty);

            Assert.AreEqual(_settings.SourcePath, NEW_SOURCE_PATH);
        }

        [Test]
        public void TargetPathEventHandler_WhenTextChanged_ShouldSetTextInSettings()
        {
            _targetPathMock
                .SetupGet(t => t.Text)
                .Returns(NEW_TARGET_PATH);

            _targetPathMock.Raise(
                e => e.TextChanged += null,
                _targetPathMock.Object,
                EventArgs.Empty);

            Assert.AreEqual(_settings.TargetPath, NEW_TARGET_PATH);
        }

        [Test]
        [TestCase(true, "yes", "occupied")]
        [TestCase(false, "no", "free")]
        public void SourceStatusImage_SourcePathIsChanged_ShouldUpdateImage(
            bool fileExists,
            string imageName,
            string tooltipPart
        )
        {
            // Since it will be called in the constructor, we reset the calls
            _fileOperationsMock.ResetCalls();
            _fileOperationsMock
                .Setup(op => op.Exists(NEW_SOURCE_PATH))
                .Returns(fileExists);

            _settings.SourcePath = NEW_SOURCE_PATH;

            _fileOperationsMock.Verify
            (
                op => op.Exists(NEW_SOURCE_PATH),
                Times.Once
            );

            Assert.AreEqual(imageName, _sourceStatusImage.Object.Name);
            Assert.IsTrue(Regex.IsMatch(
                    _sourceStatusImage.Object.Tooltip, 
                    tooltipPart
                ));
        }

        [Test]
        [TestCase(true, "no", "occupied")]
        [TestCase(false, "yes", "free")]
        public void TargetStatusImage_TargetPathIsChanged_ShouldUpdateImage(
            bool fileExists,
            string imageName,
            string tooltipPart
        )
        {
            // Arrange
            // Since it will be called in the constructor, we reset the calls
            _fileOperationsMock.ResetCalls();
            _fileOperationsMock
                .Setup(op => op.Exists(NEW_TARGET_PATH))
                .Returns(fileExists);

            // Act
            _settings.TargetPath = NEW_TARGET_PATH;

            // Assert
            _fileOperationsMock.Verify
            (
                op => op.Exists(NEW_TARGET_PATH),
                Times.Once
            );

            Assert.AreEqual(imageName, _targetStatusImage.Object.Name);
            Assert.IsTrue(Regex.IsMatch(
                    _targetStatusImage.Object.Tooltip, 
                    tooltipPart
                ));
        }

        #endregion

        #region App Settings Event Handlers Tests

        [Test]
        public void AppSettingsEventHandler_WhenSourcePathChanged_ShouldSetSourcePathControlText()
        {
            _settings.SourcePath = NEW_SOURCE_PATH;

            _sourcePathMock.VerifySet(
                t => t.Text = NEW_SOURCE_PATH,
                Times.Once
            );
        }

        [Test]
        public void AppSettingsEventHandler_WhenTargetPathChanged_ShouldSetTargetPathControlText()
        {
            _settings.TargetPath = NEW_TARGET_PATH;

            _targetPathMock.VerifySet(
                t => t.Text = NEW_TARGET_PATH,
                Times.Once
            );
        }

        [Test]
        public void AppSettingsEventHandler_WhenRequireConfrimStatusChanged_ShouldSetRequireConfirmControl()
        {
            _settings.RequiresConfirmation = !_settings.RequiresConfirmation;

            _requireConfirmToggleButtonMock.VerifySet(
                t => t.IsActive = _settings.RequiresConfirmation,
                Times.Once
            );
        }

        #endregion
    }
}


