using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Shared.Formatter
{
    public static class RussianNumberToWordsPluralizerSpecs
    {
        [Tags("BL", "Formatter")]
        [Subject(typeof(RussianNumberToWordsConverter))]
        public class WhenReadingNumber
        {
            static RussianNumberToWordsConverter _russianNumberToWordsConverter;

            static Dictionary<int, string> Data;
            static string[] Results;

            Establish context = () =>
                {
                    Data = new Dictionary<int, string>()
                        {
                            { 3, "три" },
                            { 4, "четыре" },
                            { 5, "пять" },
                            { 6, "шесть" },
                            { 7, "семь" },
                            { 8, "восемь" },
                            { 9, "девять" },
                            { 10, "десять" },
                            { 11, "одиннадцать" },
                            { 12, "двенадцать" },
                            { 13, "тринадцать" },
                            { 14, "четырнадцать" },
                            { 15, "пятнадцать" },
                            { 16, "шестнадцать" },
                            { 17, "семнадцать" },
                            { 18, "восемнадцать" },
                            { 19, "девятнадцать" },
                            { 20, "двадцать" },
                            { 30, "тридцать" },
                            { 40, "сорок" },
                            { 50, "пятьдесят" },
                            { 60, "шестьдесят" },
                            { 70, "семьдесят" },
                            { 80, "восемьдесят" },
                            { 90, "девяносто" },
                            { 100, "сто" },
                            { 1000, "одна тысяча" },
                            { 2000, "две тысячи" },
                            { 3000, "три тысячи" },
                            { 5000, "пять тысяч" },
                            { 11000, "одиннадцать тысяч" },
                            { 1000000, "один миллион" },
                            { 2000000, "два миллиона" },
                            { 3000000, "три миллиона" },
                            { 5000000, "пять миллионов" },
                            { 1000000000, "один миллиард" },
                            { 2000000000, "два миллиарда" },
                        };

                    _russianNumberToWordsConverter = new RussianNumberToWordsConverter(true);
                };

            Because of = () => Results = Data.Keys.Select(x => _russianNumberToWordsConverter.Convert(x)).ToArray();

            It should_return_expected_results = () => Results.Should().Equal(Data.Values);
        }

        [Tags("BL", "Formatter")]
        [Subject(typeof(RussianNumberToWordsConverter))]
        public class WhenReadingNegativeNumber
        {
            static RussianNumberToWordsConverter _russianNumberToWordsConverter;

            static int NumberToRead;
            static string Result;
            static string ExpectedResult;

            Establish context = () =>
                {
                    NumberToRead = -3;
                    ExpectedResult = "минус три";
                    _russianNumberToWordsConverter = new RussianNumberToWordsConverter(true);
                };

            Because of = () => Result = _russianNumberToWordsConverter.Convert(NumberToRead);

            It should_return_expected_results = () => Result.Should().Be(ExpectedResult);
        }

        [Tags("BL", "Formatter")]
        [Subject(typeof(RussianNumberToWordsConverter))]
        public class WhenReadingComplicatedNumber
        {
            static RussianNumberToWordsConverter _russianNumberToWordsConverter;

            static int NumberToRead;
            static string Result;
            static string ExpectedResult;

            Establish context = () =>
            {
                NumberToRead = -1234567895;
                ExpectedResult = "минус один миллиард двести тридцать четыре миллиона пятьсот шестьдесят семь тысяч восемьсот девяносто пять";
                _russianNumberToWordsConverter = new RussianNumberToWordsConverter(true);
            };

            Because of = () => Result = _russianNumberToWordsConverter.Convert(NumberToRead);

            It should_return_expected_results = () => Result.Should().Be(ExpectedResult);
        }

        [Tags("BL", "Formatter")]
        [Subject(typeof(RussianNumberToWordsConverter))]
        public class WhenReadingNumberForMaleObject
        {
            static RussianNumberToWordsConverter _russianNumberToWordsConverter;

            static Dictionary<int, string> Data;
            static string[] Results;

            Establish context = () =>
            {
                Data = new Dictionary<int, string>()
                        {
                            { 1, "один" },
                            { 2, "два" },
                            { 21, "двадцать один" },
                            { 22, "двадцать два" },
                        };

                _russianNumberToWordsConverter = new RussianNumberToWordsConverter(true);
            };

            Because of = () => Results = Data.Keys.Select(x => _russianNumberToWordsConverter.Convert(x)).ToArray();

            It should_return_expected_results = () => Results.Should().Equal(Data.Values);
        }

        [Tags("BL", "Formatter")]
        [Subject(typeof(RussianNumberToWordsConverter))]
        public class WhenReadingNumberForFemaleObject
        {
            static RussianNumberToWordsConverter _russianNumberToWordsConverter;

            static Dictionary<int, string> Data;
            static string[] Results;

            Establish context = () =>
            {
                Data = new Dictionary<int, string>()
                        {
                            { 1, "одна" },
                            { 2, "две" },
                            { 21, "двадцать одна" },
                            { 22, "двадцать две" },
                        };

                _russianNumberToWordsConverter = new RussianNumberToWordsConverter(false);
            };

            Because of = () => Results = Data.Keys.Select(x => _russianNumberToWordsConverter.Convert(x)).ToArray();

            It should_return_expected_results = () => Results.Should().Equal(Data.Values);
        }
    }
}
