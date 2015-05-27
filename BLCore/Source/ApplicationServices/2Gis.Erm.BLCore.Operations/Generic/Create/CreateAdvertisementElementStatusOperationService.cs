using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Create;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Create
{
    public class CreateAdvertisementElementStatusOperationService : ICreateOperationService<AdvertisementElementStatus>
    {
        public long Create(IDomainEntityDto entityDto)
        {
            throw new InvalidOperationException("AdvertisementElementStatus can't be directly created. " +
                                                "Element statuses created through the Advertisement create operation or AdvertisementTemplate filling");
        }
    }
}