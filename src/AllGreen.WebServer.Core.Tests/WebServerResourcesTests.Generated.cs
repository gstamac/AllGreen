using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.WebServer.Core.Tests
{
	partial class WebServerResourcesTests
	{
		// DataRow(@"")
		[TestMethod]
		[Description("FileDoesntExist(@\"\")")]
		public void FileDoesntExist____()
		{
			FileDoesntExist(@"");
		}
		// DataRow(@"nonexistent.html")
		[TestMethod]
		[Description("FileDoesntExist(@\"nonexistent.html\")")]
		public void FileDoesntExist___nonexistent_html_()
		{
			FileDoesntExist(@"nonexistent.html");
		}
		// DataRow(@"Client/allgreen.js")
		[TestMethod]
		[Description("FileDoesntExist(@\"Client/allgreen.js\")")]
		public void FileDoesntExist___Client_allgreen_js_()
		{
			FileDoesntExist(@"Client/allgreen.js");
		}
		// DataRow(@"/Client/allgreen.js")
		[TestMethod]
		[Description("FileDoesntExist(@\"/Client/allgreen.js\")")]
		public void FileDoesntExist____Client_allgreen_js_()
		{
			FileDoesntExist(@"/Client/allgreen.js");
		}
		// DataRow(@"~internal~/Client/allgreen.js")
		[TestMethod]
		[Description("FileExists(@\"~internal~/Client/allgreen.js\")")]
		public void FileExists____internal__Client_allgreen_js_()
		{
			FileExists(@"~internal~/Client/allgreen.js");
		}
		// DataRow(@"~internal~/Client/client.html")
		[TestMethod]
		[Description("FileExists(@\"~internal~/Client/client.html\")")]
		public void FileExists____internal__Client_client_html_()
		{
			FileExists(@"~internal~/Client/client.html");
		}
		// DataRow(@"~internal~/Client/client.css")
		[TestMethod]
		[Description("FileExists(@\"~internal~/Client/client.css\")")]
		public void FileExists____internal__Client_client_css_()
		{
			FileExists(@"~internal~/Client/client.css");
		}
		// DataRow(@"~internal~/Client/runner.html")
		[TestMethod]
		[Description("FileExists(@\"~internal~/Client/runner.html\")")]
		public void FileExists____internal__Client_runner_html_()
		{
			FileExists(@"~internal~/Client/runner.html");
		}
		// DataRow(@"~internal~/Client/reporter.js")
		[TestMethod]
		[Description("FileExists(@\"~internal~/Client/reporter.js\")")]
		public void FileExists____internal__Client_reporter_js_()
		{
			FileExists(@"~internal~/Client/reporter.js");
		}
		// DataRow(@"~internal~/Client/ReporterAdapters/jasmineAdapter.js")
		[TestMethod]
		[Description("FileExists(@\"~internal~/Client/ReporterAdapters/jasmineAdapter.js\")")]
		public void FileExists____internal__Client_ReporterAdapters_jasmineAdapter_js_()
		{
			FileExists(@"~internal~/Client/ReporterAdapters/jasmineAdapter.js");
		}
		// DataRow(@"~internal~/Scripts/jquery.js")
		[TestMethod]
		[Description("FileExists(@\"~internal~/Scripts/jquery.js\")")]
		public void FileExists____internal__Scripts_jquery_js_()
		{
			FileExists(@"~internal~/Scripts/jquery.js");
		}
		// DataRow(@"~internal~/Scripts/jquery.signalR.js")
		[TestMethod]
		[Description("FileExists(@\"~internal~/Scripts/jquery.signalR.js\")")]
		public void FileExists____internal__Scripts_jquery_signalR_js_()
		{
			FileExists(@"~internal~/Scripts/jquery.signalR.js");
		}
		// DataRow(@"/~internal~/Scripts/jquery.signalR.js")
		[TestMethod]
		[Description("FileExists(@\"/~internal~/Scripts/jquery.signalR.js\")")]
		public void FileExists_____internal__Scripts_jquery_signalR_js_()
		{
			FileExists(@"/~internal~/Scripts/jquery.signalR.js");
		}
	}
}
