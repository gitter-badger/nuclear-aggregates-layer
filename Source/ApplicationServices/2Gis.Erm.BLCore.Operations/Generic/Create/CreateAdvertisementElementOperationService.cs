using System;

using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Create
{
    public sealed class CreateAdvertisementElementOperationService : CreateOperationServiceBase<AdvertisementElement, AdvertisementElementDomainEntityDto>
    {
        protected override long Create(AdvertisementElement entity, AdvertisementElementDomainEntityDto entityDto)
        {
            throw new InvalidOperationException("AdvertisementElement can't be directly created. Elements created through the Advertisement create operation or AdvertisementTemplate filling");
        }
    }
}
