﻿namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement
{
    public sealed class NotifyAboutAdvertisementElementValidationStatusChangedIdentity : OperationIdentityBase<NotifyAboutAdvertisementElementValidationStatusChangedIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.NotifyAboutAdvertisementElementValidationStatusChangedIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Уведомление об изменении статуса выверки элемента рекламного материала (например, отклонении)";
            }
        }
    }
}