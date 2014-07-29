﻿using System;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    public sealed class ProcessOrderProlongationRequestMassOperation : IProcessOrderProlongationRequestMassOperation
    {
        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly IBasicOrderProlongationOperationLogic _basicOrderProlongationOperation;
        private readonly ICommonLog _logger;

        public ProcessOrderProlongationRequestMassOperation(
            IOrderProcessingRequestService orderProcessingRequestService,
            IBasicOrderProlongationOperationLogic basicOrderProlongationOperation,
            ICommonLog logger)
        {
            _orderProcessingRequestService = orderProcessingRequestService;
            _basicOrderProlongationOperation = basicOrderProlongationOperation;
            _logger = logger;
        }

        public void ProcessAll()
        {
            foreach (var request in _orderProcessingRequestService.GetPrologationRequestsToProcess())
            {
                try
                {
                    _basicOrderProlongationOperation.ExecuteRequest(request);
                }
                catch (BusinessLogicException ex)
                {
                    _logger.ErrorEx(ex, "Cant'n prolongate order by request Id = " + request.Id);
                }
                catch (Exception ex)
                {
                    _logger.FatalEx(ex, "Cant'n prolongate order by request Id = " + request.Id);
                }
            }
        }
    }
}
