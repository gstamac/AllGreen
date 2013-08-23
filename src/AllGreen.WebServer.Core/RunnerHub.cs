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
    public class RunnerHub : Hub
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

        public void Reset()
        {
        }

        public void Started()
        {
            
        }

        public void SpecUpdated(Spec spec)
        {
            _Reporter.SpecUpdated(spec);
        }

        public void Finished()
        {
            
        }
    }
}