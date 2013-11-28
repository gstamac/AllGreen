using System.Collections.Generic;
using System;

namespace AllGreen.WebServer.Core
{
    public interface IFileViewer
    {
        void Open(FileLocation FileLocation);
    }
}
