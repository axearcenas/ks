using System;
using System.Globalization;
using Xamarin.Forms;

namespace KeepSafe
{
    public class IntEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Enum)
            {
                return (int)value;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return System.Enum.ToObject(targetType, value);
            }
            return 0;
        }
    }
}
