using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class HryvniaFormatter : CurrencyFormatter, IFormatter
    {
        public HryvniaFormatter()
            : base("uk-UA")
        {
        }
    }
}