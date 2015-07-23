using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup;
using AllGreen.Core;
using Caliburn.Micro;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using TemplateAttributes;
using TinyIoC;
using System.Windows;

namespace AllGreen.Runner.WPF.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
    public class ExternalFileViewer : IFileViewer
    {
        public void Open(string fullPath, int lineNumber, int columnNumber)
        {
            if (!String.IsNullOrEmpty(fullPath))
            {
                //System.Diagnostics.Process process = new System.Diagnostics.Process() { EnableRaisingEvents = false };
                ////process.StartInfo.FileName = "editplus.exe";
                ////process.StartInfo.Arguments = String.Format("-e {0} -cursor {1}:{2}", fullPath, lineNumber, columnNumber);
                ////process.StartInfo.FileName = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe";
                ////process.StartInfo.Arguments = String.Format("/edit \"{0}\" /command \"edit.goto {1}\"", fullPath, lineNumber, columnNumber);
                //process.StartInfo.FileName = @"C:\WINDOWS\system32\WindowsPowerShell\v1.0\powershell.exe";
                //process.StartInfo.Arguments = String.Format("/edit \"{0}\" /command \"edit.goto {1}\"", fullPath, lineNumber, columnNumber);
                //process.Start();

                EnvDTE.DTE dte = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.12.0");
                dte.ExecuteCommand("File.OpenFile", fullPath);
                dte.ActiveDocument.Selection.MoveToDisplayColumn(lineNumber, columnNumber);
                dte.MainWindow.Activate();
            }
        }
    }
}
