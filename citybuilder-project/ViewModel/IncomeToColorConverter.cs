using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace citybuilder_project.ViewModel
{
    public class IncomeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int income)
            {
                return income >= 0 ? Brushes.Green : Brushes.Red;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
