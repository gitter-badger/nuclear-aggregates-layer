using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Create;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    // FIXME {all, 26.02.2014}: при разделении на create и update usecase удалить данный proxy класс
    [Obsolete]
    public class ModifyAdvertisementElementOperationService : IModifyBusinessModelEntityService<AdvertisementElement>
    {
        private readonly ICreateOperationService<AdvertisementElement> _createAdvertisementElementOperationService;
        private readonly IUpdateAdvertisementElementAndSetAsReadyForVerificationOperationService _updateAdvertisementElementOperationService;

        public ModifyAdvertisementElementOperationService(
            ICreateOperationService<AdvertisementElement> createAdvertisementElementOperationService,
            IUpdateAdvertisementElementAndSetAsReadyForVerificationOperationService updateAdvertisementElementOperationService)
        {
            _createAdvertisementElementOperationService = createAdvertisementElementOperationService;
            _updateAdvertisementElementOperationService = updateAdvertisementElementOperationService;
        }

        // Virtual for interception
        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdvertisementElementDomainEntityDto)domainEntityDto;
            if (dto.IsNew())
            {
                return _createAdvertisementElementOperationService.Create(dto);
            }

            _updateAdvertisementElementOperationService.UpdateAndSetAsReadyForVerification(dto);

            return dto.Id;
        }
    }
}