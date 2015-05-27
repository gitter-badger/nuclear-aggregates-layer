using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement
{
    public class TransferAdvertisementElementToReadyForValidationIdentity : OperationIdentityBase<TransferAdvertisementElementToReadyForValidationIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.TransferAdvertisementElementToReadyForValidationIdentity; }
        }

        public override string Description
        {
            get { return "Перемещение в статус 'Готов к выверке'"; }
        }
    }
}