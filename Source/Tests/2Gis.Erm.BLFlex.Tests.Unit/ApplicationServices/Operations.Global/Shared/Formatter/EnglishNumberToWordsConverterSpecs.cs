using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Shared.Formatter
{
    public static class EnglishNumberToWordsConverterSpecs
    {
        [Tags("BL", "Formatter")]
        [Subject(typeof(EnglishNumberToWordsConverter))]
        public class WhenReadingNumber
        {
            static EnglishNumberToWordsConverter _englishNumberToWordsConverter;

            static Dictionary<int, string> Data;
            static string[] Results;

            Establish context = () =>
                {
                    Data = new Dictionary<int, string>()
                        {
                            { 1, "one" },
                            { 2, "two" },
                            { 3, "three" },
                            { 4, "four" },
                            { 5, "five" },
                            { 6, "six" },
                            { 7, "seven" },
                            { 8, "eight" },
                            { 9, "nine" },
                            { 10, "ten" },
                            { 11, "eleven" },
                            { 12, "twelve" },
                            { 13, "thirteen" },
                            { 14, "fourteen" },
                            { 15, "fifteen" },
                            { 16, "sixteen" },
                            { 17, "seventeen" },
                            { 18, "eighteen" },
                            { 19, "nineteen" },
                            { 20, "twenty" },
                            { 21, "twenty-one" },
                            { 22, "twenty-two" },
                            { 30, "thirty" },
                            { 40, "forty" },
                            { 50, "fifty" },
                            { 60, "sixty" },
                            { 70, "seventy" },
                            { 80, "eighty" },
                            { 90, "ninety" },
                            { 99, "ninety-nine" },
                            { 100, "one hundred" },
                            { 119, "one hundred and nineteen" },
                            { 200, "two hundred" },
                            { 900, "nine hundred" },
                            { 1000, "one thousand" },
                            { 2000, "two thousand" },
                            { 10000, "ten thousand" },
                            { 11000, "eleven thousand" },
                            { 20000, "twenty thousand" },
                            { 21000, "twenty-one thousand" },
                            { 100000, "one hundred thousand" },
                            { 999000, "nine hundred and ninety-nine thousand" },
                            { 1000000, "one million" },
                            { 10000000, "ten million" },
                            { 1000000000, "one billion" },
                            { 2000000000, "two billion" },
                            { 6018, "six thousand and eighteen" },
                            { 6000018, "six million and eighteen" },
                            { 6005018, "six million five thousand and eighteen" },
                        };

                    _englishNumberToWordsConverter = new EnglishNumberToWordsConverter();
                };

            Because of = () => Results = Data.Keys.Select(x => _englishNumberToWordsConverter.Convert(x)).ToArray();

            It should_return_expected_results = () => Results.Should().Equal(Data.Values);
        }
    }
}
