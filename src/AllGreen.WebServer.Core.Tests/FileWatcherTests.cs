using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Owin;
using TinyIoC;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class FileWatcherTests
    {
        [TestMethod]
        public void Test()
        {
            IRunnerHub runnerHub = Mock.Of<IRunnerHub>();
            Mock<IFolderWatcher> folderWatcher1Mock = new Mock<IFolderWatcher>();
            Mock<IFolderWatcher> folderWatcher2Mock = new Mock<IFolderWatcher>();
            using (FileWatcher fileWatcher = new FileWatcher(runnerHub, new IFolderWatcher[] { folderWatcher1Mock.Object, folderWatcher2Mock.Object }))
            {
                folderWatcher1Mock.Raise(fw => fw.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "", ""));
                folderWatcher2Mock.Raise(fw => fw.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "", ""));
                Mock.Get<IRunnerHub>(runnerHub).Verify(rh => rh.ReloadAll(), Times.Exactly(2));
            }
        }
    }
}
