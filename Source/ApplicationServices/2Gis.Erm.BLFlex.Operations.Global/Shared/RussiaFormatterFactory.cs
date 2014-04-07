using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public sealed class RussiaFormatterFactory : CommonFormatterFactory, IRussiaAdapted
    {
        public RussiaFormatterFactory()
        {
            SetFormat(FormatType.LongDate, "{0:dd MMMM yyyy}");
            SetFormat(FormatType.ShortDate, "{0:dd.MM.yy}");
            SetFormat(FormatType.Money, new RoublesFormatter());
        }
    }
}
