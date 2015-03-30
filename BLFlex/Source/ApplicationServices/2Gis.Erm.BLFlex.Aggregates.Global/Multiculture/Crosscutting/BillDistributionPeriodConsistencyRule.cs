using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public sealed class BillDistributionPeriodConsistencyRule : IBillConsistencyRule
    {
        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            foreach (var bill in bills)
            {
                if (bill.BeginDistributionDate > bill.EndDistributionDate)
                {
                    report = string.Format(BLCore.Resources.Server.Properties.BLResources.BeginDistributionDatePlanMustBeLessThanEndDistributionDateForPayment, bill.BillNumber);
                    return false;
                }
            }

            report = null;
            return true;
        }
    }
}