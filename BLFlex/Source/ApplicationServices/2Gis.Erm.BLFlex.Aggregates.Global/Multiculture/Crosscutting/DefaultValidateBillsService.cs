using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting
{
    public class DefaultValidateBillsService : IValidateBillsService, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        public DefaultValidateBillsService()
        {
        }

        public bool PreValidate(IEnumerable<Bill> bills, out string report)
        {
            report = null;
            return true;
        }

        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            if (order.WorkflowStepId != OrderState.OnRegistration)
            {
                report = BLCore.Resources.Server.Properties.BLResources.CantEditBillsWhenOrderIsNotOnRegistration;
                return false;
            }

            foreach (var bill in bills)
            {
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