using System;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    internal class MoneyToWordsFormatter : IFormatter
    {
        private readonly INumberToWordsConverter _numberToWordsConverter;
        private readonly IWordPluralizer _baseWordPluralizer;
        private readonly IWordPluralizer _tokenMoneyReader;

        public MoneyToWordsFormatter(INumberToWordsConverter numberToWordsConverter, IWordPluralizer baseWordPluralizer, IWordPluralizer tokenMoneyReader)
        {
            _baseWordPluralizer = baseWordPluralizer;
            _tokenMoneyReader = tokenMoneyReader;
            _numberToWordsConverter = numberToWordsConverter;
        }

        public string Format(object data)
        {
            var val = Convert.ToDecimal(data);

            var roundedVal = Math.Round(val, 2, MidpointRounding.ToEven);

            var n = (int)roundedVal;
            var remainder = (int)(roundedVal * 100) - n * 100;

            return string.Format("{0} {1} {2} {3}",
                                 _numberToWordsConverter.Convert(n),
                                 _baseWordPluralizer.GetPluralFor(n),
                                 remainder.ToString("00"),
                                 _tokenMoneyReader.GetPluralFor(remainder));
		}
    }
}
