using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class GetInitPaymentsInfoHandler : RequestHandler<GetInitPaymentsInfoRequest, GetInitPaymentsInfoResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IOrderRepository _orderRepository;

        public GetInitPaymentsInfoHandler(ISubRequestProcessor subRequestProcessor, IOrderRepository orderRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _orderRepository = orderRepository;
        }

        protected override GetInitPaymentsInfoResponse Handle(GetInitPaymentsInfoRequest request)
        {
            if (!request.OrderId.HasValue)
            {
                throw new NotificationException(BLResources.CantGetOrderIdentifier);
            }

            var orderInfo = _orderRepository.GetOrderInfoForInitPayments(request.OrderId.Value);

            if (!orderInfo.IsOnRegistration)
            {
                throw new NotificationException(BLResources.CantEditBillsWhenOrderIsNotOnRegistration);
            }

            var paymentAmount = request.PaymentAmount ?? orderInfo.ReleaseCountPlan;
            var distibuteBillPaymentsResponse = (DistributeBillPaymentsResponse)
                                                _subRequestProcessor.HandleSubRequest(new DistributeBillPaymentsRequest 
                                                                                            {
                                                                                                PaymentType = request.PaymentType,
                                                                                                OrderPayablePlan = orderInfo.PayablePlan,
                                                                                                OrderReleaseCount = orderInfo.ReleaseCountPlan,
                                                                                                BeginDistributionDate = orderInfo.BeginDistributionDate,
                                                                                                EndDistributionDate = orderInfo.EndDistributionDate,
                                                                                                SignUpDate = orderInfo.SignupDate,
                                                                                                PaymentDatePlanEvaluator = request.PaymentDatePlanEvaluator,
                                                                                                PaymentAmount = paymentAmount,
                                                                                            },
                                                    Context);

            return new GetInitPaymentsInfoResponse
                       {
                           PaymentsInfo = new
                                              {
                                                  OrderSum = orderInfo.PayablePlan,
                                                  OrderReleaseCount = orderInfo.ReleaseCountPlan,
                                                  distibuteBillPaymentsResponse.Payments,
                                                  orderInfo.BillsCount
                                              }
                       };
        }
    }
}
