using System.Globalization;

namespace DoubleGis.Erm.Platform.Common.PrintFormEngine
{
    internal sealed class StringFormatter : IFormatter
    {
        private readonly string _format;

        public StringFormatter(): this("{0}") { }

        public StringFormatter(string format)
        {
            _format = format;
        }

        public string Format(object data)
        {
            return string.Format(CultureInfo.CurrentCulture, _format, data);
        }
    }
}