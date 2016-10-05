using System;
using NUnit;
using NUnit.Framework;
using System.IO;
using SymlinkMaker.Core;

namespace SymlinkMaker.CLI
{
    [TestFixture]
    public class ProgramTest
    {
        [OneTimeSetUp]
        public void SetUp() {}

//        [Test]
//        public void ProcessArgs_ShouldThrowException_IfNoArgs(){                        
//            Assert.Catch(() => MainClass.ProcessArgs(new string[0]));
//        }
//
//        [Test]
//        public void DoWholeProcess_ShouldThrowException_IfNoSourceDirName(){                        
//            Assert.Catch(() => MainClass.DoWholeProcess(null, "test_path", new EmptyFileOperations()));
//            Assert.Catch(() => MainClass.DoWholeProcess("", "test_path", new EmptyFileOperations()));
//        }
//
//        [Test]
//        public void DoWholeProcess_ShouldThrowException_IfNoTargetDirName(){                        
//            Assert.Catch(() => MainClass.DoWholeProcess("test_path", null, new EmptyFileOperations()));
//            Assert.Catch(() => MainClass.DoWholeProcess("test_path", "", new EmptyFileOperations()));
//        }
//
//        [Test]
//        public void DoWholeProcess_ShouldReturnFalse_IfSourceDirNameIsInvalid(){                        
//            Assert.IsFalse(MainClass.DoWholeProcess(
//                "/test_path", 
//                "/test_wrong_path", 
//                new FailingFileOperations(), 
//                false));
//        }
//
//        [Test]
//        public void DoWholeProcess_ShouldReturnFalse_IfTargetDirNameIsInvalid(){                        
//            Assert.IsFalse(MainClass.DoWholeProcess(
//                "/test_path", 
//                "/test_wrong_path", 
//                new FailingFileOperations(),
//                false));
//        }

        [OneTimeTearDown]
        public void TearDown() {}

        private class EmptyFileOperations : IFileOperations {

            public void CreateSymbolicLink (string sourceDirName, string targetDirName) {}

            public void CopyDirectory (string sourceDirName, string targetDirName, bool copySubDirs) {}

            public void DeleteDirectory (string path, bool recursive) {}

            public bool DirectoryExists (string path) { return true; }
        }

        private class FailingFileOperations : IFileOperations {

            public void CreateSymbolicLink (string sourceDirName, string targetDirName) {
                throw new Exception();
            }

            public void CopyDirectory (string sourceDirName, string targetDirName, bool copySubDirs) {
                throw new Exception();
            }

            public void DeleteDirectory (string path, bool recursive) {
                throw new Exception();
            }

            public bool DirectoryExists (string path) { 
                throw new Exception(); 
            }
        }
    }


}

