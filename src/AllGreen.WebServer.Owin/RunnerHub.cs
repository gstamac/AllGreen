using System;
using System.Threading.Tasks;
using AllGreen.WebServer.Core;
using Microsoft.AspNet.SignalR;
using UAParser;

namespace AllGreen.WebServer.Owin
{
    public class RunnerHub : Hub, IRunnerHub
    {
        private readonly IReporter _Reporter;

        public RunnerHub(IReporter reporter)
        {
            _Reporter = reporter;
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
            ClientInfo clientInfo = uaParser.Parse(userAgent);
            string os = clientInfo.OS.ToString();
            string device = clientInfo.Device.ToString();
            if (device == "Other") device = "";
            string browser = clientInfo.UserAgent.ToString();
            return String.Format("{0} {1} {2}", os, device, browser).Replace("  ", " ");
        }
    }
}