using System.Collections.Generic;
using System.IO;

namespace AllGreen.WebServer.Core
{
    public class DynamicScriptList : IScriptList
    {
        private readonly string _RootFolder;
        IEnumerable<FolderFilter> _Filters = new List<FolderFilter>();
        private readonly IFileLocator _FileLocator;

        public DynamicScriptList(string rootFolder, IEnumerable<FolderFilter> folderFilters, IFileLocator fileLocator)
        {
            _RootFolder = rootFolder;
            _Filters = folderFilters;
            _FileLocator = fileLocator;
        }

        public IEnumerable<string> Files
        {
            get
            {
                foreach (FolderFilter rule in _Filters)
                {
                    foreach (string file in GetFilesForFolderFilter(rule))
                    {
                        if (file.StartsWith(_RootFolder))
                            yield return file.Substring(_RootFolder.Length + 1).Replace('\\', '/');
                        else
                            yield return file.Replace('\\', '/');
                    }
                }
            }
        }

        IEnumerable<string> GetFilesForFolderFilter(FolderFilter folderFilter)
        {
            string[] files;
            string startFolder = Path.Combine(_RootFolder, folderFilter.Folder);
            if (_FileLocator.GetFiles(startFolder, folderFilter.FilePattern, folderFilter.IncludeSubfolders, out files))
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

                    if (_FileLocator.GetFiles(folder, folderFilter.FilePattern, false, out files))
                        foreach (string file in files) yield return file;

                    if (folderFilter.IncludeSubfolders && _FileLocator.GetFolders(folder, out files))
                    {
                        foreach (string subfolder in files) folders.Enqueue(subfolder);
                    }
                }
            }
        }
    }
}