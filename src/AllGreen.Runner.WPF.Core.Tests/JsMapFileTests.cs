using System;
using System.Linq;
using AllGreen.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;
using FluentAssertions;
using AllGreen.Runner.WPF.Core.ViewModels;

namespace AllGreen.Runner.WPF.Core.Tests
{
    [TestClass]
    public class JsMapFileTests
    {
        [TestMethod]
        public void CreateFromStringTest()
        {
            JsMapFile jsMapFile = JsMapFile.CreateFromString("{\"version\":3,\"file\":\"testScript.js\",\"sourceRoot\":\"\",\"sources\":[\"testScript.ts\"],\"names\":[\"name1\",\"name2\"],"
                + "\"mappings\":\"AAEAA,gEAFgE;AAE5D,IAAA,CAAC,GAAG,EAAE,CAAC;AAAC,CAAC,IAAI,EAAE,CAAC;;\"}");

            jsMapFile.Version.Should().Be(3);
            jsMapFile.OutputFile.Should().Be("testScript.js");
            jsMapFile.SourceFiles.ShouldAllBeEquivalentTo(new string[] { "testScript.ts" });
            jsMapFile.Names.ShouldAllBeEquivalentTo(new string[] { "name1", "name2" });
            jsMapFile.Mappings.Select(m => m.ToString()).ShouldAllBeEquivalentTo(new string[]{
                "name1 in testScript.ts:3:1 => 1:1",
                "in testScript.ts:1:65 => 1:65",
                "in testScript.ts:3:5 => 2:1",
                "in testScript.ts:3:5 => 2:5",
                "in testScript.ts:3:6 => 2:6",
                "in testScript.ts:3:9 => 2:9",
                "in testScript.ts:3:11 => 2:11",
                "in testScript.ts:3:12 => 2:12",
                "in testScript.ts:3:13 => 3:1",
                "in testScript.ts:3:14 => 3:2",
                "in testScript.ts:3:18 => 3:6",
                "in testScript.ts:3:20 => 3:8",
                "in testScript.ts:3:21 => 3:9",
            });
            /*
            ([2,0](#0)=>[0,0]) | ([0,64](#0)=>[0,64])
            ([2,4](#0)=>[1,0]) | ([2,4](#0)=>[1,4]) | ([2,5](#0)=>[1,5]) | ([2,8](#0)=>[1,8]) | ([2,10](#0)=>[1,10]) | ([2,11](#0)=>[1,11])
            ([2,12](#0)=>[2,0]) | ([2,13](#0)=>[2,1]) | ([2,17](#0)=>[2,5]) | ([2,19](#0)=>[2,7]) | ([2,20](#0)=>[2,8])
             */
        }

        [TestMethod]
        public void MappingBase64StringErrorTest()
        {
            Action action = () => JsMapFile.CreateFromString("{\"version\":3,\"file\":\"\",\"sourceRoot\":\"\",\"sources\":[],\"names\":[],\"mappings\":\"AAEA,gEAFg\"}").Mappings.ToArray();

            action.ShouldThrow<IndexOutOfRangeException>();

            action = () => JsMapFile.CreateFromString("incorrect map file format").Mappings.ToArray();

            action.ShouldThrow<Exception>();

            action = () => JsMapFile.CreateFromString("{\"version\":3,\"file\":\"\",\"sourceRoot\":\"\",\"sources\":[\"source\"],\"names\":[],\"mappings\":\"AAEA,gEAFg\"}").Mappings.ToArray();

            action.ShouldThrow<FormatException>("Expected more digits in base 64 VLQ value.");

            action = () => JsMapFile.CreateFromString("{\"version\":3,\"file\":\"\",\"sourceRoot\":\"\",\"sources\":[\"source\"],\"names\":[],\"mappings\":\"AAEA,gEAF\"}").Mappings.ToArray();

            action.ShouldThrow<FormatException>("Expected 1, 4 or 5 values in base 64 VLQ.");
        }
    }
}
