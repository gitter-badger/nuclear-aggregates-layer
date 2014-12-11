using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public sealed class BillDatesInvariant : IBillInvariant
    {
        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            foreach (var bill in bills)
            {
                if (bill.PaymentDatePlan > bill.BeginDistributionDate)
                {
                    report = String.Format(BLCore.Resources.Server.Properties.BLResources.PaymentDatePlanMustBeLessThanBeginDistributionDateForPayment, bill.BillNumber);
                    return false;
                }

                if (bill.BeginDistributionDate > bill.EndDistributionDate)
                {
                    report = String.Format(BLCore.Resources.Server.Properties.BLResources.BeginDistributionDatePlanMustBeLessThanEndDistributionDateForPayment, bill.BillNumber);
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