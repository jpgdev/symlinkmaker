using NUnit.Framework;
using Moq;
using System.Collections.Generic;

namespace SymlinkMaker.Core.Tests
{
    [TestFixture]
    public class CommandAdapterTests
    {
        private Mock<CommandAdapter> _commandAdapterMock;
        private Mock<ICommand> _commandMock;

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void TextFixtureSetUp()
        {
            _commandMock = new Mock<ICommand>();

            _commandAdapterMock = new Mock<CommandAdapter>(_commandMock.Object)
            {
                CallBase = true
            };
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _commandAdapterMock.Reset();
            _commandMock.Reset();
        }

        #endregion

        #region Run method tests

        [Test]
        public void Execute_WhenRequireConfirmationIsTrue_ShouldAddConfirmationHandlerToCommand()
        {
            _commandAdapterMock.Object.Execute(null, true);

            _commandMock.Verify(
                command => command.RegisterPreExecutionValidation(
                    It.IsAny<Operation>()
                ),
                Times.Once
            );
        }

        [Test]
        public void Execute_WhenRequireConfirmationIsFalse_ShouldNotAddConfirmationHandlerToCommand()
        {
            _commandAdapterMock.Object.Execute(null, false);

            _commandMock.Verify(
                command => command.RegisterPreExecutionValidation(
                    It.IsAny<Operation>()
                ),
                Times.Never
            );
        }

        [Test]
        public void Execute_WhenRequireConfirmationIsTrue_ShouldOnlyHaveASingleConfirmationHandlerInCommand()
        {
            _commandAdapterMock.Object.Execute(null, true);
            _commandAdapterMock.Object.Execute(null, true);

            _commandMock.Verify(
                command => command.RegisterPreExecutionValidation(
                    It.IsAny<Operation>()
                ),
                Times.Exactly(2)
            );

            _commandMock.Verify(
                command => command.UnregisterPreExecutionValidation(
                    It.IsAny<Operation>()
                ),
                Times.Exactly(2)
            );
        }

        [Test]
        public void Execute_WhenRequireConfirmationIsChangedfromTrueToFalse_ShouldOnlyNotHaveAConfirmationHandlerInCommand()
        {
            _commandAdapterMock.Object.Execute(null, true);
            _commandAdapterMock.Object.Execute(null, false);

            _commandMock.Verify(
                command => command.RegisterPreExecutionValidation(
                    It.IsAny<Operation>()
                ),
                Times.Once
            );

            _commandMock.Verify(
                command => command.UnregisterPreExecutionValidation(
                    It.IsAny<Operation>()
                ),
                Times.Exactly(2)
            );
        }

        #endregion

        [Test]
        public void CommandAdapter_WhenDisposed_ShouldUnregisterConfirmationHandler()
        {
            var tmpCommandMock = new Mock<ICommand>();
            var tmpCommandAdapterMock = new Mock<CommandAdapter>(tmpCommandMock.Object)
            {
                CallBase = true
            };
           
            // Check that before Dispose(), the Unregister has never been called.
            tmpCommandMock.Verify(
                command => command.UnregisterPreExecutionValidation(
                    It.IsAny<Operation>()
                ),
                Times.Never
            );

            tmpCommandAdapterMock.Object.Dispose();

            // NOTE : We can't be sure that the Operation passed to 
            //        UnregisterPreRunValidation is the ConfirmationHandler,
            //        It could be another Operation
            tmpCommandMock.Verify(
                command => command.UnregisterPreExecutionValidation(
                    It.IsAny<Operation>()
                ),
                Times.Once
            );
        }

        [Test]
        public void GetArgsValuesFromArgsNames_WithValidParams_ShouldReturnTheValues()
        {
            var arguments = new Dictionary<string, string>()
            {
                { "arg1", "value1" },
                { "arg2", "value2" },
                { "arg3", "value3" }
            };

            string[] argNames = { "arg2", "arg1" };
            
            string[] values = CommandAdapter.GetArgsValuesFromArgsName(
                                  arguments, 
                                  argNames);

            Assert.AreEqual(values.Length, argNames.Length);

            for (int i = 0; i < values.Length; i++)
                Assert.AreEqual(values[i], arguments[argNames[i]]);
        }

        [Test]
        public void GetArgsValuesFromArgsNames_WithMissingParams_ShouldThrow()
        {
            var arguments = new Dictionary<string, string>()
            {
                { "arg1", "value1" },
                { "arg2", "value2" }
            };

            string[] argNames = { "arg3", "arg1" };

            Assert.Throws(
                Is.TypeOf(typeof(KeyNotFoundException)),
                () => CommandAdapter.GetArgsValuesFromArgsName(
                    arguments,
                    argNames)
            );
        }
    }
}

