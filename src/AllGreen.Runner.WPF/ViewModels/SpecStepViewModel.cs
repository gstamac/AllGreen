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
        FileLocation ErrorLocation { get; set; }
        FileLocation MappedLocation { get; set; }
        SpecStatus Status { get; set; }
        BindableCollection<SpecTraceStepViewModel> Trace { get; set; }
    }

    [ImplementPropertyChangedCaliburn(typeof(ISpecStepViewModel))]
    public partial class SpecStepViewModel : ISpecStepViewModel
    {
        public override string ToString()
        {
            return String.Format("{0} {1}{2}{3}{2}", Message, Status, Environment.NewLine, String.Join(Environment.NewLine, Trace.Select(t => t.Message)));
        }

        public static SpecStepViewModel Create(SpecStep specStep, IFileLocationParser fileLocationParser, IFileLocationMapper fileLocationMapper)
        {
            SpecStepViewModel specStepViewModel = new SpecStepViewModel
                        {
                            Message = specStep.Message,
                            ErrorLocation = fileLocationParser.Parse(specStep.ErrorLocation),
                            Status = specStep.Status,
                            Trace = ParseTrace(specStep.Trace, fileLocationParser, fileLocationMapper)
                        };
            if (specStepViewModel.ErrorLocation != null)
                specStepViewModel.MappedLocation = fileLocationMapper.Map(specStepViewModel.ErrorLocation);
            return specStepViewModel;
        }

        private static BindableCollection<SpecTraceStepViewModel> ParseTrace(string trace, IFileLocationParser fileLocationParser, IFileLocationMapper fileLocationMapper)
        {
            BindableCollection<SpecTraceStepViewModel> traceLines = new BindableCollection<SpecTraceStepViewModel>();

            if (!String.IsNullOrEmpty(trace))
            {
                foreach (string traceLine in trace.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    traceLines.Add(SpecTraceStepViewModel.Create(traceLine, fileLocationParser, fileLocationMapper));
                }
            }

            return traceLines;
        }
    }
}
