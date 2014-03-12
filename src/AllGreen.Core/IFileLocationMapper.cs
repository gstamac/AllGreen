using System;
using System.Linq;

namespace AllGreen.Core
{
    public interface IFileLocationMapper
    {
        FileLocation Map(FileLocation fileLocation);
    }
}
