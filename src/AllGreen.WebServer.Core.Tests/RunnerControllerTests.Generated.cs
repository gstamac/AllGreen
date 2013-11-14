using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.WebServer.Core.Tests
{
	partial class RunnerControllerTests
	{
		// DataTestMethod(@"Runner/Client/allgreen.js", @"text/js")
		[TestMethod]
		[Description("GetResponeTest(@\"Runner/Client/allgreen.js\", @\"text/js\")")]
		public void GetResponeTest___Runner_Client_allgreen_js_____text_js_()
		{
			GetResponeTest(@"Runner/Client/allgreen.js", @"text/js");
		}
		// DataTestMethod(@"Runner/Client/client.html", @"text/html")
		[TestMethod]
		[Description("GetResponeTest(@\"Runner/Client/client.html\", @\"text/html\")")]
		public void GetResponeTest___Runner_Client_client_html_____text_html_()
		{
			GetResponeTest(@"Runner/Client/client.html", @"text/html");
		}
		// DataTestMethod(@"Runner/Scripts/jquery.js", @"text/js")
		[TestMethod]
		[Description("GetResponeTest(@\"Runner/Scripts/jquery.js\", @\"text/js\")")]
		public void GetResponeTest___Runner_Scripts_jquery_js_____text_js_()
		{
			GetResponeTest(@"Runner/Scripts/jquery.js", @"text/js");
		}
	}
}
