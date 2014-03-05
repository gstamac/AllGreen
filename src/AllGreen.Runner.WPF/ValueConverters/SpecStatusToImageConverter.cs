using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using Microsoft.Owin.Hosting;
using TemplateAttributes;
using TinyIoC;

namespace AllGreen.Runner.WPF.ValueConverters
{
    [ValueConversion(typeof(SpecStatus), typeof(System.Windows.Media.Imaging.BitmapImage))]
    public class SpecStatusToImageConverter : MarkupExtension, IValueConverter
    {
        public SpecStatusToImageConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SpecStatus)
            {
                switch ((SpecStatus)value)
                {
                    case SpecStatus.Running:
                        return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/refresh.png", UriKind.Relative));
                    case SpecStatus.Failed:
                        return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/delete.png", UriKind.Relative));
                    case SpecStatus.Passed:
                        return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/check.png", UriKind.Relative));
                    case SpecStatus.Skipped:
                        return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/pause.png", UriKind.Relative));
                    case SpecStatus.Undefined:
                    default:
                        return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/help.png", UriKind.Relative));
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
