using System.Globalization;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class TengeFormatter : CurrencyFormatter, IFormatter
    {
        public TengeFormatter()
            : base(CustomizeCurrency())
        {
        }

        private static NumberFormatInfo CustomizeCurrency()
        {
            var baseCulture = CultureInfo.GetCultureInfo("kk-KZ");
            var modifiedFormat = (NumberFormatInfo)baseCulture.NumberFormat.Clone();
            modifiedFormat.CurrencyPositivePattern = 3;
            modifiedFormat.CurrencyDecimalSeparator = ",";
            modifiedFormat.CurrencySymbol = "עד.";
            
            return modifiedFormat;
        }
    }
}