using System.Collections.Generic;
using System;

namespace AllGreen.WebServer.Core
{
    public class FileLocation
    {
        public string Filename { get; set; }
        public string FullPath { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }

        public FileLocation(string filename, string fullPath, int lineNumber, int columnNumber = 0)
        {
            Filename = filename;
            FullPath = fullPath;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        public override string ToString()
        {
            if (ColumnNumber > 0)
                return String.Format("{0}:{1}:{2}", Filename, LineNumber, ColumnNumber);
            else
                return String.Format("{0}:{1}", Filename, LineNumber);
        }
    }
}