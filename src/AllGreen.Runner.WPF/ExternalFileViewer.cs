using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using TemplateAttributes;
using TinyIoC;
using System.Windows;

namespace AllGreen.Runner.WPF
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
    public class ExternalFileViewer : IFileViewer
    {
        public void Open(string fullPath, int lineNumber, int columnNumber)
        {
            if (!String.IsNullOrEmpty(fullPath))
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process() { EnableRaisingEvents = false };
                process.StartInfo.FileName = "editplus.exe";
                process.StartInfo.Arguments = String.Format("-e {0} -cursor {1}:{2}", fullPath, lineNumber, columnNumber);
                process.Start();
            }
        }
    }
}
