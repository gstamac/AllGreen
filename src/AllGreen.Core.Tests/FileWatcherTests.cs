using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;

namespace AllGreen.Core.Tests
{
    [TestClass]
    public class FileWatcherTests
    {
        [TestMethod]
        public void Test()
        {
            TinyIoCContainer tinyIoCContainer = new TinyIoCContainer();
            IRunnerBroadcaster runnerBroadcaster = Mock.Of<IRunnerBroadcaster>();
            tinyIoCContainer.Register<IRunnerBroadcaster>(runnerBroadcaster);
            Mock<IFolderWatcher> folderWatcher1Mock = new Mock<IFolderWatcher>();
            Mock<IFolderWatcher> folderWatcher2Mock = new Mock<IFolderWatcher>();
            using (FileWatcher fileWatcher = new FileWatcher(tinyIoCContainer, new IFolderWatcher[] { folderWatcher1Mock.Object, folderWatcher2Mock.Object }))
            {
                folderWatcher1Mock.Raise(fw => fw.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "", ""));
                folderWatcher2Mock.Raise(fw => fw.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "", ""));
                Mock.Get<IRunnerBroadcaster>(runnerBroadcaster).Verify(rh => rh.StartAll(), Times.Exactly(2));
            }
        }
    }
}
