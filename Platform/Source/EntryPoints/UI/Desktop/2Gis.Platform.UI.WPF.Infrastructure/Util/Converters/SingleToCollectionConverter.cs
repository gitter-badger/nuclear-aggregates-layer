using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Converters
{
    public sealed class SingleToCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is IEnumerable)
            {
                return value;
            }

            return new[] { value };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
