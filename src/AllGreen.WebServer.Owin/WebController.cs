using System;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using AllGreen.Core;

namespace AllGreen.WebServer.Owin
{
    public class WebController : ApiController
    {
        private const string DEFAULT_URL = "~internal~/Client/client.html";
        private IWebResources _WebResources;

        public WebController(IWebResources webResources)
        {
            _WebResources = webResources;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Request.DumpToConsole();

            System.Web.Http.Routing.IHttpRouteData routeData = Request.GetRouteData();
            string path = routeData.Values.ContainsKey("path") ? routeData.Values["path"] as string : null;
            //Console.WriteLine(String.Format("FILES PATH: {0}", path));

            if (String.IsNullOrEmpty(path)) path = DEFAULT_URL;

            return ServeFile(path);
        }

        private HttpResponseMessage ServeFile(string path)
        {
            string result = _WebResources.GetContent(path);

            HttpResponseMessage response = Request.CreateStringResponse(result, "text/" + Path.GetExtension(path).Substring(1).ToLower());
            response.SetNoCacheHeaders();

            return response;
        }
    }
}
