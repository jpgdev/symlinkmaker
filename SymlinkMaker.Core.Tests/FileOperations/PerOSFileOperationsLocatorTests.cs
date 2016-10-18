using System;
using NUnit.Framework;
using Moq;

namespace SymlinkMaker.Core.Tests
{
    [TestFixture]
    public class PerOSFileOperationsLocatorTests
    {
        private FileSystemOperationsLocator _locator;

        #region Set Up / Tear Down

        [TestFixtureSetUp]
        public void BeforeEachSetUp()
        {
            _locator = new FileSystemOperationsLocator(
                new Mock<IDirectory>().Object,
                new Mock<IFile>().Object
            );
        }

        [TestFixtureTearDown]
        public void AfterEachTearDown()
        {
            _locator = null;
        }

        #endregion

        [Test]
        public void Get_WhenUnix_ShouldReturnUnixFileOperations()
        {
            var fileOperations = _locator.Get(PlatformID.Unix);
            Assert.AreEqual(
                typeof(UnixDirectoryOperations),
                fileOperations.GetType());
        }

        [Test]
        public void Get_WhenWindows_ShouldReturnWindowsFileOperations()
        {
            var winPlatforms = new []
            {
                PlatformID.Win32NT,
                PlatformID.Win32S,
                PlatformID.WinCE,
                PlatformID.Win32Windows
            };

            foreach (var platform in winPlatforms)
            {
                var fileOperations = _locator.Get(platform);
                Assert.AreEqual(
                    typeof(WindowsDirectoryOperations),
                    fileOperations.GetType());
            }
        }

        [Test]
        public void Get_WhenMacOSX_ShouldPlatformNotSupportedException()
        {
            Assert.Throws(
                Is.TypeOf(typeof(PlatformNotSupportedException))
                    .And.Property("Message")
                    .Contains(PlatformID.MacOSX.ToString()),
                () => _locator.Get(PlatformID.MacOSX)
            );
        }
    }
}

