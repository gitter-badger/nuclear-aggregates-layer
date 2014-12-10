﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Bills;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting
{
    public class ChileValidateBillsService : IValidateBillsService, IChileAdapted
    {
        private static readonly Regex ChileBillNumberPattern = new Regex(@"^\d{7}$", RegexOptions.Compiled);
        private readonly IFinder _finder;

        public ChileValidateBillsService(IFinder finder)
        {
            _finder = finder;
        }

        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            var billNumbers = bills.Select(x => x.BillNumber).ToArray();
            var billIds = bills.Where(x => !x.IsNew()).Select(x => x.Id).ToArray();

            var duplicateBillNumbers = billNumbers.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
            if (duplicateBillNumbers.Any())
            {
                report = string.Format(BLResources.DublicateBillNumbers, string.Join(", ", duplicateBillNumbers));
                return false;
            }

            var wrongBillNumbers = billNumbers.Where(x => ChileBillNumberPattern.IsMatch(x)).ToArray();
            if (wrongBillNumbers.Any())
            {
                report = string.Format(BLResources.WrongBillNumbers, string.Join(", ", wrongBillNumbers));
                return false;
            }

            // simple validation
            var createBillsPayablePlan = bills.OrderBy(x => x.PayablePlan).Sum(x => x.PayablePlan);
            if (createBillsPayablePlan != order.PayablePlan)
            {
                report = BLCore.Resources.Server.Properties.BLResources.BillsPayableSumNotEqualsToOrderPayable;
                return false;
            }

            // numbers validation
            var existingNumbers = _finder.Find(Specs.Find.ActiveAndNotDeleted<Bill>() && BillSpecifications.Find.ByNumbers(billNumbers) && !Specs.Find.ByIds<Bill>(billIds))
                                         .Select(x => x.BillNumber)
                                         .ToArray();

            if (existingNumbers.Any())
            {
                report = string.Format(BLResources.ExistingBillNumbers, string.Join(", ", existingNumbers));
                return false;
            }

            // dates validation
            foreach (var bill in bills)
            {
                if (bill.PaymentDatePlan > bill.BeginDistributionDate)
                {
                    report = string.Format(BLCore.Resources.Server.Properties.BLResources.PaymentDatePlanMustBeLessThanBeginDistributionDateForPayment, bill.BillNumber);
                    return false;
                }

                if (bill.BeginDistributionDate > bill.EndDistributionDate)
                {
                    report = string.Format(BLCore.Resources.Server.Properties.BLResources.BeginDistributionDatePlanMustBeLessThanEndDistributionDateForPayment, bill.BillNumber);
                    return false;
                }

                var endOfPaymentDatePlan = bill.PaymentDatePlan.GetEndOfTheDay();
                var endOfCheckPeriod = bill.BeginDistributionDate.GetPrevMonthLastDate();
                if (order.SignupDate > bill.PaymentDatePlan && endOfPaymentDatePlan <= endOfCheckPeriod)
                {
                    report = BLCore.Resources.Server.Properties.BLResources.BillPaymentDatePlanMustBeInCorrectPeriod;
                    return false;
                }
            }

            report = null; 
            return true;
        }
    }
}