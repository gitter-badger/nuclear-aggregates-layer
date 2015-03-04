using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Machine.Specifications;

using Moq;

using Nuclear.Tracing.API;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests
{
    [Tags("BL")]
    [Subject(typeof(ProcessOrderProlongationRequestMassOperation))]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's test.")]
    public class ProcessOrderProlongationRequestMassOperationSpecs
    {
        private class Context
        {
            protected static IOrderProcessingRequestService orderProcessingRequestService;
            protected static IBasicOrderProlongationOperationLogic basicOrderProlongationOperation;
            protected static ITracer tracer;

            protected static List<OrderProcessingRequest> activeOrderProcessingRequests;
            private static ProcessOrderProlongationRequestMassOperation target;

            private Establish context = () =>
            {
                orderProcessingRequestService = Mock.Of<IOrderProcessingRequestService>();
                basicOrderProlongationOperation = Mock.Of<IBasicOrderProlongationOperationLogic>();
                tracer = Mock.Of<ITracer>();

                activeOrderProcessingRequests = new List<OrderProcessingRequest>();

                Mock.Get(orderProcessingRequestService)
                    .Setup(x => x.GetProlongationRequestsToProcess())
                    .Returns(activeOrderProcessingRequests);

                target = new ProcessOrderProlongationRequestMassOperation(
                    orderProcessingRequestService,
                    basicOrderProlongationOperation,
                    tracer);
            };

            private Because of = () => target.ProcessAll();
        }

        private class When_there_is_no_active_OrderProcessingRequest : Context
        {
            private It should_no_process = () => 
                Mock.Get(basicOrderProlongationOperation)
                    .Verify(x => x.ExecuteRequest(Moq.It.IsAny<OrderProcessingRequest>()), Times.Never);
        }

        private class When_there_is_one_active_OrderProcessingRequest : Context
        {
            private static OrderProcessingRequest orderProcessingRequest;

            private Establish context = () =>
                {
                    orderProcessingRequest = new OrderProcessingRequest();

                    activeOrderProcessingRequests.Add(orderProcessingRequest);
                };

            private It should_processed = () => 
                Mock.Get(basicOrderProlongationOperation)
                    .Verify(x => x.ExecuteRequest(orderProcessingRequest), Times.Once);
        }

        private class When_there_are_two_active_OrderProcessingRequest : Context
        {
            private static OrderProcessingRequest orderProcessingRequest1;
            private static OrderProcessingRequest orderProcessingRequest2;

            private Establish context = () =>
            {
                orderProcessingRequest1 = new OrderProcessingRequest();
                orderProcessingRequest2 = new OrderProcessingRequest();

                activeOrderProcessingRequests.Add(orderProcessingRequest1);
                activeOrderProcessingRequests.Add(orderProcessingRequest2);
            };

            private It should_processed_first = () =>
                Mock.Get(basicOrderProlongationOperation)
                    .Verify(x => x.ExecuteRequest(orderProcessingRequest1), Times.Once);

            private It should_processed_second = () =>
                Mock.Get(basicOrderProlongationOperation)
                    .Verify(x => x.ExecuteRequest(orderProcessingRequest2), Times.Once);
        }

        private class When_process_throws_exception : When_there_is_one_active_OrderProcessingRequest
        {
            private static Exception exception;

            private Establish context = () =>
                {
                    exception = new Exception();

                    Mock.Get(basicOrderProlongationOperation)
                        .Setup(x => x.ExecuteRequest(Moq.It.IsAny<OrderProcessingRequest>()))
                        .Throws(exception);
                };

            private It should_log_exception = () =>
                                              Mock.Get(tracer)
                                                  .Verify(x => x.Fatal(exception, Moq.It.IsAny<string>()));
        }
    }
}
