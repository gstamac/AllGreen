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
		// DataTestMethod(@"~internal~/content.html", @"text/html")
		[TestMethod]
		[Description("GetRespone(@\"~internal~/content.html\", @\"text/html\")")]
		public void GetRespone____internal__content_html_____text_html_()
		{
			GetRespone(@"~internal~/content.html", @"text/html");
		}
		// DataTestMethod(@"~internal~/Client/content.html", @"text/html")
		[TestMethod]
		[Description("GetRespone(@\"~internal~/Client/content.html\", @\"text/html\")")]
		public void GetRespone____internal__Client_content_html_____text_html_()
		{
			GetRespone(@"~internal~/Client/content.html", @"text/html");
		}
		// DataTestMethod(@"~internal~/Client/content.css", @"text/css")
		[TestMethod]
		[Description("GetRespone(@\"~internal~/Client/content.css\", @\"text/css\")")]
		public void GetRespone____internal__Client_content_css_____text_css_()
		{
			GetRespone(@"~internal~/Client/content.css", @"text/css");
		}
		// DataTestMethod(@"content.html", @"text/html")
		[TestMethod]
		[Description("GetRespone(@\"content.html\", @\"text/html\")")]
		public void GetRespone___content_html_____text_html_()
		{
			GetRespone(@"content.html", @"text/html");
		}
	}
}
