using System;
using System.Text;

namespace DoubleGis.Erm.Platform.Common.PrintFormEngine
{
    internal sealed class RublesInWordsFormatter : IFormatter
	{
		private static readonly string[] Hunds =
		{
			"", "��� ", "������ ", "������ ", "��������� ",
			"������� ", "�������� ", "������� ", "��������� ", "��������� "
		};

		private static readonly string[] Tens =
		{
			"", "������ ", "�������� ", "�������� ", "����� ", "��������� ",
			"���������� ", "��������� ", "����������� ", "��������� "
		};

		public string Format(object data)
		{
            return ToWords(Convert.ToDecimal(data));
		}

        // if you wnat to refactor this function, be sure that 2.745 -> 2.74, and 2.755 -> 2.76
        private static string ToWords(decimal val)
        {
            var minus = false;
            if (val < 0m)
            {
                val = -val;
                minus = true;
            }

            var roundedVal = Math.Round(val, 2, MidpointRounding.ToEven);

            var n = (long)roundedVal;
            var remainder = (long)(roundedVal * 100) - n * 100;

            var resultBuilder = new StringBuilder();

            if (0 == n) resultBuilder.Append("0 ");
            resultBuilder.Append(n%1000 != 0 ? ToWords(n, true, "�����", "�����", "������") : "������");

            n /= 1000;

            resultBuilder.Insert(0, ToWords(n, false, "������", "������", "�����"));
            n /= 1000;

            resultBuilder.Insert(0, ToWords(n, true, "�������", "��������", "���������"));
            n /= 1000;

            resultBuilder.Insert(0, ToWords(n, true, "��������", "���������", "����������"));
            n /= 1000;

            resultBuilder.Insert(0, ToWords(n, true, "��������", "���������", "����������"));
            n /= 1000;

            resultBuilder.Insert(0, ToWords(n, true, "���������", "����������", "�����������"));
            if (minus) resultBuilder.Insert(0, "����� ");

            if (resultBuilder[resultBuilder.Length - 1] != ' ')
                resultBuilder.Append(' ');

            resultBuilder.Append(remainder.ToString("00 "));
            resultBuilder.Append(Case(remainder, "�������", "�������", "������"));

            return resultBuilder.ToString();
        }

        private static string ToWords(long val, bool male, string one, string two, string five)
        {
            string[] frac20 =
            {
                "", "���� ", "��� ", "��� ", "������ ", "���� ", "����� ",
                "���� ", "������ ", "������ ", "������ ", "����������� ",
                "���������� ", "���������� ", "������������ ", "���������� ",
                "����������� ", "���������� ", "������������ ", "������������ "
            };

            var num = val % 1000;
            if (0 == num) return "";
            if (num < 0) throw new ArgumentOutOfRangeException("val", "�������� �� ����� ���� �������������");
            if (!male)
            {
                frac20[1] = "���� ";
                frac20[2] = "��� ";
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
    }
}
