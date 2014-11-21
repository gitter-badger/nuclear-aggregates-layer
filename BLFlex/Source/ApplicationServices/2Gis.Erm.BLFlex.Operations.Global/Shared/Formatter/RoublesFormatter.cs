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
            // ������� � ���, ��� � ������ �������� �������� ������ ������, � ��� ��������� ������ ����������������� ���������.
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