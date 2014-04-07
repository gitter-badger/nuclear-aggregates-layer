using System.Globalization;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class PesoFormatter : CurrencyFormatter, IFormatter
    {
        public PesoFormatter() : base(CreateChileanPesoFormat())
        {
        }

        private static NumberFormatInfo CreateChileanPesoFormat()
        {
            // согласно https://jira.2gis.ru/browse/ERM-3545 в „или используем неродной формат денег
            var numberFormatInfo = (NumberFormatInfo)new CultureInfo("es-CL").NumberFormat.Clone();
            numberFormatInfo.CurrencySymbol = "pesos";
            numberFormatInfo.CurrencyPositivePattern = 3;

            return numberFormatInfo;
        }
    }
}