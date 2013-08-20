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
    public static class HttpResponseMessageExtensions
    {
        public static void SetNoCacheHeaders(this HttpResponseMessage response)
        {
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Pragma", "no-cache");
            if (response.Content != null)
                response.Content.Headers.Expires = new DateTimeOffset(DateTime.Now.AddDays(-1));
        }

    }
}
