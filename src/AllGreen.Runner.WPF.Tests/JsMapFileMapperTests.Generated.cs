using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.Runner.WPF.Tests
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("AddDataMsTests.tt", "")]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	partial class JsMapFileMapperTests
	{
		// DataTestMethod(@"/file.js", @"C:\Resources\file.js", 1, @"/file.ts", @"C:\Resources\file.ts", 3, 1)
		[TestMethod]
		[Description("Test(@\"/file.js\", @\"C:\\Resources\\file.js\", 1, @\"/file.ts\", @\"C:\\Resources\\file.ts" +
		    "\", 3, 1)")]
		public void Test____file_js_____C__Resources_file_js___1_____file_ts_____C__Resources_file_ts___3__1()
		{
			Test(@"/file.js", @"C:\Resources\file.js", 1, @"/file.ts", @"C:\Resources\file.ts", 3, 1);
		}
		// DataTestMethod("", null, 20)
		[TestMethod]
		[Description("NotFoundTest(\"\", null, 20)")]
		public void NotFoundTest_____null__20()
		{
			NotFoundTest("", null, 20);
		}
		// DataTestMethod(@"/file1.js", @"C:\Resources\file1.js", 20)
		[TestMethod]
		[Description("NotFoundTest(@\"/file1.js\", @\"C:\\Resources\\file1.js\", 20)")]
		public void NotFoundTest____file1_js_____C__Resources_file1_js___20()
		{
			NotFoundTest(@"/file1.js", @"C:\Resources\file1.js", 20);
		}
		// DataTestMethod(@"/file2.js", @"C:\Resources\file2.js", 20)
		[TestMethod]
		[Description("NotFoundTest(@\"/file2.js\", @\"C:\\Resources\\file2.js\", 20)")]
		public void NotFoundTest____file2_js_____C__Resources_file2_js___20()
		{
			NotFoundTest(@"/file2.js", @"C:\Resources\file2.js", 20);
		}
	}
}
