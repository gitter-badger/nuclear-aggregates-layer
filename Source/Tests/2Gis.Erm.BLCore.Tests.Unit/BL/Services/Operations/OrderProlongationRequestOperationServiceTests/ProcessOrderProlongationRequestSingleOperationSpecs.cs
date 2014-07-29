using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests
{
    [Tags("BL")]
    [Subject(typeof(ProcessOrderProlongationRequestSingleOperation))]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's test.")]
    public class ProcessOrderProlongationRequestSingleOperationSpecs
    {
        private class When_ProcessSingle_called
        {
            private const long ORDER_PROCESSING_REQUSET_ID = 1111111111111111;

            private static ProcessOrderProlongationRequestSingleOperation target;
            private static OrderProcessingResult result;
            private static OrderProcessingResult expectedResult;

            private static IBasicOrderProlongationOperationLogic basicOrderProlongationOperation;

            private static OrderProcessingRequest orderProcessingRequest;

            private Establish context = () =>
                {
                    var orderProcessingRequestService = Mock.Of<IOrderProcessingRequestService>();
                    basicOrderProlongationOperation = Mock.Of<IBasicOrderProlongationOperationLogic>();

                    orderProcessingRequest = new OrderProcessingRequest();
                    expectedResult = new OrderProcessingResult();

                    Mock.Get(orderProcessingRequestService)
                        .Setup(x => x.GetPrologationRequestToProcess(ORDER_PROCESSING_REQUSET_ID))
                        .Returns(orderProcessingRequest);

                    Mock.Get(basicOrderProlongationOperation)
                        .Setup(x => x.ExecuteRequest(orderProcessingRequest))
                        .Returns(expectedResult);

                    target = new ProcessOrderProlongationRequestSingleOperation(orderProcessingRequestService, basicOrderProlongationOperation);
                };

            private Because of = () => result = target.ProcessSingle(ORDER_PROCESSING_REQUSET_ID);

            private It should_be_process_OrderProcessingRequest = () =>
                                          Mock.Get(basicOrderProlongationOperation)
                                              .Verify(x => x.ExecuteRequest(orderProcessingRequest), Times.Once);

            private It should_returns_expectedResult = () => result.Should().Be(expectedResult);
        }
    }
}
