using System;
using System.Globalization;
using System.Windows.Data;

namespace MomenMedmSys.WPF.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string format && format.Contains('|'))
            {
                var parts = format.Split('|');
                return (bool)value ? parts[0] : parts[1];
            }
            return (bool)value ? "True" : "False";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
