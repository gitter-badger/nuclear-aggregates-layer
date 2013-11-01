using System;
using System.Globalization;
using System.Windows.Data;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Platform.UI.WPF.Shell.Presentation.Shell.AvalonDockUtils
{
    /// <summary>
    /// Workaround, чтобы избегать ошибок binding при смене activecontent avalodock dockingmanager (
    /// визуально отображаемых как красная рамка вокруг панели avalondock dockingmanager)
    /// Причина появления красной рамки - при twoway binding на activecontent, если активированная панель не IDocument (tab в documentpane), 
    /// а, например, панель навигации, или панель сообщений - перед тем как вызвать setter у DocumentManager ожидающий IDocument срабатывает defaultvalueconverter,
    /// который проверив целевой тип IDocument, и имеющееся значение - например палель навигации бросает exception о невозможности приведения типов.
    /// Решение пока использовать связку из спец. valueconverter и спец. маркерного документа
    /// </summary>
    public sealed class AvalonDockActiveContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is IDocument)
            {
                return value;
            }

            return new NotDocumentMarker();
        }
    }
}
