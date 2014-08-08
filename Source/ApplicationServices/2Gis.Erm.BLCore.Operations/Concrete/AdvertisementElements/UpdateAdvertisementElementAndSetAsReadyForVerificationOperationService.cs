using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Update;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements
{
    public class UpdateAdvertisementElementAndSetAsReadyForVerificationOperationService :
        IUpdateAdvertisementElementAndSetAsReadyForVerificationOperationService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly IUpdateOperationService<AdvertisementElement> _updateAdvertisementElementOperationService;
        private readonly IChangeAdvertisementElementStatusOperationService _changeAdvertisementElementStatusOperationService;

        public UpdateAdvertisementElementAndSetAsReadyForVerificationOperationService(
            IOperationScopeFactory operationScopeFactory,
            IAdvertisementReadModel advertisementReadModel,
            IUpdateOperationService<AdvertisementElement> updateAdvertisementElementOperationService,
            IChangeAdvertisementElementStatusOperationService changeAdvertisementElementStatusOperationService)
        {
            _operationScopeFactory = operationScopeFactory;
            _advertisementReadModel = advertisementReadModel;
            _updateAdvertisementElementOperationService = updateAdvertisementElementOperationService;
            _changeAdvertisementElementStatusOperationService = changeAdvertisementElementStatusOperationService;
        }

        public void UpdateAndSetAsReadyForVerification(AdvertisementElementDomainEntityDto advertisementElementDomainEntityDto)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<UpdateAdvertisementElementAndSetAsReadyForVerificationIdentity>())
            {
                var validationState = _advertisementReadModel.GetAdvertisementElementValidationState(advertisementElementDomainEntityDto.Id);
                if (validationState.NeedsValidation)
                {
                    if ((AdvertisementElementStatusValue)validationState.CurrentStatus.Status != AdvertisementElementStatusValue.Draft)
                    {
                        throw new EditingNotDraftAdvertisementElementException(string.Format(BLResources.NonDraftAdvertisementElementEditing,
                                                                                             advertisementElementDomainEntityDto.Id));
                    }

                    _changeAdvertisementElementStatusOperationService.ChangeStatus(advertisementElementDomainEntityDto.Id,
                                                                                   AdvertisementElementStatusValue.ReadyForValidation,
                                                                                   null);
                }

                _updateAdvertisementElementOperationService.Update(advertisementElementDomainEntityDto);

                scope.Updated<AdvertisementElement>(advertisementElementDomainEntityDto.Id);
                scope.Complete();
            }
        }
    }
}