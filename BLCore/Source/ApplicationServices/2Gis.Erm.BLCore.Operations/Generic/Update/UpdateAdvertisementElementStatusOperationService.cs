using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Update;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Update
{
    public class UpdateAdvertisementElementStatusOperationService : IUpdateOperationService<AdvertisementElementStatus>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IGetDomainEntityDtoService<AdvertisementElement> _getAdvertisementElementDomainEntityDtoService;
        private readonly IUpdateOperationService<AdvertisementElement> _updateAdvertisementElementOperationService;
        private readonly IChangeAdvertisementElementStatusOperationService _changeAdvertisementElementStatusOperationService;

        public UpdateAdvertisementElementStatusOperationService(
            IOperationScopeFactory operationScopeFactory,
            IGetDomainEntityDtoService<AdvertisementElement> getAdvertisementElementDomainEntityDtoService,
            IUpdateOperationService<AdvertisementElement> updateAdvertisementElementOperationService,
            IChangeAdvertisementElementStatusOperationService changeAdvertisementElementStatusOperationService)
        {
            _operationScopeFactory = operationScopeFactory;
            _getAdvertisementElementDomainEntityDtoService = getAdvertisementElementDomainEntityDtoService;
            _updateAdvertisementElementOperationService = updateAdvertisementElementOperationService;
            _changeAdvertisementElementStatusOperationService = changeAdvertisementElementStatusOperationService;
        }

        public void Update(IDomainEntityDto entityDto)
        {
            var advertisementElementStatusDomainEntityDto = (AdvertisementElementStatusDomainEntityDto)entityDto;
            var status = (AdvertisementElementStatusValue)advertisementElementStatusDomainEntityDto.Status;
            var reasons = advertisementElementStatusDomainEntityDto.Reasons
                             .Select(reason => new AdvertisementElementDenialReason
                                 {
                                     AdvertisementElementId = advertisementElementStatusDomainEntityDto.Id,
                                     Comment = reason.Comment,
                                     DenialReasonId = reason.Id,
                                 })
                             .ToArray();

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ChangeAdvertisementElementStatusIdentity>())
            {
                _changeAdvertisementElementStatusOperationService.ChangeStatus(advertisementElementStatusDomainEntityDto.Id, status, reasons);

                UpdateAdvertisementElement(advertisementElementStatusDomainEntityDto);

                operationScope.Complete();
            }
        }

        private void UpdateAdvertisementElement(AdvertisementElementStatusDomainEntityDto advertisementElementStatusDomainEntityDto)
        {
            var advertisementElementDomainEntityDto = (AdvertisementElementDomainEntityDto)
                                                      _getAdvertisementElementDomainEntityDtoService.GetDomainEntityDto(
                                                          advertisementElementStatusDomainEntityDto.Id,
                                                          false,
                                                          null,
                                                          EntityType.Instance.None(),
                                                          string.Empty);

            advertisementElementStatusDomainEntityDto.TransferRestrictionValuesTo(advertisementElementDomainEntityDto);
            advertisementElementStatusDomainEntityDto.TransferTextValuesTo(advertisementElementDomainEntityDto);
            advertisementElementStatusDomainEntityDto.TransferFileValuesTo(advertisementElementDomainEntityDto);
            advertisementElementStatusDomainEntityDto.TransferPeriodValuesTo(advertisementElementDomainEntityDto);
            advertisementElementStatusDomainEntityDto.TransferFasCommentValuesTo(advertisementElementDomainEntityDto);
            advertisementElementStatusDomainEntityDto.TransferLinkValuesTo(advertisementElementDomainEntityDto);
            advertisementElementStatusDomainEntityDto.TransferTimestampTo(advertisementElementDomainEntityDto);

            _updateAdvertisementElementOperationService.Update(advertisementElementDomainEntityDto);
        }
    }
}