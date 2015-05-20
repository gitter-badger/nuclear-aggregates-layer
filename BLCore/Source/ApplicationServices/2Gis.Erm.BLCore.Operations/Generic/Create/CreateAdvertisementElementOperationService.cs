using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Create;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Create
{
    public sealed class CreateAdvertisementElementOperationService : ICreateOperationService<AdvertisementElement>
    {
        public long Create(IDomainEntityDto entityDto)
        {
            throw new InvalidOperationException("AdvertisementElement can't be directly created. " +
                                                "Elements created through the Advertisement create operation or AdvertisementTemplate filling");
        }
    }
}
