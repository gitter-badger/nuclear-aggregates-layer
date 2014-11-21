using System;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    public class RussianWordPluralizer : IWordPluralizer
    {
        private readonly string _currencyNameForOne;
        private readonly string _currencyNameForTwoThreeAndFour;
        private readonly string _currencyNameForFiveAndMore;

        public RussianWordPluralizer(string currencyNameForOne, string currencyNameForTwoThreeAndFour, string currencyNameForFiveAndMore)
        {
            _currencyNameForOne = currencyNameForOne;
            _currencyNameForTwoThreeAndFour = currencyNameForTwoThreeAndFour;
            _currencyNameForFiveAndMore = currencyNameForFiveAndMore;
        }

        public string GetPluralFor(long numberToRead)
        {
            numberToRead = Math.Abs(numberToRead);

            return numberToRead % 1000 != 0
                       ? Case(numberToRead % 1000, _currencyNameForOne, _currencyNameForTwoThreeAndFour, _currencyNameForFiveAndMore)
                       : _currencyNameForFiveAndMore;
        }

        private static string Case(long val, string one, string two, string five)
        {
            var t = (val % 100 > 20) ? val % 10 : val % 20;

            switch (t)
            {
                case 1:
                    return one;
                case 2:
                case 3:
                case 4:
                    return two;
                default:
                    return five;
            }
        }
    }
}
