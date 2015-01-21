using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement
{
    public sealed class ChangeAdvertisementElementStatusIdentity : OperationIdentityBase<ChangeAdvertisementElementStatusIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeAdvertisementElementStatus;
            }
        }

        public override string Description
        {
            get
            {
                return "Смена статуса ЭРМ";
            }
        }
    }
}