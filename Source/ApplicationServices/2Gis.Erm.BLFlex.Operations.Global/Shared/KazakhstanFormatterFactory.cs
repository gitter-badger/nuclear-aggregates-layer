using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public sealed class KazakhstanFormatterFactory : CommonFormatterFactory, IKazakhstanAdapted
    {
        private const int TengeCurrencyIsoCode = 398;

        public KazakhstanFormatterFactory()
        {
            SetFormat(FormatType.LongDate, new KazakhstanLongDateFormatter());
            SetFormat(FormatType.ShortDate, "{0:dd.MM.yy}");
            SetFormat(FormatType.Money, new TengeFormatter());

            SetMoneyWordsFormatter(TengeCurrencyIsoCode,
                                   new MoneyToWordsFormatter(
                                       new RussianNumberToWordsConverter(true),
                                       new TengeInRussianPluralizer(),
                                       new TiynInRussianPluralizer()));
        }
    }
}