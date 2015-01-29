using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    public sealed class RussianPercentToWordsFormatter : IFormatter
    {
        private static readonly IWordPluralizer IntegerWord = new RussianWordPluralizer("целая", "целых", "целых");
        private static readonly IWordPluralizer PercentWord = new RussianWordPluralizer("процент", "процента", "процентов");
        private static readonly IDictionary<int, IWordPluralizer> FractionalWords
            = new Dictionary<int, IWordPluralizer>
                  {
                      { 10, new RussianWordPluralizer("десятая", "десятых", "десятых") },
                      { 100, new RussianWordPluralizer("сотая", "сотых", "сотых") },
                      { 1000, new RussianWordPluralizer("тысячная", "тысячных", "тысячных") },
                      { 10000, new RussianWordPluralizer("десятитысячная", "десятитысячных", "десятитысячных") },
                  };

        private readonly int _significantDigitsNumber;

        public RussianPercentToWordsFormatter(int significantDigitsNumber)
        {
            if (significantDigitsNumber > Math.Log10(FractionalWords.Keys.Max()))
            {
                var message = string.Format("Невозможно вывести проценты с точностью до {0} знаков после запятой, нужно дополнить описание",
                                            significantDigitsNumber);
                throw new ArgumentException(message);
            }

            _significantDigitsNumber = significantDigitsNumber;
        }

        public string Format(object data)
        {
            var value = Convert.ToDecimal(data);

            var integerPart = (int)Math.Floor(value);
            var fractionalPart = value - integerPart;
            var displayedFractionalPart = (int)(fractionalPart * (int)Math.Pow(10, _significantDigitsNumber));

            if (displayedFractionalPart == 0)
            {
                // например, "один процент"
                var maleFormatter = new RussianNumberToWordsConverter(true);
                return string.Format("{0} {1}", maleFormatter.Convert(integerPart), PercentWord.GetPluralFor(integerPart));
            }

            var femaleFormatter = new RussianNumberToWordsConverter(false);
            displayedFractionalPart = Convert.ToInt32(displayedFractionalPart.ToString().TrimEnd('0'));
            var fractionName = (int)(Math.Pow(10, Math.Floor(Math.Log10(displayedFractionalPart)) + 1));

            if (integerPart == 0)
            {
                // например, "одна десятая процента"
                return string.Format("{0} {1} {2}",
                                     femaleFormatter.Convert(displayedFractionalPart),
                                     FractionalWords[fractionName].GetPluralFor(displayedFractionalPart),
                                     PercentWord.GetPluralFor(2));
            }

            // например, "одна целая одна десятая процента"
            return string.Format("{0} {1} {2} {3} {4}",
                                 femaleFormatter.Convert(integerPart),
                                 IntegerWord.GetPluralFor(integerPart),
                                 femaleFormatter.Convert(displayedFractionalPart),
                                 FractionalWords[fractionName].GetPluralFor(displayedFractionalPart),
                                 PercentWord.GetPluralFor(2));
        }
    }
}
