using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.MultiCulture.Concrete
{
    static class RegionalNumberNotSupportedServiceSpecs
    {
        [Tags("BL", "OrderNumber")]
        [Subject(typeof(EvaluateOrderNumberWithoutRegionalService))]
        class WhenCallForOldOrderNumberFormatWithoutReservedNumber
        {
            static Action Action;
            protected static EvaluateOrderNumberWithoutRegionalService OrderNumberService;
            Establish context = () => OrderNumberService = new EvaluateOrderNumberWithoutRegionalService(Moq.It.IsAny<string>(), Moq.It.IsAny<IEnumerable<IOrderNumberGenerationStrategy>>());
            Because of = () => Action = () => OrderNumberService.EvaluateRegional(string.Empty, string.Empty, string.Empty, 0);
            It should_throw_exception = () => Action.ShouldThrow<Exception>();
        }
    }
}
