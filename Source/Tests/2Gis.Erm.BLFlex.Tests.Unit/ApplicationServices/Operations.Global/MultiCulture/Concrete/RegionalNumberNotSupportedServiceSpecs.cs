using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;

using FluentAssertions;

using Machine.Specifications;

using Moq;

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
            protected static EvaluateOrderNumberWithoutRegionalService  OrderNumberService;
            Establish context = () => OrderNumberService = new EvaluateOrderNumberWithoutRegionalService(Moq.It.IsAny<string>(), Moq.It.IsAny<IEnumerable<OrderNumberGenerationStrategy>>());
            Because of = () => Action = () => OrderNumberService.EvaluateRegional(string.Empty, string.Empty, string.Empty, 0);
            It should_throw_exception = () => Action.ShouldThrow<Exception>();
        }
    }
}
