using System;
using System.Globalization;
using System.Threading;

using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.Formatter
{
    public static class ChileFormatterSpecs
    {
        [Tags("BL", "Formatter", "Chile")]
        [Subject(typeof(GenerateOrderNumberHandler))]
        public abstract class FormatterContext
        {
            protected static IFormatterFactory _factory;

            Establish context = () =>
            {
                _factory = new ChileFormatterFactory();
            };
        }

        public class WhenFormattingMoney : FormatterContext
        {
            private static IFormatter _formatter;

            Because of = () => 
                _formatter = _factory.Create(typeof(decimal), FormatType.Money, 0);

            It should_format_as_peso = () => _formatter.Format(10374788.00).Should().Be("10.374.788,00 pesos");

            It should_not_depend_on_thread_culture = () =>
            {
                var culture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                try
                {
                    _formatter.Format(1.0M).Should().Be("1,00 pesos");
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
    }
}