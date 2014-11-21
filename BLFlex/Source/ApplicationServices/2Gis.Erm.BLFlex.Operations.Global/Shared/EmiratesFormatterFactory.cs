using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public class EmiratesFormatterFactory : CommonFormatterFactory, IEmiratesAdapted
    {
        public const int DirhamCurrencyIsoCode = 784;

        public EmiratesFormatterFactory()
        {
            SetFormat(FormatType.LongDate, new EmiratesDateFormatter());
            SetFormat(FormatType.ShortDate, new EmiratesDateFormatter()); // В ТЗ пока нет различия форматов - всюду одинаково
            SetFormat(FormatType.Money, new DirhamFormatter());

            SetMoneyWordsFormatter(DirhamCurrencyIsoCode,
                       new MoneyToWordsFormatter(
                           new EnglishNumberToWordsConverter(),
                           new DirhamInEnglishPluralizer(),
                           new FilsPluralizer()));

        }
    }
}
