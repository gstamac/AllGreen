using System;
using System.Net.Http;

namespace AllGreen.WebServer.Owin
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
