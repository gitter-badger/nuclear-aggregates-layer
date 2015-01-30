using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Shared.Formatter
{
    public static class RussianPercentToWordsFormatterSpecs
    {
        [Tags("BL", "Formatter")]
        [Subject(typeof(RussianPercentToWordsFormatter))]
        public class WhenReadingNumber
        {
            static RussianPercentToWordsFormatter _converter;

            static Dictionary<decimal, string> Data;
            static string[] Results;

            Establish context = () =>
                {
                    Data = new Dictionary<decimal, string>()
                        {
                            { 1, "один процент" },
                            { 2, "два процента" },
                            { 5, "пять процентов" },

                            { 1.1m, "одна целая одна десятая процента" },
                            { 2.2m, "две целых две десятых процента" },
                            { 5.5m, "пять целых пять десятых процента" },

                            { 0.1m, "одна десятая процента" },
                            { 0.11m, "одиннадцать сотых процента" },
                            { 0.111m, "сто одиннадцать тысячных процента" },
                            { 0.1111m, "одна тысяча сто одиннадцать десятитысячных процента" },
                        };

                    _converter = new RussianPercentToWordsFormatter(4);
                };

            Because of = () => Results = Data.Keys.Select(x => _converter.Format(x)).ToArray();

            It should_return_expected_results = () => Results.Should().Equal(Data.Values);
        }
    }
}
