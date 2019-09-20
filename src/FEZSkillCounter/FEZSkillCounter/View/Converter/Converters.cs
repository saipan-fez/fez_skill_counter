using System;
using System.Globalization;
using System.Windows.Data;

namespace FEZSkillCounter.View.Converter
{
    public class BoolToOnOffStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as bool? ?? false) ? "ON" : "OFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as string) == "ON" ? true : false;
        }
    }
}
