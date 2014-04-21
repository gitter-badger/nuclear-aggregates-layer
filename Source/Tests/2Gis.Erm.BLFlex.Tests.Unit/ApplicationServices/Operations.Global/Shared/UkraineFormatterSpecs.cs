using System;

using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.Formatter
{
    public static class UkrainianFormatterSpecs
    {
        [Tags("BL", "Formatter", "Ukraine")]
        [Subject(typeof(GenerateOrderNumberHandler))]
        public abstract class FormatterContext
        {
            protected static IFormatterFactory _factory;

            Establish context = () =>
            {
                _factory = new UkraineFormatterFactory();
            };
        }

        public class WhenUsingMoneyWordsFormat : FormatterContext
        {
            private static IFormatter _formatter;

            Because of = () =>
                _formatter = _factory.Create(typeof(decimal), FormatType.MoneyWords, 980);

            It should_use_plurals_correctly_1 = () => _formatter.Format(1M).Should().Be("одна гривна 00 копеек");
            It should_use_plurals_correctly_2 = () => _formatter.Format(2M).Should().Be("две гривны 00 копеек");
            It should_use_plurals_correctly_5 = () => _formatter.Format(5M).Should().Be("пять гривен 00 копеек");
            It should_use_teens_correctly = () => _formatter.Format(11M).Should().Be("одиннадцать гривен 00 копеек");
            It should_use_bankers_rounding_1 = () => _formatter.Format(1.115M).Should().Be("одна гривна 12 копеек");
            It should_use_bankers_rounding_2 = () => _formatter.Format(1.125M).Should().Be("одна гривна 12 копеек");
        }

        public class WhenUsingMoneyWordsUpperStartFormat : FormatterContext
        {
            private static IFormatter _formatter;

            Because of = () =>
                _formatter = _factory.Create(typeof(decimal), FormatType.MoneyWordsUpperStart, 980);

            It should_make_first_letter_capital = () => _formatter.Format(1M).Should().Be("Одна гривна 00 копеек");
        }

        public class WhenFormattingShortDate : FormatterContext
        {
            private static IFormatter _formatter;

            Because of = () =>
                _formatter = _factory.Create(typeof(DateTime), FormatType.ShortDate, 0);

            It should_give_empty_string_for_null = () => _formatter.Format(null).Should().Be(string.Empty);
        }

        public class WhenRequestingPercentFormatter : FormatterContext
        {
            private static IFormatter _formatter;

            Because of = () => _formatter = _factory.Create(null, FormatType.Percents, 0);

            It should_provide_a_formatter = () => _formatter.Should().NotBeNull();
        }
    }
}