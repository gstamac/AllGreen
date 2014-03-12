using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AllGreen.Core
{
    public class JsMapFile
    {
        public int Version { get; set; }
        public string OutputFile { get; set; }
        public string[] SourceFiles { get; set; }
        public string[] Names { get; set; }
        public IEnumerable<JsMappingSegment> Mappings { get; set; }

        public static JsMapFile CreateFromString(string content)
        {
            try
            {
                JObject jObject = JObject.Parse(content);
                JsMapFile mapFile = new JsMapFile
                {
                    Version = (int)jObject.SelectToken("version"),
                    OutputFile = (string)jObject.SelectToken("file"),
                    SourceFiles = ((JArray)jObject.GetValue("sources")).Select(jt => (string)jt).ToArray(),
                    Names = ((JArray)jObject.GetValue("names")).Select(jt => (string)jt).ToArray()
                };
                mapFile.Mappings = ParseMappings(mapFile, (string)jObject.SelectToken("mappings"));

                return mapFile;
            }
            catch
            {
            }
            return null;
        }

        private static IEnumerable<JsMappingSegment> ParseMappings(JsMapFile jsMapFile, string mappings)
        {
            int currentLine = 1;
            int previousSourceIndex = 0;
            int previousSourceStartingLine = 1;
            int previousSourceStartingColumn = 1;
            int previousSourceNameIndex = 0;
            foreach (string mappingLine in mappings.Split(';'))
            {
                if (!String.IsNullOrEmpty(mappingLine))
                {
                    int previousGeneratedColumn = 1;
                    foreach (string segment in mappingLine.Split(','))
                    {
                        int[] map = VLQDecode(segment);
                        JsMappingSegment mapping = new JsMappingSegment
                        {
                            GeneratedLine = currentLine,
                            GeneratedColumn = previousGeneratedColumn + map[0]
                        };
                        if (map.Length > 1)
                        {
                            previousSourceIndex += map[1];
                            mapping.Source = jsMapFile.SourceFiles[previousSourceIndex];
                            previousSourceStartingLine += map[2];
                            mapping.SourceStartingLine = previousSourceStartingLine;
                            previousSourceStartingColumn += map[3];
                            mapping.SourceStartingColumn = previousSourceStartingColumn;
                            if (map.Length > 4)
                            {
                                previousSourceNameIndex += map[4];
                                mapping.SourceName = jsMapFile.Names[previousSourceNameIndex];
                            }
                        }
                        yield return mapping;
                        previousGeneratedColumn = mapping.GeneratedColumn;
                    }
                }
                currentLine++;
            }
        }

        private static int[] VLQDecode(string base64String)
        {
            List<int> res = new List<int>();
            int value = 0;
            int shift = 0;
            foreach (char vlqChar in base64String)
            {
                byte digit = FromBase64Char(vlqChar);
                bool cont = (digit & 0x20) == 0x20;
                digit &= 0x1F;
                value = value + (digit << shift);
                shift += 5;
                if (!cont)
                {
                    value = ((value & 1) == 1) ? -(value >> 1) : (value >> 1);
                    res.Add(value);
                    shift = 0;
                    value = 0;
                }
            }
            if (shift > 0)
                throw new FormatException("Expected more digits in base 64 VLQ value.");
            if (res.Count != 1 && res.Count != 4 && res.Count != 5)
                throw new FormatException("Expected 1, 4 or 5 values in base 64 VLQ.");
            return res.ToArray();
        }

        private static byte FromBase64Char(char c)
        {
            return (byte)"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".IndexOf(c);
        }
    }

    public class JsMappingSegment
    {
        public int GeneratedLine { get; set; }
        public int GeneratedColumn { get; set; }
        public string Source { get; set; }
        public int SourceStartingLine { get; set; }
        public int SourceStartingColumn { get; set; }
        public string SourceName { get; set; }

        public override string ToString()
        {
            return String.Format("{5} in {2}:{3}:{4} => {0}:{1}", GeneratedLine, GeneratedColumn, Source, SourceStartingLine, SourceStartingColumn, SourceName).Trim();
        }
    }
}
