using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.WebServer.Core.Tests
{
	partial class WebControllerTests
	{
		// DataTestMethod(@"script.js", @"text/js")
		[TestMethod]
		[Description("GetRespone(@\"script.js\", @\"text/js\")")]
		public void GetRespone___script_js_____text_js_()
		{
			GetRespone(@"script.js", @"text/js");
		}
		// DataTestMethod(@"content.html", @"text/html")
		[TestMethod]
		[Description("GetRespone(@\"content.html\", @\"text/html\")")]
		public void GetRespone___content_html_____text_html_()
		{
			GetRespone(@"content.html", @"text/html");
		}
		// DataTestMethod(@"Client/content.html", @"text/html")
		[TestMethod]
		[Description("GetRespone(@\"Client/content.html\", @\"text/html\")")]
		public void GetRespone___Client_content_html_____text_html_()
		{
			GetRespone(@"Client/content.html", @"text/html");
		}
		// DataTestMethod(@"Client/content.css", @"text/css")
		[TestMethod]
		[Description("GetRespone(@\"Client/content.css\", @\"text/css\")")]
		public void GetRespone___Client_content_css_____text_css_()
		{
			GetRespone(@"Client/content.css", @"text/css");
		}
		// DataTestMethod(@"Files/content.html", @"text/html")
		[TestMethod]
		[Description("GetRespone(@\"Files/content.html\", @\"text/html\")")]
		public void GetRespone___Files_content_html_____text_html_()
		{
			GetRespone(@"Files/content.html", @"text/html");
		}
	}
}
