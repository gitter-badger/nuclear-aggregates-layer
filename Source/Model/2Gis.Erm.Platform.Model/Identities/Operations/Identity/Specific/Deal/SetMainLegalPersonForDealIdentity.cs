using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal
{
    [DataContract]
    public sealed class SetMainLegalPersonForDealIdentity : OperationIdentityBase<SetMainLegalPersonForDealIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.SetMainLegalPersonForDealIdentity; }
        }

        public override string Description
        {
            get { return "SetLegalPersonAsMainForDeal"; }
        }
    }
}