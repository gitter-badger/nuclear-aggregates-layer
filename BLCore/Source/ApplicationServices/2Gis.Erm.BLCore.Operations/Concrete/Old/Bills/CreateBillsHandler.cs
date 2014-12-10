using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bills;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class CreateBillsHandler : RequestHandler<CreateBillsRequest, EmptyResponse>
    {
        private readonly IDeleteOrderBillsOperationService _deleteOrderBillsOperationService; 
        private readonly ICreateBillsAggregateService _createAggregateService;
        private readonly IOrderReadModel _orderReadModel;

        private readonly IEvaluateBillNumberService _evaluateBillNumberService;
        private readonly IValidateBillsService _validateBillsService;
        private readonly IBusinessModelSettings _businessModelSettings;
        
        public CreateBillsHandler(
            IBusinessModelSettings businessModelSettings,
            IDeleteOrderBillsOperationService deleteOrderBillsOperationService,
            ICreateBillsAggregateService createAggregateService,
            IOrderReadModel orderReadModel,
            IEvaluateBillNumberService evaluateBillNumberService,
            IValidateBillsService validateBillsService)
        {
            _deleteOrderBillsOperationService = deleteOrderBillsOperationService;
            _createAggregateService = createAggregateService;
            _orderReadModel = orderReadModel;
            _evaluateBillNumberService = evaluateBillNumberService;
            _validateBillsService = validateBillsService;
            _businessModelSettings = businessModelSettings;
        }

        protected override EmptyResponse Handle(CreateBillsRequest request)
        {
            if (request.CreateBillInfos == null || request.CreateBillInfos.Length == 0)
            {
                return Response.Empty;
            }

            var orderInfo = _orderReadModel.GetOrderSecure(request.OrderId);

            // do not insert calculations in LINQ, this cannot keep high precision
            var orderVatRatio = (orderInfo.PayablePlan != 0m) ? orderInfo.VatPlan / (orderInfo.PayablePlan - orderInfo.VatPlan) : 0m;

            var billsToCreate = new List<Bill>();
            var billsPayablePlanSum = 0m;
            var billsVatPlanSum = 0m;

            // create all bills except last with regular PayablePlan
            for (var i = 0; i < request.CreateBillInfos.Length - 1; i++)
            {
                var createBillInfo = request.CreateBillInfos[i];
                var bill = CreateBill(createBillInfo, request, orderInfo);

                bill.PayablePlan = Math.Round(createBillInfo.PayablePlan, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
                billsPayablePlanSum += bill.PayablePlan;

                var payablePlanWithoutVat = createBillInfo.PayablePlan / (1 + orderVatRatio);
                bill.VatPlan = Math.Round(createBillInfo.PayablePlan - payablePlanWithoutVat, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
                billsVatPlanSum += bill.VatPlan;

                billsToCreate.Add(bill);
            }

            // correct PayablePlan for last bill
            var lastCreateBillInfo = request.CreateBillInfos[request.CreateBillInfos.Length - 1];
            var lastBill = CreateBill(lastCreateBillInfo, request, orderInfo);

            lastBill.PayablePlan = orderInfo.PayablePlan - billsPayablePlanSum;
            lastBill.VatPlan = orderInfo.VatPlan - billsVatPlanSum;

            billsToCreate.Add(lastBill);

            if (billsToCreate.Count == 1)
            {
                // FIXME {all, 29.01.2014}: при рефакторинге ApplicationService нужно перенести использование evaluateBillNumberService в запиливаемый CreateBillAggregateService
                billsToCreate.Single().BillNumber = _evaluateBillNumberService.Evaluate(request.CreateBillInfos.Single().BillNumber, orderInfo.Number);
            }
            else
            {
                for (var i = 0; i < billsToCreate.Count; i++)
                {
                    // FIXME {all, 29.01.2014}: при рефакторинге ApplicationService нужно перенести использование evaluateBillNumberService в запиливаемый CreateBillAggregateService
                    billsToCreate[i].BillNumber = _evaluateBillNumberService.Evaluate(request.CreateBillInfos[i].BillNumber, orderInfo.Number, i + 1);
                }
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
                _deleteOrderBillsOperationService.Delete(request.OrderId);

                string report;
                if (!_validateBillsService.Validate(billsToCreate, orderInfo, out report))
                {
                    throw new NotificationException(report);
                }

                _createAggregateService.Create(billsToCreate);

                transaction.Complete();
            }

            return Response.Empty;
        }

        private static Bill CreateBill(CreateBillInfo createBillInfo, CreateBillsRequest request, Order order)
        {
            return new Bill
            {
                OrderId = request.OrderId,
                OwnerCode = order.OwnerCode,
                BillDate = order.CreatedOn,

                BeginDistributionDate = createBillInfo.BeginDistributionDate,
                EndDistributionDate = createBillInfo.EndDistributionDate,
                PaymentDatePlan = createBillInfo.PaymentDatePlan,

                IsActive = true
            };
        }
    }
}
