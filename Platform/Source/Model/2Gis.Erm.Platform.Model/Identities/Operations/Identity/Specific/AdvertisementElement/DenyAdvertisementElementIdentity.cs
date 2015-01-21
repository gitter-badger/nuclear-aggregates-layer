using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement
{
    public sealed class DenyAdvertisementElementIdentity : OperationIdentityBase<DenyAdvertisementElementIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.DenyAdvertisementElementIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Отклонение ЭРМ";
            }
        }
    }
}