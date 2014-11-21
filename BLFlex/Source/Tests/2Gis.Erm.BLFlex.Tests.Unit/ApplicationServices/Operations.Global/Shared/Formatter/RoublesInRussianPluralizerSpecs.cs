using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Shared.Formatter
{
    public static class RoublesInRussianPluralizerSpecs
    {
        [Tags("BL", "Formatter")]
        [Subject(typeof(RoublesInRussianPluralizer))]
        public class WhenReadingNumber
        {
            static RoublesInRussianPluralizer _wordPluralizer;

            static Dictionary<int, string> Data;
            static string[] Results;

            Establish context = () =>
            {
                Data = new Dictionary<int, string>
                        {
                            { 1, "рубль" },
                            { 2, "рубля" },
                            { 3, "рубля" },
                            { 4, "рубля" },
                            { 5, "рублей" },
                            { 6, "рублей" },
                            { 7, "рублей" },
                            { 8, "рублей" },
                            { 9, "рублей" },
                            { 10, "рублей" },
                            { 11, "рублей" },
                            { 12, "рублей" },
                            { 21, "рубль" },
                            { 22, "рубля" },
                            { 23, "рубля" },
                            { 25, "рублей" },
                            { 26, "рублей" },
                            { 31, "рубль" },
                            { 32, "рубля" },
                            { 33, "рубля" },
                            { 35, "рублей" },
                            { 36, "рублей" },
                            { 41, "рубль" },
                            { 42, "рубля" },
                            { 43, "рубля" },
                            { 45, "рублей" },
                            { 46, "рублей" },
                            { 51, "рубль" },
                            { 52, "рубля" },
                            { 53, "рубля" },
                            { 55, "рублей" },
                            { 56, "рублей" },
                            { 61, "рубль" },
                            { 62, "рубля" },
                            { 63, "рубля" },
                            { 65, "рублей" },
                            { 66, "рублей" },
                            { 71, "рубль" },
                            { 72, "рубля" },
                            { 73, "рубля" },
                            { 75, "рублей" },
                            { 76, "рублей" },
                            { 81, "рубль" },
                            { 82, "рубля" },
                            { 83, "рубля" },
                            { 85, "рублей" },
                            { 86, "рублей" },
                            { 91, "рубль" },
                            { 92, "рубля" },
                            { 93, "рубля" },
                            { 95, "рублей" },
                            { 96, "рублей" },
                            { 100, "рублей" },
                            { 1000, "рублей" },
                            { 1000000, "рублей" },
                            { 1000000000, "рублей" }
                        };

                _wordPluralizer = new RoublesInRussianPluralizer();
            };

            Because of = () => Results = Data.Keys.Select(x => _wordPluralizer.GetPluralFor(x)).ToArray();

            It should_return_expected_results = () => Results.Should().Equal(Data.Values);
        }

        [Tags("BL", "Formatter")]
        [Subject(typeof(RoublesInRussianPluralizer))]
        public class WhenReadingComplicatedNumber
        {
            static RoublesInRussianPluralizer _wordPluralizer;

            static int NumberToRead;
            static string Result;
            static string ExpectedResult;

            Establish context = () =>
            {
                NumberToRead = -1234567892;
                ExpectedResult = "рубля";
                _wordPluralizer = new RoublesInRussianPluralizer();
            };

            Because of = () => Result = _wordPluralizer.GetPluralFor(NumberToRead);

            It should_return_expected_results = () => Result.Should().Be(ExpectedResult);
        }
    }
}
