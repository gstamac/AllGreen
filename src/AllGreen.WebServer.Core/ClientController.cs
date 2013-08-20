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
    public class ClientController : ApiController
    {
        private IWebResources _WebResources;
        private IRunnerResources _RunnerResources;

        public ClientController(IWebResources webResources, IRunnerResources runnerResources)
        {
            _WebResources = webResources;
            _RunnerResources = runnerResources;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Request.DumpToConsole();

            System.Web.Http.Routing.IHttpRouteData routeData = Request.GetRouteData();
            string path = routeData.Values.ContainsKey("path") ? routeData.Values["path"] as string : null;
            Console.WriteLine(String.Format("CLIENT PATH: {0}", path));

            return ServeClientFile(path);
        }

        private HttpResponseMessage ServeClientFile(string path)
        {
            string result = _WebResources.GetContent(String.Format(@"Client/{0}", path));

            if (path.EndsWith("runner.html"))
            {
                result = ModifyRunnerHtml(result);
            }

            HttpResponseMessage response = Request.CreateStringResponse(result, GetMediaType(path));
            response.SetNoCacheHeaders();
            return response;
        }

        private string ModifyRunnerHtml(string result)
        {
            IEnumerable<string> scriptFiles = _RunnerResources.GetScriptFiles();
            string scripts = String.Join("", scriptFiles.Select(scriptFile => String.Format("<script src=\"{0}\"></script>", scriptFile)));
            return result.Replace("<!--%SCRIPTS%-->", scripts);
        }

        private static string GetMediaType(string path)
        {
            string fileType = Path.GetExtension(path);
            if (!String.IsNullOrEmpty(fileType))
                fileType = fileType.Substring(1).ToLower();
            return @"text/" + fileType;
        }
    }
}