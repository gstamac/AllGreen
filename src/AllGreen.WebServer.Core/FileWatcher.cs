using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AllGreen.WebServer.Core
{
    public class FileWatcher : IDisposable
    {
        List<FileSystemWatcher> _FileSystemWatchers;
        IRunnerHub _RunnerHub;

        public FileWatcher(IRunnerHub runnerHub)
        {
            _RunnerHub = runnerHub;
            _FileSystemWatchers = new List<FileSystemWatcher>();
        }

        public void WatchFile(string fileName)
        {
            AddWatcher(Path.GetDirectoryName(fileName), Path.GetFileName(fileName));
        }

        public void WatchFolder(string path)
        {
            AddWatcher(path);
        }

        private void AddWatcher(string path, string filter = null)
        {
            FileSystemWatcher watcher = (filter == null) ? new FileSystemWatcher(path) : new FileSystemWatcher(path, filter);
            watcher.Changed += watcher_Changed;
            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.Renamed += watcher_Renamed;
            watcher.EnableRaisingEvents = true;
            _FileSystemWatchers.Add(watcher);
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _RunnerHub.ReloadAll();
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            _RunnerHub.ReloadAll();
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            _RunnerHub.ReloadAll();
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            _RunnerHub.ReloadAll();
        }

        public void Dispose()
        {
            foreach (FileSystemWatcher watcher in _FileSystemWatchers)
            {
                watcher.Changed -= watcher_Changed;
                watcher.Created -= watcher_Created;
                watcher.Deleted -= watcher_Deleted;
                watcher.Renamed -= watcher_Renamed;
                watcher.Dispose();
            }
            _FileSystemWatchers.Clear();
        }
    }
}
