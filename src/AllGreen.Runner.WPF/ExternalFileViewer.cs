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
    //ncrunch: no coverage start
    public class ExternalFileViewer : IFileViewer
    {
        public void Open(FileLocation FileLocation)
        {
            if (!String.IsNullOrEmpty(FileLocation.FullPath))
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process() { EnableRaisingEvents = false };
                process.StartInfo.FileName = "editplus.exe";
                process.StartInfo.Arguments = String.Format("-e {0} -cursor {1}:0", FileLocation.FullPath, FileLocation.LineNumber);
                process.Start();
            }
        }
    }
    //ncrunch: no coverage start
}
