using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public sealed class BillSummConsistencyRule : IBillConsistencyRule
    {
        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            // simple validation
            var createBillsPayablePlan = bills.OrderBy(x => x.PayablePlan).Sum(x => x.PayablePlan);
            if (bills.Any() && createBillsPayablePlan != order.PayablePlan)
            {
                report = BLCore.Resources.Server.Properties.BLResources.BillsPayableSumNotEqualsToOrderPayable;
                return false;
            }

            report = null;
            return true;
        }
    }
}