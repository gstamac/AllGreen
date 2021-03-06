﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using AllGreen.Core;
using Microsoft.AspNet.SignalR;
using TinyIoC;
using AllGreen.WebServer.Owin;
using AllGreen.Runner.WPF.Core.ViewModels;
using AllGreen.Runner.WPF.Core;

namespace AllGreen.Runner.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private FileWatcher _FileWatcher;

        protected void Application_Startup(object sender, StartupEventArgs e)
        {
            //XmlConfiguration configuration = XmlConfiguration.LoadFrom(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AllGreen\\AllGreen\\AllGreen.config");
            XmlConfiguration configuration = GetDummyConfig();

            TinyIoCContainer resourceResolver = new TinyIoCContainer();
            resourceResolver.Register<IConfiguration>(configuration);
            CompositeWebResources webResources = new CompositeWebResources();
            DynamicScriptList scriptList = new DynamicScriptList(configuration.RootFolder, configuration.ServedFolderFilters, configuration.ExcludeServedFolderFilters, new FileSystem());
            webResources.Add(new WebServerResources(scriptList));
            webResources.Add(new FileSystemResources(configuration.RootFolder, scriptList, new FileSystem()));
            resourceResolver.Register<IWebResources>(webResources);
            resourceResolver.Register<IRunnerHub, RunnerHub>();
            resourceResolver.Register<IRunnerBroadcaster>((ioc, npo) => new RunnerBroadcaster(GlobalHost.ConnectionManager.GetHubContext<RunnerHub>().Clients));
            resourceResolver.Register<IServerStarter>(new OwinServerStarter(resourceResolver));
            resourceResolver.Register<IFileViewer, ExternalFileViewer>();
            resourceResolver.Register<IFileLocationParser>(new FileLocationParser(configuration.ServerUrl, webResources));
            resourceResolver.Register<IFileLocationMapper>(new JsMapFileMapper(new FileSystem()));
            resourceResolver.Register<IClipboard>(new ClipboardProxy());

            _FileWatcher = new FileWatcher(resourceResolver, configuration.WatchedFolderFilters.Select(ff => 
                new FolderWatcher(Path.GetFullPath(Path.Combine(configuration.RootFolder, ff.Folder)), ff.FilePattern, ff.IncludeSubfolders)));

            MainWindow mainWindow = new MainWindow() { DataContext = new MainViewModel(resourceResolver) };
            mainWindow.Show();
        }

        protected XmlConfiguration GetDummyConfig()
        {
            return new XmlConfiguration()
            {
                ServerUrl = @"http://localhost:8080",
                RootFolder = @"C:\Work\Projects\AllGreen\src\AllGreen.WebServer.Resources",
                ServedFolderFilters = new List<FolderFilter>(
                    new FolderFilter[] { 
                        new FolderFilter() { Folder = "Scripts", FilePattern = "jasmine.js", IncludeSubfolders = false },
                        new FolderFilter() { Folder = "Client/ReporterAdapters", FilePattern = "jasmineAdapter.js", IncludeSubfolders = false },
                        new FolderFilter() { Folder = "Test", FilePattern = "testScript.js", IncludeSubfolders = false },
                    }),
                ExcludeServedFolderFilters = new List<FolderFilter>(
                    new FolderFilter[] { 
                        new FolderFilter() { Folder = "Scripts", FilePattern = "*.min.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "Scripts", FilePattern = "*.intellisense.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "Client", FilePattern = "*.min.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "spec", FilePattern = "*.min.js", IncludeSubfolders = true },
                    }),
                WatchedFolderFilters = new List<FolderFilter>() { 
                    new FolderFilter() { Folder = "Test", FilePattern = "*.js", IncludeSubfolders = false }
                }
            };
        }

        protected XmlConfiguration GetAllGreenProjectConfig()
        {
            return new XmlConfiguration()
            {
                ServerUrl = @"http://localhost:8080",
                RootFolder = @"C:\Work\Projects\AllGreen\src\AllGreen.WebServer.Resources",
                ServedFolderFilters = new List<FolderFilter>(
                    new FolderFilter[] { 
                        new FolderFilter() { Folder = "Scripts", FilePattern = "jasmine.js", IncludeSubfolders = false },
                        //new FolderFilter() { Folder = "Scripts", FilePattern = "jquery-2.1.0.js", IncludeSubfolders = false },
                        //new FolderFilter() { Folder = "Scripts", FilePattern = "jasmine-jquery.js", IncludeSubfolders = false },
                        //new FolderFilter() { Folder = "Client", FilePattern = "*.html", IncludeSubfolders = false },
                        new FolderFilter() { Folder = "Client/ReporterAdapters", FilePattern = "jasmineAdapter.js", IncludeSubfolders = false },
                        new FolderFilter() { Folder = "spec", FilePattern = "*.js", IncludeSubfolders = true },
                    }),
                ExcludeServedFolderFilters = new List<FolderFilter>(
                    new FolderFilter[] { 
                        new FolderFilter() { Folder = "Scripts", FilePattern = "*.min.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "Scripts", FilePattern = "*.intellisense.js", IncludeSubfolders = true },
                        new FolderFilter() { Folder = "Client", FilePattern = "*.min.js", IncludeSubfolders = true },
                    }),
                WatchedFolderFilters = new List<FolderFilter>() { 
                    new FolderFilter() { Folder = "Test", FilePattern = "*.js", IncludeSubfolders = false }
                }
            };
        }
    }
}