using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace citybuilder_project.ViewModel
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isTrue = (bool)value;
            string param = parameter as string ?? "Red:Green";
            string[] colors = param.Split(':');

            string colorName = isTrue ? colors[0] : colors[1];
            return new BrushConverter().ConvertFromString(colorName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
