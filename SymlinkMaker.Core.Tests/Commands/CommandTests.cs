using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace SymlinkMaker.Core.Tests
{
    [TestFixture]
    public class CommandTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_WithoutAnOperation_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException)).And
                    .Property("Message").Contains("operation"),
                () => new Command(null)
            );
        }

        [Test]
        public void Constructor_WithOnlyAnOperation_ShouldWork()
        {
            var command = new Command(args => true);
            Assert.IsTrue(command.Execute(null));
        }

        #endregion

        #region Events Tests

        [Test]
        public void Execute_WithAFailingOperation_ShouldCallOnFailureEvent()
        {
            bool called = false;
            var command = new Command(args => false);
            command.Failed += (obj, args) =>
            {
                Assert.AreEqual(args.Status, CommandStatus.Failed);
                called = true;  
            };
            
            command.Execute(null);

            Assert.IsTrue(called);
        }

        [Test]
        public void Execute_WithASucceedingOperation_ShouldCallOnSuccessEvent()
        {
            bool called = false;
            var command = new Command(args => true);

            command.Succeeded += (obj, args) =>
            {
                Assert.AreEqual(args.Status, CommandStatus.Succeeded);
                called = true;  
            };
            command.Execute(null);

            Assert.IsTrue(called);
        }

        [Test]
        public void Execute_WithASucceedingOperation_ShouldCallOnFinishEvent()
        {
            bool called = false;
            var cmd = new Command(args => true);
            cmd.Finished += (obj, args) => called = true;
            cmd.Execute(null);

            Assert.IsTrue(called);
        }

        [Test]
        public void Execute_WithAFailingOperation_ShouldCallOnFinishEvent()
        {
            bool called = false;
            var command = new Command(args => true);

            command.Finished += (obj, args) => called = true;
            command.Execute(null);

            Assert.IsTrue(called);
        }

        [Test]
        public void Execute_WithThrowingOperation_ShouldCallOnExceptionEvent()
        {
            bool called = false;
            var command = new Command(args =>
                {
                    throw new Exception();
                });

            command.ExceptionThrown += (obj, args) =>
            {
                Assert.AreEqual(args.Status, CommandStatus.Running);
                called = true;  
            };
            
            command.Execute(null);

            Assert.IsTrue(called);
        }

        [Test]
        public void Execute_WithMissingRequiredArguments_ShouldCallOnExceptionEvent()
        {
            bool called = false;
            const string requiredParam = "requiredParam1";
            var args = new Dictionary<string, string>()
            {
                { "wrongParam", "value" }
            };

            var command = new Command(
                              a => true, 
                              new [] { requiredParam }
                          );
            command.ExceptionThrown += (obj, eventArgs) =>
            { 
                var exception = eventArgs.Exception;

                Assert.IsNotNull(exception);
                Assert.AreEqual(exception.GetType(), typeof(ArgumentException));
                StringAssert.Contains(requiredParam, exception.Message);

                called = true; 
            };

            command.Execute(args);

            Assert.IsTrue(called);
        }

        #endregion

        #region Run method Tests

        [Test]
        public void Execute_WithRequiredArguments_ShouldSucceed()
        {
            const string requiredParam = "requiredParam1";
            var args = new Dictionary<string, string>()
            {
                { requiredParam, "value" }
            };

            var command = new Command(
                              a => true, 
                              new [] { requiredParam }
                          );
            bool result = command.Execute(args);

            Assert.IsTrue(result);
        }

        [Test]
        public void Execute_WithFailingPreRunValidation_ShouldFail()
        {
            var command = new Command(
                              a => true, 
                              a => false);

            bool result = command.Execute(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Execute_WithSucceedingPreRunValidation_ShouldSucceed()
        {
            var command = new Command(
                              a => true, 
                              a => true);

            bool result = command.Execute(null);

            Assert.IsTrue(result);
        }

        [Test]
        public void Execute_WithAFailingPreRunValidation_ShouldFailAndNotCallTheOperation()
        {
            bool called = false;
            var command = new Command(a =>
                {
                    called = true;
                    return true;
                });

            command.RegisterPreExecutionValidation(args => true);
            command.RegisterPreExecutionValidation(args => false);
            command.RegisterPreExecutionValidation(args => true);

            bool result = command.Execute(null);

            Assert.IsFalse(result);
            Assert.IsFalse(called);
        }


        [Test]
        public void Execute_BeforeRunningTheCommand_ShouldHaveAStatusOfPreRun()
        {
            bool called = false;
            var command = new Command(a => true);

            command.ExceptionThrown += (cmd, cmdEventArgs) =>
            {
                Assert.AreEqual(CommandStatus.PreRun, cmdEventArgs.Status);
                called = true;
            };

            command.RegisterPreExecutionValidation(args =>
                {
                    throw new Exception("Should break before the command is ran");
                });

            command.Execute(null);

            Assert.IsTrue(called);
        }


        [Test]
        public void Execute_WhileRunningTheCommand_ShouldHaveAStatusOfRunning()
        {
            bool called = false;
            var command = new Command(a =>
                {
                    throw new Exception("Should break before the command is ran");
                });

            command.ExceptionThrown += (cmd, cmdEventArgs) =>
            {
                Assert.AreEqual(CommandStatus.Running, cmdEventArgs.Status);
                called = true;
            };

            command.Execute(null);

            Assert.IsTrue(called);
        }

        #endregion

        #region Register / Unregister PreExecutionValidation Tests

        [Test]
        public void RegisterPreRunValidation_WithValidDelegate_ShouldAddToPreRunValidation()
        {
            var command = new Command(a => true);

            command.RegisterPreExecutionValidation(args => false);

            bool result = command.Execute(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void UnregisterPreRunValidation_WithValidDelegate_ShouldRemoveFromPreRunValidation()
        {
            Operation preRunValidation = (args) => false;
            var command = new Command(a => true);

            command.RegisterPreExecutionValidation(preRunValidation);
            command.UnregisterPreExecutionValidation(preRunValidation);

            bool result = command.Execute(null);

            Assert.IsTrue(result);
        }

        [Test]
        public void UnregisterPreRunValidation_WithANotRegisteredDelegate_ShouldNotDoAnything()
        {
            var command = new Command(a => true);

            command.UnregisterPreExecutionValidation(args => false);
        }

        [Test]
        public void UnregisterPreRunValidation_WithANullValue_ShouldNotDoAnything()
        {
            var command = new Command(a => true);

            command.UnregisterPreExecutionValidation(null);
        }

        #endregion
    }
}

