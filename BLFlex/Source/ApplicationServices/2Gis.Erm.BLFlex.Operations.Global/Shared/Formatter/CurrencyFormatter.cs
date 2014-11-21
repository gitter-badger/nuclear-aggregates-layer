using System.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal abstract class CurrencyFormatter
    {
        private readonly string _format;
        private readonly NumberFormatInfo _numberFormatInfo;

        protected CurrencyFormatter(NumberFormatInfo numberFormatInfo)
            : this("{0:C}", numberFormatInfo)

        {
        }

        protected CurrencyFormatter(string culture)
            : this("{0:C}", new CultureInfo(culture).NumberFormat)
        {
        }

        private CurrencyFormatter(string format, NumberFormatInfo numberFormatInfo)
        {
            _format = format;
            _numberFormatInfo = numberFormatInfo;
        }

        public string Format(object data)
        {
            return string.Format(_numberFormatInfo, _format, data);
        }
    }
}
