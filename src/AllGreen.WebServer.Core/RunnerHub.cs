using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Owin;

namespace AllGreen.WebServer.Core
{
    public class RunnerHub : Hub, IReporter
    {
        private readonly IHubContext _HubContext;
        private readonly IReporter _Reporter;

        public RunnerHub(IHubContext hubContext, IReporter reporter)
        {
            _HubContext = hubContext;
            _Reporter = reporter;
        }

        public void Reload()
        {
            _HubContext.Clients.All.reload();
        }

        public void Reset(Guid connectionId)
        {
            _Reporter.Reset(connectionId);
        }

        public void Started(Guid connectionId, int totalTests)
        {
            _Reporter.Started(connectionId, totalTests);
        }

        public void SpecUpdated(Guid connectionId, Spec spec)
        {
            _Reporter.SpecUpdated(connectionId, spec);
        }

        public void Finished(Guid connectionId)
        {
            _Reporter.Finished(connectionId);
        }

        public void Register(Guid connectionId, string userAgent)
        {
            _Reporter.Register(connectionId, userAgent);   
        }
    }
}