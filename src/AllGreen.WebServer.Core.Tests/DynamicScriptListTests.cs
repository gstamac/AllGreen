using System;
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
        private Mock<IFileLocator> _FileLocatorMock;

        [TestInitialize]
        public void Setup()
        {
            _FileLocatorMock = new Mock<IFileLocator>();
        }

        [TestMethod]
        public void EmptyListTest()
        {
            IConfiguration configuration = CreateConfiguration(new FolderFilter[0]);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, _FileLocatorMock.Object);
            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[0]);
        }

        [TestMethod]
        public void NonrecursiveFolderSearch()
        {
            SetupGetFiles("Scripts", @"*.js", false, new string[] { @"file1.js", @"file2.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = false } };
            IConfiguration configuration = CreateConfiguration(folderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, _FileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js" });
        }

        [TestMethod]
        public void RootFolderSearch()
        {
            SetupGetFiles("", @"*.js", false, new string[] { @"file1.js", @"file2.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"", FilePattern = "*.js", IncludeSubfolders = false } };
            IConfiguration configuration = CreateConfiguration(folderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, _FileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"file1.js", @"file2.js" });
        }

        [TestMethod]
        public void SubfolderSearchManual()
        {
            SetupGetFiles("Scripts", @"*.js", false, new string[] { @"file1.js", @"file2.js" });
            SetupGetFolders("Scripts", new string[] { "Sub" });
            SetupGetFiles(@"Scripts\Sub", @"*.js", false, new string[] { @"file3.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            IConfiguration configuration = CreateConfiguration(folderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, _FileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void SubfolderSearchAutomatic()
        {
            SetupGetFiles("Scripts", @"*.js", true, new string[] { @"file1.js", @"file2.js", @"Sub\file3.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            IConfiguration configuration = CreateConfiguration(folderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, _FileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void MultipleFolderFilters()
        {
            SetupGetFiles("Scripts", @"*.js", false, new string[] { @"file1.js", @"file2.js" });
            SetupGetFiles(@"Scripts\Sub", @"*.js", false, new string[] { @"file3.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = false }, new FolderFilter { Folder = @"Scripts\Sub", FilePattern = "*.js", IncludeSubfolders = false } };
            IConfiguration configuration = CreateConfiguration(folderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, _FileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void ExcludeFiles()
        {
            SetupGetFiles("Scripts", @"*.js", true, new string[] { @"file1.js", @"file1.min.js", @"file2.js", @"file2.min.js", @"Sub\file3.js", @"Sub\file3.min.js" });
            SetupGetFiles("Scripts", @"*.min.js", true, new string[] { @"file1.min.js", @"file2.min.js", @"Sub\file3.min.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = true } };
            FolderFilter[] excludeFolderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.min.js", IncludeSubfolders = true } };
            IConfiguration configuration = CreateConfiguration(folderFilterList, excludeFolderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, _FileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        [TestMethod]
        public void ExcludeFilesInSubfolders()
        {
            SetupGetFiles("Scripts", @"*.js", false, new string[] { @"file1.js", @"file1.min.js", @"file2.js", @"file2.min.js" });
            SetupGetFiles(@"Scripts\Sub", @"*.js", false, new string[] { @"file3.js", @"file3.min.js" });
            SetupGetFiles("Scripts", @"*.min.js", true, new string[] { @"file1.min.js", @"file2.min.js", @"Sub\file3.min.js" });

            FolderFilter[] folderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.js", IncludeSubfolders = false }, new FolderFilter { Folder = @"Scripts\Sub", FilePattern = "*.js", IncludeSubfolders = false } };
            FolderFilter[] excludeFolderFilterList = new FolderFilter[] { new FolderFilter { Folder = @"Scripts", FilePattern = "*.min.js", IncludeSubfolders = true } };
            IConfiguration configuration = CreateConfiguration(folderFilterList, excludeFolderFilterList);
            DynamicScriptList dynamicFileList = new DynamicScriptList(configuration, _FileLocatorMock.Object);

            dynamicFileList.Files.ShouldAllBeEquivalentTo(new string[] { @"Scripts/file1.js", @"Scripts/file2.js", @"Scripts/Sub/file3.js" });
        }

        private static IConfiguration CreateConfiguration(FolderFilter[] folderFilterList, FolderFilter[] excludeFolderFilterList = null)
        {
            return Mock.Of<IConfiguration>(c => 
                c.RootFolder == @"C:\www" && 
                c.ServedFolderFilters == folderFilterList && 
                c.ExcludeServedFolderFilters == (excludeFolderFilterList ?? new FolderFilter[0]));
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
