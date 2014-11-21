using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Shared.Formatter
{
    public static class KopekInRussianPluralizerSpecs
    {
        [Tags("BL", "Formatter")]
        [Subject(typeof(KopekInRussianPluralizer))]
        public class WhenReadingNumber
        {
            static KopekInRussianPluralizer _wordPluralizer;

            static Dictionary<int, string> Data;
            static string[] Results;

            Establish context = () =>
            {
                Data = new Dictionary<int, string>
                        {
                            { 1, "копейка" },
                            { 2, "копейки" },
                            { 3, "копейки" },
                            { 4, "копейки" },
                            { 5, "копеек" },
                            { 6, "копеек" },
                            { 7, "копеек" },
                            { 8, "копеек" },
                            { 9, "копеек" },
                            { 10, "копеек" },
                            { 11, "копеек" },
                            { 12, "копеек" },
                            { 21, "копейка" },
                            { 22, "копейки" },
                            { 23, "копейки" },
                            { 25, "копеек" },
                            { 26, "копеек" },
                            { 31, "копейка" },
                            { 32, "копейки" },
                            { 33, "копейки" },
                            { 35, "копеек" },
                            { 36, "копеек" },
                            { 41, "копейка" },
                            { 42, "копейки" },
                            { 43, "копейки" },
                            { 45, "копеек" },
                            { 46, "копеек" },
                            { 51, "копейка" },
                            { 52, "копейки" },
                            { 53, "копейки" },
                            { 55, "копеек" },
                            { 56, "копеек" },
                            { 61, "копейка" },
                            { 62, "копейки" },
                            { 63, "копейки" },
                            { 65, "копеек" },
                            { 66, "копеек" },
                            { 71, "копейка" },
                            { 72, "копейки" },
                            { 73, "копейки" },
                            { 75, "копеек" },
                            { 76, "копеек" },
                            { 81, "копейка" },
                            { 82, "копейки" },
                            { 83, "копейки" },
                            { 85, "копеек" },
                            { 86, "копеек" },
                            { 91, "копейка" },
                            { 92, "копейки" },
                            { 93, "копейки" },
                            { 95, "копеек" },
                            { 96, "копеек" },
                            { 100, "копеек" },
                            { 1000, "копеек" },
                            { 1000000, "копеек" },
                            { 1000000000, "копеек" }
                        };

                _wordPluralizer = new KopekInRussianPluralizer();
            };

            Because of = () => Results = Data.Keys.Select(x => _wordPluralizer.GetPluralFor(x)).ToArray();

            It should_return_expected_results = () => Results.Should().Equal(Data.Values);
        }

        [Tags("BL", "Formatter")]
        [Subject(typeof(KopekInRussianPluralizer))]
        public class WhenReadingComplicatedNumber
        {
            static KopekInRussianPluralizer _wordPluralizer;

            static int NumberToRead;
            static string Result;
            static string ExpectedResult;

            Establish context = () =>
            {
                NumberToRead = -1234567892;
                ExpectedResult = "копейки";
                _wordPluralizer = new KopekInRussianPluralizer();
            };

            Because of = () => Result = _wordPluralizer.GetPluralFor(NumberToRead);

            It should_return_expected_results = () => Result.Should().Be(ExpectedResult);
        }
    }
}
