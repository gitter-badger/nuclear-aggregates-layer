using System;
using System.Text;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    public sealed class RussianNumberToWordsConverter : INumberToWordsConverter
    {
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

        private static string ToWords(long val, bool male, string one, string two, string five)
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

            resultBuilder.Append(Case(num, one, two, five));

            if (resultBuilder.Length != 0) resultBuilder.Append(" ");
            return resultBuilder.ToString();
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

        public string Convert(int number)
        {
            var minus = false;
            if (number < 0m)
            {
                number = -number;
                minus = true;
            }

            var resultBuilder = new StringBuilder();

            if (0 == number) resultBuilder.Append("0 ");
            resultBuilder.Append(number % 1000 != 0
                                     ? ToWords(number, _isObjectMale, string.Empty, string.Empty, string.Empty)
                                     : string.Empty);

            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, false, "тысяча", "тысячи", "тысяч")); // COMMENT {all, 21.04.2014}: RussianWordPluralizer?
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, true, "миллион", "миллиона", "миллионов"));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, true, "миллиард", "миллиарда", "миллиардов"));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, true, "триллион", "триллиона", "триллионов"));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, true, "триллиард", "триллиарда", "триллиардов"));
            if (minus) resultBuilder.Insert(0, "минус ");

            return resultBuilder.ToString().Trim();
        }
    }
}
