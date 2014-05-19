using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.WorkflowProcessing
{
    public sealed class ProcessOrderOnArchiveToApprovedHandler : RequestHandler<ProcessOrderOnArchiveToApprovedRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IDealRepository _dealRepository;

        public ProcessOrderOnArchiveToApprovedHandler(ISubRequestProcessor subRequestProcessor, IDealRepository dealRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _dealRepository = dealRepository;
        }

        protected override EmptyResponse Handle(ProcessOrderOnArchiveToApprovedRequest request)
        {
            var order = request.Order;

            if (order == null)
            {
                throw new ArgumentException("Order must be supplied");
            }

            //� ������ ���� ������� �������� "������". � ������ �������� ���� �� ������ ������� � ������.
            if (order.DealId.HasValue)
            {
                _dealRepository.SetOrderApprovedForReleaseStage(order.DealId.Value);
            }
            _subRequestProcessor.HandleSubRequest(new CalculateReleaseWithdrawalsRequest { Order = request.Order, UpdateAmountToWithdrawOnly = true}, Context);

            return Response.Empty;
        }
    }
}