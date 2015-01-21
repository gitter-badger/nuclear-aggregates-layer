using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public class ReportsServiceIdentity : OperationIdentityBase<ReportsServiceIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ReportsServiceIdentity; }
        }

        public override string Description
        {
            get { return "Reports"; }
        }
    }
}