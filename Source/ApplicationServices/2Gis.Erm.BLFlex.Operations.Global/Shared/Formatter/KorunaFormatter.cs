using System.Globalization;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class KorunaFormatter : CurrencyFormatter, IFormatter
    {
        public KorunaFormatter() : base("cs-CZ")
        {
        }
    }
}