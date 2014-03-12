using DoubleGis.Erm.BL.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.Platform.API.Core.Settings;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BL.Tests.Unit.BL.CostCalculationService
{
    public class CostCalculatorSpecs
    {
        [Tags("BL")]
        [Subject(typeof(CostCalculator))]
        public abstract class CostCalculatorContext
        {
            private Establish context = () =>
                {
                    Calculator = new CostCalculator(Mock.Of<IAppSettings>());
                };

            protected static CostCalculator Calculator { get; set; }
            protected static DiscountInfo Discount { get; set; }
        }

        private class When_calculating_discount_with_empty_price_and_discount_with_cash : CostCalculatorContext
        {
            private Because of = () =>
            {
                Discount = Calculator.CalculateDiscount(0, 100, 5, false);
            };

            private It should_have_zero_discount_sum = () => Discount.Sum.Should().Equals(decimal.Zero);
            private It should_have_zero_discount_percent = () => Discount.Percent.Should().Equals(decimal.Zero);
            private It should_be_calculated_via_cash = () => Discount.CalculateDiscountViaPercent.Should().BeFalse();
        }

        private class When_calculating_discount_with_empty_price_and_discount_with_percent : CostCalculatorContext
        {
            private Because of = () =>
            {
                Discount = Calculator.CalculateDiscount(0, 100, 5, true);
            };

            private It should_have_zero_discount_sum = () => Discount.Sum.Should().Equals(decimal.Zero);
            private It should_have_zero_discount_percent = () => Discount.Percent.Should().Equals(decimal.Zero);
            private It should_be_calculated_via_percent = () => Discount.CalculateDiscountViaPercent.Should().BeTrue();
        }
    }
}
