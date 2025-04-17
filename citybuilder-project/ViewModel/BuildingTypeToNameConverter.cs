using System;
using System.Globalization;
using System.Windows.Data;
using citybuilder_project.Model;

namespace citybuilder_project.ViewModel
{
    public class BuildingTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BuildingType buildingType)
            {
                return buildingType.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
