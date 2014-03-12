using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.Runner.WPF.Core.Tests
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("AddDataMsTests.tt", "")]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	partial class SpecTraceStepViewModelTests
	{
		// DataTestMethod(@"level2@http://localhost:8080/Test/testScript.js:28", @"http://localhost:8080/Test/testScript.js:28", @"level2", @"/Test/testScript.js", 28)
		[TestMethod]
		[Description("ParseFirefoxTrace(@\"level2@http://localhost:8080/Test/testScript.js:28\", @\"http:/" +
		    "/localhost:8080/Test/testScript.js:28\", @\"level2\", @\"/Test/testScript.js\", 28)")]
		public void ParseFirefoxTrace___level2_http___localhost_8080_Test_testScript_js_28_____http___localhost_8080_Test_testScript_js_28_____level2______Test_testScript_js___28()
		{
			ParseFirefoxTrace(@"level2@http://localhost:8080/Test/testScript.js:28", @"http://localhost:8080/Test/testScript.js:28", @"level2", @"/Test/testScript.js", 28);
		}
		// DataTestMethod(@"level1@http://localhost:8080/Test/testScript.js:24", @"http://localhost:8080/Test/testScript.js:24", @"level1", @"/Test/testScript.js", 24)
		[TestMethod]
		[Description("ParseFirefoxTrace(@\"level1@http://localhost:8080/Test/testScript.js:24\", @\"http:/" +
		    "/localhost:8080/Test/testScript.js:24\", @\"level1\", @\"/Test/testScript.js\", 24)")]
		public void ParseFirefoxTrace___level1_http___localhost_8080_Test_testScript_js_24_____http___localhost_8080_Test_testScript_js_24_____level1______Test_testScript_js___24()
		{
			ParseFirefoxTrace(@"level1@http://localhost:8080/Test/testScript.js:24", @"http://localhost:8080/Test/testScript.js:24", @"level1", @"/Test/testScript.js", 24);
		}
		// DataTestMethod(@"@http://localhost:8080/Test/testScript.js:34", @"http://localhost:8080/Test/testScript.js:34", "<anonymous>", @"/Test/testScript.js", 34)
		[TestMethod]
		[Description("ParseFirefoxTrace(@\"@http://localhost:8080/Test/testScript.js:34\", @\"http://local" +
		    "host:8080/Test/testScript.js:34\", \"<anonymous>\", @\"/Test/testScript.js\", 34)")]
		public void ParseFirefoxTrace____http___localhost_8080_Test_testScript_js_34_____http___localhost_8080_Test_testScript_js_34_____anonymous_______Test_testScript_js___34()
		{
			ParseFirefoxTrace(@"@http://localhost:8080/Test/testScript.js:34", @"http://localhost:8080/Test/testScript.js:34", "<anonymous>", @"/Test/testScript.js", 34);
		}
		// DataTestMethod(@"   at level2 (http://localhost:8080/Test/testScript.js:28:9)", @"http://localhost:8080/Test/testScript.js:28:9", @"level2", @"/Test/testScript.js", 28, 9)
		[TestMethod]
		[Description("ParseIETrace(@\"   at level2 (http://localhost:8080/Test/testScript.js:28:9)\", @\"h" +
		    "ttp://localhost:8080/Test/testScript.js:28:9\", @\"level2\", @\"/Test/testScript.js\"" +
		    ", 28, 9)")]
		public void ParseIETrace______at_level2__http___localhost_8080_Test_testScript_js_28_9______http___localhost_8080_Test_testScript_js_28_9_____level2______Test_testScript_js___28__9()
		{
			ParseIETrace(@"   at level2 (http://localhost:8080/Test/testScript.js:28:9)", @"http://localhost:8080/Test/testScript.js:28:9", @"level2", @"/Test/testScript.js", 28, 9);
		}
		// DataTestMethod(@"   at level1 (http://localhost:8080/Test/testScript.js:24:9)", @"http://localhost:8080/Test/testScript.js:24:9", @"level1", @"/Test/testScript.js", 24, 9)
		[TestMethod]
		[Description("ParseIETrace(@\"   at level1 (http://localhost:8080/Test/testScript.js:24:9)\", @\"h" +
		    "ttp://localhost:8080/Test/testScript.js:24:9\", @\"level1\", @\"/Test/testScript.js\"" +
		    ", 24, 9)")]
		public void ParseIETrace______at_level1__http___localhost_8080_Test_testScript_js_24_9______http___localhost_8080_Test_testScript_js_24_9_____level1______Test_testScript_js___24__9()
		{
			ParseIETrace(@"   at level1 (http://localhost:8080/Test/testScript.js:24:9)", @"http://localhost:8080/Test/testScript.js:24:9", @"level1", @"/Test/testScript.js", 24, 9);
		}
		// DataTestMethod(@"   at Anonymous function (http://localhost:8080/Test/testScript.js:34:17)", @"http://localhost:8080/Test/testScript.js:34:17", "Anonymous function", @"/Test/testScript.js", 34, 17)
		[TestMethod]
		[Description("ParseIETrace(@\"   at Anonymous function (http://localhost:8080/Test/testScript.js" +
		    ":34:17)\", @\"http://localhost:8080/Test/testScript.js:34:17\", \"Anonymous function" +
		    "\", @\"/Test/testScript.js\", 34, 17)")]
		public void ParseIETrace______at_Anonymous_function__http___localhost_8080_Test_testScript_js_34_17______http___localhost_8080_Test_testScript_js_34_17____Anonymous_function______Test_testScript_js___34__17()
		{
			ParseIETrace(@"   at Anonymous function (http://localhost:8080/Test/testScript.js:34:17)", @"http://localhost:8080/Test/testScript.js:34:17", "Anonymous function", @"/Test/testScript.js", 34, 17);
		}
		// DataTestMethod(@"http://localhost:8080/Client/testScript.js:9")
		[TestMethod]
		[Description("ParseIncorrectJavascriptTrace(@\"http://localhost:8080/Client/testScript.js:9\")")]
		public void ParseIncorrectJavascriptTrace___http___localhost_8080_Client_testScript_js_9_()
		{
			ParseIncorrectJavascriptTrace(@"http://localhost:8080/Client/testScript.js:9");
		}
	}
}
