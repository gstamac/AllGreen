using System.Collections.Generic;
using System;

namespace AllGreen.Core
{
    public interface IFileViewer
    {
        void Open(string fullPath, int lineNumber, int columnNumber);
    }
}
