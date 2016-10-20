using System;
using NUnit.Framework;
using Moq;
using Moq.Protected;
using SymlinkMaker.Core;
using System.Collections.Generic;

namespace SymlinkMaker.CLI.Tests
{

    [TestFixture]
    public class CLICommandAdapterTests
    {
        private Mock<ICommand> _commandMock;
        private Mock<IConsoleHelper> _consoleHelperMock;
        private CLICommandAdapter _commandAdapter;

        private Operation _showTitle;
        private const string _commandTitle = "This will do something to {0} and {1}";
        private readonly string[] _commandTitleArgs = { "path", "secondPath" };

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _commandMock = new Mock<ICommand>();
            // Add the ShowTitle delegate to the list
            _commandMock
                .Setup(cmd => cmd.RegisterPreExecutionValidation(It.IsAny<Operation>()))
                .Callback<Operation>(op => _showTitle = op);
            
            _consoleHelperMock = new Mock<IConsoleHelper>();
        }

        [SetUp]
        public void BeforeEachSetUp()
        {
            _commandAdapter = new CLICommandAdapter(
                _commandMock.Object,
                _consoleHelperMock.Object,
                _commandTitle,
                _commandTitleArgs
            );
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _commandMock.Reset();

            _consoleHelperMock.Reset();
        }

        #endregion

        [Test]
        public void Constructor_WithNullConsoleHelper_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException)).And
                    .Property("Message").Contains("consoleHelper"),
                () => new CLICommandAdapter(new Mock<ICommand>().Object, null)
            );
        }

        #region Event handlers tests

        [Test]
        public void OnSuccess_ShouldWriteToConsoleHelper()
        {
            _commandMock.Raise(
                cmd => cmd.Succeeded += null, 
                (EventArgs)null
            );

            _consoleHelperMock.Verify
            (
                console => console.WriteLineColored(
                    "DONE",
                    _commandAdapter.DoneColor
                ),
                Times.Once
            );
        }

        [Test]
        public void OnFailure_ShouldWriteToConsoleHelper()
        {
            _commandMock.Raise(
                cmd => cmd.Failed += null, 
                (EventArgs)null
            );

            _consoleHelperMock.Verify
            (
                console => console.WriteLineColored(
                    "FAILED",
                    _commandAdapter.ErrorColor
                ),
                Times.Once
            );
        }

        [Test]
        public void OnException_ShouldWriteToConsoleHelper()
        {
            _commandMock.Raise(
                cmd => cmd.ExceptionThrown += null, 
                new CommandExceptionEventArgs(
                    new Dictionary<string, string>(),
                    new Mock<Exception>().Object
                )
            );

            _consoleHelperMock.Verify
            (
                console => console.WriteColored(
                    It.IsAny<string>(),
                    _commandAdapter.ErrorColor,
                    null,
                    Console.Error.WriteLine
                ),
                Times.Once
            );
        }

        #endregion

        [Test]
        public void ShowTitle_WithArguments_ShouldWriteTheCorrectMessage()
        {
            IDictionary<string, string> args = new Dictionary<string, string>();

            foreach (var arg in _commandTitleArgs)
                args.Add(arg, arg);

            string message = string.Format(_commandTitle, _commandTitleArgs);

            _showTitle(args);

            _consoleHelperMock.Verify
            (
                console => console.WriteColored(
                    message,
                    _commandAdapter.TitleColor
                ),

                Times.Once
            );
        }

        [Test]
        public void ConfirmationHandler_WhenNisPressed_ShouldReturnFalse()
        {
            var nKey = new ConsoleKeyInfo(
                           'N',
                           ConsoleKey.N,
                           false,
                           false,
                           false);
            Operation confirmationHandler = null;

            // Register a value to return when the ReadKey() is called
            _consoleHelperMock
                .Setup(helper => helper.ReadKey())
                .Returns(nKey);

            // Call the ConfirmationHandler Operation when it is registered
            _commandMock
                .Setup(cmd => cmd.RegisterPreExecutionValidation(It.IsAny<Operation>()))
                .Callback<Operation>(op => confirmationHandler = op);
           
            // Run the command to get the ConfirmationHandler
            _commandAdapter.Execute(null);

            Assert.IsFalse(confirmationHandler(null));
        }

        [Test]
        public void ConfirmationHandler_WhenNotNisPressed_ShouldReturnTrue()
        {
            var pKey = new ConsoleKeyInfo(
                           'p',
                           ConsoleKey.P,
                           false,
                           false,
                           false);
            
            Operation confirmationHandler = null;

            // Register a value to return when the ReadKey() is called
            _consoleHelperMock
                .Setup(helper => helper.ReadKey())
                .Returns(pKey);

            // Call the ConfirmationHandler Operation when it is registered
            _commandMock
                .Setup(cmd => cmd.RegisterPreExecutionValidation(It.IsAny<Operation>()))
                .Callback<Operation>(op => confirmationHandler = op);
           
            // Run the command to get the ConfirmationHandler
            _commandAdapter.Execute(null);

            Assert.IsTrue(confirmationHandler(null));
        }

        [Test]
        public void ConfirmationHandler_WhenCalled_ShouldShowCorrectOptions()
        {
            // Call the ConfirmationHandler Operation when it is registered
            _commandMock
                .Setup(cmd => cmd.RegisterPreExecutionValidation(It.IsAny<Operation>()))
                .Callback<Operation>(op => op(null));
           
            // Run the command to get the ConfirmationHandler
            _commandAdapter.Execute(null);

            // Check that the message shown was correct
            _consoleHelperMock.Verify
            (
                console => console.WriteColored(
                    " (Y/n)?",
                    _commandAdapter.ConfirmColor
                ),
                Times.Once
            );
        }
    }


}

