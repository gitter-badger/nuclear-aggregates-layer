using System;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public abstract class CommonFormatterFactory : FormatterFactoryBase
    {
        private const int RublesCurrencyIsoCode = 643;

        protected CommonFormatterFactory()
        {
            SetFormat(FormatType.Percents, "{0:N2}%");
            SetFormat(FormatType.Number, "{0:N2}");
            SetFormat(FormatType.NumberN0, "{0:N0}");

            SetTypeFormat(typeof(decimal), FormatType.Money);
            SetTypeFormat(typeof(DateTime), FormatType.LongDate);

            SetMoneyWordsFormatter(RublesCurrencyIsoCode, new RublesInWordsFormatter());
        }
    }
}
