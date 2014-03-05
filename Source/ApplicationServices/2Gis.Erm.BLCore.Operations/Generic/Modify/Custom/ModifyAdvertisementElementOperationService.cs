using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Create;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Update;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    // FIXME {all, 26.02.2014}: при разделении на create и update usecase удалить данный proxy класс
    [Obsolete]
    public class ModifyAdvertisementElementOperationService : IModifyBusinessModelEntityService<AdvertisementElement>
    {
        private readonly ICreateOperationService<AdvertisementElement> _createAdvertisementElementOperationService;
        private readonly IUpdateOperationService<AdvertisementElement> _updateAdvertisementElementOperationService;

        public ModifyAdvertisementElementOperationService(
            ICreateOperationService<AdvertisementElement> createAdvertisementElementOperationService,
            IUpdateOperationService<AdvertisementElement> updateAdvertisementElementOperationService)
        {
            _createAdvertisementElementOperationService = createAdvertisementElementOperationService;
            _updateAdvertisementElementOperationService = updateAdvertisementElementOperationService;
        }

        // Virtual for interception
        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            long entityId = domainEntityDto.Id;
            if (domainEntityDto.IsNew())
            {
                entityId = _createAdvertisementElementOperationService.Create(domainEntityDto);
            }
            else
            {
                _updateAdvertisementElementOperationService.Update(domainEntityDto);
            }

            return entityId;
        }
    }
}