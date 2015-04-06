using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public sealed class LockedOrderConsistencyRule : IBillConsistencyRule
    {
        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            // order state validation
            if (order.WorkflowStepId != OrderState.OnRegistration)
            {
                report = BLResources.CantEditBillsWhenOrderIsNotOnRegistration;
                return false;
            }

            report = null;
            return true;
        }
    }
}