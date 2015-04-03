using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;

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
            protected static IOrderReadModel OrderReadModel;
            protected static IOperationScopeFactory ScopeFactory;

            private Establish context = () =>
                {
                    UserContext = SetupUserContext();
                    PublicService = SetupPublicService();
                    SecurityServiceEntityAccess = SetupSecurityServiceEntityAccess();
                    OrderRepository = SetupOrderRepository();
                    ScopeFactory = SetupScopeFactory();
                    OrderReadModel = SetupOrderReadModel();

                    Target = new CopyOrderOperationService(
                        UserContext, 
                        PublicService, 
                        SecurityServiceEntityAccess, 
                        OrderRepository, 
                        ScopeFactory,
                        OrderReadModel,
                        SetupOrderNumberService());
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

                OrderToCopy = new Order { Id = ORDER_TO_COPY_ID, WorkflowStepId = OrderState.OnTermination };

                Mock.Get(orderRepository)
                    .Setup(x => x.CreateCopiedOrder(Moq.It.IsAny<Order>(), Moq.It.IsAny<OrderPositionWithAdvertisementsDto[]>()))
                    .Returns<Order, IEnumerable<OrderPositionWithAdvertisementsDto>>((o, pos) => { o.Id += 1; return o; }); // увы, метод CreateCopiedOrder изменяет входящий в него объект
                
                return orderRepository;
            }

            private static IOrderReadModel SetupOrderReadModel()
            {
                var orderReadModel = Mock.Of<IOrderReadModel>();
                var orderPositions = new OrderPositionWithAdvertisementsDto[0];
                var distributiuonDatesDto = new DistributionDatesDto();
                var releaseNumbersDto = new ReleaseNumbersDto();

                OrderToCopy = new Order { Id = ORDER_TO_COPY_ID, WorkflowStepId = OrderState.OnTermination };

                Mock.Get(orderReadModel).Setup(x => x.GetOrderSecure(ORDER_TO_COPY_ID)).Returns(OrderToCopy);

                Mock.Get(orderReadModel).Setup(x => x.GetOrderPositionsWithAdvertisements(ORDER_TO_COPY_ID)).Returns(orderPositions);

                Mock.Get(orderReadModel)
                    .Setup(x => x.GetOrderOrganizationUnitsSyncCodes(Moq.It.IsAny<long[]>()))
                    .Returns<long[]>(ids => ids.Distinct().ToDictionary(i => i, i => (i * 10).ToString()));

                Mock.Get(orderReadModel)
                    .Setup(x => x.CalculateDistributionDates(
                        Moq.It.IsAny<DateTime>(),
                        Moq.It.IsAny<int>(),
                        Moq.It.IsAny<int>()))
                    .Returns(distributiuonDatesDto);

                Mock.Get(orderReadModel)
                    .Setup(x => x.CalculateReleaseNumbers(
                        Moq.It.IsAny<long>(),
                        Moq.It.IsAny<DateTime>(),
                        Moq.It.IsAny<int>(),
                        Moq.It.IsAny<int>()))
                    .Returns(releaseNumbersDto);

                return orderReadModel;
            }

            private static ISecurityServiceEntityAccess SetupSecurityServiceEntityAccess()
            {
                var securityServiceEntityAccess = Mock.Of<ISecurityServiceEntityAccess>();

                // считаем, что доступ есть всегда
                Mock.Get(securityServiceEntityAccess)
                    .Setup(x => x.HasEntityAccess(
                        Moq.It.IsAny<EntityAccessTypes>(),
                        Moq.It.IsAny<IEntityType>(),
                        Moq.It.IsAny<long>(),
                        Moq.It.IsAny<long?>(),
                        Moq.It.IsAny<long>(),
                        Moq.It.IsAny<long?>()))
                    .Returns(true);

                return securityServiceEntityAccess;
            }

            private static IEvaluateOrderNumberService SetupOrderNumberService()
            {
                var service = Mock.Of<IEvaluateOrderNumberService>();

                Mock.Get(service)
                    .Setup(s => s.Evaluate(Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<long?>()))
                    .Returns(string.Empty);

                Mock.Get(service)
                    .Setup(s => s.EvaluateRegional(Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<long?>()))
                    .Returns(string.Empty);

                return service;
            }

            private static IPublicService SetupPublicService()
            {
                var publicService = Mock.Of<IPublicService>();
                var recalculateOrderDiscountResponse = new RecalculateOrderDiscountResponse();

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
