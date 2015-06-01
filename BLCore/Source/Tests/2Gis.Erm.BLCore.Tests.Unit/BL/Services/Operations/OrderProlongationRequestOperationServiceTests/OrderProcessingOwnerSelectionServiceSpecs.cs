using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Model.Metadata.Operations.Identity.Specific.OrderProcessingRequest;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test.")]
    public class OrderProcessingOwnerSelectionServiceSpecs
    {
        private class Context
        {
            protected static IOrderProcessingRequestService OrderProcessingRequestService;
            protected static IOperationScopeFactory ScopeFactory;
            protected static IUserReadModel UserReadModel;
            protected static ISecurityServiceUserIdentifier SecurityServiceUserIdentifier;

            protected static OrderProcessingRequest OrderProcessingRequest;
            protected static ICollection<IMessageWithType> Messages;

            protected static OrderProcessingOwnerSelectionService Target;
            protected static User Result;
            protected static User ExpectedUser;

            private Establish context = () =>
                {
                    UserReadModel = Mock.Of<IUserReadModel>();
                    SecurityServiceUserIdentifier = Mock.Of<ISecurityServiceUserIdentifier>();
                    OrderProcessingRequestService = Mock.Of<IOrderProcessingRequestService>();

                    ScopeFactory = Mock.Of<IOperationScopeFactory>();
                    Mock.Get(ScopeFactory).Setup(x => x.CreateNonCoupled<SelectOrderProcessingOwnerIdentity>()).Returns(Mock.Of<IOperationScope>());

                    OrderProcessingRequest = new OrderProcessingRequest();
                    Messages = new List<IMessageWithType>();

                    Target = new OrderProcessingOwnerSelectionService(UserReadModel, OrderProcessingRequestService, SecurityServiceUserIdentifier, ScopeFactory);
                };

            private Because of = () => Result = Target.FindOwner(OrderProcessingRequest, Messages);
        }

        private class OrderProlongationContext : Context
        {
            protected const long BASE_ORDER_ID = 111111111111;

            private Establish context = () =>
                {
                    OrderProcessingRequest.RequestType = OrderProcessingRequestType.ProlongateOrder;
                    OrderProcessingRequest.BaseOrderId = BASE_ORDER_ID;
                };
        }

        private class OrderCreationContext : Context
        {
            private Establish context = () =>
                {
                    OrderProcessingRequest.RequestType = OrderProcessingRequestType.CreateOrder;
                };
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingOwnerSelectionService))]
        private class When_prolongation_and_base_order_owner_is_nice : OrderProlongationContext
        {
            private Establish context = () =>
                {
                    const long USER_ID = 22222222222222;

                    ExpectedUser = new User { Id = USER_ID };

                    Mock.Get(OrderProcessingRequestService)
                        .Setup(x => x.GetOrderDto(BASE_ORDER_ID))
                        .Returns(() => new OrderProcessingRequestOrderDto { OwnerCode = USER_ID, Id = BASE_ORDER_ID, Number = "XYZ" });

                    Mock.Get(UserReadModel)
                        .Setup(x => x.GetNotServiceUser(USER_ID))
                        .Returns(ExpectedUser);
                };

            private It should_returns_him = () => Result.Should().Be(ExpectedUser);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingOwnerSelectionService))]
        private class When_prolongation_and_base_order_owner_is_rejected : OrderProlongationContext
        {
            private Establish context = () =>
            {
                const long USER_ID = 22222222222222;
                const long FIRM_ID = 22222222222223;
                const long CLIENT_ID = 22222222222224;

                ExpectedUser = new User { Id = USER_ID };

                OrderProcessingRequest.FirmId = FIRM_ID;

                var client = new OrderProcessingRequestFirmDto.ClientDto
                    {
                        Id = CLIENT_ID,
                        OwnerCode = USER_ID
                    };

                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetFirmDto(FIRM_ID))
                    .Returns(new OrderProcessingRequestFirmDto { Client = client });

                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetOrderDto(Moq.It.IsAny<long>()))
                    .Returns<long>(orderId => new OrderProcessingRequestOrderDto { OwnerCode = 0, Id = orderId, Number = "XYZ_" + orderId });

                Mock.Get(UserReadModel)
                    .Setup(x => x.GetNotServiceUser(USER_ID))
                    .Returns(ExpectedUser);
            };

            private It should_returns_firm_client_owner = () => Result.Should().Be(ExpectedUser);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingOwnerSelectionService))]
        private class When_prolongation_and_firm_client_owner_is_rejected : OrderProlongationContext
        {
            private Establish context = () =>
            {
                const long FIRM_ID = 22222222222223;
                const long ORGANIZATION_UNIT_ID = 22222222222224;

                ExpectedUser = new User();

                OrderProcessingRequest.FirmId = FIRM_ID;

                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetFirmDto(FIRM_ID))
                    .Returns(new OrderProcessingRequestFirmDto
                        {
                            OrganizationUnitId = ORGANIZATION_UNIT_ID, 
                            Client = new OrderProcessingRequestFirmDto.ClientDto()
                        });

                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetOrderDto(Moq.It.IsAny<long>()))
                    .Returns<long>(orderId => new OrderProcessingRequestOrderDto { OwnerCode = 0, Id = orderId, Number = "XYZ_" + orderId });

                Mock.Get(UserReadModel)
                    .Setup(x => x.GetOrganizationUnitDirector(ORGANIZATION_UNIT_ID))
                    .Returns(ExpectedUser);
            };

            private It should_returns_organization_unit_director = () => Result.Should().Be(ExpectedUser);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingOwnerSelectionService))]
        private class When_prolongation_and_organization_unit_director_is_rejected : OrderProlongationContext
        {
            private Establish context = () =>
            {
                const long FIRM_ID = 22222222222223;
                const long ORGANIZATION_UNIT_ID = 22222222222224;

                var reserveUserInfo = new UserInfo(27, "reserve", string.Empty);
                ExpectedUser = new User { Id = reserveUserInfo.Code, Account = reserveUserInfo.Account };

                OrderProcessingRequest.FirmId = FIRM_ID;

                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetFirmDto(FIRM_ID))
                    .Returns(new OrderProcessingRequestFirmDto
                    {
                        OrganizationUnitId = ORGANIZATION_UNIT_ID,
                        Client = new OrderProcessingRequestFirmDto.ClientDto()
                    });

                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetOrderDto(Moq.It.IsAny<long>()))
                    .Returns<long>(orderId => new OrderProcessingRequestOrderDto { OwnerCode = 0, Id = orderId, Number = "XYZ_" + orderId });

                Mock.Get(SecurityServiceUserIdentifier)
                    .Setup(x => x.GetReserveUserIdentity())
                    .Returns(reserveUserInfo);

                Mock.Get(UserReadModel)
                    .Setup(x => x.GetUser(reserveUserInfo.Code))
                    .Returns(ExpectedUser);
            };

            private It should_returns_reserve_user = () => Result.Should().Be(ExpectedUser);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingOwnerSelectionService))]
        private class When_creation_and_base_order_owner_is_rejected : OrderCreationContext
        {
            private Establish context = () =>
            {
                const long USER_ID = 22222222222222;
                const long FIRM_ID = 22222222222223;
                const long CLIENT_ID = 22222222222224;

                ExpectedUser = new User { Id = USER_ID };

                OrderProcessingRequest.FirmId = FIRM_ID;

                var client = new OrderProcessingRequestFirmDto.ClientDto
                {
                    Id = CLIENT_ID,
                    OwnerCode = USER_ID
                };

                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetFirmDto(FIRM_ID))
                    .Returns(new OrderProcessingRequestFirmDto { Client = client });

                Mock.Get(UserReadModel)
                    .Setup(x => x.GetNotServiceUser(USER_ID))
                    .Returns(ExpectedUser);
            };

            private It should_returns_firm_client_owner = () => Result.Should().Be(ExpectedUser);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingOwnerSelectionService))]
        private class When_creation_and_firm_client_owner_is_rejected : OrderCreationContext
        {
            private Establish context = () =>
            {
                const long FIRM_ID = 22222222222223;
                const long ORGANIZATION_UNIT_ID = 22222222222224;

                ExpectedUser = new User();

                OrderProcessingRequest.FirmId = FIRM_ID;

                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetFirmDto(FIRM_ID))
                    .Returns(new OrderProcessingRequestFirmDto
                    {
                        OrganizationUnitId = ORGANIZATION_UNIT_ID,
                        Client = new OrderProcessingRequestFirmDto.ClientDto()
                    });

                Mock.Get(UserReadModel)
                    .Setup(x => x.GetOrganizationUnitDirector(ORGANIZATION_UNIT_ID))
                    .Returns(ExpectedUser);
            };

            private It should_returns_organization_unit_director = () => Result.Should().Be(ExpectedUser);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingOwnerSelectionService))]
        private class When_creation_and_organization_unit_director_is_rejected : OrderCreationContext
        {
            private Establish context = () =>
            {
                const long FIRM_ID = 22222222222223;
                const long ORGANIZATION_UNIT_ID = 22222222222224;

                var reserveUserInfo = new UserInfo(27, "reserve", string.Empty);
                ExpectedUser = new User { Id = reserveUserInfo.Code, Account = reserveUserInfo.Account };

                OrderProcessingRequest.FirmId = FIRM_ID;

                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetFirmDto(FIRM_ID))
                    .Returns(new OrderProcessingRequestFirmDto
                    {
                        OrganizationUnitId = ORGANIZATION_UNIT_ID,
                        Client = new OrderProcessingRequestFirmDto.ClientDto()
                    });

                Mock.Get(SecurityServiceUserIdentifier)
                    .Setup(x => x.GetReserveUserIdentity())
                    .Returns(reserveUserInfo);

                Mock.Get(UserReadModel)
                    .Setup(x => x.GetUser(reserveUserInfo.Code))
                    .Returns(ExpectedUser);
            };

            private It should_returns_reserve_user = () => Result.Should().Be(ExpectedUser);
        }
    }
}