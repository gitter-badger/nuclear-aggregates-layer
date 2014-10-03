﻿using System;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel
{
    public static partial class ClientSpecs
    {
        public static class DenormalizedClientLinks
        {
            public static class Find
            {
                public static FindSpecification<DenormalizedClientLink> ByMasterAndChildNodes(long masterClientId, long childClientId)
                {
                    return new FindSpecification<DenormalizedClientLink>(x => x.MasterClientId == masterClientId ||
                                                                              x.MasterClientId == childClientId ||
                                                                              x.ChildClientId == masterClientId ||
                                                                              x.ChildClientId == childClientId);
                }

                public static FindSpecification<DenormalizedClientLink> ByGrapKeys(params Guid[] keys)
                {
                    return new FindSpecification<DenormalizedClientLink>(x => keys.Contains(x.GraphKey));
                }
            }
        }
    }
}