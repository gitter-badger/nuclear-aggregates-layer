using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public class UkraineFormatterFactory : CommonFormatterFactory, IUkraineAdapted
    {
        private const int HryvniaCurrencyIsoCode = 980;

        public UkraineFormatterFactory()
        {
            // FIXME {all, 07.04.2014}: адаптировать дл€ ”краины
            SetFormat(FormatType.LongDate, "{0:dd MMMM yyyy}");
            SetFormat(FormatType.ShortDate, "{0:dd.MM.yy}");
            SetFormat(FormatType.Money, new HryvniaFormatter());

            SetMoneyWordsFormatter(HryvniaCurrencyIsoCode,
                                   new MoneyToWordsFormatter(
                                       new RussianNumberToWordsConverter(false),
                                       new HryvniaInRussianPluralizer(),
                                       new KopekInRussianPluralizer()));
        }
    }
}