using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllGreen.WebServer.Core
{
    public interface IReporter
    {
        void SpecUpdated(Spec spec);
    }
}
