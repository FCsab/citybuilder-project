using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace citybuilder_project.ViewModel
{
    public class NegativeCyclesToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int negativeCycles)
            {
                return negativeCycles > 0 ? Brushes.Red : Brushes.Black;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
