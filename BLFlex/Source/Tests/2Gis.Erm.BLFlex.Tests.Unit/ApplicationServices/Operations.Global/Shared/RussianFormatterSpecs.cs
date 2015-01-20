using System;
using System.Globalization;
using System.Threading;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.Formatter
{
    public static class RussianFormatterSpecs
    {
        [Tags("BL", "Formatter", "Russia")]
        [Subject(typeof(RussiaFormatterFactory))]
        public abstract class FormatterContext
        {
            protected static IFormatterFactory _factory;

            Establish context = () =>
            {
                _factory = new RussiaFormatterFactory(Mock.Of<IGlobalizationSettings>());
            };
        }

        public class WhenFormattingNumber : FormatterContext
        {
            private static IFormatter _formatter;

            Because of = () => 
                _formatter = _factory.Create(typeof(decimal), FormatType.Unspecified, 0);

            // для типа decimal у нас по умолчанию применяется денежный формат
            It should_format_as_roubles = () => _formatter.Format(1.0M).Should().Be("1,00р.");
            
            // Обрати внимание, форматтер словами использует банковское округление, в то время как цифрами - обычное.
            // Стоит сделать и тут банковское, но тогда может не сойтись сумма по позициям и выводимое значение.
            It should_not_use_bankers_rounding = () => _formatter.Format(1.125M).Should().Be("1,13р.");

            It should_not_depend_on_thread_culture = () =>
                {
                    var culture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    try
                    {
                        _formatter.Format(1.0M).Should().Be("1,00р.");
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = culture;
                    }
                };
        }

        public class WhenUsingMoneyWordsFormat : FormatterContext
        {
            private static IFormatter _formatter;

            Because of = () =>
                _formatter = _factory.Create(typeof(decimal), FormatType.MoneyWords, 643);

            It should_use_plurals_correctly_1 = () => _formatter.Format(1M).Should().Be("один рубль 00 копеек");
            It should_use_plurals_correctly_2 = () => _formatter.Format(2M).Should().Be("два рубля 00 копеек");
            It should_use_plurals_correctly_5 = () => _formatter.Format(5M).Should().Be("пять рублей 00 копеек");
            It should_use_teens_correctly = () => _formatter.Format(11M).Should().Be("одиннадцать рублей 00 копеек");
            It should_use_bankers_rounding_1 = () => _formatter.Format(1.115M).Should().Be("один рубль 12 копеек");
            It should_use_bankers_rounding_2 = () => _formatter.Format(1.125M).Should().Be("один рубль 12 копеек");
        }

        public class WhenUsingMoneyWordsUpperStartFormat : FormatterContext
        {
            private static IFormatter _formatter;

            Because of = () =>
                _formatter = _factory.Create(typeof(decimal), FormatType.MoneyWordsUpperStart, 643);

            It should_make_first_letter_capital = () => _formatter.Format(1M).Should().Be("Один рубль 00 копеек");
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