﻿using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.DI.Config
{
    static class ChileOrderNumberConfigurationSpecs
    {
        [Tags("BL", "Chile", "Order")]
        [Subject(typeof(IEvaluateOrderNumberService))]
        class When_using_order_generator_for_Chile : OrderNumberGeneratingTestContext<IEvaluateOrderNumberService, ContainerFactory.ForChile>
        {
            Because of = () => GeneratedNumber = Service.Evaluate(null, Source1CSyncCode, Dest1CSyncCode, ReservedNumber);
            It generated_number_should_start_with_correct_prefix = () => GeneratedNumber.Should().StartWith("ORD");
        }

        [Tags("BL", "Chile", "Order")]
        [Subject(typeof(IEvaluateOrderNumberService))]
        class When_trying_generate_regional_number_for_Chile : OrderNumberGeneratingTestContext<IEvaluateOrderNumberService, ContainerFactory.ForChile>
        {
            Because of = () => Exception = Catch.Exception(() => Service.EvaluateRegional(null, Source1CSyncCode, Dest1CSyncCode, ReservedNumber));
            It should_be_not_supported = () => Exception.Should().BeOfType<NotSupportedException>();
        }
    }
}
