using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement
{
    public sealed class ApproveAdvertisementElementIdentity : OperationIdentityBase<ApproveAdvertisementElementIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ApproveAdvertisementElementIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Одобрение ЭРМ";
            }
        }
    }
}