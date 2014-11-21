using System;
using System.Globalization;
using System.Windows.Data;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Converters
{
    public sealed class NullToBooleanConverter : IValueConverter
    {
        public NullToBooleanConverter()
        {
            ValueForNull = false;
            ValueForNotNull = true;
        }

        public bool ValueForNull { get; set; }
        public bool ValueForNotNull { get; set; }

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
