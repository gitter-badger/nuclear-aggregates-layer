using System.Globalization;
using System.Threading;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;
using System;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.Formatter
{
    public static class CyprusFormatterSpecs
    {
        [Tags("BL", "Formatter", "Cyprus")]
        [Subject(typeof(CyprusFormatterFactory))]
        public abstract class FormatterContext
        {
            protected static IFormatterFactory _factory;

            Establish context = () =>
            {
                _factory = new CyprusFormatterFactory();
            };
        }

        public class WhenFormattingMoney : FormatterContext
        {
            private static IFormatter _formatter;

            Because of = () => 
                _formatter = _factory.Create(typeof(decimal), FormatType.Money, 0);

            It should_format_as_euro = () => _formatter.Format(761.6).Should().Be("761,60 €");

            It should_not_depend_on_thread_culture = () =>
            {
                var culture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                try
                {
                    _formatter.Format(1.0M).Should().Be("1,00 €");
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                }
            };

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