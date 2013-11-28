
namespace AllGreen.WebServer.Core
{
    public interface IRunnerHub
    {
        void Reset();
        void Started(int totalTests);
        void SpecUpdated(Spec spec);
        void Finished();
        void Register();
    }
}