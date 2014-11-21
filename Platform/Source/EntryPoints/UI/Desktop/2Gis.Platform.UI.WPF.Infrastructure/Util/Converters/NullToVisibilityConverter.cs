using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Converters
{
    public sealed class NullToVisibilityConverter : IValueConverter
    {
        public NullToVisibilityConverter()
        {
            ValueForNull = Visibility.Collapsed;
            ValueForNotNull = Visibility.Visible;
        }

        public Visibility ValueForNull { get; set; }
        public Visibility ValueForNotNull { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? ValueForNull : ValueForNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
