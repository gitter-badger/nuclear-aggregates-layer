using System;
using System.Globalization;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class KazakhstanLongDateFormatter : IFormatter
    {
        private const string DateFormat = "dd MMMM yyyy";
        private static readonly DateTimeFormatInfo RussianDateTimeFormat = CultureInfo.CreateSpecificCulture("ru-RU").DateTimeFormat;

        public string Format(object data)
        {
            var date = (DateTime)data;
            return date.ToString(DateFormat, RussianDateTimeFormat);
        }
    }
}
