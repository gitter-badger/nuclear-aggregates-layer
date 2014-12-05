using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Bills;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting
{
    public class ChileValidateBillsService : IValidateBillsService, IChileAdapted
    {
        private const int BillNumberLength = 7;
        private readonly IFinder _finder;

        public ChileValidateBillsService(IFinder finder)
        {
            _finder = finder;
        }

        public bool PreValidate(IEnumerable<Bill> bills, out string report)
        {
            report = null;
            var billNumbers = bills.Select(x => x.BillNumber).ToArray();

            var duplicateBillNumbers = billNumbers.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
            if (duplicateBillNumbers.Any())
            {
                report = string.Format(BLResources.DublicateBillNumbers,
                                       string.Join(", ", duplicateBillNumbers));
                return false;
            }

            var wrongBillNumbers = billNumbers.Where(x => string.IsNullOrWhiteSpace(x) ||
                                                          x.Length != BillNumberLength ||
                                                          !x.All(c => c >= '0' && c <= '9'))
                                              .ToArray();
            if (wrongBillNumbers.Any())
            {
                report = string.Format(BLResources.WrongBillNumbers,
                                       string.Join(", ", wrongBillNumbers));
                return false;
            }

            return true;
        }

        public bool Validate(IEnumerable<Bill> bills, out string report)
        {
            report = null;
            var billNumbers = bills.Select(x => x.BillNumber).ToArray();
            var biilIds = bills.Where(x => !x.IsNew()).Select(x => x.Id).ToArray();

            var existingNumbers =
                _finder.Find(Specs.Find.ActiveAndNotDeleted<Bill>()
                             && BillSpecifications.Find.ByNumbers(billNumbers)
                             && !Specs.Find.ByIds<Bill>(biilIds))
                       .Select(x => x.BillNumber).ToArray();

            if (existingNumbers.Any())
            {
                report = string.Format(BLResources.ExistingBillNumbers, string.Join(", ", existingNumbers));
                return false;
            }

            foreach (var bill in bills)
            {
                var orderInfo = _finder.Find(Specs.Find.ById<Order>(bill.OrderId))
                                       .Select(x => new
                                                        {
                                                            IsOrderActive = x.WorkflowStepId == OrderState.OnRegistration,
                                                            SignupDate = x.SignupDate,
                                                        })
                                       .Single();

                if (!orderInfo.IsOrderActive)
                {
                    report = BLCore.Resources.Server.Properties.BLResources.CantEditBillsWhenOrderIsNotOnRegistration;
                    return false;
                }

                var endOfPaymentDatePlan = new DateTime(bill.PaymentDatePlan.Year, bill.PaymentDatePlan.Month, bill.PaymentDatePlan.Day)
                                    .AddDays(1)
                                    .AddSeconds(-1);

                var endOfCheckPeriod = bill.BeginDistributionDate.AddMonths(-1).GetEndPeriodOfThisMonth();
                if (orderInfo.SignupDate > bill.PaymentDatePlan && endOfPaymentDatePlan <= endOfCheckPeriod)
                {
                    report = BLCore.Resources.Server.Properties.BLResources.BillPaymentDatePlanMustBeInCorrectPeriod;
                    return false;
                }
            }

            return true;
        }
    }
}