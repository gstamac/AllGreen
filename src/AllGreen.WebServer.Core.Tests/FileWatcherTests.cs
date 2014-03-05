using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class FileWatcherTests
    {
        [TestMethod]
        public void Test()
        {
            TinyIoCContainer tinyIoCContainer = new TinyIoCContainer();
            IRunnerClients runnerClients = Mock.Of<IRunnerClients>();
            tinyIoCContainer.Register<IRunnerClients>(runnerClients);
            Mock<IFolderWatcher> folderWatcher1Mock = new Mock<IFolderWatcher>();
            Mock<IFolderWatcher> folderWatcher2Mock = new Mock<IFolderWatcher>();
            using (FileWatcher fileWatcher = new FileWatcher(tinyIoCContainer, new IFolderWatcher[] { folderWatcher1Mock.Object, folderWatcher2Mock.Object }))
            {
                folderWatcher1Mock.Raise(fw => fw.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "", ""));
                folderWatcher2Mock.Raise(fw => fw.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "", ""));
                Mock.Get<IRunnerClients>(runnerClients).Verify(rh => rh.ReloadAll(), Times.Exactly(2));
            }
        }
    }
}
