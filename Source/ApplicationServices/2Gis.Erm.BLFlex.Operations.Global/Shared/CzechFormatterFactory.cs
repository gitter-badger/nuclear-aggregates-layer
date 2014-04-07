using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public class CzechFormatterFactory : CommonFormatterFactory, ICzechAdapted
    {
        public CzechFormatterFactory()
        {
            SetFormat(FormatType.LongDate, "{0:d'. 'MMMM' 'yyyy}");
            SetFormat(FormatType.ShortDate, "{0:d'. 'M'. 'yyyy}");
            SetFormat(FormatType.Money, new KorunaFormatter());
        }
    }
}
