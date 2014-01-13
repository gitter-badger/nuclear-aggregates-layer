using System;
using System.Globalization;
using System.Windows.Data;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Basic
{
    public class EnumFieldToDescriptionConverter : IMultiValueConverter
    {
        private static readonly EnumFieldToDescriptionConverter ConverterInstance = new EnumFieldToDescriptionConverter();

        public static EnumFieldToDescriptionConverter Instance
        {
            get { return ConverterInstance; }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                throw new ArgumentException();
            }

            var enumValue = values[0];

            if (enumValue == null)
            {
                return null;
            }

            if (enumValue is string)
            {
                return enumValue;
            }

            return ConvertToString((Enum)enumValue, (CultureInfo)values[1]);
        }

        private static string ConvertToString(Enum enumValue, CultureInfo targetCulture)
        {
            var type = enumValue.GetType();
            var name = type.GetEnumName(enumValue);

            var localizedName = EnumResources.ResourceManager.GetString(type.Name + name, targetCulture);
            return localizedName ?? name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Can't convert back");
        }
    }
}