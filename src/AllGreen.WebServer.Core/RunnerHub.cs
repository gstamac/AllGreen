using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using UAParser;

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

        public override Task OnConnected()
        {
            _Reporter.Connected(new Guid(Context.ConnectionId), CleanupUserAgent(Context.Headers["User-Agent"]));
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            _Reporter.Disconnected(new Guid(Context.ConnectionId));
            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            _Reporter.Reconnected(new Guid(Context.ConnectionId));
            return base.OnReconnected();
        }

        public void Register(Guid connectionId, string userAgent)
        {
            _Reporter.Register(connectionId, CleanupUserAgent(userAgent));
            Clients.Caller.reload();
        }

        private string CleanupUserAgent(string userAgent)
        {
            if (String.IsNullOrEmpty(userAgent)) return "";

            Parser uaParser = Parser.GetDefault();
            return uaParser.Parse(userAgent).ToString();
        }
    }
}