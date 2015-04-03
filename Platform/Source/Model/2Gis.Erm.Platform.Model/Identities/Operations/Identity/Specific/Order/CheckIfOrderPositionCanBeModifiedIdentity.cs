using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class CheckIfOrderPositionCanBeModifiedIdentity : OperationIdentityBase<CheckIfOrderPositionCanBeModifiedIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CheckIfOrderPositionCanBeModifiedIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "CheckIfOrderPositionCanBeModified";
            }
        }
    }
}