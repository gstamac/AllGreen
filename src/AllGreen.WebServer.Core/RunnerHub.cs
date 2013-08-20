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
        public static void Reload()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RunnerHub>();
            context.Clients.All.reload();
            //Clients.All.reload();
        }
    }
}
