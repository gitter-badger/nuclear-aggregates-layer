using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations
{
    public interface IBulkCreateAdvertisementElementsForAdvertisementAggregateService : IAggregateSpecificOperation<Advertisement, BulkCreateIdentity>
    {
        void Create(IEnumerable<AdvertisementElementCreationDto> advertisementElements, long advertisementId, long ownerCode);
    }
}
