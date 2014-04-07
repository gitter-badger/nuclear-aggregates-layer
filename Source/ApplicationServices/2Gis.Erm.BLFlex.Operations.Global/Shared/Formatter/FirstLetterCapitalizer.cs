using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class FirstLetterCapitalizer : IFormatter
    {
        private readonly IFormatter _formatter;

        public FirstLetterCapitalizer(IFormatter formatter)
        {
            _formatter = formatter;
        }

        public string Format(object data)
        {
            var formatted = _formatter.Format(data);
            if (string.IsNullOrEmpty(formatted))
            {
                return formatted;
            }

            var builder = new StringBuilder(formatted.Length);
            builder.Append(char.ToUpper(formatted[0]));
            builder.Append(formatted.Skip(1).ToArray());
            return builder.ToString();
        }
    }
}