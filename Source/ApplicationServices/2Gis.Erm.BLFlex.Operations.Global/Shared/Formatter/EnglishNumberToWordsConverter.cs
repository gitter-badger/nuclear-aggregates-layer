using System;
using System.Text;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    public sealed class EnglishNumberToWordsConverter : INumberToWordsConverter
    {
        private static readonly string[] Tens =
            {
                string.Empty,
                "ten",
                "twenty",
                "thirty",
                "forty",
                "fifty",
                "sixty",
                "seventy",
                "eighty",
                "ninety"
            };

        private static readonly string[] Ones =
            {
                " ",
                "one ",
                "two ",
                "three ",
                "four ",
                "five ",
                "six ",
                "seven ",
                "eight ",
                "nine "
            };

        private static readonly string[] Teens =
            {
                "ten ",
                "eleven ",
                "twelve ",
                "thirteen ",
                "fourteen ",
                "fifteen ",
                "sixteen ",
                "seventeen ",
                "eighteen ",
                "nineteen "
            };

        public string Convert(int number)
        {
            var isNegative = false;
            if (number < 0m)
            {
                number = -number;
                isNegative = true;
            }

            var resultBuilder = new StringBuilder();

            if (0 == number)
            {
                resultBuilder.Append("0 ");
            }



            resultBuilder.Append(number % 1000 != 0
                                     ? ToWords(number, string.Empty)
                                     : string.Empty);

            if (number > 100 && number % 100 != 0 && !resultBuilder.ToString().Contains(" and "))
            {
                resultBuilder.Insert(0, "and ");
            }

            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, "thousand"));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, "million"));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, "billion"));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, "trillion"));
            number /= 1000;

            resultBuilder.Insert(0, ToWords(number, "quadrillion"));
            if (isNegative)
            {
                resultBuilder.Insert(0, "minus ");
            }

            return resultBuilder.ToString().Trim();
        }

        private static string ToWords(long val, string rank)
        {
            var num = val % 1000;
            if (0 == num)
            {
                return string.Empty;
            }

            if (num < 0)
            {
                throw new ArgumentOutOfRangeException("val", "Параметр не может быть отрицательным");
            }

            var resultBuilder = new StringBuilder();

            var hundredsValue = num / 100;
            var tensValue = (num % 100) / 10;
            var onesValue = num % 10;

            if (hundredsValue > 0)
            {
                resultBuilder.Append(Ones[hundredsValue]);
                resultBuilder.Append("hundred");
                if (tensValue > 0 || onesValue > 0)
                {
                    resultBuilder.Append(" and ");
                }
            }

            if (tensValue == 1)
            {
                resultBuilder.Append(Teens[onesValue]);
            }
            else
            {
                resultBuilder.Append(Tens[tensValue]);
                if (tensValue > 1 && onesValue > 0)
                {
                    resultBuilder.Append("-");
                }

                resultBuilder.Append(Ones[onesValue]);
            }

            resultBuilder.Append(rank);

            if (resultBuilder.Length != 0)
            {
                resultBuilder.Append(" ");
            }

            return resultBuilder.ToString();
        }
    }
}
