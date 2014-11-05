using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal sealed class TengeFormatter : CurrencyFormatter, IFormatter
    {
        public TengeFormatter()
            : base("kk-KZ")
        {
        }
    }
}