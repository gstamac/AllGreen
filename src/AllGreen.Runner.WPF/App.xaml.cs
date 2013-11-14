using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using AllGreen.WebServer.Core;
using TinyIoC;

namespace AllGreen.Runner.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected void Application_Startup(object sender, StartupEventArgs e)
        {
            //XmlConfiguration configuration = XmlConfiguration.LoadFrom(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AllGreen\\AllGreen\\AllGreen.config");
            XmlConfiguration configuration = GetDummyConfig();

            TinyIoCContainer resourceResolver = new TinyIoCContainer();
            resourceResolver.Register<IConfiguration>(configuration);
            CompositeWebResources webResources = new CompositeWebResources();
            webResources.Add(new WebServerResources(new DynamicScriptList(configuration.RootFolder, configuration.ServedFolderFilters, configuration.ExcludeServedFolderFilters, new SystemFileLocator())));
            webResources.Add(new FileSystemResources(configuration.RootFolder, new FileSystemReader()));
            resourceResolver.Register<IWebResources>(webResources);
            resourceResolver.Register<IRunnerHub, RunnerHub>();

            MainWindow mainWindow = new MainWindow() { DataContext = new MainViewModel(resourceResolver) };
            mainWindow.Show();
        }

        protected XmlConfiguration GetDummyConfig()
        {
            return new XmlConfiguration()
            {
                RootFolder = @"C:\Work\Projects\AllGreen\src\AllGreen.WebServer.Resources",
                ServerUrl = @"http://localhost:8080",
                ServedFolderFilters = new List<FolderFilter>(
                    new FolderFilter[] { 
                        //new FolderFilter() { Folder = "Scripts", FilePattern = "jasmine.js", IncludeSubfolders = false },
                        //new FolderFilter() { Folder = "Client", FilePattern = "testScript.js", IncludeSubfolders = false },
                        //new FolderFilter() { Folder = "Client/ReporterAdapters", FilePattern = "jasmineAdapter.js", IncludeSubfolders = false },

                        //new FolderFilter() { Folder = "Scripts", FilePattern = "jasmine.js", IncludeSubfolders = false },
                        new FolderFilter() { Folder = "Scripts", FilePattern = "*.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "Client", FilePattern = "*.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "spec", FilePattern = "*.js", IncludeSubfolders = true },
                    }),
                ExcludeServedFolderFilters = new List<FolderFilter>(
                    new FolderFilter[] { 
                        new FolderFilter() { Folder = "Scripts", FilePattern = "*.min.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "Scripts", FilePattern = "*.intellisense.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "Client", FilePattern = "*.min.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "spec", FilePattern = "*.min.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "Client", FilePattern = "allgreen.js", IncludeSubfolders = false },
                        new FolderFilter() { Folder = "Client", FilePattern = "hub.js", IncludeSubfolders = false },
                        new FolderFilter() { Folder = "Client", FilePattern = "reporter.js", IncludeSubfolders = false },
                        new FolderFilter() { Folder = "Client", FilePattern = "testScript.js", IncludeSubfolders = false },
                    }),
                WatchedFolderFilters = new List<FolderFilter>()
            };
        }
    }
}
