using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using TemplateAttributes;

namespace AllGreen.Runner.WPF
{
    public interface IRunnerViewModel
    {
        string ConnectionId { get; set; }
        string Name { get; set; }
        string UserAgent { get; set; }
        string Status { get; set; }
        Brush Background { get; set; }
    }

    [ImplementPropertyChangedCaliburn(typeof(IRunnerViewModel))]
    public partial class RunnerViewModel : IRunnerViewModel
    {
        private static Random _Random = new Random();

        public RunnerViewModel()
        {
            _Background = new SolidColorBrush(Color.FromArgb(50, (byte)_Random.Next(0, 255), (byte)_Random.Next(0, 255), (byte)_Random.Next(0, 255)));
            _Background.Freeze();
        }

        protected void OnConnectionIdChanged(string oldConnectionId, string newConnectionId)
        {
            if (String.IsNullOrEmpty(Name))
                Name = newConnectionId;
        }

        protected void OnUserAgentChanged(string oldUserAgent, string newUserAgent)
        {
            if (String.IsNullOrEmpty(newUserAgent)) return;

            int i = newUserAgent.IndexOf('(');
            if (i >= 0)
                Name = newUserAgent.Substring(0, i).Trim();
            else
                Name = newUserAgent;
        }
    }
}
