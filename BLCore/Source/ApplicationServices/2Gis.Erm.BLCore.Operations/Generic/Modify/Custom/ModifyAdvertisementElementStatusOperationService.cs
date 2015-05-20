using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Create;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Update;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    // FIXME {all, 18.07.2014}: при разделении на create и update usecase удалить данный proxy класс
    [Obsolete]
    public class ModifyAdvertisementElementStatusOperationService : IModifyBusinessModelEntityService<AdvertisementElementStatus>
    {
        private readonly ICreateOperationService<AdvertisementElementStatus> _createAdvertisementElementStatusOperationService;
        private readonly IUpdateOperationService<AdvertisementElementStatus> _updateAdvertisementElementStatusOperationService;

        public ModifyAdvertisementElementStatusOperationService(
            ICreateOperationService<AdvertisementElementStatus> createAdvertisementElementStatusOperationService,
            IUpdateOperationService<AdvertisementElementStatus> updateAdvertisementElementStatusOperationService)
        {
            _createAdvertisementElementStatusOperationService = createAdvertisementElementStatusOperationService;
            _updateAdvertisementElementStatusOperationService = updateAdvertisementElementStatusOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            if (domainEntityDto.IsNew())
            {
                return _createAdvertisementElementStatusOperationService.Create(domainEntityDto);
            }

            _updateAdvertisementElementStatusOperationService.Update(domainEntityDto);
            return domainEntityDto.Id;
        }
    }
}