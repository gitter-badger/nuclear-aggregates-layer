using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Converters
{
    public sealed class TypeToVisibilityConverter : IValueConverter 
    {
        public Type VisibleType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return VisibleType.IsInstanceOfType(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}