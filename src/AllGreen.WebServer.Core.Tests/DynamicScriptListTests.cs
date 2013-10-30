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
            FolderFilter[] excludeFolderFilterList = new FolderFilter[0];
            IConfiguration configuration = Mock.Of<IConfiguration>(c => c.RootFolder == @"C:\www" && c.ServedFolderFilters == new FolderFilter[0] && c.ExcludeServedFolderFilters == excludeFolderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, fileLocator);
            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[0]);
        }

        [TestMethod]
        public void NonrecursiveFolderSearch()
        {
            Mock<IFileLocator> fileLocatorMock = new Mock<IFileLocator>();
            string[] files = new string[] { @"C:\www\Scripts\file1.js", @"C:\www\Scripts\file2.js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts", @"*.js", false, out files)).Returns(true);

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = false } };
            FolderFilter[] excludeFolderFilterList = new FolderFilter[0];
            IConfiguration configuration = Mock.Of<IConfiguration>(c => c.RootFolder == @"C:\www" && c.ServedFolderFilters == folderFilterList && c.ExcludeServedFolderFilters == excludeFolderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, fileLocatorMock.Object);
            
            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js" });
        }

        [TestMethod]
        public void RootFolderSearch()
        {
            Mock<IFileLocator> fileLocatorMock = new Mock<IFileLocator>();
            string[] files = new string[] { @"C:\www\Scripts\file1.js", @"C:\www\Scripts\file2.js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts", @"*.js", false, out files)).Returns(true);

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"", FilePattern = "*.js", IncludeSubfolders = false } };
            FolderFilter[] excludeFolderFilterList = new FolderFilter[0];
            IConfiguration configuration = Mock.Of<IConfiguration>(c => c.RootFolder == @"C:\www\Scripts" && c.ServedFolderFilters == folderFilterList && c.ExcludeServedFolderFilters == excludeFolderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, fileLocatorMock.Object);

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
            FolderFilter[] excludeFolderFilterList = new FolderFilter[0];
            IConfiguration configuration = Mock.Of<IConfiguration>(c => c.RootFolder == @"C:\www" && c.ServedFolderFilters == folderFilterList && c.ExcludeServedFolderFilters == excludeFolderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, fileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void SubfolderSearchAutomatic()
        {
            Mock<IFileLocator> fileLocatorMock = new Mock<IFileLocator>();
            string[] files = new string[] { @"C:\www\Scripts\file1.js", @"C:\www\Scripts\file2.js", @"C:\www\Scripts\Sub\file3.js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts", @"*.js", true, out files)).Returns(true);

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            FolderFilter[] excludeFolderFilterList = new FolderFilter[0];
            IConfiguration configuration = Mock.Of<IConfiguration>(c => c.RootFolder == @"C:\www" && c.ServedFolderFilters == folderFilterList && c.ExcludeServedFolderFilters == excludeFolderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, fileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void ExcludeFiles()
        {
            Mock<IFileLocator> fileLocatorMock = new Mock<IFileLocator>();
            string[] files = new string[] { @"C:\www\Scripts\file1.js", @"C:\www\Scripts\file1.min.js", @"C:\www\Scripts\file2.js", @"C:\www\Scripts\file2.min.js", @"C:\www\Scripts\Sub\file3.js", @"C:\www\Scripts\Sub\file3.min,js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts", @"*.js", true, out files)).Returns(true);
            string[] files2 = new string[] { @"C:\www\Scripts\file1.min.js", @"C:\www\Scripts\file2.min.js", @"C:\www\Scripts\Sub\file3.min,js" };
            fileLocatorMock.Setup(fl => fl.GetFiles(@"C:\www\Scripts", @"*.min.js", true, out files2)).Returns(true);

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            FolderFilter[] excludeFolderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.min.js", IncludeSubfolders = true } };
            IConfiguration configuration = Mock.Of<IConfiguration>(c => c.RootFolder == @"C:\www" && c.ServedFolderFilters == folderFilterList && c.ExcludeServedFolderFilters == excludeFolderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, fileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }
    }
}
