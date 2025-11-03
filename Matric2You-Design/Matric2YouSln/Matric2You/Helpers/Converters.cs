using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Matric2You.Helpers
{
    // True when value is not null
    public class NotNullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // True when value is null
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // Maps percentage (0..100) to ProgressBar-friendly0..1 range
    public class PercentTo0to1Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return 0d;
            if (value is double d) return Math.Clamp(d / 100d, 0d, 1d);
            if (double.TryParse(System.Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var p))
                return Math.Clamp(p / 100d, 0d, 1d);
            return 0d;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d) return Math.Clamp(d * 100d, 0d, 100d);
            return 0d;
        }
    }
}
