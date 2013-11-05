using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.WebServer.Core.Tests
{
	partial class FilesControllerTests
	{
		// DataTestMethod(@"script.js", @"text/js")
		[TestMethod]
		[Description("GetClientResponse(@\"script.js\", @\"text/js\")")]
		public void GetClientResponse___script_js_____text_js_()
		{
			GetClientResponse(@"script.js", @"text/js");
		}
		// DataTestMethod(@"content.html", @"text/html")
		[TestMethod]
		[Description("GetClientResponse(@\"content.html\", @\"text/html\")")]
		public void GetClientResponse___content_html_____text_html_()
		{
			GetClientResponse(@"content.html", @"text/html");
		}
	}
}
