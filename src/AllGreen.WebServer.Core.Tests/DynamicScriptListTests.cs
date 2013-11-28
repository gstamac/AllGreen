using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class DynamicScriptListTests
    {
        private Mock<IFileSystem> _FileLocatorMock;

        [TestInitialize]
        public void Setup()
        {
            _FileLocatorMock = new Mock<IFileSystem>();
        }

        [TestMethod]
        public void EmptyListTest()
        {
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", new List<FolderFilter>(), new List<FolderFilter>(), _FileLocatorMock.Object);
            dynamicFileList.Scripts.ShouldAllBeEquivalentTo(new string[0]);
        }

        [TestMethod]
        public void NonrecursiveFolderSearch()
        {
            SetupGetFiles("Scripts", @"*.js", false, new string[] { @"file1.js", @"file2.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = false } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList.ToList(), new List<FolderFilter>(), _FileLocatorMock.Object);

            dynamicFileList.Scripts.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js" });
        }

        [TestMethod]
        public void RootFolderSearch()
        {
            SetupGetFiles("", @"*.js", false, new string[] { @"file1.js", @"file2.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"", FilePattern = "*.js", IncludeSubfolders = false } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList.ToList(), new List<FolderFilter>(), _FileLocatorMock.Object);

            dynamicFileList.Scripts.ShouldAllBeEquivalentTo(new string[] { @"file1.js", @"file2.js" });
        }

        [TestMethod]
        public void SubfolderSearchManual()
        {
            SetupGetFiles("Scripts", @"*.js", false, new string[] { @"file1.js", @"file2.js" });
            SetupGetFolders("Scripts", new string[] { "Sub" });
            SetupGetFiles(@"Scripts\Sub", @"*.js", false, new string[] { @"file3.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList.ToList(), new List<FolderFilter>(), _FileLocatorMock.Object);

            dynamicFileList.Scripts.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void SubfolderSearchAutomatic()
        {
            SetupGetFiles("Scripts", @"*.js", true, new string[] { @"file1.js", @"file2.js", @"Sub\file3.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList.ToList(), new List<FolderFilter>(), _FileLocatorMock.Object);

            dynamicFileList.Scripts.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void MultipleFolderFilters()
        {
            SetupGetFiles("Scripts", @"*.js", false, new string[] { @"file1.js", @"file2.js" });
            SetupGetFiles(@"Scripts\Sub", @"*.js", false, new string[] { @"file3.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = false }, new FolderFilter { Folder = @"Scripts\Sub", FilePattern = "*.js", IncludeSubfolders = false } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList.ToList(), new List<FolderFilter>(), _FileLocatorMock.Object);

            dynamicFileList.Scripts.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void ExcludeFiles()
        {
            SetupGetFiles("Scripts", @"*.js", true, new string[] { @"file1.js", @"file1.min.js", @"file2.js", @"file2.min.js", @"Sub\file3.js", @"Sub\file3.min.js" });
            SetupGetFiles("Scripts", @"*.min.js", true, new string[] { @"file1.min.js", @"file2.min.js", @"Sub\file3.min.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            FolderFilter[] excludeFolderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.min.js", IncludeSubfolders = true } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList.ToList(), excludeFolderFilterList.ToList(), _FileLocatorMock.Object);

            dynamicFileList.Scripts.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void ExcludeFilesInSubfolders()
        {
            SetupGetFiles("Scripts", @"*.js", false, new string[] { @"file1.js", @"file1.min.js", @"file2.js", @"file2.min.js" });
            SetupGetFiles(@"Scripts\Sub", @"*.js", false, new string[] { @"file3.js", @"file3.min.js" });
            SetupGetFiles("Scripts", @"*.min.js", true, new string[] { @"file1.min.js", @"file2.min.js", @"Sub\file3.min.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = false }, new FolderFilter { Folder = @"Scripts\Sub", FilePattern = "*.js", IncludeSubfolders = false } };
            FolderFilter[] excludeFolderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.min.js", IncludeSubfolders = true } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList.ToList(), excludeFolderFilterList.ToList(), _FileLocatorMock.Object);

            dynamicFileList.Scripts.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void ExcludeDuplicates()
        {
            SetupGetFiles("Scripts", @"file2.js", false, new string[] { @"file2.js" });
            SetupGetFiles("Scripts", @"*.js", false, new string[] { @"file1.js", @"file2.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "file2.js", IncludeSubfolders = false }, new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = false } };
            DynamicScriptList dynamicFileList = new DynamicScriptList(@"C:\www", folderFilterList.ToList(), new List<FolderFilter>(), _FileLocatorMock.Object);

            dynamicFileList.Scripts.Should().HaveCount(2);
            dynamicFileList.Scripts.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js" });
        }

        private void SetupGetFiles(string searchFolder, string fileFilter, bool includeSubfolders, string[] files)
        {
            string[] fullFiles = files.Select(f => Path.Combine(@"C:\www", searchFolder, f)).ToArray();
            _FileLocatorMock.Setup(fl => fl.GetFiles(Path.Combine(@"C:\www", searchFolder), fileFilter, includeSubfolders, out fullFiles)).Returns(true);
        }

        private void SetupGetFolders(string searchFolder, string[] subfolders)
        {
            string[] fullSubFolders = subfolders.Select(f => Path.Combine(@"C:\www", searchFolder, f)).ToArray();
            _FileLocatorMock.Setup(fl => fl.GetFolders(Path.Combine(@"C:\www", searchFolder), out fullSubFolders)).Returns(true);
        }

    }
}
