using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using AllGreen.WebServer.Core;
using TemplateAttributes;

namespace AllGreen.Runner.WPF
{
    public class SpecStatusViewModel
    {
        public SpecStatus Status { get; set; }
        public UInt64 Time { get; set; }
        public int Duration { get; set; }
        public RunnerViewModel Runner { get; set; }

        private string _FormattedString;

        public override string ToString()
        {
            if (String.IsNullOrEmpty(_FormattedString))
            {
                string durationString = "";
                if (Duration < 1000)
                    durationString = String.Format("{0} ms", Duration);
                else
                    durationString = String.Format("{0:0.000} s", (float)Duration / 1000);
                _FormattedString = String.Format("{0} in {1}", Status, durationString);
            }
            return _FormattedString;
        }
    }
}
