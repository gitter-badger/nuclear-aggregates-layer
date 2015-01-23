using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    public sealed class ProcessOrderOnArchiveToApprovedHandler : RequestHandler<ProcessOrderOnArchiveToApprovedRequest, EmptyResponse>
    {
        private readonly IActualizeOrderAmountToWithdrawOperationService _actualizeOrderAmountToWithdrawOperationService;
        private readonly IDealRepository _dealRepository;

        public ProcessOrderOnArchiveToApprovedHandler(
            IActualizeOrderAmountToWithdrawOperationService actualizeOrderAmountToWithdrawOperationService, 
            IDealRepository dealRepository)
        {
            _actualizeOrderAmountToWithdrawOperationService = actualizeOrderAmountToWithdrawOperationService;
            _dealRepository = dealRepository;
        }

        protected override EmptyResponse Handle(ProcessOrderOnArchiveToApprovedRequest request)
        {
            var order = request.Order;
            if (order == null)
            {
                throw new ArgumentException("Order must be supplied");
            }

            // У заказа есть связный документ "Сделка". В сделке изменить этап на «Заказ одобрен в выпуск».
            if (order.DealId.HasValue)
            {
                _dealRepository.SetOrderApprovedForReleaseStage(order.DealId.Value);
            }

            _actualizeOrderAmountToWithdrawOperationService.Actualize(order.Id);
            return Response.Empty;
        }
    }
}