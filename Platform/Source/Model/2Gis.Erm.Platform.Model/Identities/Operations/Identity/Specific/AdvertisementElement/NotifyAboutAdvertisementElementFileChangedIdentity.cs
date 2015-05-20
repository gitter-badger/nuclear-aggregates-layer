using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement
{
    public sealed class NotifyAboutAdvertisementElementFileChangedIdentity : OperationIdentityBase<NotifyAboutAdvertisementElementFileChangedIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.NotifyAboutAdvertisementElementFileChangedIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Уведомление об изменении файлового элемента рекламного материала";
            }
        }
    }
}