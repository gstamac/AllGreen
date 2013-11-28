using System;
using System.Linq;
using System.Windows.Threading;
using AllGreen.Runner.WPF.ViewModels;
using AllGreen.WebServer.Core;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF
{
    public interface IFileLocationMapper
    {
        FileLocation Map(string filename, int lineNumber);
    }
}
