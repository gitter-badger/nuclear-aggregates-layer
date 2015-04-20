using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement
{
    public sealed class NotifyAboutAdvertisementElementRejectionIdentity : OperationIdentityBase<NotifyAboutAdvertisementElementRejectionIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.NotifyAboutAdvertisementElementRejectionIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Уведомление об отклонении элемента рекламного материала";
            }
        }
    }
}