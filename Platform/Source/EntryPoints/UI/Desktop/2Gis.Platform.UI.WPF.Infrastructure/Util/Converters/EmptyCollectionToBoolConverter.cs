using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Converters
{
    public sealed class EmptyCollectionToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }

            var collection = value as IEnumerable;
            if (collection == null)
            {
                throw new InvalidOperationException("Specified instance of type " + value.GetType() + " is not collection");
            }

            foreach (var item in collection)
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
