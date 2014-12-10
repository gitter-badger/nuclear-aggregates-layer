using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting
{
    public class DefaultValidateBillsService : IValidateBillsService, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            // order state validation
            if (order.WorkflowStepId != OrderState.OnRegistration)
            {
                report = BLResources.CantEditBillsWhenOrderIsNotOnRegistration;
                return false;
            }

            // simple validation
            var createBillsPayablePlan = bills.OrderBy(x => x.PayablePlan).Sum(x => x.PayablePlan);
            if (createBillsPayablePlan != order.PayablePlan)
            {
                report = BLResources.BillsPayableSumNotEqualsToOrderPayable;
                return false;
            }

            // dates validation
            foreach (var bill in bills)
            {
                if (bill.PaymentDatePlan > bill.BeginDistributionDate)
                {
                    report = string.Format(BLResources.PaymentDatePlanMustBeLessThanBeginDistributionDateForPayment, bill.BillNumber);
                    return false;
                }

                if (bill.BeginDistributionDate > bill.EndDistributionDate)
                {
                    report = string.Format(BLResources.BeginDistributionDatePlanMustBeLessThanEndDistributionDateForPayment, bill.BillNumber);
                    return false;
                }

                var endOfPaymentDatePlan = bill.PaymentDatePlan.GetEndOfTheDay();
                var endOfCheckPeriod = bill.BeginDistributionDate.GetPrevMonthLastDate();
                if (order.SignupDate > bill.PaymentDatePlan && endOfPaymentDatePlan <= endOfCheckPeriod)
                {
                    report = BLResources.BillPaymentDatePlanMustBeInCorrectPeriod;
                    return false;
                }
            }

            report = null;
            return true;
        }
    }
}