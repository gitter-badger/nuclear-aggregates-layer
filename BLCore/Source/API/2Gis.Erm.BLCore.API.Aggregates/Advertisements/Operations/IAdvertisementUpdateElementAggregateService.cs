﻿using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations
{
    public interface IAdvertisementUpdateElementAggregateService : IAggregateSpecificService<Advertisement, UpdateIdentity>
    {
        void Update(
            IEnumerable<AdvertisementElement> advertisementElements,
            AdvertisementElementTemplate elementTemplate,
            string plainText,
            string formattedText);
    }
}