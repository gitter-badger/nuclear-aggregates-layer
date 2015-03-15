using System;
using System.Text;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    public sealed class RussianNumberToWordsConverter : INumberToWordsConverter
    {
        private static readonly IWordPluralizer Thousand = new RussianWordPluralizer("тысяча", "тысячи", "тысяч");
        private static readonly IWordPluralizer Million = new RussianWordPluralizer("миллион", "миллиона", "миллионов");
        private static readonly IWordPluralizer Milliard = new RussianWordPluralizer("миллиард", "миллиарда", "миллиардов");
        private static readonly IWordPluralizer Trillion = new RussianWordPluralizer("триллион", "триллиона", "триллионов");
        private static readonly IWordPluralizer Trilliard = new RussianWordPluralizer("триллиард", "триллиарда", "триллиардов");

        private readonly bool _isObjectMale;

        public RussianNumberToWordsConverter(bool isObjectMale)
        {
            _isObjectMale = isObjectMale;
        }

        private static readonly string[] Hunds =
		{
			"", "сто ", "двести ", "триста ", "четыреста ",
			"пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
		};

        private static readonly string[] Tens =
		{
			"", "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
			"шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
		};

        public string Convert(long number)
        {
            var minus = false;
            if (number < 0m)
            {
                number = -number;
                minus = true;
            }

            var resultBuilder = new StringBuilder();

            if (0 == number) resultBuilder.Append("0 ");
            
            resultBuilder.Append(number % 1000 != 0 ? ToWords(number, _isObjectMale, null) : string.Empty);
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, false, Thousand));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, true, Million));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, true, Milliard));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, true, Trillion));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, true, Trilliard));
            if (minus) resultBuilder.Insert(0, "минус ");

            return resultBuilder.ToString().Trim();
        }

        private static string ToWords(long val, bool male, IWordPluralizer word)
        {
            string[] frac20 =
            {
                "", "один ", "два ", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
            };

            var num = val % 1000;
            if (0 == num)
            {
                return "";
            }

            if (num < 0)
            {
                throw new ArgumentOutOfRangeException("val", "Параметр не может быть отрицательным");
            }

            if (!male)
            {
                frac20[1] = "одна ";
                frac20[2] = "две ";
            }

            var resultBuilder = new StringBuilder(Hunds[num / 100]);

            if (num % 100 < 20)
            {
                resultBuilder.Append(frac20[num % 100]);
            }
            else
            {
                resultBuilder.Append(Tens[num % 100 / 10]);
                resultBuilder.Append(frac20[num % 10]);
            }

            if (word != null)
            {
                resultBuilder.Append(word.GetPluralFor(num));
            }

            if (resultBuilder.Length != 0) resultBuilder.Append(" ");
            return resultBuilder.ToString();
        }
    }
}
