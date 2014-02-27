using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class CreateBillsHandler : RequestHandler<CreateBillsRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;

        private readonly IEvaluateBillNumberService _evaluateBillNumberService;
        private readonly IValidateBillsService _validateBillsService;
        private readonly IAppSettings _appSettings;

        public CreateBillsHandler(ISubRequestProcessor subRequestProcessor,
            IOrderRepository orderRepository,
            IOrderReadModel orderReadModel,
            IEvaluateBillNumberService evaluateBillNumberService,
            IValidateBillsService validateBillsService,
            IAppSettings appSettings)
        {
            _subRequestProcessor = subRequestProcessor;
            _orderRepository = orderRepository;
            _orderReadModel = orderReadModel;
            _evaluateBillNumberService = evaluateBillNumberService;
            _validateBillsService = validateBillsService;
            _appSettings = appSettings;
        }

        protected override EmptyResponse Handle(CreateBillsRequest request)
        {
            if (request.CreateBillInfos == null || request.CreateBillInfos.Length == 0)
            {
                return Response.Empty;
            }

            var orderInfo = _orderReadModel.GetOrder(request.OrderId);

            // do not insert calculations in LINQ, this cannot keep high precision
            var orderVatRatio = (orderInfo.PayablePlan != 0m) ? orderInfo.VatPlan / (orderInfo.PayablePlan - orderInfo.VatPlan) : 0m;

            // simple validation
            var createBillsPayablePlan = request.CreateBillInfos.OrderBy(x => x.PayablePlan).Sum(x => x.PayablePlan);
            if (createBillsPayablePlan != orderInfo.PayablePlan)
            {
                throw new NotificationException(BLResources.BillsPayableSumNotEqualsToOrderPayable);
            }

            var billsToCreate = new List<Bill>();

                if (request.CreateBillInfos.Length == 1)
                {
                var createBillInfo = request.CreateBillInfos[0];
                var bill = CreateBill(createBillInfo, request, orderInfo);

                    bill.BillDate = orderInfo.CreatedOn;
                    // FIXME {all, 29.01.2014}: при рефакторинге ApplicationService нужно перенести использование evaluateBillNumberService в запиливаемый CreateBillAggregateService
                bill.BillNumber = _evaluateBillNumberService.Evaluate(createBillInfo.BillNumber, orderInfo.Number);

                    bill.PayablePlan = orderInfo.PayablePlan;
                    bill.VatPlan = orderInfo.VatPlan;

                billsToCreate.Add(bill);
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
                        // FIXME {all, 29.01.2014}: при рефакторинге ApplicationService нужно перенести использование evaluateBillNumberService в запиливаемый CreateBillAggregateService
                    bill.BillNumber = _evaluateBillNumberService.Evaluate(createBillInfo.BillNumber, orderInfo.Number, i + 1);

                    bill.PayablePlan = Math.Round(createBillInfo.PayablePlan, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
                        billsPayablePlanSum += bill.PayablePlan;

                        var payablePlanWithoutVat = createBillInfo.PayablePlan / (1 + orderVatRatio);
                    bill.VatPlan = Math.Round(createBillInfo.PayablePlan - payablePlanWithoutVat, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
                        billsVatPlanSum += bill.VatPlan;

                    billsToCreate.Add(bill);
                    }

                    // correct PayablePlan for last bill
                    var lastCreateBillInfo = request.CreateBillInfos[request.CreateBillInfos.Length - 1];
                    var lastBill = CreateBill(lastCreateBillInfo, request, orderInfo);

                    lastBill.BillDate = orderInfo.CreatedOn;
                    // FIXME {all, 29.01.2014}: при рефакторинге ApplicationService нужно перенести использование evaluateBillNumberService в запиливаемый CreateBillAggregateService
                lastBill.BillNumber = _evaluateBillNumberService.Evaluate(lastCreateBillInfo.BillNumber, orderInfo.Number, request.CreateBillInfos.Length);

                lastBill.PayablePlan = Math.Round(orderInfo.PayablePlan - billsPayablePlanSum, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
                lastBill.VatPlan = Math.Round(orderInfo.VatPlan - billsVatPlanSum, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);

                billsToCreate.Add(lastBill);
            }

            {
                string report;
                if (!_validateBillsService.PreValidate(billsToCreate, out report))
                {
                    throw new NotificationException(report);
                }
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                // delete previous bills
                _subRequestProcessor.HandleSubRequest(new DeleteBillsRequest { OrderId = request.OrderId }, Context);

                string report;
                if (!_validateBillsService.Validate(billsToCreate, out report))
                {
                    throw new NotificationException(report);
                }

                foreach (var bill in billsToCreate)
                {
                    _orderRepository.CreateOrUpdate(bill);
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
