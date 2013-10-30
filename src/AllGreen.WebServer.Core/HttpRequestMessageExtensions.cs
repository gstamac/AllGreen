using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AllGreen.WebServer.Core
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpResponseMessage CreateStringResponse(this HttpRequestMessage request, string content, string mediaType)
        {
            if (content == null)
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(content, Encoding.UTF8, mediaType);
                return response;
            }
        }

        //ncrunch: no coverage start
        public static void DumpToConsole(this HttpRequestMessage request)
        {
            Console.WriteLine("------------- REQUEST --------------");
            Console.WriteLine(DateTime.Now.ToString("u"));
            System.Web.Http.Routing.IHttpRouteData routeData = request.GetRouteData();
            Console.WriteLine(String.Format("CONTROLLER: {0}", routeData.Values["controller"]));
            Console.WriteLine(String.Format("ACTION: {0}", routeData.Values.ContainsKey("action") ? routeData.Values["action"] : ""));
            Console.WriteLine(String.Format("METHOD: {0}", request.Method));
            int contentLength = request.Content.ReadAsStringAsync().Result.Length;
            Console.WriteLine(String.Format("{0}\n\n{1}\n\nCONTENT LENGTH: {2}", request.RequestUri, request.Headers, contentLength));
            if (contentLength <= 50)
                Console.WriteLine(String.Format("CONTENT: {0}", request.Content.ReadAsStringAsync().Result));
            Console.WriteLine("------------------------------------");
        }
        //ncrunch: no coverage end
    }
}
