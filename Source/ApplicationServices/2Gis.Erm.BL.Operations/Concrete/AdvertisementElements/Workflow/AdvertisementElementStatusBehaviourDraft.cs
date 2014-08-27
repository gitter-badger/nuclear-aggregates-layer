using System.Collections.Generic;
using DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements.Workflow;

namespace DoubleGis.Erm.BL.Operations.Concrete.AdvertisementElements.Workflow
{
    public class AdvertisementElementStatusBehaviourDraft : AdvertisementElementStatusBehaviour
    {
        private readonly ITransferAdvertisementElementToReadyForValidationOpeationService _transferAdvertisementElementToReadyForValidationOpeationService;
        
        public AdvertisementElementStatusBehaviourDraft(
            ITransferAdvertisementElementToReadyForValidationOpeationService transferAdvertisementElementToReadyForValidationOpeationService)
        {
            _transferAdvertisementElementToReadyForValidationOpeationService = transferAdvertisementElementToReadyForValidationOpeationService;
        }

        protected override IEnumerable<IAdvertisementElementStatusChangingStrategy> ChangeToReadyForValidation()
        {
            return new[] { _transferAdvertisementElementToReadyForValidationOpeationService };
        }

        protected override IEnumerable<IAdvertisementElementStatusChangingStrategy> ChangeToDraft()
        {
            return new[] { NullAdvertisementElementStatusChangingStrategy.Instance };
        }
    }
}