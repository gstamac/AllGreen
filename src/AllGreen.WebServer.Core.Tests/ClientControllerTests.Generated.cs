using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.WebServer.Core.Tests
{
	partial class ClientControllerTests
	{
		// DataRow(@"/Client/allgreen.js", @"Client/allgreen.js", @"text/js")
		[TestMethod]
		[Description("GetClientResponse(@\"/Client/allgreen.js\", @\"Client/allgreen.js\", @\"text/js\")")]
		public void GetClientResponse____Client_allgreen_js_____Client_allgreen_js_____text_js_()
		{
			GetClientResponse(@"/Client/allgreen.js", @"Client/allgreen.js", @"text/js");
		}
		// DataRow(@"/Client/client.html", @"Client/client.html", @"text/html")
		[TestMethod]
		[Description("GetClientResponse(@\"/Client/client.html\", @\"Client/client.html\", @\"text/html\")")]
		public void GetClientResponse____Client_client_html_____Client_client_html_____text_html_()
		{
			GetClientResponse(@"/Client/client.html", @"Client/client.html", @"text/html");
		}
	}
}
