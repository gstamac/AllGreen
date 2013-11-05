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
            XmlConfiguration configuration = new XmlConfiguration("");

            TinyIoCContainer resourceResolver = new TinyIoCContainer();
            resourceResolver.Register<IConfiguration>(configuration);
            CompositeWebResources webResources = new CompositeWebResources();
            webResources.Add(new EmbededResources(Assembly.Load("AllGreen.WebServer.Resources")));
            webResources.Add(new FileSystemResources(configuration));
            resourceResolver.Register<IWebResources>(webResources);
            resourceResolver.Register<IRunnerResources>(new RunnerResources(new DynamicScriptList(configuration, new SystemFileLocator())));
            resourceResolver.Register<IRunnerHub, RunnerHub>();

            MainWindow mainWindow = new MainWindow() { DataContext = new MainViewModel(resourceResolver) };
            mainWindow.Show();
        }
    }
}
