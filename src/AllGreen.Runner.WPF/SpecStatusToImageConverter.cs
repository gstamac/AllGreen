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

namespace AllGreen.Runner.WPF
{
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
            switch ((SpecStatus)value)
            {
                case SpecStatus.Undefined:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/help.png", UriKind.Relative));
                case SpecStatus.Running:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/refresh.png", UriKind.Relative));
                case SpecStatus.Failed:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/delete.png", UriKind.Relative));
                case SpecStatus.Passed:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/check.png", UriKind.Relative));
                case SpecStatus.Skipped:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/pause.png", UriKind.Relative));
                default:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/help.png", UriKind.Relative));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class NotConverter : MarkupExtension, IValueConverter
    {
        public NotConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
