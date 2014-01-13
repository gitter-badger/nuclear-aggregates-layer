using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // 2+ \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations.Special\OrderProcessingRequest
    public sealed class ProcessOrderProlongationRequestSingleOperation : IProcessOrderProlongationRequestSingleOperation
    {
        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly IBasicOrderProlongationOperationLogic _basicOrderProlongationOperation;

        public ProcessOrderProlongationRequestSingleOperation(
            IOrderProcessingRequestService orderProcessingRequestService,
            IBasicOrderProlongationOperationLogic basicOrderProlongationOperation)
        {
            _orderProcessingRequestService = orderProcessingRequestService;
            _basicOrderProlongationOperation = basicOrderProlongationOperation;
        }

        public OrderProcessingResult ProcessSingle(long orderProcessingRequestId)
        {
            var request = _orderProcessingRequestService.GetPrologationRequestToProcess(orderProcessingRequestId);
            return _basicOrderProlongationOperation.ExecuteRequest(request);
        }
    }
}
