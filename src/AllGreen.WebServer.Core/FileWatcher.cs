using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNet.SignalR;

namespace AllGreen.WebServer.Core
{
    public class FileWatcher : IDisposable
    {
        IRunnerClients _RunnerClients;
        IEnumerable<IFolderWatcher> _FolderWatchers;

        public FileWatcher(IRunnerClients runnerClients, IEnumerable<IFolderWatcher> folderWatchers)
        {
            _RunnerClients = runnerClients;
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
            _RunnerClients.ReloadAll();
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