using System;
using System.Collections.Generic;
using System.IO;

namespace AllGreen.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
    public class FolderWatcher : IFolderWatcher, IDisposable
    {
        FileSystemWatcher _FileSystemWatcher;

        public FolderWatcher(string path, string filter, bool includeSubfolders)
        {
            _FileSystemWatcher = new FileSystemWatcher(path, filter);
            _FileSystemWatcher.Changed += _FileSystemWatcher_Changed;
            _FileSystemWatcher.Created += _FileSystemWatcher_Changed;
            _FileSystemWatcher.Deleted += _FileSystemWatcher_Changed;
            _FileSystemWatcher.Renamed += _FileSystemWatcher_Changed;
            _FileSystemWatcher.IncludeSubdirectories = includeSubfolders;
            _FileSystemWatcher.EnableRaisingEvents = true;
        }

        void _FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Changed != null) Changed(this, e);
        }

        public event FileSystemEventHandler Changed;

        public void Dispose()
        {
            _FileSystemWatcher.Changed -= _FileSystemWatcher_Changed;
            _FileSystemWatcher.Created -= _FileSystemWatcher_Changed;
            _FileSystemWatcher.Deleted -= _FileSystemWatcher_Changed;
            _FileSystemWatcher.Renamed -= _FileSystemWatcher_Changed;
            _FileSystemWatcher.Dispose();
        }
    }
}
