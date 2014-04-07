using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public sealed class CyprusFormatterFactory : FormatterFactoryBase, ICyprusAdapted
    {
        public CyprusFormatterFactory()
        {
            SetFormat(FormatType.LongDate, "{0:dd MMMM yyyy}");
            SetFormat(FormatType.ShortDate, "{0:dd.MM.yy}");
            SetFormat(FormatType.Money, new EuroFormatter());
        }
    }
}
