using NUnit.Framework;
using Moq;

namespace SymlinkMaker.Core.Tests
{
    [TestFixture]
    public class AppSettingsTests
    {
        private AppSettings _appSettings;

        #region Set Up / Tear Down

        [SetUp]
        public void BeforeEachSetUp()
        {
            _appSettings = new AppSettings();
        }

        [TearDown]
        public void AfterEachTearDown()
        {
            _appSettings = null;
        }

        #endregion

        [Test]
        public void RequiresConfirmation_WhenSet_ShouldTriggerNotifyPropertyChanged()
        {
            bool called = false;
            _appSettings.PropertyChanged += (sender, e) =>
            {
                Assert.AreEqual("RequiresConfirmation", e.PropertyName);
                Assert.AreEqual(true, (sender as AppSettings).RequiresConfirmation);
                called = true;  
            };
            _appSettings.RequiresConfirmation = true;

            Assert.IsTrue(called);
        }

        [Test]
        public void SourcePath_WhenSet_ShouldTriggerNotifyPropertyChanged()
        {
            bool called = false;
            const string newPath = "/home/super/path";

            _appSettings.PropertyChanged += (sender, e) =>
            {
                Assert.AreEqual(nameof(_appSettings.SourcePath), e.PropertyName);
                Assert.AreEqual(newPath, (sender as AppSettings).SourcePath);
                called = true;  
            };
            
            _appSettings.SourcePath = newPath;

            Assert.IsTrue(called);
        }

        [Test]
        public void TargetPath_WhenSet_ShouldTriggerNotifyPropertyChanged()
        {
            bool called = false;
            const string newPath = "/home/super/path";

            _appSettings.PropertyChanged += (sender, e) =>
            {
                Assert.AreEqual(nameof(_appSettings.TargetPath), e.PropertyName);
                Assert.AreEqual(newPath, (sender as AppSettings).TargetPath);
                called = true;  
            };
            
            _appSettings.TargetPath = newPath;

            Assert.IsTrue(called);
        }

        [Test]
        public void FileOperations_WhenSet_ShouldTriggerNotifyPropertyChanged()
        {
            bool called = false;
            var fileOperationsMock = new Mock<IFileSystemOperations>();

            _appSettings.PropertyChanged += (sender, e) =>
            {
                Assert.AreEqual(nameof(_appSettings.FileOperations), e.PropertyName);
                Assert.AreEqual(
                    fileOperationsMock.Object, 
                    (sender as AppSettings).FileOperations);
                called = true;  
            };
            
            _appSettings.FileOperations = fileOperationsMock.Object;

            Assert.IsTrue(called);
        }
    }
}

