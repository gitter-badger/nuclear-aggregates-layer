using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.OrderProcessingRequestTests
{
    public class CancelOrderProcessingRequestOperationServiceSpecs
    {
        private abstract class MockContext
        {
            protected static CancelOrderProcessingRequestOperationService CancelOrderProcessingRequestOperationService { get; set; }

            protected static IFinder Finder { get; set; }
            protected static IOrderProcessingRequestService OrderProcessingRequestService { get; set; }
            protected static IOperationScopeFactory ScopeFactory { get; set; }
            protected static OrderProcessingRequest OrderProcessingRequest { get; set; }
            protected static Mock<IOperationScopeFactory> ScopeFactoryMock { get; set; }
            protected static Mock<IOrderProcessingRequestService> OrderProcessingRequestServiceMock { get; set; }
            protected static Mock<IOperationScope> OperationScopeMock { get; set; }

            private Establish context = () =>
                {
                    OrderProcessingRequest = GetOrderProcessingRequest();
                    var finderMock = new Mock<IFinder>();
                    finderMock.Setup(x => x.Find(Moq.It.IsAny<FindSpecification<OrderProcessingRequest>>()))
                              .Returns(new QueryableSequence<OrderProcessingRequest>(new[] { OrderProcessingRequest }.AsQueryable()));

                    Finder = finderMock.Object;
                    OrderProcessingRequestServiceMock = new Mock<IOrderProcessingRequestService>();
                    OrderProcessingRequestService = OrderProcessingRequestServiceMock.Object;
                    OperationScopeMock = new Mock<IOperationScope>();
                    ScopeFactoryMock = new Mock<IOperationScopeFactory>();
                    ScopeFactoryMock.Setup(x => x.CreateNonCoupled<CancelOrderProcessingRequestIdentity>()).Verifiable();
                    ScopeFactoryMock.Setup(x => x.CreateNonCoupled<CancelOrderProcessingRequestIdentity>()).Returns(OperationScopeMock.Object);

                    ScopeFactory = ScopeFactoryMock.Object;
                    CancelOrderProcessingRequestOperationService = new CancelOrderProcessingRequestOperationService(OrderProcessingRequestService,
                                                                                                                    Finder,
                                                                                                                    ScopeFactory);
                };

            protected static OrderProcessingRequest GetOrderProcessingRequest()
            {
                return new OrderProcessingRequest
                    {
                        Id = 1,
                        IsActive = true,
                        IsDeleted = false,
                    };
            }
        }

        [Subject(typeof(CancelOrderProcessingRequestOperationService))]
        private class When_request_is_canceled : MockContext
        {
            private static Exception exception;

            private Establish context = () =>
                {
                    OrderProcessingRequest.State = OrderProcessingRequestState.Cancelled;
                };

            private Because of = () => exception = Catch.Exception(() => CancelOrderProcessingRequestOperationService.CancelRequest(OrderProcessingRequest.Id));

            private It bussinessLogicException_should_be_thrown = () => exception.Should().BeOfType<BusinessLogicException>();
        }

        [Subject(typeof(CancelOrderProcessingRequestOperationService))]
        private class When_request_is_completed : MockContext
        {
            private static Exception exception;

            private Establish context = () =>
                {
                    OrderProcessingRequest.State = OrderProcessingRequestState.Completed;
                };

            private Because of = () => exception = Catch.Exception(() => CancelOrderProcessingRequestOperationService.CancelRequest(OrderProcessingRequest.Id));

            private It bussinessLogicException_should_be_thrown = () => exception.Should().BeOfType<BusinessLogicException>();
        }

        [Subject(typeof(CancelOrderProcessingRequestOperationService))]
        private class When_request_state_is_ok : MockContext
        {
            private static Exception exception;

            private Establish context = () =>
                {
                    OrderProcessingRequest.State = OrderProcessingRequestState.Opened;
                };

            private Because of = () => CancelOrderProcessingRequestOperationService.CancelRequest(OrderProcessingRequest.Id);

            private It operation_scope_should_be_created =
                () => ScopeFactoryMock.Verify(x => x.CreateNonCoupled<CancelOrderProcessingRequestIdentity>(), Times.Once);

            private It request_state_should_become_cancelled = () => OrderProcessingRequest.State.Should().Be(OrderProcessingRequestState.Cancelled);

            private It save_request_method_should_be_called =
                () => OrderProcessingRequestServiceMock.Verify(x => x.Update(OrderProcessingRequest), Times.Once);

            private It message_should_be_saved =
                () =>
                OrderProcessingRequestServiceMock.Verify(
                    x => x.SaveMessagesForOrderProcessingRequest(OrderProcessingRequest.Id, Moq.It.IsAny<IMessageWithType[]>()),
                    Times.Once);

            private It operation_scope_should_be_completed =
                () => OperationScopeMock.Verify(x => x.Complete(), Times.Once);
        }
    }
}
