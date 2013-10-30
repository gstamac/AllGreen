using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class DynamicScriptListTests
    {
        [TestMethod]
        public void EmptyListTest()
        {
            IFileLocator fileLocator = Mock.Of<IFileLocator>();
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\", new FolderFilter[0], fileLocator);
            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[0]);
        }

        [TestMethod]
        public void NonrecursiveFolderSearch()
        {
            Mock<IFileLocator> fileLocatorMock = new Mock<IFileLocator>();
            string[] files = new string[] { @"C:\www\Scripts\file1.js", @"C:\www\Scripts\file2.js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts", @"*.js", false, out files)).Returns(true);

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = false } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList, fileLocatorMock.Object);
            
            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js" });
        }

        [TestMethod]
        public void RootFolderSearch()
        {
            Mock<IFileLocator> fileLocatorMock = new Mock<IFileLocator>();
            string[] files = new string[] { @"C:\www\Scripts\file1.js", @"C:\www\Scripts\file2.js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts", @"*.js", false, out files)).Returns(true);

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"", FilePattern = "*.js", IncludeSubfolders = false } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www\Scripts", folderFilterList, fileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"file1.js", @"file2.js" });
        }

        [TestMethod]
        public void SubfolderSearchManual()
        {
            Mock<IFileLocator> fileLocatorMock = new Mock<IFileLocator>();
            string[] files = new string[] { @"C:\www\Scripts\file1.js", @"C:\www\Scripts\file2.js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts", @"*.js", false, out files)).Returns(true);
            string[] subfolders = new string[] { @"C:\www\Scripts\Sub" };
            fileLocatorMock.Setup(fl => fl.GetFolders(@"C:\www\Scripts", out subfolders)).Returns(true);
            string[] files2 = new string[] { @"C:\www\Scripts\Sub\file3.js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts\Sub", @"*.js", false, out files2)).Returns(true);
            
            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList, fileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void SubfolderSearchAutomatic()
        {
            Mock<IFileLocator> fileLocatorMock = new Mock<IFileLocator>();
            string[] files = new string[] { @"C:\www\Scripts\file1.js", @"C:\www\Scripts\file2.js", @"C:\www\Scripts\Sub\file3.js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts", @"*.js", true, out files)).Returns(true);

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList, fileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }
    }
}
