using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.WebServer.Core.Tests
{
	partial class AllGreenWebServerResources_EmbededResourcesTests
	{
		// DataRow(@"Client/allgreen.js")
		[TestMethod]
		[Description("FileExists(@\"Client/allgreen.js\")")]
		public void FileExists___Client_allgreen_js_()
		{
			FileExists(@"Client/allgreen.js");
		}
		// DataRow(@"Client/client.html")
		[TestMethod]
		[Description("FileExists(@\"Client/client.html\")")]
		public void FileExists___Client_client_html_()
		{
			FileExists(@"Client/client.html");
		}
		// DataRow(@"Client/client.css")
		[TestMethod]
		[Description("FileExists(@\"Client/client.css\")")]
		public void FileExists___Client_client_css_()
		{
			FileExists(@"Client/client.css");
		}
		// DataRow(@"Client/runner.html")
		[TestMethod]
		[Description("FileExists(@\"Client/runner.html\")")]
		public void FileExists___Client_runner_html_()
		{
			FileExists(@"Client/runner.html");
		}
		// DataRow(@"Client/reporter.js")
		[TestMethod]
		[Description("FileExists(@\"Client/reporter.js\")")]
		public void FileExists___Client_reporter_js_()
		{
			FileExists(@"Client/reporter.js");
		}
		// DataRow(@"Client/ReporterAdapters/jasmineAdapter.js")
		[TestMethod]
		[Description("FileExists(@\"Client/ReporterAdapters/jasmineAdapter.js\")")]
		public void FileExists___Client_ReporterAdapters_jasmineAdapter_js_()
		{
			FileExists(@"Client/ReporterAdapters/jasmineAdapter.js");
		}
		// DataRow(@"Scripts/jquery.js")
		[TestMethod]
		[Description("FileExists(@\"Scripts/jquery.js\")")]
		public void FileExists___Scripts_jquery_js_()
		{
			FileExists(@"Scripts/jquery.js");
		}
		// DataRow(@"Scripts/jquery.signalR.js")
		[TestMethod]
		[Description("FileExists(@\"Scripts/jquery.signalR.js\")")]
		public void FileExists___Scripts_jquery_signalR_js_()
		{
			FileExists(@"Scripts/jquery.signalR.js");
		}
		// DataRow(@"/Scripts/jquery.signalR.js")
		[TestMethod]
		[Description("FileExists(@\"/Scripts/jquery.signalR.js\")")]
		public void FileExists____Scripts_jquery_signalR_js_()
		{
			FileExists(@"/Scripts/jquery.signalR.js");
		}
	}
}
