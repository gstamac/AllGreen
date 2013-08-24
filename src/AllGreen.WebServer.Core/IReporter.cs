using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllGreen.WebServer.Core
{
    public interface IReporter
    {
        void Register(Guid connectionId, string userAgent);
        void Reset(Guid connectionId);
        void Started(Guid connectionId, int totalTests);
        void SpecUpdated(Guid connectionId, Spec spec);
        void Finished(Guid connectionId);
    }
}
