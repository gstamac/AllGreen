using System;
using System.Linq;
using System.Diagnostics;

namespace AllGreen.Core
{
    public class JsMapFileMapper : IFileLocationMapper
    {
        private readonly IFileSystem _FileSystem;

        public JsMapFileMapper(IFileSystem fileSystem)
        {
            _FileSystem = fileSystem;
        }

        public FileLocation Map(FileLocation fileLocation)
        {
            if (_FileSystem.FileExists(fileLocation.FullPath + ".map"))
                return MapLocation(fileLocation);
            else
                return null;
        }

        private FileLocation MapLocation(FileLocation fileLocation)
        {
            try
            {
                JsMapFile jsMapFile = JsMapFile.CreateFromString(_FileSystem.ReadAllText(fileLocation.FullPath + ".map"));

                if (jsMapFile != null)
                {
                    JsMappingSegment mapping = jsMapFile.Mappings.Where(m => m.GeneratedLine == fileLocation.LineNumber).FirstOrDefault();

                    if (mapping != null)
                    {
                        return new FileLocation(
                            fileLocation.Filename.Replace(jsMapFile.OutputFile, mapping.Source),
                            fileLocation.FullPath.Replace(jsMapFile.OutputFile, mapping.Source),
                            mapping.SourceStartingLine,
                            mapping.SourceStartingColumn);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
    }
}
