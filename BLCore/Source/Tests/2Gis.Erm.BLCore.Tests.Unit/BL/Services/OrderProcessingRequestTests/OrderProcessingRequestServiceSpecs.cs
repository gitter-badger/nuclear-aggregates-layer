using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete.Simplified;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.OrderProcessingRequestTests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test!")]
    public static class OrderProcessingRequestServiceSpecs
    {
        [Tags("BL")]
        [Subject(typeof(OrderProcessingRequestService))]
        private class When_getting_active_order_processing_requests
        {
            private const long OPENED_REQUEST_ID = 10;
            private static IOrderProcessingRequestService SimplifiedModelConsumer;
            private static IEnumerable<OrderProcessingRequest> ResultRequests;

            Establish context = () =>
                {
                    var storage = new[]
                    {
                        CreateOrderProcessingRequest(OPENED_REQUEST_ID - 2, OrderProcessingRequestState.Cancelled, OrderProcessingRequestType.ProlongateOrder),
                        CreateOrderProcessingRequest(OPENED_REQUEST_ID - 1, OrderProcessingRequestState.Completed, OrderProcessingRequestType.ProlongateOrder),
                        CreateOrderProcessingRequest(OPENED_REQUEST_ID, OrderProcessingRequestState.Opened, OrderProcessingRequestType.ProlongateOrder),
                        CreateOrderProcessingRequest(OPENED_REQUEST_ID + 1, OrderProcessingRequestState.Pending, OrderProcessingRequestType.ProlongateOrder),
                        CreateOrderProcessingRequest(OPENED_REQUEST_ID + 2, OrderProcessingRequestState.Undefined, OrderProcessingRequestType.ProlongateOrder),

                        CreateOrderProcessingRequest(OPENED_REQUEST_ID + 3, OrderProcessingRequestState.Opened, OrderProcessingRequestType.CreateOrder),
                        CreateOrderProcessingRequest(OPENED_REQUEST_ID + 4, OrderProcessingRequestState.Opened, OrderProcessingRequestType.Undefined),

                        CreateNotActiveOpenedOrderProlongateRequest(OPENED_REQUEST_ID + 5),
                        CreateDeletedOpenedOrderProlongateRequest(OPENED_REQUEST_ID + 6)
                    };

                    var operationScopeFactory = Mock.Of<IOperationScopeFactory>();
                    var requestRepository = Mock.Of<IRepository<OrderProcessingRequest>>();
                    var requestMessageRepository = Mock.Of<IRepository<OrderProcessingRequestMessage>>();
                    var identityProvider = Mock.Of<IIdentityProvider>();
                    var finder = Mock.Of<IFinder>();

                    Mock.Get(finder)
                        .Setup(x => x.Find(Moq.It.IsAny<FindSpecification<OrderProcessingRequest>>()))
                        .Returns<FindSpecification<OrderProcessingRequest>>(x => storage.AsQueryable().Where(x).AsQueryable());

                    SimplifiedModelConsumer = new OrderProcessingRequestService(operationScopeFactory,
                                                                                requestRepository,
                                                                                requestMessageRepository,
                                                                                identityProvider,
                                                                                finder);
                };

            Because of = () =>
                ResultRequests = SimplifiedModelConsumer.GetProlongationRequestsToProcess();

            It should_return_only_opened_order_processing_requests = () =>
                ResultRequests.All(x => x.State == OrderProcessingRequestState.Opened).Should().BeTrue();
                
            private static OrderProcessingRequest CreateNotActiveOpenedOrderProlongateRequest(long id)
            {
                var result = CreateOrderProcessingRequest(id, OrderProcessingRequestState.Opened, OrderProcessingRequestType.ProlongateOrder);
                result.IsActive = false;
                return result;
            }

            private static OrderProcessingRequest CreateDeletedOpenedOrderProlongateRequest(long id)
            {
                var result = CreateOrderProcessingRequest(id, OrderProcessingRequestState.Opened, OrderProcessingRequestType.ProlongateOrder);
                result.IsDeleted = true;
                return result;
            }

            private static OrderProcessingRequest CreateOrderProcessingRequest(long id, OrderProcessingRequestState state, OrderProcessingRequestType type)
            {
                return new OrderProcessingRequest
                    {
                        Id = id,
                        State = state,
                        RequestType = type,
                        IsActive = true,
                        IsDeleted = false
                    };
            }
        }
    }
}
