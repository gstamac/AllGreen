using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.WebServer.Core.Tests
{
	partial class ScriptsControllerTests
	{
		// DataTestMethod(@"jquery.js", @"text/js")
		[TestMethod]
		[Description("GetClientResponse(@\"jquery.js\", @\"text/js\")")]
		public void GetClientResponse___jquery_js_____text_js_()
		{
			GetClientResponse(@"jquery.js", @"text/js");
		}
		// DataTestMethod(@"jquery.signalR.js", @"text/js")
		[TestMethod]
		[Description("GetClientResponse(@\"jquery.signalR.js\", @\"text/js\")")]
		public void GetClientResponse___jquery_signalR_js_____text_js_()
		{
			GetClientResponse(@"jquery.signalR.js", @"text/js");
		}
	}
}
