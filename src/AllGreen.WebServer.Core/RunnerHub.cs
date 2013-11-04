using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using UAParser;

namespace AllGreen.WebServer.Core
{
    public class RunnerHub : Hub, IRunnerHub
    {
        private readonly IReporter _Reporter;

        public RunnerHub(IReporter reporter)
        {
            _Reporter = reporter;
        }

        public void ReloadAll()
        {
            // TODO: Cancel previous run
            Clients.All.reload();
        }

        public void Reset()
        {
            _Reporter.Reset(Context.ConnectionId);
        }

        public void Started(int totalTests)
        {
            _Reporter.Started(Context.ConnectionId, totalTests);
        }

        public void SpecUpdated(Spec spec)
        {
            //Console.WriteLine(String.Format("==> SPEC UPDATED {0}, {1}, {2}", spec.GetFullName(), spec.Status, spec.Time));
            _Reporter.SpecUpdated(Context.ConnectionId, spec);
        }

        public void Finished()
        {
            _Reporter.Finished(Context.ConnectionId);
        }

        public override Task OnConnected()
        {
            _Reporter.Connected(Context.ConnectionId, CleanupUserAgent(Context.Headers["User-Agent"]));
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            _Reporter.Disconnected(Context.ConnectionId);
            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            _Reporter.Reconnected(Context.ConnectionId, CleanupUserAgent(Context.Headers["User-Agent"]));
            return base.OnReconnected();
        }

        public void Register()
        {
            _Reporter.Register(Context.ConnectionId, CleanupUserAgent(Context.Headers["User-Agent"]));
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