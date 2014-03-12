using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using AllGreen.Core;
using Caliburn.Micro;
using Microsoft.Owin.Hosting;
using TemplateAttributes;
using TinyIoC;
using System.Windows.Media.Imaging;

namespace AllGreen.Runner.WPF.Core.ValueConverters
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
    [ValueConversion(typeof(SpecStatus), typeof(BitmapImage))]
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
                Assembly assembly = Assembly.GetExecutingAssembly();
                switch ((SpecStatus)value)
                {
                    case SpecStatus.Running:
                        return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/icons/refresh.png", UriKind.Absolute));
                    case SpecStatus.Failed:
                        return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/icons/delete.png", UriKind.Absolute));
                    case SpecStatus.Passed:
                        return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/icons/check.png", UriKind.Absolute));
                    case SpecStatus.Skipped:
                        return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/icons/pause.png", UriKind.Absolute));
                    case SpecStatus.Undefined:
                    default:
                        return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/icons/help.png", UriKind.Absolute));
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
