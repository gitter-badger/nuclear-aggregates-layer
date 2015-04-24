using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
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
    public class OrderProlongationRequestOperationServiceSpecs
    {
        private static LegalPersonProfile GetLegalPersonProfile(long id, long legalPersonId, bool isMainProfile)
        {
            return new LegalPersonProfile
            {
                Id = id,
                LegalPersonId = legalPersonId,
                IsMainProfile = isMainProfile,
                IsActive = true,
                IsDeleted = false
            };
        }

        private abstract class OrderProlongationRequestOperationServiceSpecsContext
        {
            private Establish context = () =>
                {
                    Mocks = new MockRepository(MockBehavior.Strict);

                    OrderProcessingRequestService = Mocks.Create<IOrderProcessingRequestService>();
                    OrderProcessingSettings = Mocks.Create<IOrderProcessingSettings>();
                    SecureFinder = new FakeSecureFinder();
                    UserReadModel = Mock.Of<IUserReadModel>();
                    SecurityServiceUserIdentifier = Mock.Of<ISecurityServiceUserIdentifier>();

                    Mock.Get(UserReadModel)
                        .Setup(x => x.GetNotServiceUser(Moq.It.IsAny<long>()))
                        .Returns<long>(id => Users.FirstOrDefault(x => x.Id == id));

                    var reserveUserInfo = new UserInfo(27, "reserve", string.Empty);
                    Mock.Get(UserReadModel)
                        .Setup(x => x.GetUser(reserveUserInfo.Code))
                        .Returns(new User { Id = reserveUserInfo.Code, Account = reserveUserInfo.Account });
                    Mock.Get(SecurityServiceUserIdentifier)
                        .Setup(x => x.GetReserveUserIdentity())
                        .Returns(reserveUserInfo);

                    Service = new CreateOrderProlongationRequestOperationService(
                        OrderProcessingSettings.Object,
                        UserReadModel,
                        OrderProcessingRequestService.Object,
                        SecurityServiceUserIdentifier,
                        Mock.Of<IOrderReadModel>(),
                        Mock.Of<ILegalPersonReadModel>());
                };

            protected static MockRepository Mocks { get; private set; }
            protected static Mock<IOrderProcessingRequestService> OrderProcessingRequestService { get; private set; }
            protected static FakeSecureFinder SecureFinder { get; private set; }
            protected static IUserReadModel UserReadModel;
            protected static ISecurityServiceUserIdentifier SecurityServiceUserIdentifier;
            protected static Mock<IOrderProcessingSettings> OrderProcessingSettings { get; private set; }
            protected static CreateOrderProlongationRequestOperationService Service { get; private set; }
            protected static IEnumerable<User> Users;
        }

        [Tags("BL")]
        [Subject(typeof(CreateOrderProlongationRequestOperationService))]
        private sealed class When_base_order_has_no_legal_person_profile : OrderProlongationRequestOperationServiceSpecsContext
        {
            const long MAIN_ID = 90000000000000;
            const long ORDER_ID = 1000000000000000;
            const long OWNER_CODE = 100000000000001;
            const long LEGAL_PERSON_ID = 100000000000002;
            const short RELEASE_COUNT_PLAN = 4;

            private Establish context = () =>
                {
                    var order = new Order
                        {
                            Id = ORDER_ID,
                            OwnerCode = OWNER_CODE,
                            LegalPersonId = LEGAL_PERSON_ID
                        };

                    var user = new User { Id = OWNER_CODE, IsActive = true };
                    var satelliteLegalPersonProfile = GetLegalPersonProfile(0, LEGAL_PERSON_ID, isMainProfile: false);
                    var mainLegalPersonProfile = GetLegalPersonProfile(MAIN_ID, LEGAL_PERSON_ID, isMainProfile: true);

                    Users = new[] { user };
                    SecureFinder.Storage.Add(order);
                    SecureFinder.Storage.Add(satelliteLegalPersonProfile);
                    SecureFinder.Storage.Add(mainLegalPersonProfile);
                    OrderProcessingSettings.Setup(x => x.OrderRequestProcessingHoursAmount).Returns(1);
                    OrderProcessingRequestService
                        .Setup(x => x.Create(
                            Moq.It.Is<OrderProcessingRequest>(o => o.LegalPersonProfileId == MAIN_ID)));
                };

            private Because of = () => 
                Service.CreateOrderProlongationRequest(ORDER_ID, RELEASE_COUNT_PLAN, null);

            private It should_uses_main_legal_person_profile = () => Mocks.VerifyAll();
        }

        [Tags("BL")]
        [Subject(typeof(CreateOrderProlongationRequestOperationService))]
        private sealed class When_base_order_has_no_legal_person_profile_and_no_main_legal_person_profile : OrderProlongationRequestOperationServiceSpecsContext
        {
            const long ORDER_ID = 1000000000000000;
            const long OWNER_CODE = 100000000000001;
            const long LEGAL_PERSON_ID = 100000000000002;
            const int RELEASE_COUNT_PLAN = 4;

            private static Exception exception;

            private Establish context = () =>
                {
                    var order = new Order
                        {
                            Id = ORDER_ID,
                            OwnerCode = OWNER_CODE,
                            LegalPersonId = LEGAL_PERSON_ID
                        };

                    var user = new User { Id = OWNER_CODE, IsActive = true };
                    var satelliteLegalPersonProfile = GetLegalPersonProfile(0, LEGAL_PERSON_ID, isMainProfile: false);

                    Users = new[] { user };
                    SecureFinder.Storage.Add(order);
                    SecureFinder.Storage.Add(satelliteLegalPersonProfile);
                };

            private Because of = () => exception = Catch.Exception(() => Service.CreateOrderProlongationRequest(ORDER_ID, RELEASE_COUNT_PLAN, null));

            private It should_throwns_business_logic_exception = () => exception.Should().BeOfType<BusinessLogicException>();

            private It should_mocks_verified_successfully = () => Mocks.VerifyAll();
        }
    }
}
