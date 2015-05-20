using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.DI.Config
{
    static class RussiaOrderNumberConfigurationSpecs
    {
        [Tags("BL", "Russia", "Order")]
        [Subject(typeof(IEvaluateOrderNumberService))]
        class When_using_order_generator_for_Russia : OrderNumberGeneratingTestContext<IEvaluateOrderNumberService, ContainerFactory.ForRussia>
        {
            Because of = () => GeneratedNumber = Service.Evaluate(null, Source1CSyncCode, Dest1CSyncCode, ReservedNumber, OrderType.Sale);
            It generated_number_should_start_with_correct_prefix = () => GeneratedNumber.Should().StartWith("аг");
        }

        [Tags("BL", "Russia", "Order")]
        [Subject(typeof(IEvaluateOrderNumberService))]
        class When_trying_generate_regional_number_for_Russia : OrderNumberGeneratingTestContext<IEvaluateOrderNumberService, ContainerFactory.ForRussia>
        {
            Because of = () => GeneratedNumber = Service.EvaluateRegional(null, Source1CSyncCode, Dest1CSyncCode, ReservedNumber);
            It generated_number_should_start_with_correct_prefix = () => GeneratedNumber.Should().StartWith("нт");
        }
    }
}