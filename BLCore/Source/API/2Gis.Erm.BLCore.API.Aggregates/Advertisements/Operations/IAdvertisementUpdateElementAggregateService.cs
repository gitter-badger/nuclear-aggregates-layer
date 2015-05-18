using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations
{
    public interface IAdvertisementUpdateElementAggregateService : IAggregateSpecificOperation<Advertisement, UpdateIdentity>
    {
        void Update(
            IEnumerable<AdvertisementElement> advertisementElements,
            AdvertisementElementTemplate elementTemplate,
            string plainText,
            string formattedText);
    }
}