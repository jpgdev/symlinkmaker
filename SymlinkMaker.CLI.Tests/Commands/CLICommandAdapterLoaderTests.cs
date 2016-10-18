using System;
using NUnit.Framework;
using System.Collections.Generic;
using Moq;
using SymlinkMaker.Core;

namespace SymlinkMaker.CLI.Tests
{
    [TestFixture]
    public class CLICommandAdapterLoaderTests
    {
        private IDictionary<CommandType, ICommand> _fakeBaseCommands;
        private CLICommandAdaptersLoader _commandsAdapterLoader;
        private Mock<IConsoleHelper> _consoleHelperMock;

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _consoleHelperMock = new Mock<IConsoleHelper>();
            _fakeBaseCommands = new Dictionary<CommandType, ICommand>();

            var commandTypes = new []
            {
                CommandType.Copy,
                CommandType.Move,
                CommandType.Delete,
                CommandType.CreateSymLink,
                CommandType.All,
                CommandType.ShowHelp
            };

            // Create Fake commands
            foreach (var type in commandTypes)
                _fakeBaseCommands[type] = new Mock<ICommand>().Object;

            _commandsAdapterLoader = new CLICommandAdaptersLoader(
                _fakeBaseCommands, 
                _consoleHelperMock.Object);

        }

        [Test]
        public void Constructor_WithoutValidBaseCommands_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                    .And.Property("ParamName").Contains("baseCommands"),
                () => new CLICommandAdaptersLoader(
                    null,
                    _consoleHelperMock.Object)
            );    
        }

        [Test]
        public void Load_WithCommands_ShouldReturnValidAdapterBasedOnValidCommands()
        {
            var commands = _commandsAdapterLoader.Load();

            // Verify that the command adapter was based on the right base command
            foreach (var type in _fakeBaseCommands.Keys)
            {
                Assert.IsTrue(commands.ContainsKey(type));
                Assert.AreEqual(
                    commands[type].Command,
                    _fakeBaseCommands[type]);
            }
        }

    }
}

