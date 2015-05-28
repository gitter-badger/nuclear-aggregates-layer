using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

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

                public static FindSpecification<DenormalizedClientLink> ClientChild(long clientId)
                {
                    return new FindSpecification<DenormalizedClientLink>(x => x.MasterClientId == clientId && x.IsLinkedDirectly);
                }
            }
        }
    }
}