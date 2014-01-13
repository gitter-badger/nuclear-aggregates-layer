using System;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class CreateBillsHandler : RequestHandler<CreateBillsRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IOrderRepository _orderRepository;

        public CreateBillsHandler(ISubRequestProcessor subRequestProcessor, IOrderRepository orderRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _orderRepository = orderRepository;
        }

        protected override EmptyResponse Handle(CreateBillsRequest request)
        {
            if (request.CreateBillInfos == null || request.CreateBillInfos.Length == 0)
            {
                return Response.Empty;
            }

            var orderInfo = _orderRepository.GetOrder(request.OrderId);

            // do not insert calculations in LINQ, this cannot keep high precision
            var orderVatRatio = (orderInfo.PayablePlan != 0m) ? orderInfo.VatPlan / (orderInfo.PayablePlan - orderInfo.VatPlan) : 0m;

            // simple validation
            var createBillsPayablePlan = request.CreateBillInfos.OrderBy(x => x.PayablePlan).Sum(x => x.PayablePlan);
            if (createBillsPayablePlan != orderInfo.PayablePlan)
            {
                throw new NotificationException(BLResources.BillsPayableSumNotEqualsToOrderPayable);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                // delete previous bills
                _subRequestProcessor.HandleSubRequest(new DeleteBillsRequest { OrderId = request.OrderId }, Context);

                if (request.CreateBillInfos.Length == 1)
                {
                    var bill = CreateBill(request.CreateBillInfos[0], request, orderInfo);

                    bill.BillDate = orderInfo.CreatedOn;
                    bill.BillNumber = string.Format(BLResources.BillNumberFormat, orderInfo.Number);

                    bill.PayablePlan = orderInfo.PayablePlan;
                    bill.VatPlan = orderInfo.VatPlan;

                    _orderRepository.CreateOrUpdate(bill);
                }
                else
                {
                    var billsPayablePlanSum = 0m;
                    var billsVatPlanSum = 0m;

                    // create all bills except last with regular PayablePlan
                    for (var i = 0; i < request.CreateBillInfos.Length - 1; i++)
                    {
                        var createBillInfo = request.CreateBillInfos[i];
                        var bill = CreateBill(createBillInfo, request, orderInfo);

                        bill.BillDate = orderInfo.CreatedOn;
                        bill.BillNumber = string.Format(BLResources.BillNumberFormat, orderInfo.Number + '/' + (i + 1));

                        bill.PayablePlan = Math.Round(createBillInfo.PayablePlan, 2, MidpointRounding.ToEven);
                        billsPayablePlanSum += bill.PayablePlan;

                        var payablePlanWithoutVat = createBillInfo.PayablePlan / (1 + orderVatRatio);
                        bill.VatPlan = Math.Round(createBillInfo.PayablePlan - payablePlanWithoutVat, 2, MidpointRounding.ToEven);
                        billsVatPlanSum += bill.VatPlan;

                        _orderRepository.CreateOrUpdate(bill);
                    }

                    // correct PayablePlan for last bill
                    var lastCreateBillInfo = request.CreateBillInfos[request.CreateBillInfos.Length - 1];
                    var lastBill = CreateBill(lastCreateBillInfo, request, orderInfo);

                    lastBill.BillDate = orderInfo.CreatedOn;
                    lastBill.BillNumber = string.Format(BLResources.BillNumberFormat, orderInfo.Number + '/' + request.CreateBillInfos.Length);

                    lastBill.PayablePlan = Math.Round(orderInfo.PayablePlan - billsPayablePlanSum, 2, MidpointRounding.ToEven);
                    lastBill.VatPlan = Math.Round(orderInfo.VatPlan - billsVatPlanSum, 2, MidpointRounding.ToEven);

                    _orderRepository.CreateOrUpdate(lastBill);
                }

                transaction.Complete();
            }

            return Response.Empty;
        }

        private static Bill CreateBill(CreateBillInfo createBillInfo, CreateBillsRequest request, Order order)
        {
            // simple validations
            if (createBillInfo.PaymentDatePlan > createBillInfo.BeginDistributionDate)
            {
                throw new NotificationException(
                    string.Format(BLResources.PaymentDatePlanMustBeLessThanBeginDistributionDateForPayment, createBillInfo.PaymentNumber));
            }

            if (createBillInfo.BeginDistributionDate > createBillInfo.EndDistributionDate)
            {
                throw new NotificationException(
                    string.Format(BLResources.BeginDistributionDatePlanMustBeLessThanEndDistributionDateForPayment, createBillInfo.PaymentNumber));
            }

            return new Bill
            {
                OrderId = request.OrderId,
                OwnerCode = order.OwnerCode,

                BeginDistributionDate = createBillInfo.BeginDistributionDate,
                EndDistributionDate = createBillInfo.EndDistributionDate,
                PaymentDatePlan = createBillInfo.PaymentDatePlan,

                IsActive = true
            };
        }
    }
}
