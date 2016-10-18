using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SymlinkMaker.Core;

namespace SymlinkMaker.CLI.Tests
{

    [TestFixture]
    public class CLIApplicationTests
    {
        private CLIApplication _app;
        private Mock<AppSettings> _appSettingsMock;
        private Dictionary<CommandType, Mock<CommandAdapter>> _commandsMock;
        private Dictionary<CommandType, CommandAdapter> _commands;
        private Mock<IConsoleHelper> _consoleHelperMock;
        private Mock<ICLICommandParser> _commandParserMock;


        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _appSettingsMock = new Mock<AppSettings>();
            _commandParserMock = new Mock<ICLICommandParser>();
            _commandsMock = new Dictionary<CommandType, Mock<CommandAdapter>>();
            _commands = new Dictionary<CommandType, CommandAdapter>();

            CommandType[] typesToTest =
                {
                CommandType.Copy,
                CommandType.Move,
                CommandType.Delete,
                CommandType.CreateSymLink,
                CommandType.ShowHelp,
                CommandType.All
            };

            foreach (var commandType in typesToTest)
            {
                var commandMock = new Mock<ICommand>();
                var mock = new Mock<CommandAdapter>(
                               commandMock.Object 
                           );
                _commandsMock.Add(commandType, mock);
                _commands.Add(commandType, mock.Object);
            }

            _consoleHelperMock = new Mock<IConsoleHelper>();

            _app = new CLIApplication(
                _appSettingsMock.Object,
                _commands,
                _consoleHelperMock.Object,
                _commandParserMock.Object
            );
        }


        [TestFixtureTearDown]
        public void AfterAllTearDown()
        {
        }

        [SetUp]
        public void BeforeEachSetUp()
        {
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _commandParserMock.Reset();
            _appSettingsMock.Reset();
            _consoleHelperMock.Reset();

            foreach (var cmd in _commandsMock.Values)
                cmd.Reset();
        }

        #endregion

        [Test]
        public void Constructor_WithNullConsoleHelper_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                    .And.Property("Message").Contains("consoleHelper"),
                () => new CLIApplication(
                    _appSettingsMock.Object,
                    _commands,
                    null,
                    _commandParserMock.Object)
            );
        }

        [Test]
        public void Constructor_WithNullCommandParser_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                    .And.Property("Message").Contains("commandParser"),
                () => new CLIApplication(
                    _appSettingsMock.Object,
                    _commands,
                    _consoleHelperMock.Object,
                    null)
            );
        }

        #region Run method Tests

        [Test]
        public void Run_WithNullArgs_ShouldCallCommandParser()
        {
            _app.Run(null);

            _commandParserMock.Verify(
                parser => parser.ParseArgs(null),
                Times.Once
            );
        }

        [Test]
        public void Run_WithNoArgs_ShouldCallCommandParser()
        {
            string[] args = { };
            _app.Run(args);

            _commandParserMock.Verify(
                parser => parser.ParseArgs(args),
                Times.Once
            );
        }

        [Test]
        public void Run_WithArgs_ShouldRunCommand()
        {
            var fakeCommandInfo = new CLICommandInfo(
                CommandType.Copy,
                new Mock<IDictionary<string, string>>().Object,
                true);

            string[] args = {
                "command_name"   
            };

            _commandParserMock
                .Setup(parser => parser.ParseArgs(args))
                .Returns(fakeCommandInfo);

            _app.Run(args);

            _commandsMock[fakeCommandInfo.Type].Verify(
                cmd => cmd.Run(fakeCommandInfo.Arguments, fakeCommandInfo.RequiresConfirm),
                Times.Once
            );
        }

        [Test]
        public void Run_WithInvalidCommandInArgs_ShouldWriteExceptionToConsole()
        {
            var fakeCommandInfo = new CLICommandInfo(
                CommandType.None,
                new Mock<IDictionary<string, string>>().Object,
                true);

            string[] args = {
                "command_name"   
            };

            _commandParserMock
                .Setup(parser => parser.ParseArgs(args))
                .Returns(fakeCommandInfo);

            _app.Run(args);

            _consoleHelperMock.Verify(
                helper => helper.WriteColored(
                    // Example message for the Regex:
                    /*
                       ERROR: 
                       'None' is not an existing command.
                     */
                    It.IsRegex($"ERROR:(?s).*'{fakeCommandInfo.Type.ToString()}' is not an existing command."),
                    _app.ErrorColor,
                    null, 
                    Console.Error.WriteLine
                ),
                Times.Once
            );
        }

        #endregion
    }
}

