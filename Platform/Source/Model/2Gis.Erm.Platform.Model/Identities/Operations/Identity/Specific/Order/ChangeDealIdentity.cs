using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class ChangeDealIdentity : OperationIdentityBase<ChangeDealIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeDealIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Сменить сделку";
            }
        }
    }
}