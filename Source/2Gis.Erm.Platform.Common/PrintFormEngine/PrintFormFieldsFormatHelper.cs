using System;

namespace DoubleGis.Erm.Platform.Common.PrintFormEngine
{
    public static class PrintFormFieldsFormatHelper
    {
        public const string LongDateFormat = "{0:dd MMMM yyyy}";
        public const string ShortDateFormat = "{0:dd.MM.yy}";

        public static string FormatLongDate(DateTime date)
        {
            return string.Format(LongDateFormat, date);
        }

        public static string FormatShortDate(DateTime date)
        {
            return string.Format(ShortDateFormat, date);
        }

        public static string FormatShortDate(DateTime? date)
        {
            return date.HasValue ? string.Format(ShortDateFormat, date) : string.Empty;
        }
    }
}
