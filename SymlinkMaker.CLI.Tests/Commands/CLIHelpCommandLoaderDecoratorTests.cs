using Moq;
using NUnit.Framework;
using SymlinkMaker.Core;
using System;
using System.Collections.Generic;

namespace SymlinkMaker.CLI.Tests
{
    [TestFixture]
    public class CLIHelpCommandLoaderDecoratorTests
    {
        private CLIHelpCommandLoaderDecorator _commandLoaderDecorator;
        private Mock<ICommandsLoader> _baseCommandsLoaderMock;
        private Mock<IConsoleHelper> _consoleHelperMock;

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _baseCommandsLoaderMock = new Mock<ICommandsLoader>();
            _consoleHelperMock = new Mock<IConsoleHelper>();

            _commandLoaderDecorator = new CLIHelpCommandLoaderDecorator(
                _baseCommandsLoaderMock.Object,
                _consoleHelperMock.Object
            );
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _consoleHelperMock.Reset();
            _baseCommandsLoaderMock.Reset();
        }

        #endregion

        [Test]
        public void Constructor_WithoutValidBaseCommands_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                .And.Property("ParamName").Contains("fileCommandsLoader"),
                () => new CLIHelpCommandLoaderDecorator(
                    null,
                    _consoleHelperMock.Object)
            );    
        }

        
        [Test]
        public void Constructor_WithoutValidConsoleHelper_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                .And.Property("ParamName").Contains("consoleHelper"),
                () => new CLIHelpCommandLoaderDecorator(
                    _baseCommandsLoaderMock.Object,
                    null)
            );    
        }

        [Test]
        public void Load_WithValidBaseCommands_ShouldAddHelpCommandToExistingCommands()
        {
            var loadedCmds = new Dictionary<CommandType, ICommand>();

            _baseCommandsLoaderMock
                .Setup(loader => loader.Load())
                .Returns(loadedCmds);

            var commands = _commandLoaderDecorator.Load();

            Assert.IsTrue(commands.ContainsKey(CommandType.ShowHelp));
            Assert.AreSame(loadedCmds, commands);
        }
    }
}