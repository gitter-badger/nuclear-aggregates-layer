using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class EuroFormatter : CurrencyFormatter, IFormatter
    {
        public EuroFormatter() : base("el-GR")
        {
        }
    }
}