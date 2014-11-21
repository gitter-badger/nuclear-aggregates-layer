using System;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public abstract class CommonFormatterFactory : FormatterFactoryBase
    {
        protected CommonFormatterFactory()
        {
            SetFormat(FormatType.Percents, "{0:N2}%");
            SetFormat(FormatType.Number, "{0:N2}");
            SetFormat(FormatType.NumberN0, "{0:N0}");

            SetTypeFormat(typeof(decimal), FormatType.Money);
            SetTypeFormat(typeof(DateTime), FormatType.LongDate);
        }
    }
}
