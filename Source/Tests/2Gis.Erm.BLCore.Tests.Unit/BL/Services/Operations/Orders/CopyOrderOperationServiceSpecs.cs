using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.Orders
{
    [Subject(typeof(CopyOrderOperationService))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test!")]
    internal class CopyOrderOperationServiceSpecs
    {
        [Tags("BL")]
        private class CopyOrderOperationServiceSpecsContext
        {
            protected const long ORDER_TO_COPY_ID = 111111111111;

            protected static CopyOrderOperationService Target;
            protected static Order OrderToCopy;

            protected static IUserContext UserContext;
            protected static IPublicService PublicService;
            protected static ISecurityServiceEntityAccess SecurityServiceEntityAccess;
            protected static IOrderRepository OrderRepository;
            protected static IOperationScopeFactory ScopeFactory;

            private Establish context = () =>
                {
                    UserContext = SetupUserContext();
                    PublicService = SetupPublicService();
                    SecurityServiceEntityAccess = SetupSecurityServiceEntityAccess();
                    OrderRepository = SetupOrderRepository();
                    ScopeFactory = SetupScopeFactory();

                    Target = new CopyOrderOperationService(
                        UserContext, 
                        PublicService, 
                        SecurityServiceEntityAccess, 
                        OrderRepository, 
                        ScopeFactory,
                        null,
                        null);
                };

            private static IOperationScopeFactory SetupScopeFactory()
            {
                var scopeFactory = Mock.Of<IOperationScopeFactory>();
                var scope = Mock.Of<IOperationScope>();

                Mock.Get(scopeFactory)
                    .Setup(x => x.CreateNonCoupled<CopyOrderIdentity>())
                    .Returns(scope);

                Mock.Get(scope)
                    .Setup(x => x.Updated<Order>(Moq.It.IsAny<long>()))
                    .Returns(scope);

                return scopeFactory;
            }

            private static IOrderRepository SetupOrderRepository()
            {
                var orderRepository = Mock.Of<IOrderRepository>();
                var orderPositions = new OrderPositionWithAdvertisementsDto[0];
                var releaseNumbersDto = new ReleaseNumbersDto();
                var distributiuonDatesDto = new DistributionDatesDto();

                OrderToCopy = new Order { Id = ORDER_TO_COPY_ID, WorkflowStepId = (int)OrderState.OnTermination };

                Mock.Get(orderRepository).Setup(x => x.GetOrder(ORDER_TO_COPY_ID)).Returns(OrderToCopy);

                Mock.Get(orderRepository).Setup(x => x.GetOrderPositionsWithAdvertisements(ORDER_TO_COPY_ID)).Returns(orderPositions);

                Mock.Get(orderRepository)
                    .Setup(x => x.CreateCopiedOrder(OrderToCopy, orderPositions))
                    .Returns<Order, IEnumerable<OrderPositionWithAdvertisementsDto>>((o, pos) => { o.Id += 1; return o; }); // увы, метод CreateCopiedOrder изменяет входящий в него объект

                Mock.Get(orderRepository)
                    .Setup(x => x.CalculateDistributionDates(
                        Moq.It.IsAny<DateTime>(),
                        Moq.It.IsAny<int>(),
                        Moq.It.IsAny<int>()))
                    .Returns(distributiuonDatesDto);

                Mock.Get(orderRepository)
                    .Setup(x => x.CalculateReleaseNumbers(
                        Moq.It.IsAny<long>(),
                        Moq.It.IsAny<DateTime>(),
                        Moq.It.IsAny<int>(),
                        Moq.It.IsAny<int>()))
                    .Returns(releaseNumbersDto);
                
                return orderRepository;
            }

            private static ISecurityServiceEntityAccess SetupSecurityServiceEntityAccess()
            {
                var securityServiceEntityAccess = Mock.Of<ISecurityServiceEntityAccess>();

                // считаем, что доступ есть всегда
                Mock.Get(securityServiceEntityAccess)
                    .Setup(x => x.HasEntityAccess(
                        Moq.It.IsAny<EntityAccessTypes>(),
                        Moq.It.IsAny<EntityName>(),
                        Moq.It.IsAny<long>(),
                        Moq.It.IsAny<long?>(),
                        Moq.It.IsAny<long>(),
                        Moq.It.IsAny<long?>()))
                    .Returns(true);

                return securityServiceEntityAccess;
            }

            private static IPublicService SetupPublicService()
            {
                var publicService = Mock.Of<IPublicService>();
                var orderNumberResponse = new GenerateOrderNumberResponse();
                var recalculateOrderDiscountResponse = new RecalculateOrderDiscountResponse();

                Mock.Get(publicService)
                    .Setup(x => x.Handle(Moq.It.IsAny<GenerateOrderNumberRequest>()))
                    .Returns(orderNumberResponse);

                Mock.Get(publicService)
                    .Setup(x => x.Handle(Moq.It.IsAny<RecalculateOrderDiscountRequest>()))
                    .Returns(recalculateOrderDiscountResponse);

                return publicService;
            }

            private static IUserContext SetupUserContext()
            {
                var userContext = Mock.Of<IUserContext>();
                var userIdentity = Mock.Of<IUserIdentity>();

                Mock.Get(userContext).Setup(x => x.Identity).Returns(userIdentity);

                return userContext;
            }
        }

        private class When_copy_order_technical_termination : CopyOrderOperationServiceSpecsContext
        {
            private static Order newOrder;

            private Establish context = () => Mock.Get(OrderRepository)
                                                  .Setup(x => x.Update(Moq.It.IsAny<Order>()))
                                                  .Returns<Order>(x =>
                                                      {
                                                          newOrder = x;
                                                          return 1;
                                                      });

            private Because of = () => Target.CopyOrder(ORDER_TO_COPY_ID, isTechnicalTermination: true);

            private It new_order_TechnicallyTerminatedOrderId_field_should_be_equal_base_order_id = () => newOrder.TechnicallyTerminatedOrderId.Should().Be(ORDER_TO_COPY_ID);
        }
    }
}
