using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace appLauncher.Core.Converters
{
    /// <summary>
    /// Converts a boolean value to a Visibility enumeration.
    /// If IsInverse is true, it inverts the boolean before conversion.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsInverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                if (IsInverse)
                {
                    boolValue = !boolValue;
                }
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
