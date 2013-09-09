using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllGreen.WebServer.Core
{
    public interface IReporter
    {
        void Connected(Guid connectionId, string userAgent);
        void Reconnected(Guid connectionId);
        void Disconnected(Guid connectionId);
        void Register(Guid connectionId, string userAgent);
        void Reset(Guid connectionId);
        void Started(Guid connectionId, int totalTests);
        void Finished(Guid connectionId);
        void SpecUpdated(Guid connectionId, Spec spec);
    }
}
