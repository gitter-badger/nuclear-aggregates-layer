using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class CloseWithDenialIdentity : OperationIdentityBase<CloseWithDenialIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CloseWithDenialIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Закрыть отказом";
            }
        }
    }
}