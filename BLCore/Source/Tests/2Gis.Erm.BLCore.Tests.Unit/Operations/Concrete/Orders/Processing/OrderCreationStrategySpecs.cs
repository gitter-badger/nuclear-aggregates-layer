using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing
{
    public static class OrderCreationStrategySpecs
    {
        public class TestableOrderCreationStrategy : OrderCreationStrategy
        {
            public TestableOrderCreationStrategy()
                : base((IUserContext)null,
                       (IOrderRepository)null,
                       (IUseCaseResumeContext<EditOrderRequest>)null,
                       (IProjectService)null,
                       (IOperationScope)null,
                       (IUserRepository)null,
                       MockOrderReadModel(),
                       (IAccountRepository)null,
                       MockEvaluateOrderNumberService(),
                       MockLegalPersonReadModel())
            {
            }

            private static ILegalPersonReadModel MockLegalPersonReadModel()
            {
                var mock = Mock.Of<ILegalPersonReadModel>();
                Mock.Get(mock)
                    .Setup(x => x.GetLegalPersonProfileIds(Moq.It.IsAny<long>()))
                    .Returns(new long[0]);

                return mock;
            }

            private static IEvaluateOrderNumberService MockEvaluateOrderNumberService()
            {
                var mock = Mock.Of<IEvaluateOrderNumberService>();
                Mock.Get(mock)
                    .Setup(x => x.Evaluate(Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<long?>()))
                    .Returns("AnyNumber");

                Mock.Get(mock)
                    .Setup(x => x.EvaluateRegional(Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<long?>()))
                    .Returns("AnyNumber");

                return mock;
            }

            private static IOrderReadModel MockOrderReadModel()
            {
                var mock = Mock.Of<IOrderReadModel>();
                Mock.Get(mock)
                    .Setup(x => x.IsOrganizationUnitsBothBranches(Moq.It.IsAny<long>(), Moq.It.IsAny<long>()))
                    .Returns(false);
                Mock.Get(mock)
                    .Setup(x => x.GetOrderOrganizationUnitsSyncCodes(Moq.It.IsAny<long[]>()))
                    .Returns<long[]>(ids => ids.Distinct().ToDictionary(i => i, i => (i * 100).ToString()));

                return mock;
            }

            public void TestActualizeOrderNumber(Order order, long? reservedNumberDigit)
            {
                base.ActualizeOrderNumber(order, reservedNumberDigit);
            }
        }

        abstract class OrderNumberActualizationContext
        {
            protected static TestableOrderCreationStrategy Strategy;
            protected static Order Order;

            Establish context = () =>
                {
                    Strategy = new TestableOrderCreationStrategy();
                    Order = new Order();
                };
        }

        class When_order_has_number : OrderNumberActualizationContext
        {
            private const string UserOrderNumber = "UserEnteredThisNumber";
            Establish regional_order_with_number = () =>
                {
                    Order.DestOrganizationUnitId = Order.SourceOrganizationUnitId + 1;
                    Order.Number = UserOrderNumber;
                };

            Because of = () => Strategy.TestActualizeOrderNumber(Order, null);

            It should_not_change_number = () => Order.Number.Should().BeEquivalentTo(UserOrderNumber);
            It should_set_regional_number = () => Order.RegionalNumber.Should().NotBeNullOrEmpty();
        }

        class When_order_is_not_regional : OrderNumberActualizationContext
        {
            Because of = () => Strategy.TestActualizeOrderNumber(Order, null);

            It should_set_number = () => Order.Number.Should().NotBeNullOrEmpty();
            It should_not_set_regional_number = () => Order.RegionalNumber.Should().BeNull();
        }

        class When_order_is_regional : OrderNumberActualizationContext
        {
            Establish regional_order = () => Order.DestOrganizationUnitId = Order.SourceOrganizationUnitId + 1;

            Because of = () => Strategy.TestActualizeOrderNumber(Order, null);

            It should_set_number = () => Order.Number.Should().NotBeNullOrEmpty();
            It should_set_regional_number = () => Order.RegionalNumber.Should().NotBeNullOrEmpty();
        }
    }
}
