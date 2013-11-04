using System;
using System.Collections.Generic;
using System.IO;

namespace AllGreen.WebServer.Core
{
    public class FileWatcher : IDisposable
    {
        IRunnerHub _RunnerHub;
        IEnumerable<IFolderWatcher> _FolderWatchers;

        public FileWatcher(IRunnerHub runnerHub, IEnumerable<IFolderWatcher> folderWatchers)
        {
            _RunnerHub = runnerHub;
            _FolderWatchers = folderWatchers;

            RegisterWatchers();
        }


        private void RegisterWatchers()
        {
            foreach (IFolderWatcher folderWatcher in _FolderWatchers)
            {
                folderWatcher.Changed += folderWatcher_Changed;
            }
        }

        void folderWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _RunnerHub.ReloadAll();
        }

        public void Dispose()
        {
            foreach (IFolderWatcher folderWatcher in _FolderWatchers)
            {
                folderWatcher.Changed -= folderWatcher_Changed;
                folderWatcher.Dispose();
            }
        }
    }
}