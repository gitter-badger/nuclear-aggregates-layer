using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public sealed class RussiaFormatterFactory : CommonFormatterFactory, IRussiaAdapted
    {
        private const int RublesCurrencyIsoCode = 643;

        public RussiaFormatterFactory(IBusinessModelSettings settings)
        {
            SetFormat(FormatType.LongDate, "{0:dd MMMM yyyy}");
            SetFormat(FormatType.ShortDate, "{0:dd.MM.yy}");
            SetFormat(FormatType.Money, new RoublesFormatter());
            SetFormat(FormatType.PercentWords, new RussianPercentToWordsFormatter(settings.SignificantDigitsNumber));

            SetMoneyWordsFormatter(RublesCurrencyIsoCode,
                                   new MoneyToWordsFormatter(
                                       new RussianNumberToWordsConverter(true),
                                       new RoublesInRussianPluralizer(),
                                       new KopekInRussianPluralizer()));
        }
    }
}
