using System;
using System.Windows.Data;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class InverseBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new InvalidOperationException("The value to convert  must be a boolean");
            }

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
