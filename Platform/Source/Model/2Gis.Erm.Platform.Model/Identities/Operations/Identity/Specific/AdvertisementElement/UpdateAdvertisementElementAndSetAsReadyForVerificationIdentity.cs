using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement
{
    public sealed class UpdateAdvertisementElementAndSetAsReadyForVerificationIdentity :
        OperationIdentityBase<UpdateAdvertisementElementAndSetAsReadyForVerificationIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.UpdateAdvertisementElementAndSetAsReadyForVerificationIdentity; }
        }

        public override string Description
        {
            get { return "Редактируем ЭРМ и выставляем статус 'Готоов к верификации'"; }
        }
    }
}