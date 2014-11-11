using System;
using System.Collections.Generic;
using System.IO;
using TinyIoC;

namespace AllGreen.Core
{
    public class FileWatcher : IDisposable
    {
        private readonly TinyIoCContainer _ResourceResolver;
        private readonly IEnumerable<IFolderWatcher> _FolderWatchers;
        
        public FileWatcher(TinyIoCContainer resourceResolver, IEnumerable<IFolderWatcher> folderWatchers)
        {
            _ResourceResolver = resourceResolver;
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
            IRunnerBroadcaster runnerBroadcaster = _ResourceResolver.Resolve<IRunnerBroadcaster>();
            runnerBroadcaster.StartAll();
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