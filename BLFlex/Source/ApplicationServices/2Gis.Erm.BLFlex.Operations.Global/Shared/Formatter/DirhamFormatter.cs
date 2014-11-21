using System.Globalization;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class DirhamFormatter : CurrencyFormatter, IFormatter
    {
        public DirhamFormatter()
            : base(CreateEmiratesDirhamFormat())
        {
        }

        private static NumberFormatInfo CreateEmiratesDirhamFormat()
        {
            // В Эмиратах используем неродной формат денег (родой пишется арабской вязью, причем символ валюты стоит перед числом)
            var numberFormatInfo = (NumberFormatInfo)new CultureInfo("ar-AE").NumberFormat.Clone();
            numberFormatInfo.CurrencySymbol = "AED";
            numberFormatInfo.CurrencyPositivePattern = 3;

            return numberFormatInfo;
        }
    }
}