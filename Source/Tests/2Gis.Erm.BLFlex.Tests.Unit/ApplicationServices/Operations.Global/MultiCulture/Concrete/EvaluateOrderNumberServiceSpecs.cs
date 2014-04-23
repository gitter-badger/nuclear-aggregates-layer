using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.MultiCulture.Concrete
{
    static class EvaluateOrderNumberServiceSpecs
    {
        private const string NewOrderNumberFormat = "АБВГ_32-01-35537-API";
        private const string OldOrderNumberFormat = "АБВГ_50-50-35537";

        [Tags("BL", "OrderNumber")]
        [Subject(typeof(EvaluateOrderNumberService))]
        public abstract class EvaluateOrderNumberServiceContext
        {
            protected static EvaluateOrderNumberService OrderNumberService;
            protected static string GeneratedNumber;

            Establish context = () =>
                {
                    var orderRepositoryMock = new Mock<IOrderReadModel>();
                    orderRepositoryMock.Setup(x => x.GetOrderOrganizationUnitsSyncCodes(Moq.It.IsAny<long[]>()))
                                       .Returns((long[] orderIds) => orderIds.ToDictionary(i => i, i => i.ToString()));

                    OrderNumberService = new EvaluateOrderNumberService("АБВГ_{0}-{1}-{2}", "АБВГ_{0}-{1}-{2}", OrderNumberGenerationStrategies.ForRussia);
                };
        }

        class WhenCallForOldOrderNumberFormatWithoutReservedNumber : EvaluateOrderNumberServiceContext
        {
            Because of = () => GeneratedNumber = OrderNumberService.Evaluate(OldOrderNumberFormat, "666", "999", null);
            It order_number_should_be_with_BZ_prefix_and_with_predefined_order_number = () => GeneratedNumber.Should().Be("АБВГ_666-999-35537");
        }

        class WhenCallForNewOrderNumberFormatWithoutReservedNumber : EvaluateOrderNumberServiceContext
        {
            Because of = () => GeneratedNumber = OrderNumberService.Evaluate(NewOrderNumberFormat, "666", "999", null);
            It order_number_should_be_with_BZ_prefix_and_with_predefined_order_number = () => GeneratedNumber.Should().Be("АБВГ_666-999-35537");
        }

        class WhenCallWithoutOrderNumberFormatWithReservedNumber : EvaluateOrderNumberServiceContext
        {
            Because of = () => GeneratedNumber = OrderNumberService.Evaluate(null, "666", "999", 35537);
            It order_number_should_be_with_BZ_prefix_and_with_reserved_order_number = () => GeneratedNumber.Should().Be("АБВГ_666-999-35537");
        }

        class WhenCallWithOrderNumberInFluentFormat : EvaluateOrderNumberServiceContext
        {
            Because of = () => GeneratedNumber = OrderNumberService.Evaluate("123456", "666", "999", null);
            It order_number_should_not_be_changed = () => GeneratedNumber.Should().Be("123456");
        }

        class WhenCallWithoutOrderNumberAndWithoutReservedNumber : EvaluateOrderNumberServiceContext
        {
            static Action Action;
            Because of = () => Action = () => OrderNumberService.Evaluate(null, "666", "999", null);
            It exception_of_type_NotificationException_should_be_thrown = () => Action.ShouldThrow<NotificationException>();
        }
    }
}
