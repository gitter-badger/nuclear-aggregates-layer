using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.DI.Config
{
    static class CyprusOrderNumberConfigurationSpecs
    {
        [Tags("BL", "Cyprus", "Order")]
        [Subject(typeof(IEvaluateOrderNumberService))]
        class When_using_order_generator_for_Cyprus : OrderNumberGeneratingTestContext<IEvaluateOrderNumberService, ContainerFactory.ForCyprus>
        {
            Because of = () => GeneratedNumber = Service.Evaluate(null, Source1CSyncCode, Dest1CSyncCode, ReservedNumber, OrderType.Sale);
            It generated_number_should_start_with_correct_prefix = () => GeneratedNumber.Should().StartWith("INV");
        }

        [Tags("BL", "Cyprus", "Order")]
        [Subject(typeof(IEvaluateOrderNumberService))]
        class When_trying_generate_regional_number_for_Cyprus : OrderNumberGeneratingTestContext<IEvaluateOrderNumberService, ContainerFactory.ForCyprus>
        {
            Because of = () => Exception = Catch.Exception(() => Service.EvaluateRegional(null, Source1CSyncCode, Dest1CSyncCode, ReservedNumber));
            It should_be_not_supported = () => Exception.Should().BeOfType<NotSupportedException>();
        }
    }
}