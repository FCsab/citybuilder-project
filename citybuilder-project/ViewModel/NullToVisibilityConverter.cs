using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace citybuilder_project.ViewModel
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter != null && bool.TryParse(parameter.ToString(), out bool result) && result;
            return value == null ? (invert ? Visibility.Visible : Visibility.Collapsed) : (invert ? Visibility.Collapsed : Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
