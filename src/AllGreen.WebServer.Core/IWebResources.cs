using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;

namespace AllGreen.WebServer.Core
{
    public interface IWebResources
    {
        string GetContent(string path);
    }
}