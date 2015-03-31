using System;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    public class RussianWordPluralizer : IWordPluralizer
    {
        private readonly string _formForOne;
        private readonly string _formForTwoThreeAndFour;
        private readonly string _formForFiveAndMore;

        public RussianWordPluralizer(string formForOne, string formForTwoThreeAndFour, string formForFiveAndMore)
        {
            _formForOne = formForOne;
            _formForTwoThreeAndFour = formForTwoThreeAndFour;
            _formForFiveAndMore = formForFiveAndMore;
        }

        public string GetPluralFor(long numberToRead)
        {
            numberToRead = Math.Abs(numberToRead);

            return numberToRead % 1000 != 0
                       ? Case(numberToRead % 1000, _formForOne, _formForTwoThreeAndFour, _formForFiveAndMore)
                       : _formForFiveAndMore;
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
