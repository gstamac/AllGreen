using System;
using System.Linq;
using AllGreen.WebServer.Core;
using TemplateAttributes;
using System.Windows;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF.ViewModels
{
    internal interface ISpecStepViewModel
    {
        string Message { get; set; }
        SpecStatus Status { get; set; }
        FileLocation ScriptLocation { get; set; }
        BindableCollection<SpecTraceStepViewModel> Trace { get; set; }
    }

    [ImplementPropertyChangedCaliburn(typeof(ISpecStepViewModel))]
    public partial class SpecStepViewModel : ISpecStepViewModel
    {
        public override string ToString()
        {
            return String.Format("{0} {1}{2}{3}{2}", Message, Status, Environment.NewLine, String.Join(Environment.NewLine, Trace.Select(t => t.Message)));
        }

        public static SpecStepViewModel Create(SpecStep specStep, IFileLocationMapper fileLocationMapper)
        {
            SpecStepViewModel specStepViewModel = new SpecStepViewModel
                        {
                            Message = specStep.Message,
                            Status = specStep.Status,
                            ScriptLocation = null,
                            Trace = ParseTrace(specStep.Trace, fileLocationMapper)
                        };
            if (specStep.Filename != null)
            {
                specStepViewModel.ScriptLocation = fileLocationMapper.Map(specStep.Filename, specStep.LineNumber);
            }
            return specStepViewModel;
        }

        private static BindableCollection<SpecTraceStepViewModel> ParseTrace(string trace, IFileLocationMapper fileLocationMapper)
        {
            BindableCollection<SpecTraceStepViewModel> traceLines = new BindableCollection<SpecTraceStepViewModel>();

            foreach (string traceLine in trace.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                traceLines.Add(SpecTraceStepViewModel.Create(traceLine, fileLocationMapper));
            }

            return traceLines;
        }
    }
}
