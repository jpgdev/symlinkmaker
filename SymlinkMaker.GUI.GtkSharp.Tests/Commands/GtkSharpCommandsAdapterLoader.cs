using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using SymlinkMaker.Core;
using SymlinkMaker.GUI.GTKSharp;
using System;

namespace SymlinkMaker.GUI.GtkSharp.Tests
{

    [TestFixture]
    public class GtkSharpCommandsAdapterLoaderTests
    {
         #region Fields

        private IDictionary<CommandType, ICommand> _fakeBaseCommands;
        private GtkSharpCommandAdaptersLoader _commandsAdapterLoader;
        private Mock<IDialogHelper> _dialogHelper;

        #endregion


        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {
            _dialogHelper = new Mock<IDialogHelper>();
            _fakeBaseCommands = new Dictionary<CommandType, ICommand>();

            var commandTypes = new []
                {
                    CommandType.Copy,
                    CommandType.Move,
                    CommandType.Delete,
                    CommandType.CreateSymLink,
                    CommandType.All
                };

            // Create Fake commands
            foreach (var type in commandTypes)
                _fakeBaseCommands[type] = new Mock<ICommand>().Object;

            _commandsAdapterLoader = new GtkSharpCommandAdaptersLoader(
                _fakeBaseCommands, 
                _dialogHelper.Object);
        }

        #endregion

        [Test]
        public void Constructor_WithoutValidBaseCommands_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException))
                .And.Property("ParamName").Contains("baseCommands"),
                () => new GtkSharpCommandAdaptersLoader(
                    null,
                    _dialogHelper.Object)
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
