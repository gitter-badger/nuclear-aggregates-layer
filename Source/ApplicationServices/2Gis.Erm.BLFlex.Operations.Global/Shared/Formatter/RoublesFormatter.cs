using System.Globalization;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class RoublesFormatter : CurrencyFormatter, IFormatter
    {
        public RoublesFormatter()
            : base(CustomiseNumberFormatIfNeeded())
        {
        }

        private static NumberFormatInfo CustomiseNumberFormatIfNeeded()
        {
            // связано с тем, что в разных системах прописан разный формат, а нам требуется строго детерминированное поведение.
            var numberFormat = new CultureInfo("ru-RU").NumberFormat;
            if (numberFormat.CurrencyPositivePattern == 1)
            {
                return numberFormat;
            }

            numberFormat = (NumberFormatInfo)numberFormat.Clone();
            numberFormat.CurrencyPositivePattern = 1;

            return numberFormat;
        }
    }
}