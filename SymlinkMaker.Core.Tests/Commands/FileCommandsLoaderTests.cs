using System;
using NUnit.Framework;
using SymlinkMaker.Core;
using Moq;
using System.Collections.Generic;

namespace SymlinkMaker.Core.Tests
{
    [TestFixture]
    public class FileCommandsLoaderTests
    {
        private const string FAKE_SOURCE_PATH = "sourcePath123";
        private const string FAKE_TARGET_PATH = "targetPath123";

        private Mock<IFileSystemOperations> _fileOperationsMock;
        private ICommandsLoader _commandsLoader;


        private IDictionary<string, string> _fakeArgs;

        #region Set Up & Tear Down

        [TestFixtureSetUp]
        public void SetUp()
        {
            _fileOperationsMock = new Mock<IFileSystemOperations>();
            _fileOperationsMock
                .Setup(fileOps => fileOps.Move(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .Returns(true); 
            
            _fileOperationsMock
                .Setup(fileOps => fileOps.Delete(It.IsAny<string>()))
                .Returns(true);

            _fileOperationsMock
                .Setup(fileOps => fileOps.Copy(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .Returns(true); 

            _fileOperationsMock
                .Setup(fileOps => fileOps.CreateSymbolicLink(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
                .Returns(true);

            _commandsLoader = new FileCommandsLoader(_fileOperationsMock.Object);
            _fakeArgs = new Dictionary<string, string>()
            {
                { "sourcePath", FAKE_SOURCE_PATH },
                { "targetPath", FAKE_TARGET_PATH },
            };
        }

        [TearDown]
        public void OnEachTestTearDown()
        {
            _fileOperationsMock.ResetCalls();
        }
      
        #endregion

        [Test]
        public void Constructor_NullIFileOperations_ShouldThrow()
        {
            Assert.Throws(
                Is.TypeOf(typeof(ArgumentNullException)).And
                    .Property("Message").Contains("fileOperations"),
                () => new FileCommandsLoader(null)
            );
        }

        [Test]
        public void Load_ShouldReturnAllCommands()
        {
            var commands = _commandsLoader.Load();

            Assert.IsTrue(commands.ContainsKey(CommandType.Copy));
            Assert.IsTrue(commands.ContainsKey(CommandType.CreateSymLink));
            Assert.IsTrue(commands.ContainsKey(CommandType.Delete));
            Assert.IsTrue(commands.ContainsKey(CommandType.Move));
            Assert.IsTrue(commands.ContainsKey(CommandType.All));
        }

        #region Command behavior tests

        [Test]
        public void Command_Copy_ShouldCallTheRightFunction()
        {
            var commands = _commandsLoader.Load();
            var copyCommand = commands[CommandType.Copy];

            copyCommand.Execute(_fakeArgs);

            _fileOperationsMock.Verify(
                fileOps => fileOps.Copy(FAKE_SOURCE_PATH, FAKE_TARGET_PATH), 
                Times.Once);
        }

        [Test]
        public void Command_Move_ShouldCallTheRightFunction()
        {
            var commands = _commandsLoader.Load();
            var copyCommand = commands[CommandType.Move];

            copyCommand.Execute(_fakeArgs);

            _fileOperationsMock.Verify(
                fileOps => fileOps.Move(FAKE_SOURCE_PATH, FAKE_TARGET_PATH), 
                Times.Once);
        }

        [Test]
        public void Command_Delete_ShouldCallTheRightFunction()
        {
            var commands = _commandsLoader.Load();
            var deleteCommand = commands[CommandType.Delete];

            deleteCommand.Execute(_fakeArgs);

            _fileOperationsMock.Verify(
                fileOps => fileOps.Delete(FAKE_SOURCE_PATH), 
                Times.Once);
        }

        [Test]
        public void Command_CreateSymlink_ShouldCallTheRightFunction()
        {
            var commands = _commandsLoader.Load();
            var symlinkCommand = commands[CommandType.CreateSymLink];

            symlinkCommand.Execute(_fakeArgs);

            _fileOperationsMock.Verify(
                fileOps => fileOps.CreateSymbolicLink(
                    FAKE_SOURCE_PATH,
                    FAKE_TARGET_PATH), 
                Times.Once);
        }

        [Test]
        public void Command_All_ShouldCallTheRightFunctions()
        {
            var commands = _commandsLoader.Load();
            var allCommand = commands[CommandType.All];

            allCommand.Execute(_fakeArgs);

            _fileOperationsMock.Verify(
                fileOps => fileOps.Move(FAKE_SOURCE_PATH, FAKE_TARGET_PATH),
                Times.Once);
            
            _fileOperationsMock.Verify(
                fileOps => fileOps.CreateSymbolicLink(
                    FAKE_SOURCE_PATH,
                    FAKE_TARGET_PATH),
                Times.Once);
        }

        #endregion
    }
}

