using System;
using Microsoft.AspNet.SignalR.Hubs;
using AllGreen.Core;

namespace AllGreen.WebServer.Owin
{
    public class RunnerBroadcaster : IRunnerBroadcaster
    {
        private readonly IHubConnectionContext _HubConnectionContext;

        public RunnerBroadcaster(IHubConnectionContext hubConnectionContext)
        {
            this._HubConnectionContext = hubConnectionContext;
        }
        public void StartAll()
        {
            _HubConnectionContext.All.runTests();
        }
    }
}
