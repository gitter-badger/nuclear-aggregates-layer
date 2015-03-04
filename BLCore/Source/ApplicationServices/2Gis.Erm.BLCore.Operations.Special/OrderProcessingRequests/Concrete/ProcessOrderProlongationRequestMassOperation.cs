using System;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    public sealed class ProcessOrderProlongationRequestMassOperation : IProcessOrderProlongationRequestMassOperation
    {
        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly IBasicOrderProlongationOperationLogic _basicOrderProlongationOperation;
        private readonly ITracer _tracer;

        public ProcessOrderProlongationRequestMassOperation(
            IOrderProcessingRequestService orderProcessingRequestService,
            IBasicOrderProlongationOperationLogic basicOrderProlongationOperation,
            ITracer tracer)
        {
            _orderProcessingRequestService = orderProcessingRequestService;
            _basicOrderProlongationOperation = basicOrderProlongationOperation;
            _tracer = tracer;
        }

        public void ProcessAll()
        {
            foreach (var request in _orderProcessingRequestService.GetProlongationRequestsToProcess())
            {
                try
                {
                    _basicOrderProlongationOperation.ExecuteRequest(request);
                }
                catch (BusinessLogicException ex)
                {
                    _tracer.Error(ex, "Cant'n prolongate order by request Id = " + request.Id);
                }
                catch (Exception ex)
                {
                    _tracer.Fatal(ex, "Cant'n prolongate order by request Id = " + request.Id);
                }
            }
        }
    }
}
