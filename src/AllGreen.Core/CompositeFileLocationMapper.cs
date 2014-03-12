using System;
using System.Collections.Generic;
using System.Linq;

namespace AllGreen.Core
{
    public class CompositeFileLocationMapper : IFileLocationMapper
    {
        List<IFileLocationMapper> _List;

        public CompositeFileLocationMapper()
        {
            _List = new List<IFileLocationMapper>();
        }

        public void Add(IFileLocationMapper fileLocationMapper)
        {
            _List.Add(fileLocationMapper);
        }

        public FileLocation Map(FileLocation fileLocation)
        {
            foreach (IFileLocationMapper fileLocationMapper in _List)
            {
                FileLocation newFileLocation = fileLocationMapper.Map(fileLocation);
                if (newFileLocation != null)
                    fileLocation = newFileLocation;
            }
            return fileLocation;
        }
    }
}
