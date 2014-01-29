using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.Runner.WPF.Tests
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("AddDataMsTests.tt", "")]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	partial class FileLocationParserTests
	{
		// DataTestMethod(@"file1.js:10", "file1.js", 10, 0)
		[TestMethod]
		[Description("Test(@\"file1.js:10\", \"file1.js\", 10, 0)")]
		public void Test___file1_js_10____file1_js___10__0()
		{
			Test(@"file1.js:10", "file1.js", 10, 0);
		}
		// DataTestMethod(@"/folder1/file1.js:10", @"/folder1/file1.js", 10, 0)
		[TestMethod]
		[Description("Test(@\"/folder1/file1.js:10\", @\"/folder1/file1.js\", 10, 0)")]
		public void Test____folder1_file1_js_10______folder1_file1_js___10__0()
		{
			Test(@"/folder1/file1.js:10", @"/folder1/file1.js", 10, 0);
		}
		// DataTestMethod(@"file1.js:10:20", "file1.js", 10, 20)
		[TestMethod]
		[Description("Test(@\"file1.js:10:20\", \"file1.js\", 10, 20)")]
		public void Test___file1_js_10_20____file1_js___10__20()
		{
			Test(@"file1.js:10:20", "file1.js", 10, 20);
		}
		// DataTestMethod(@"file1.js", "file1.js", 0, 0)
		[TestMethod]
		[Description("Test(@\"file1.js\", \"file1.js\", 0, 0)")]
		public void Test___file1_js____file1_js___0__0()
		{
			Test(@"file1.js", "file1.js", 0, 0);
		}
		// DataTestMethod(@"http://localhost:8080/file1.js:10", "/file1.js", 10, 0)
		[TestMethod]
		[Description("ServerUrlTest(@\"http://localhost:8080/file1.js:10\", \"/file1.js\", 10, 0)")]
		public void ServerUrlTest___http___localhost_8080_file1_js_10_____file1_js___10__0()
		{
			ServerUrlTest(@"http://localhost:8080/file1.js:10", "/file1.js", 10, 0);
		}
		// DataTestMethod(@"http://localhost:8080/folder1/file1.js:10", @"/folder1/file1.js", 10, 0)
		[TestMethod]
		[Description("ServerUrlTest(@\"http://localhost:8080/folder1/file1.js:10\", @\"/folder1/file1.js\"," +
		    " 10, 0)")]
		public void ServerUrlTest___http___localhost_8080_folder1_file1_js_10______folder1_file1_js___10__0()
		{
			ServerUrlTest(@"http://localhost:8080/folder1/file1.js:10", @"/folder1/file1.js", 10, 0);
		}
		// DataTestMethod(@"http://localhost:8080/file1.js:10:20", "/file1.js", 10, 20)
		[TestMethod]
		[Description("ServerUrlTest(@\"http://localhost:8080/file1.js:10:20\", \"/file1.js\", 10, 20)")]
		public void ServerUrlTest___http___localhost_8080_file1_js_10_20_____file1_js___10__20()
		{
			ServerUrlTest(@"http://localhost:8080/file1.js:10:20", "/file1.js", 10, 20);
		}
		// DataTestMethod(@"http://localhost:8080/Test/testScript.js:24:9", "/Test/testScript.js", 24, 9)
		[TestMethod]
		[Description("ServerUrlTest(@\"http://localhost:8080/Test/testScript.js:24:9\", \"/Test/testScript" +
		    ".js\", 24, 9)")]
		public void ServerUrlTest___http___localhost_8080_Test_testScript_js_24_9_____Test_testScript_js___24__9()
		{
			ServerUrlTest(@"http://localhost:8080/Test/testScript.js:24:9", "/Test/testScript.js", 24, 9);
		}
		// DataTestMethod(@"http://localhost:8080/file1.js", "/file1.js", 0, 0)
		[TestMethod]
		[Description("ServerUrlTest(@\"http://localhost:8080/file1.js\", \"/file1.js\", 0, 0)")]
		public void ServerUrlTest___http___localhost_8080_file1_js_____file1_js___0__0()
		{
			ServerUrlTest(@"http://localhost:8080/file1.js", "/file1.js", 0, 0);
		}
		// DataTestMethod(@"/file1.js:10", @"/file1.js", @"C:\content\file1.js")
		[TestMethod]
		[Description("WebResourcesTest(@\"/file1.js:10\", @\"/file1.js\", @\"C:\\content\\file1.js\")")]
		public void WebResourcesTest____file1_js_10______file1_js_____C__content_file1_js_()
		{
			WebResourcesTest(@"/file1.js:10", @"/file1.js", @"C:\content\file1.js");
		}
		// DataTestMethod(@"/folder1/file1.js:10", @"/folder1/file1.js", @"C:\content\folder1/file1.js")
		[TestMethod]
		[Description("WebResourcesTest(@\"/folder1/file1.js:10\", @\"/folder1/file1.js\", @\"C:\\content\\fold" +
		    "er1/file1.js\")")]
		public void WebResourcesTest____folder1_file1_js_10______folder1_file1_js_____C__content_folder1_file1_js_()
		{
			WebResourcesTest(@"/folder1/file1.js:10", @"/folder1/file1.js", @"C:\content\folder1/file1.js");
		}
		// DataTestMethod(@"/file1.js:10:20", @"/file1.js", @"C:\content\file1.js")
		[TestMethod]
		[Description("WebResourcesTest(@\"/file1.js:10:20\", @\"/file1.js\", @\"C:\\content\\file1.js\")")]
		public void WebResourcesTest____file1_js_10_20______file1_js_____C__content_file1_js_()
		{
			WebResourcesTest(@"/file1.js:10:20", @"/file1.js", @"C:\content\file1.js");
		}
		// DataTestMethod(@"/file1.js", @"/file1.js", @"C:\content\file1.js")
		[TestMethod]
		[Description("WebResourcesTest(@\"/file1.js\", @\"/file1.js\", @\"C:\\content\\file1.js\")")]
		public void WebResourcesTest____file1_js______file1_js_____C__content_file1_js_()
		{
			WebResourcesTest(@"/file1.js", @"/file1.js", @"C:\content\file1.js");
		}
	}
}
