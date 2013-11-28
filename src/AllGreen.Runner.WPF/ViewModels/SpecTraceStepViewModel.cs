using System;
using AllGreen.WebServer.Core;
using TemplateAttributes;
using System.Windows;
using System.Text.RegularExpressions;

namespace AllGreen.Runner.WPF.ViewModels
{
    public class SpecTraceStepViewModel
    {
        public string Message { get; private set; }
        public string MethodName { get; private set; }
        public FileLocation ScriptLocation { get; private set; }

        public static SpecTraceStepViewModel Create(string traceLine, IFileLocationMapper fileLocationMapper)
        {
            SpecTraceStepViewModel specTraceStepViewModel = new SpecTraceStepViewModel() { Message = traceLine };

            Regex regex = new Regex(@"^(.*)@(.+):(\d+)$");
            Match match = regex.Match(traceLine);
            if (match.Success)
            {
                specTraceStepViewModel.MethodName = match.Groups[1].Value;
                specTraceStepViewModel.ScriptLocation = fileLocationMapper.Map(match.Groups[2].Value, Int32.Parse(match.Groups[3].Value));
            }

            return specTraceStepViewModel;
        }
    }
}
