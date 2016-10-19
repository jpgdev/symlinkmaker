using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using System.IO;
using System.Collections.Generic;

namespace SymlinkMaker.Core.Tests
{

    [TestFixture]
    public class DirectoryOperationsTests
    {
        private Mock<DirectoryOperations> _directoryOperations;
        private Mock<IFile> _fileManager;
        private Mock<IDirectory> _directoryManager;

        private const string sourcePath = "/source/path/";
        private const string targetPath = "/target/path/";

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeAllSetUp()
        {

            _fileManager = new Mock<IFile>();
            _directoryManager = new Mock<IDirectory>();

            _directoryOperations = new Mock<DirectoryOperations>(
                _directoryManager.Object,
                _fileManager.Object
            )
            {
                // Calls the class' existing virtual methods
                CallBase = true
            };
        }
      
        [SetUp]
        public void BeforeEachSetUp()
        {
            // Always returns true for all boolean by default
            _directoryManager.SetReturnsDefault<bool>(true);
            _fileManager.SetReturnsDefault<bool>(true);
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _directoryManager.Reset();
            _fileManager.Reset();
            _directoryOperations.Reset();
        }

        #endregion

        [Test]
        public void Move_WithParameters_ShouldCallTheRightMethod()
        {
            _directoryOperations.Object.Move(sourcePath, targetPath);
            _directoryManager
                .Verify(dir => dir.Move(sourcePath, targetPath), Times.Once);
        }

        [Test]
        public void Delete_WithParameters_ShouldCallTheRightMethod()
        {
            _directoryOperations.Object.Delete(sourcePath);
            _directoryManager
                .Verify(dir => dir.Delete(sourcePath), Times.Once);
        }

        [Test]
        public void Exists_WithParameters_ShouldCallTheRightMethod()
        {
            _directoryOperations.Object.Exists(sourcePath);
            _directoryManager
                .Verify(dir => dir.Exists(sourcePath), Times.Once);
        }

        [Test]
        public void Copy_WithoutAFilemanager_ShouldThrow()
        {
            var dirOps = new Mock<DirectoryOperations>(
                             _directoryManager.Object,
                             null)
            {
                CallBase = true
            };

            Assert.Throws(
                Is.TypeOf(typeof(NullReferenceException))
                    .And.Property("Message").Contains("IFile"),
                () => dirOps.Object.Copy(sourcePath, targetPath)
            );
        }

        #region Copy method tests

        [Test]
        public void Copy_WhenSourceDirectoryDoesNotExist_ShouldThrow()
        {
            _directoryManager
                .Setup(dir => dir.Exists(sourcePath))
                .Returns(false);
           
            Assert.Throws(
                Is.TypeOf(typeof(DirectoryNotFoundException))
                    .And.Property("Message").Contains(sourcePath),
                () => _directoryOperations.Object.Copy(sourcePath, targetPath)
            );   
        }

        [Test]
        public void Copy_DirectoryWithFiles_ShouldCopyThemCorrectly()
        {
            var files = new [] { "File1", "File2", "File3" };

            _directoryManager
                .Setup(dir => dir.GetFiles(sourcePath))
                .Returns(files);

            bool result = _directoryOperations.Object.Copy(
                              sourcePath,
                              targetPath);

            foreach (var fileName in files)
            {
                _fileManager.Verify(
                    file => file.Copy(
                        Path.Combine(sourcePath, fileName),
                        Path.Combine(targetPath, fileName)
                    ),
                    Times.Once
                );
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void Copy_DirectoryWithSubDirectories_ShouldCopyThemCorrectly()
        {
            var subDirs = new [] { "Dir1", "Dir2", "Dir3" };

            _directoryManager
                .Setup(dir => dir.GetDirectories(sourcePath))
                .Returns(subDirs);

            bool result = _directoryOperations.Object.Copy(
                              sourcePath,
                              targetPath);

            foreach (var dirName in subDirs)
            {
                _directoryOperations.Verify(
                    dir => dir.Copy(
                        Path.Combine(sourcePath, dirName),
                        Path.Combine(targetPath, dirName)
                    ),
                    Times.Once
                );
            }

            Assert.IsTrue(result);
        }


        [Test]
        public void Copy_DirectoryWithSubDirsWithFiles_ShouldCopyThemCorrectly()
        {
            var subDirs = new [] { "Dir1", "Dir2", "Dir3" };
            var files = new Dictionary<string, string[]>();

            // Add files for each subDirs
            foreach (var subDir in subDirs)
            {
                string completePath = Path.Combine(sourcePath, subDir);
                files.Add(completePath, new []
                    {
                        completePath + "_File1",
                        completePath + "_File2",
                        completePath + "_File3"
                    }
                );
            }

            _directoryManager
                .Setup(dir => dir.GetDirectories(sourcePath))
                .Returns(subDirs);

            _directoryManager
                .Setup(dir => dir.GetFiles(
                    It.IsNotIn<string>(new [] { sourcePath })
                ))
                .Returns<string>(dirName => files[dirName]);

            bool result = _directoryOperations.Object.Copy(
                              sourcePath,
                              targetPath);
            
            foreach (var dirName in subDirs)
            {
                string oldDirPath = Path.Combine(sourcePath, dirName);
                string newDirPath = Path.Combine(targetPath, dirName);

                _directoryOperations.Verify(
                    dir => dir.Copy(oldDirPath, newDirPath),
                    Times.Once
                );

                foreach (var fileName in files[oldDirPath])
                {
                    _fileManager.Verify(
                        file => file.Copy(
                            Path.Combine(oldDirPath, fileName),
                            Path.Combine(newDirPath, fileName)
                        ),
                        Times.Once
                    );
                }

            }

            Assert.IsTrue(result);
        }


        [Test]
        public void Copy_DirectoryWithMultipleLayersOfSubDirs_ShouldCopyThemCorrectly()
        {
            /*
             * The recursive directory structure to test
             * sourcePath
             *     sourcePath/Dir1
             *         sourcePath/Dir1/Dir2
             *             sourcePath/Dir1/Dir2/Dir3
             */
            var dirStructure = GenerateSimpleDirectoryStructure(new [] { 
                sourcePath, 
                "Dir1", 
                "Dir2", 
                "Dir3" 
            });

            _directoryManager
                .Setup(dir => dir.GetDirectories(It.IsAny<string>()))
                .Returns<string>(dirName => dirStructure[dirName]);

            bool result = _directoryOperations.Object.Copy(
                              sourcePath,
                              targetPath);
            
            foreach (var dirKeyValue in dirStructure)
            {
                string currentPath = dirKeyValue.Key;
                // Replace the sourcePath with the target path, but keep the rest
                string newPath = currentPath.Replace(sourcePath, targetPath);

                _directoryOperations.Verify(
                    dir => dir.Copy(currentPath, newPath),
                    Times.Once
                );
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void Copy_WithNullSubDirectoriesArray_ShouldNotThrow()
        {
            _directoryManager
                .Setup(dir => dir.GetDirectories(It.IsAny<string>()))
                .Returns<IEnumerable<string>>(null);
            
            bool result = _directoryOperations.Object.Copy(sourcePath, targetPath);

            Assert.IsTrue(result);
        }

        [Test]
        public void Copy_WithNullFilesArray_ShouldNotThrow()
        {
            _directoryManager
                .Setup(dir => dir.GetFiles(It.IsAny<string>()))
                .Returns<IEnumerable<string>>(null);
            
            bool result = _directoryOperations.Object.Copy(sourcePath, targetPath);

            Assert.IsTrue(result);
        }


        #endregion

        /// <summary>
        /// Generates a simple recursive directory structure. 
        /// 
        /// Set each directories to be the parent of the next in the array.
        /// 
        /// Ex : 
        /// Input = { "dir1", "dir2", "dir3"} 
        /// 
        /// 
        /// Output = { 
        ///    { "dir1", [ "dir2" ] } , 
        ///    { "dir1/dir2", [ "dir3" ] }, 
        ///    { "dir1/dir2/dir3", null } 
        /// }
        /// </summary>
        /// <returns>The simple directory structure.</returns>
        /// <param name="directoryNames">Directory names.</param>
        private static IDictionary<string, string[]> GenerateSimpleDirectoryStructure(string[] directoryNames)
        {
            var dirStructure = new Dictionary<string, string[]>();

            // Generate recursive sub directories
            Action<string, string[]> populateDirectoryStructure = null;

            populateDirectoryStructure = (parent, remaining) =>
            {
                string current = parent != null ? 
                        Path.Combine(parent, remaining[0]) 
                        : remaining[0];

                string[] next = remaining.Length > 1 ? 
                        new [] { remaining[1] } 
                        : new string[0];

                dirStructure.Add(current, next);

                if (remaining.Length > 1)
                {
                    remaining = remaining.Skip(1).ToArray();
                    populateDirectoryStructure(current, remaining);
                }
            };

            populateDirectoryStructure(null, directoryNames);

            return dirStructure;
        }
    }


}

