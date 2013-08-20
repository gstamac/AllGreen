using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AllGreen.WebServer.Core;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;

namespace AllGreen.WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8080";

            using (WebApp.Start<OwinStartup>(url))
            {
                Console.WriteLine("Server running at " + url);
                string command = "";
                while (command != "x")
                {
                    command = Console.ReadLine();
                    RunnerHub.Reload();
                }

                Console.ReadLine();
            }
        }
    }
}
