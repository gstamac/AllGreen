using System;
using System.Collections.Generic;
using System.Windows;
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
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ObjectToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public ObjectToVisibilityConverter()
        {

        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ("Reverse".Equals(parameter))
                return value != null ? Visibility.Collapsed : Visibility.Visible;
            else
                return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
