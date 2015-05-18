using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity
{
    public class CheckRelatedActivitiesIdentity : OperationIdentityBase<CheckRelatedActivitiesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CheckRelatedActivitiesIdentity; }
        }

        public override string Description
        {
            get { return "CheckRelatedActivitiesIdentity"; }
        }
    }
}