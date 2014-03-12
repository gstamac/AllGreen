using System;
using System.Linq;

namespace AllGreen.Core
{
    public interface IFileLocationParser
    {
        FileLocation Parse(string fileLocationText);
    }
}
