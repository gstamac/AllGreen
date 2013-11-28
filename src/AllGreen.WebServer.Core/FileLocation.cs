using System.Collections.Generic;
using System;

namespace AllGreen.WebServer.Core
{
    public class FileLocation
    {
        public string Filename { get; set; }
        public string FullPath { get; set; }
        public int LineNumber { get; set; }

        public FileLocation(string filename, string fullPath, int lineNumber)
        {
            Filename = filename;
            FullPath = fullPath;
            LineNumber = lineNumber;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", Filename, LineNumber);
        }
    }
}