using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Advertisement
{
    public sealed class SelectAdvertisementToWhitelistIdentity : OperationIdentityBase<SelectAdvertisementToWhitelistIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SelectAdvertisementToWhitelistIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Выбрать РМ в белый список";
            }
        }
    }
}