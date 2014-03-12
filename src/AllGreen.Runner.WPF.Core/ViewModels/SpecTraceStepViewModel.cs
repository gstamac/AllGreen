using System;
using AllGreen.Core;
using TemplateAttributes;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AllGreen.Runner.WPF.Core.ViewModels
{
    public class SpecTraceStepViewModel
    {
        public string Message { get; private set; }
        public string MethodName { get; private set; }
        public FileLocation ScriptLocation { get; private set; }
        public FileLocation MappedLocation { get; private set; }

        public static SpecTraceStepViewModel Create(string traceLine, IFileLocationParser fileLocationParser, IFileLocationMapper fileLocationMapper)
        {
            string methodName = null;
            string scriptLocation = null;

            if (TryParseFirefoxSafari(traceLine, ref methodName, ref scriptLocation) || TryParseIEChrome(traceLine, ref methodName, ref scriptLocation))
            {
                SpecTraceStepViewModel specTraceStepViewModel = new SpecTraceStepViewModel();
                specTraceStepViewModel.Message = traceLine;
                specTraceStepViewModel.MethodName = String.IsNullOrEmpty(methodName) ? "<anonymous>" : methodName;
                specTraceStepViewModel.ScriptLocation = fileLocationParser.Parse(scriptLocation);
                specTraceStepViewModel.MappedLocation = fileLocationMapper.Map(specTraceStepViewModel.ScriptLocation);
                return specTraceStepViewModel;
            }

            return new SpecTraceStepViewModel() { Message = traceLine };
        }

        private static bool TryParseFirefoxSafari(string traceLine, ref string methodName, ref string scriptLocation)
        {
            Match match = Regex.Match(traceLine, @"^(.*)@(.*)$");
            if (match.Success)
            {
                methodName = match.Groups[1].Value;
                scriptLocation = match.Groups[2].Value;
                return true;
            }
            return false;
        }

        private static bool TryParseIEChrome(string traceLine, ref string methodName, ref string scriptLocation)
        {
            Match match = Regex.Match(traceLine, @"^ *at (.*) \((.*)\)$");
            if (match.Success)
            {
                methodName = match.Groups[1].Value;
                scriptLocation = match.Groups[2].Value;
                return true;
            }
            return false;
        }

    }
}
