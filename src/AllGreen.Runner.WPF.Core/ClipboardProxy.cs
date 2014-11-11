using System.Collections.Generic;
using System;
using System.Windows;
using AllGreen.Core;

namespace AllGreen.Runner.WPF.Core
{
    public class ClipboardProxy : IClipboard
    {
        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
