using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.Runner.WPF.Tests
{
	partial class SpecTraceStepViewModelTests
	{
		// DataTestMethod(@"jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:114", @"jasmine.ExpectationResult", @"/Scripts/jasmine.js", 114)
		[TestMethod]
		[Description("ParseJavascriptTrace(@\"jasmine.ExpectationResult@http://localhost:8080/Scripts/ja" +
		    "smine.js:114\", @\"jasmine.ExpectationResult\", @\"/Scripts/jasmine.js\", 114)")]
		public void ParseJavascriptTrace___jasmine_ExpectationResult_http___localhost_8080_Scripts_jasmine_js_114_____jasmine_ExpectationResult______Scripts_jasmine_js___114()
		{
			ParseJavascriptTrace(@"jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:114", @"jasmine.ExpectationResult", @"/Scripts/jasmine.js", 114);
		}
		// DataTestMethod(@"jasmine.Matchers.matcherFn_/<@http://localhost:8080/Scripts/jasmine.js:1240", @"jasmine.Matchers.matcherFn_/<", @"/Scripts/jasmine.js", 1240)
		[TestMethod]
		[Description("ParseJavascriptTrace(@\"jasmine.Matchers.matcherFn_/<@http://localhost:8080/Script" +
		    "s/jasmine.js:1240\", @\"jasmine.Matchers.matcherFn_/<\", @\"/Scripts/jasmine.js\", 12" +
		    "40)")]
		public void ParseJavascriptTrace___jasmine_Matchers_matcherFn____http___localhost_8080_Scripts_jasmine_js_1240_____jasmine_Matchers_matcherFn_________Scripts_jasmine_js___1240()
		{
			ParseJavascriptTrace(@"jasmine.Matchers.matcherFn_/<@http://localhost:8080/Scripts/jasmine.js:1240", @"jasmine.Matchers.matcherFn_/<", @"/Scripts/jasmine.js", 1240);
		}
		// DataTestMethod(@"@http://localhost:8080/Client/testScript.js:9", "", @"/Client/testScript.js", 9)
		[TestMethod]
		[Description("ParseJavascriptTrace(@\"@http://localhost:8080/Client/testScript.js:9\", \"\", @\"/Cli" +
		    "ent/testScript.js\", 9)")]
		public void ParseJavascriptTrace____http___localhost_8080_Client_testScript_js_9__________Client_testScript_js___9()
		{
			ParseJavascriptTrace(@"@http://localhost:8080/Client/testScript.js:9", "", @"/Client/testScript.js", 9);
		}
		// DataTestMethod(@"http://localhost:8080/Client/testScript.js:9")
		[TestMethod]
		[Description("ParseIncorrectJavascriptTrace(@\"http://localhost:8080/Client/testScript.js:9\")")]
		public void ParseIncorrectJavascriptTrace___http___localhost_8080_Client_testScript_js_9_()
		{
			ParseIncorrectJavascriptTrace(@"http://localhost:8080/Client/testScript.js:9");
		}
		// DataTestMethod(@"@http://localhost:8080/Client/testScript.js.9")
		[TestMethod]
		[Description("ParseIncorrectJavascriptTrace(@\"@http://localhost:8080/Client/testScript.js.9\")")]
		public void ParseIncorrectJavascriptTrace____http___localhost_8080_Client_testScript_js_9_()
		{
			ParseIncorrectJavascriptTrace(@"@http://localhost:8080/Client/testScript.js.9");
		}
		// DataTestMethod(@"jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:X")
		[TestMethod]
		[Description("ParseIncorrectJavascriptTrace(@\"jasmine.ExpectationResult@http://localhost:8080/S" +
		    "cripts/jasmine.js:X\")")]
		public void ParseIncorrectJavascriptTrace___jasmine_ExpectationResult_http___localhost_8080_Scripts_jasmine_js_X_()
		{
			ParseIncorrectJavascriptTrace(@"jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:X");
		}
	}
}
