using System.Globalization;
using System.Text.RegularExpressions;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class StringFormatter : IFormatter
    {
        private static readonly Regex LineFeedWithoutCarriageReturn = new Regex("(?<!\r)\n", RegexOptions.Compiled | RegexOptions.Multiline);
        private readonly string _format;

        public StringFormatter()
            : this("{0}")
        {
        }

        public StringFormatter(string format)
        {
            _format = format;
        }

        public string Format(object data)
        {
            var result = string.Format(CultureInfo.CurrentCulture, _format, data);
            return LineFeedWithoutCarriageReturn.Replace(result, "\r\n"); // Environment.NewLine тут некорректно использовать, мы хотим получить именно два символа - только их корректно обработает MSWord
        }
    }
}