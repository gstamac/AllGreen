using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AllGreen.WebServer.Core
{
    public class DynamicScriptList : IScriptList
    {
        private readonly string _RootFolder;
        private readonly IEnumerable<FolderFilter> _FolderFilters = new List<FolderFilter>();
        private readonly IEnumerable<FolderFilter> _ExcludeFolderFilters;
        private readonly IFileSystem _FileSystem;

        public DynamicScriptList(string rootFolder, List<FolderFilter> servedFolderFilters, List<FolderFilter> excludeServedFolderFilters, IFileSystem fileSystem)
        {
            _RootFolder = rootFolder;
            _FolderFilters = servedFolderFilters;
            _ExcludeFolderFilters = excludeServedFolderFilters;
            _FileSystem = fileSystem;
        }

        public IEnumerable<string> Scripts
        {
            get
            {
                int filenameOffset = _RootFolder.Length + 1;
                return GetFiles(_FolderFilters)
                    .Except(GetFiles(_ExcludeFolderFilters))
                    .Select(f => f.Substring(filenameOffset).Replace('\\', '/'))
                    .Distinct();
            }
        }

        private IEnumerable<string> GetFiles(IEnumerable<FolderFilter> filters)
        {
            foreach (FolderFilter rule in filters.ToList())
            {
                foreach (string file in GetFiles(rule))
                {
                    yield return file;
                }
            }
        }

        IEnumerable<string> GetFiles(FolderFilter folderFilter)
        {
            string[] files;
            string startFolder = Path.Combine(_RootFolder, folderFilter.Folder);
            if (_FileSystem.GetFiles(startFolder, folderFilter.FilePattern, folderFilter.IncludeSubfolders, out files))
            {
                foreach (string file in files) yield return file;
                yield break;
            }

            if (folderFilter.IncludeSubfolders)
            {
                Queue<string> folders = new Queue<string>();
                folders.Enqueue(startFolder);
                while (folders.Count != 0)
                {
                    string folder = folders.Dequeue();

                    if (_FileSystem.GetFiles(folder, folderFilter.FilePattern, false, out files))
                        foreach (string file in files) yield return file;

                    if (folderFilter.IncludeSubfolders && _FileSystem.GetFolders(folder, out files))
                    {
                        foreach (string subfolder in files) folders.Enqueue(subfolder);
                    }
                }
            }
        }
    }
}