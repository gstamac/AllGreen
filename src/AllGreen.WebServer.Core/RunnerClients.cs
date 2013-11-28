using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using UAParser;
using Microsoft.AspNet.SignalR.Hubs;

namespace AllGreen.WebServer.Core
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
