using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPersonProfile
{
    public sealed class SetAsMainLegalPersonProfileIdentity : OperationIdentityBase<SetAsMainLegalPersonProfileIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SetAsMainLegalPersonProfileIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Сделать основной";
            }
        }
    }
}