using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AllGreen.Runner.WPF.Core
{
    public class WidthConverter : IValueConverter
    {
        GridView _GridView = null;

        public WidthConverter()
        {
            _GridView = null;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_GridView == null)
            {
                _GridView = (value as ListView).View as GridView;
            }
            double total = _GridView.Columns.Take(_GridView.Columns.Count - 1).Sum(c => c.ActualWidth);
            return ((value as ListView).ActualWidth - total);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
