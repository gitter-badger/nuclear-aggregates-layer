using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Shared.Formatter
{
    public static class HryvniaInRussianPluralizerSpecs
    {
        [Tags("BL", "Formatter")]
        [Subject(typeof(HryvniaInRussianPluralizer))]
        public class WhenReadingNumber
        {
            static HryvniaInRussianPluralizer _wordPluralizer;

            static Dictionary<int, string> Data;
            static string[] Results;

            Establish context = () =>
            {
                Data = new Dictionary<int, string>
                        {
                            { 1, "гривна" },
                            { 2, "гривны" },
                            { 3, "гривны" },
                            { 4, "гривны" },
                            { 5, "гривен" },
                            { 6, "гривен" },
                            { 7, "гривен" },
                            { 8, "гривен" },
                            { 9, "гривен" },
                            { 10, "гривен" },
                            { 11, "гривен" },
                            { 12, "гривен" },
                            { 21, "гривна" },
                            { 22, "гривны" },
                            { 23, "гривны" },
                            { 25, "гривен" },
                            { 26, "гривен" },
                            { 31, "гривна" },
                            { 32, "гривны" },
                            { 33, "гривны" },
                            { 35, "гривен" },
                            { 36, "гривен" },
                            { 41, "гривна" },
                            { 42, "гривны" },
                            { 43, "гривны" },
                            { 45, "гривен" },
                            { 46, "гривен" },
                            { 51, "гривна" },
                            { 52, "гривны" },
                            { 53, "гривны" },
                            { 55, "гривен" },
                            { 56, "гривен" },
                            { 61, "гривна" },
                            { 62, "гривны" },
                            { 63, "гривны" },
                            { 65, "гривен" },
                            { 66, "гривен" },
                            { 71, "гривна" },
                            { 72, "гривны" },
                            { 73, "гривны" },
                            { 75, "гривен" },
                            { 76, "гривен" },
                            { 81, "гривна" },
                            { 82, "гривны" },
                            { 83, "гривны" },
                            { 85, "гривен" },
                            { 86, "гривен" },
                            { 91, "гривна" },
                            { 92, "гривны" },
                            { 93, "гривны" },
                            { 95, "гривен" },
                            { 96, "гривен" },
                            { 100, "гривен" },
                            { 1000, "гривен" },
                            { 1000000, "гривен" },
                            { 1000000000, "гривен" }
                        };

                _wordPluralizer = new HryvniaInRussianPluralizer();
            };

            Because of = () => Results = Data.Keys.Select(x => _wordPluralizer.GetPluralFor(x)).ToArray();

            It should_return_expected_results = () => Results.Should().Equal(Data.Values);
        }

        [Tags("BL", "Formatter")]
        [Subject(typeof(HryvniaInRussianPluralizer))]
        public class WhenReadingComplicatedNumber
        {
            static HryvniaInRussianPluralizer _wordPluralizer;

            static int NumberToRead;
            static string Result;
            static string ExpectedResult;

            Establish context = () =>
            {
                NumberToRead = -1234567892;
                ExpectedResult = "гривны";
                _wordPluralizer = new HryvniaInRussianPluralizer();
            };

            Because of = () => Result = _wordPluralizer.GetPluralFor(NumberToRead);

            It should_return_expected_results = () => Result.Should().Be(ExpectedResult);
        }
    }
}
