using System;
using System.Globalization;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    // В Эмиратах мы используем формат даты "MMMM dd, yyyy", причём запись месяца требуется не арабской вязью, а на английском языке.
    internal sealed class EmiratesDateFormatter : IFormatter
    {
        private const string DateFormat = "MMMM dd, yyyy";
        private static readonly DateTimeFormatInfo EnglishDateTimeFormat = CultureInfo.CreateSpecificCulture("en-US").DateTimeFormat;

        public string Format(object data)
        {
            var date = (DateTime)data;
            return date.ToString(DateFormat, EnglishDateTimeFormat);
        }
    }
}
