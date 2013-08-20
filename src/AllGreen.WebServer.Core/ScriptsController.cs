using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;

namespace AllGreen.WebServer.Core
{
    public class ScriptsController : ApiController
    {
        private IWebResources _WebResources;

        public ScriptsController(IWebResources webResources)
        {
            _WebResources = webResources;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Request.DumpToConsole();

            System.Web.Http.Routing.IHttpRouteData routeData = Request.GetRouteData();
            string path = routeData.Values.ContainsKey("path") ? routeData.Values["path"] as string : null;
            Console.WriteLine(String.Format("SCRIPTS PATH: {0}", path));

            return ServeScript(path);
        }

        private HttpResponseMessage ServeScript(string path)
        {
            string result = _WebResources.GetContent(String.Format(@"Scripts/{0}", path));

            return Request.CreateStringResponse(result, @"text/" + Path.GetExtension(path).Substring(1).ToLower());
        }
    }
}
