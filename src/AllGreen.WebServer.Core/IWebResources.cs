
namespace AllGreen.WebServer.Core
{
    public interface IWebResources
    {
        string GetContent(string path);
        string GetSystemFilePath(string path);
    }
}