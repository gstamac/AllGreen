using System;
using System.Collections.Generic;
using System.IO;

namespace AllGreen.WebServer.Core
{
    public class FileWatcher : IDisposable
    {
        IRunnerHub _RunnerHub;
        List<FileSystemWatcher> _FileSystemWatchers;

        public FileWatcher(IRunnerHub runnerHub, IEnumerable<FolderFilter> watchedFolderFilters)
        {
            _RunnerHub = runnerHub;
            _FileSystemWatchers = new List<FileSystemWatcher>();

            RegisterWatchers(watchedFolderFilters);
        }


        private void RegisterWatchers(IEnumerable<FolderFilter> watchedFolderFilters)
        {
            foreach (FolderFilter filter in watchedFolderFilters)
            {
                AddWatcher(filter.Folder, filter.FilePattern, filter.IncludeSubfolders);
            }
        }

        private void AddWatcher(string path, string filter, bool includeSubfolders)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Path.GetFullPath(path), filter);
            watcher.Changed += watcher_Changed;
            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.Renamed += watcher_Renamed;
            watcher.IncludeSubdirectories = includeSubfolders;
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
