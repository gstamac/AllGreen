
namespace AllGreen.WebServer.Core
{
    public interface IRunnerHub
    {
        void ReloadAll();
        void Reset();
        void Started(int totalTests);
        void SpecUpdated(Spec spec);
        void Finished();
        void Register();
    }
}