using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

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
                    AppSettings = Mocks.Create<IAppSettings>();
                    SecureFinder = new FakeSecureFinder();
                    OwnerSelectionService = Mock.Of<IOrderProcessingRequestOwnerSelectionService>();

                    Mock.Get(OwnerSelectionService)
                        .Setup(x => x.GetOwner(Moq.It.IsAny<long>()))
                        .Returns<long>(id => Users.FirstOrDefault(x => x.Id == id));

                    Service = new CreateOrderProlongationRequestOperationService(
                        AppSettings.Object,
                        SecureFinder,
                        OrderProcessingRequestService.Object,
                        OwnerSelectionService);
                };

            protected static MockRepository Mocks { get; private set; }
            protected static Mock<IOrderProcessingRequestService> OrderProcessingRequestService { get; private set; }
            protected static FakeSecureFinder SecureFinder { get; private set; }
            protected static IOrderProcessingRequestOwnerSelectionService OwnerSelectionService { get; private set; }
            protected static Mock<IAppSettings> AppSettings { get; private set; }
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
                    AppSettings.Setup(x => x.OrderRequestProcessingHoursAmount).Returns(1);
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

            private It should_throwns_business_logic_exception = () => exception.ShouldBeOfType<BusinessLogicException>();

            private It should_mocks_verified_successfully = () => Mocks.VerifyAll();
        }
    }
}
