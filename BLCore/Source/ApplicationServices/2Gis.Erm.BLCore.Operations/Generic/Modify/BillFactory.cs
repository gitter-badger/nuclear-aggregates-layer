using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public sealed class BillFactory
    {
        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IEvaluateBillNumberService _evaluateBillNumberService;
        private readonly IEvaluateBillDateService _evaluateBillDateService;

        public BillFactory(IBusinessModelSettings businessModelSettings, IEvaluateBillNumberService evaluateBillNumberService, IEvaluateBillDateService evaluateBillDateService)
        {
            _businessModelSettings = businessModelSettings;
            _evaluateBillNumberService = evaluateBillNumberService;
            _evaluateBillDateService = evaluateBillDateService;
        }

        public IEnumerable<Bill> Create(Order order, CreateBillInfo[] dtos)
        {
            var bills = new List<Bill>();
            var billsPayablePlanSum = 0m;
            var billsVatPlanSum = 0m;
            var orderVatRatio = (order.PayablePlan != 0m) ? order.VatPlan / (order.PayablePlan - order.VatPlan) : 0m;

            if (dtos == null || dtos.Length == 0)
            {
                return bills;
            }

            var numberEvaluation = dtos.Length == 1
                ? new Func<CreateBillInfo, int, string>((dto, index) => _evaluateBillNumberService.Evaluate(dto.BillNumber, order.Number))
                : new Func<CreateBillInfo, int, string>((dto, index) => _evaluateBillNumberService.Evaluate(dto.BillNumber, order.Number, index + 1));

            // По предоставленным данным создаём все счета, кроме последнего
            // последний создаётся как корректирующий, чтобы сошлись суммы в заказе и в счетах на оплату.
            for (var i = 0; i < dtos.Length - 1; i++)
            {
                var billDto = dtos[i];
                var bill = CreateBill(billDto,
                                      order,
                                      Round(billDto.PayablePlan),
                                      Round((orderVatRatio * billDto.PayablePlan) / (1 + orderVatRatio)),
                                      numberEvaluation.Invoke(billDto, i));
                bill.BillDate = _evaluateBillDateService.GetBillDate(order.CreatedOn, bill.PaymentDatePlan, i);

                billsPayablePlanSum += bill.PayablePlan;
                billsVatPlanSum += bill.VatPlan;

                bills.Add(bill);
            }

            var lastBillDto = dtos[dtos.Length - 1];
            var lastBill = CreateBill(lastBillDto,
                                      order,
                                      order.PayablePlan - billsPayablePlanSum,
                                      order.VatPlan - billsVatPlanSum,
                                      numberEvaluation.Invoke(lastBillDto, dtos.Length - 1));
            lastBill.BillDate = _evaluateBillDateService.GetBillDate(order.CreatedOn, lastBill.PaymentDatePlan, dtos.Length - 1);

            bills.Add(lastBill);

            return bills;
        }

        private decimal Round(decimal value)
        {
            return Math.Round(value, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
        }

        private Bill CreateBill(CreateBillInfo createBillInfo, Order order, decimal payablePlan, decimal vatPlan, string billNumber)
        {
            return new Bill
                       {
                           OrderId = order.Id,
                           OwnerCode = order.OwnerCode,

                           BillNumber = billNumber,
                           BeginDistributionDate = createBillInfo.BeginDistributionDate,
                           EndDistributionDate = createBillInfo.EndDistributionDate,
                           PaymentDatePlan = createBillInfo.PaymentDatePlan,

                           PayablePlan = payablePlan,
                           VatPlan = vatPlan,

                           IsActive = true
                       };
        }
    }
}