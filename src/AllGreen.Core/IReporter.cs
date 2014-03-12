
namespace AllGreen.Core
{
    public interface IReporter
    {
        void Connected(string connectionId, string userAgent);
        void Reconnected(string connectionId, string userAgent);
        void Disconnected(string connectionId);
        void Register(string connectionId, string userAgent);
        void Reset(string connectionId);
        void Started(string connectionId, int totalTests);
        void Finished(string connectionId);
        void SpecUpdated(string connectionId, Spec spec);
    }
}