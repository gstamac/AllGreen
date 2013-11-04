using System.Reflection;
using System.Windows;
using AllGreen.WebServer.Core;
using Microsoft.AspNet.SignalR;
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
            XmlConfiguration configuration = new XmlConfiguration("");

            TinyIoCContainer resourceResolver = new TinyIoCContainer();
            resourceResolver.Register<IConfiguration>(configuration);
            resourceResolver.Register<IWebResources>(new EmbededResources(Assembly.Load("AllGreen.WebServer.Resources")));
            resourceResolver.Register<IRunnerResources>(new RunnerResources(new DynamicScriptList(configuration, new SystemFileLocator())));
            resourceResolver.Register<IRunnerHub, RunnerHub>();

            MainWindow mainWindow = new MainWindow() { DataContext = new MainViewModel(resourceResolver) };
            mainWindow.Show();
        }
    }
}
