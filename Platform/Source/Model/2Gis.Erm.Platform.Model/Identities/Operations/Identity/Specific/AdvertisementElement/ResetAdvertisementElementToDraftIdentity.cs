using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement
{
    public class ResetAdvertisementElementToDraftIdentity : OperationIdentityBase<ResetAdvertisementElementToDraftIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ResetAdvertisementElementToDraftIdentity; }
        }

        public override string Description
        {
            get { return "Возврат в черновик"; }
        }
    }
}