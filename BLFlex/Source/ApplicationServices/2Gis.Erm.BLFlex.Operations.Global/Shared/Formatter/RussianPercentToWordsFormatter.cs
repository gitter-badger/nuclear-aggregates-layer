using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    public sealed class RussianPercentToWordsFormatter : IFormatter
    {
        private static readonly IWordPluralizer Integer = new RussianWordPluralizer("целая", "целых", "целых");
        private static readonly IWordPluralizer Percent = new RussianWordPluralizer("процент", "процента", "процентов");

        private static readonly IDictionary<int, IWordPluralizer> Fractional
            = new Dictionary<int, IWordPluralizer>
                  {
                      { 10, new RussianWordPluralizer("десятая", "десятых", "десятых") },
                      { 100, new RussianWordPluralizer("сотая", "сотых", "сотых") },
                      { 1000, new RussianWordPluralizer("тысячная", "тысячных", "тысячных") },
                      { 10000, new RussianWordPluralizer("десятитысячная", "десятитысячных", "десятитысячных") },
                  };

        public string Format(object data)
        {
            var value = Convert.ToDecimal(data);

            var integerPart = (int)Math.Floor(value);
            var fractionalPart = value - integerPart;
            var displayedFractionalPart = (int)(fractionalPart * Fractional.Keys.Max());

            if (displayedFractionalPart == 0)
            {
                // один процент
                var maleFormatter = new RussianNumberToWordsConverter(true);
                return string.Format("{0} {1}", maleFormatter.Convert(integerPart), Percent.GetPluralFor(integerPart));
            }

            var femaleFormatter = new RussianNumberToWordsConverter(false);
            displayedFractionalPart = Convert.ToInt32(displayedFractionalPart.ToString().TrimEnd('0'));
            var fractionName = (int)(Math.Pow(10, Math.Floor(Math.Log10(displayedFractionalPart)) + 1));

            if (integerPart == 0)
            {
                // одна десятая процента
                return string.Format("{0} {1} {2}",
                                     femaleFormatter.Convert(displayedFractionalPart),
                                     Fractional[fractionName].GetPluralFor(displayedFractionalPart),
                                     Percent.GetPluralFor(2));
            }

            // одна целая одна десятая процента
            return string.Format("{0} {1} {2} {3} {4}",
                                 femaleFormatter.Convert(integerPart),
                                 Integer.GetPluralFor(integerPart),
                                 femaleFormatter.Convert(displayedFractionalPart),
                                 Fractional[fractionName].GetPluralFor(displayedFractionalPart),
                                 Percent.GetPluralFor(2));
        }
    }
}
