using System;
using System.Globalization;
using System.Windows.Data;

namespace EducationalPlatformDesktop.Converters
{
    public class ProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                int.TryParse(values[0]?.ToString(), out int current) &&
                int.TryParse(values[1]?.ToString(), out int total))
            {
                if (total == 0) return 0;
                return (double)current / total * 100;
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}