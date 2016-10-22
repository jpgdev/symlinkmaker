using Moq;
using NUnit.Framework;
using SymlinkMaker.GUI.GtkSharp;
using System;
using SymlinkMaker.Core;
using System.Collections.Generic;
using System.Linq;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{

    [TestFixture]
    public class GtkSharpCommandAdapterTests
    {
        private readonly string SOURCE_PATH_DEFAULT = "/source/path/default/";
        private readonly string TARGET_PATH_DEFAULT = "/target/path/default/";

        #region Fields

        private Mock<ICommand> _commandMock;
        private Mock<IDialogHelper> _dialogHelper;
        private GtkSharpCommandAdapter _commandAdapter;
        private GtkSharpCommandConfirmationInfo _confirmInfo;
        private Dictionary<string, string> _basicArguments;
        private Operation _confirmationHandlerOperation = null;

        #endregion

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _basicArguments = new Dictionary<string, string>()
            {
                { "sourcePath", SOURCE_PATH_DEFAULT },
                { "targetPath", TARGET_PATH_DEFAULT }
            };

            _confirmInfo = new GtkSharpCommandConfirmationInfo(
                "Do an action from {0} to {1}", 
                new []
                {
                    "sourcePath",
                    "targetPath"
                }, 
                "Are you sure you want to do the action from {0} to {1}", 
                new []
                {
                    "sourcePath",
                    "targetPath"
                });
            
            _commandMock = new Mock<ICommand>();
            _dialogHelper = new Mock<IDialogHelper>();
        }

        [SetUp]
        public void BeforeEachSetUp()
        {
            _commandMock.SetupAllProperties();
            _commandMock
                .Setup(c => c.RegisterPreExecutionValidation(It.IsAny<Operation>()))
                .Callback<Operation>(op => _confirmationHandlerOperation = op);
            
            _commandAdapter = new GtkSharpCommandAdapter(
                _commandMock.Object,
                _dialogHelper.Object,
                _confirmInfo
            );
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _commandMock.Reset();
            _dialogHelper.Reset();
        }

        #endregion

        #region Constructor Tests

        [Test]
        public void Constructor_WithoutDialogHelper_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>()
                .And.Property("Message").Contains("dialogHelper"),
                () => new GtkSharpCommandAdapter(
                    _commandMock.Object,
                    null
                )
            );
        }

        #endregion

        #region Confirmation handler Tests

        [Test]
        public void Execute_WhenRequiresConfirmIsTrue_ShouldShowConfirmDialogWithCorrectContent()
        {
            _commandAdapter.Execute(_basicArguments, true);
            _confirmationHandlerOperation(_basicArguments);

            _dialogHelper.Verify
            (
                helper => helper.ShowDialog(
                    DialogType.Confirmation,
                    GetFormattedMessageStringWithArgs(
                        _basicArguments,
                        _confirmInfo),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [Test]
        public void Execute_WhenRequiresConfirmIsTrue_ShouldShowConfirmDialogWithCorrectTitle()
        {
            _commandAdapter.Execute(_basicArguments, true);
            _confirmationHandlerOperation(_basicArguments);

            _dialogHelper.Verify
            (
                helper => helper.ShowDialog(
                    DialogType.Confirmation,
                    It.IsAny<string>(),
                    GetFormattedTitleStringWithArgs(
                        _basicArguments,
                        _confirmInfo)
                ),
                Times.Once
            );
        }

        #endregion

        #region Failure dialog Tests

        [Test]
        public void OnCommandException_WithoutArguments_ShouldShowFailureDialog()
        {
            const string exceptionMessage = "This is an error.";

            _commandMock.Raise(
                c => c.ExceptionThrown += null,
                _commandMock.Object,
                new CommandExceptionEventArgs(
                    null,
                    new Exception(exceptionMessage),
                    CommandStatus.Running
                )
            );

            _dialogHelper.Verify(
                helper => helper.ShowDialog(
                    DialogType.Error,
                    It.IsRegex(exceptionMessage),
                    null)
            );
        }


        [Test]
        public void OnCommandExceptionOrFailure_WithArguments_ShouldShowFailureDialogWithPaths()
        {
            const string exceptionMessage = "This is an error.";

            _commandMock.Raise(
                c => c.ExceptionThrown += null,
                _commandMock.Object,
                new CommandExceptionEventArgs(
                    _basicArguments,
                    new Exception(exceptionMessage),
                    CommandStatus.Running
                )
            );

            /* Example content for the Regex:
             *
             * Error : This is an error message
             * 
             * Source: /source/path/
             * Target: /target/path/ 
             */
            string regex = string.Format(
                               "Error{2}{3}{2}Source:.*{0}{2}Target:.*{1}",
                               _basicArguments["sourcePath"],
                               _basicArguments["targetPath"],
                               "(?s).*", // This check for a NewLine character or anything else
                               exceptionMessage
                           );

            _dialogHelper.Verify(
                helper => helper.ShowDialog(
                    DialogType.Error,
                    It.IsRegex(regex),
                    null)
            );
        }

        [Test]
        public void OnCommandFailure_WithoutArguments_ShouldShowFailureDialog()
        {
            _commandMock.Raise(
                c => c.Failed += null,
                _commandMock.Object,
                new CommandEventArgs(
                    null,
                    CommandStatus.Failed
                )
            );

            _dialogHelper.Verify(
                helper => helper.ShowDialog(
                    DialogType.Error,
                    It.IsRegex("Operation failed"),
                    null)
            );
        }

        #endregion

        #region Helpers

        private static string[] GetArgsValuesFromArgsName(IDictionary<string, string> argsValues,
                                                          string[] argsNames)
        {
            return argsNames
                .Select(argName => argsValues[argName])
                .ToArray();
        }

        private static string GetFormattedTitleStringWithArgs(
            Dictionary<string, string> args, 
            GtkSharpCommandConfirmationInfo confirmInfo
        )
        {
            return FormatStringWithArgs(
                args,
                confirmInfo.DialogTitleArgsNames,
                confirmInfo.DialogTitle);
        }

        private static string GetFormattedMessageStringWithArgs(
            Dictionary<string, string> args, 
            GtkSharpCommandConfirmationInfo confirmInfo
        )
        {
            return FormatStringWithArgs(
                args,
                confirmInfo.MessageArgsNames,
                confirmInfo.Message);
        }

        private static string FormatStringWithArgs(
            Dictionary<string, string> args, 
            string[] argNames,
            string strToFormat
        )
        {

            if (strToFormat == null)
                return null;

            if (argNames == null)
                return strToFormat;

            return string.Format(
                strToFormat,
                GetArgsValuesFromArgsName(args, argNames)
            );
        }

        #endregion
    }
}




