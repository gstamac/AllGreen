using System;
using Microsoft.AspNet.SignalR.Hubs;
using AllGreen.WebServer.Core;

namespace AllGreen.WebServer.Owin
{
    public class RunnerClients : IRunnerClients
    {
        private readonly IHubConnectionContext _HubConnectionContext;

        public RunnerClients(IHubConnectionContext hubConnectionContext)
        {
            this._HubConnectionContext = hubConnectionContext;
        }
        public void ReloadAll()
        {
            _HubConnectionContext.All.reload();
        }
    }
}
