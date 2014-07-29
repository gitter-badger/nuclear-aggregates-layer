using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    public sealed class ProcessOrderOnApprovalToApprovedHandler : RequestHandler<ProcessOrderOnApprovalToApprovedRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IDealRepository _dealRepository;

        public ProcessOrderOnApprovalToApprovedHandler(ISubRequestProcessor subRequestProcessor, IDealRepository dealRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _dealRepository = dealRepository;
        }

        protected override EmptyResponse Handle(ProcessOrderOnApprovalToApprovedRequest request)
        {
            var order = request.Order;
            order.ApprovalDate = DateTime.UtcNow;

            // Проверить есть ли сборка за период, который указан в заказе
            var response = (CheckOrderReleasePeriodResponse)_subRequestProcessor.HandleSubRequest(new CheckOrderReleasePeriodRequest
                {
                    OrderId = order.Id,
                    InProgressOnly = false,
                },
                Context);
            if (!response.Success)
            {
                throw new NotificationException(response.Message.MessageText);
            }

            // У заказа есть связный документ "Сделка". В сделке изменить этап на «Заказ одобрен в выпуск».
            if (order.DealId.HasValue)
            {
                _dealRepository.SetOrderApprovedForReleaseStage(order.DealId.Value);
            }

            return Response.Empty;
        }
    }
}